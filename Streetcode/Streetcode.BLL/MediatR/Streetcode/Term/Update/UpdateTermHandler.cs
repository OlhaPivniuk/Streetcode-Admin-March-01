using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TermEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Update;

public class UpdateTermHandler :
    IRequestHandler<UpdateTermCommand, Result<UpdateTermResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public UpdateTermHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UpdateTermResponseDto>> Handle(UpdateTermCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var termToUpdate = _mapper.Map<TermEntity>(request);

        var existedTerm = await _repositoryWrapper.TermRepository
            .GetSingleOrDefaultAsync(term => term.Id == request.Id);

        if (existedTerm is null)
        {
            return TermIsNotFoundError(request);
        }

        bool isCurrentTitle = existedTerm.Title == request.Title;

        if (!isCurrentTitle && !await IsTermTitleUniqueAsync(request.Title))
        {
            return TermTitleIsNotUniqueError(request);
        }

        _repositoryWrapper.TermRepository.Update(termToUpdate);

        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToUpdateTermError(request);
        }

        var responseDto = _mapper.Map<UpdateTermResponseDto>(termToUpdate);

        return Result.Ok(responseDto);
    }

    private Result<UpdateTermResponseDto> TermIsNotFoundError(UpdateTermRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(TermEntity).Name,
            request.Id);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<UpdateTermResponseDto> FailedToUpdateTermError(UpdateTermRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.CreateFailed,
            typeof(TermEntity).Name);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private async Task<bool> IsTermTitleUniqueAsync(string? title)
    {
        var term = await _repositoryWrapper.TermRepository
            .GetFirstOrDefaultAsync(term => term.Title == title);

        return term is null;
    }

    private Result<UpdateTermResponseDto> TermTitleIsNotUniqueError(UpdateTermRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.PropertyMustBeUnique,
            nameof(request.Title),
            request.Title,
            typeof(TermEntity).Name);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}
