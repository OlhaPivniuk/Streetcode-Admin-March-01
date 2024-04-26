using System.Text;
using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TextEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Update;

public class UpdateTextHandler :
    IRequestHandler<UpdateTextCommand, Result<UpdateTextResponseDto>>
{
    private const string PREFILLEDTEXT = "Текст підготовлений спільно з ";
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public UpdateTextHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UpdateTextResponseDto>> Handle(UpdateTextCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var textToCreate = _mapper.Map<TextEntity>(request);

        var existedText = await _repositoryWrapper.TextRepository
            .GetFirstOrDefaultAsync(text => text.Id == request.Id);

        if (existedText is null)
        {
            return TextNotFoundError(request);
        }

        textToCreate.StreetcodeId = existedText.StreetcodeId;

        if (textToCreate.AdditionalText is not null && textToCreate.AdditionalText != string.Empty)
        {
            StringBuilder sb = new StringBuilder(PREFILLEDTEXT);
            textToCreate.AdditionalText = sb.Append(textToCreate.AdditionalText).ToString();
        }
        else
        {
            textToCreate.AdditionalText = null;
        }

        _repositoryWrapper.TextRepository.Update(textToCreate);

        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToCreateTextError(request);
        }

        var responseDto = _mapper.Map<UpdateTextResponseDto>(textToCreate);

        return Result.Ok(responseDto);
    }

    private Result<UpdateTextResponseDto> TextNotFoundError(UpdateTextRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(TextEntity).Name,
            request.Id);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<UpdateTextResponseDto> FailedToCreateTextError(UpdateTextRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.CreateFailed,
            typeof(TextEntity).Name);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}
