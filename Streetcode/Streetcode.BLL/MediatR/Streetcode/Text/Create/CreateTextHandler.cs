using System.Text;
using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TextEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Create;

public class CreateTextHandler :
    IRequestHandler<CreateTextCommand, Result<CreateTextResponseDto>>
{
    private const string PREFILLEDTEXT = "Текст підготовлений спільно з ";
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public CreateTextHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CreateTextResponseDto>> Handle(CreateTextCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (!await IsStreetcodeUniqueAsync(request.StreetcodeId))
        {
            return StreetcodeIsNotUniqueError(request);
        }

        if (!await IsStreetcodeExistAsync(request.StreetcodeId))
        {
            return StreetcodeNotFoundError(request);
        }

        var textToCreate = _mapper.Map<TextEntity>(request);

        if (textToCreate.AdditionalText is not null && textToCreate.AdditionalText != string.Empty)
        {
            StringBuilder sb = new StringBuilder(PREFILLEDTEXT);
            textToCreate.AdditionalText = sb.Append(textToCreate.AdditionalText).ToString();
        }
        else
        {
            textToCreate.AdditionalText = null;
        }

        var text = _repositoryWrapper.TextRepository.Create(textToCreate);

        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToCreateTextError(request);
        }

        var responseDto = _mapper.Map<CreateTextResponseDto>(text);

        return Result.Ok(responseDto);
    }

    private async Task<bool> IsStreetcodeExistAsync(int streetcodeId)
    {
        var streetcode = await _repositoryWrapper.StreetcodeRepository
            .GetFirstOrDefaultAsync(s => s.Id == streetcodeId);

        return streetcode is not null;
    }

    private Result<CreateTextResponseDto> StreetcodeNotFoundError(CreateTextRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StreetcodeContent).Name,
            request.StreetcodeId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<CreateTextResponseDto> FailedToCreateTextError(CreateTextRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.CreateFailed,
            typeof(TextEntity).Name);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private async Task<bool> IsStreetcodeUniqueAsync(int streetcodeId)
    {
        var streetcode = await _repositoryWrapper.TextRepository
            .GetFirstOrDefaultAsync(sr => sr.Id == streetcodeId);

        return streetcode is null;
    }

    private Result<CreateTextResponseDto> StreetcodeIsNotUniqueError(CreateTextRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.PotencialPrimaryKeyIsNotUnique,
            typeof(StreetcodeContent).Name,
            request.StreetcodeId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}