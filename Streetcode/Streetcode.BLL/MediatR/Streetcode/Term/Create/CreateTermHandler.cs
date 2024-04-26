using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TermEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Create;

public class CreateTermHandler :
    IRequestHandler<CreateTermCommand, Result<CreateTermResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public CreateTermHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CreateTermResponseDto>> Handle(CreateTermCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (!await IsTermTitleUniqueAsync(request.Title))
        {
            return TermTitleIsNotUniqueError(request);
        }

        var termToCreate = _mapper.Map<TermEntity>(request);

        var term = _repositoryWrapper.TermRepository.Create(termToCreate);

        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToCreateTermError(request);
        }

        var responseDto = _mapper.Map<CreateTermResponseDto>(term);

        return Result.Ok(responseDto);
    }

    private Result<CreateTermResponseDto> FailedToCreateTermError(CreateTermRequestDto request)
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

    private Result<CreateTermResponseDto> TermTitleIsNotUniqueError(CreateTermRequestDto request)
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