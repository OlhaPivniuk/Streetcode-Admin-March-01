using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TermEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Delete;

public class DeleteTermHandler :
    IRequestHandler<DeleteTermCommand, Result<DeleteTermResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteTermHandler(IRepositoryWrapper repository, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _logger = logger;
    }

    public async Task<Result<DeleteTermResponseDto>> Handle(DeleteTermCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var term = await _repositoryWrapper.TermRepository
           .GetFirstOrDefaultAsync(sr => sr.Id == request.Id);

        if (term is null)
        {
            string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(TermEntity).Name,
            request.Id);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        _repositoryWrapper.TermRepository.Delete(term);
        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToDeleteTermError(request);
        }

        var responseDto = new DeleteTermResponseDto(true);

        return Result.Ok(responseDto);
    }

    private Result<DeleteTermResponseDto> FailedToDeleteTermError(DeleteTermRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.DeleteFailed,
            typeof(TermEntity).Name,
            request.Id);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}