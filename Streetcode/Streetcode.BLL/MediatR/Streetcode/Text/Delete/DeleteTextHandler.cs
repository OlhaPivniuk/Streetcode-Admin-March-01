using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TextEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Delete;

public class DeleteTextHandler :
    IRequestHandler<DeleteTextCommand, Result<DeleteTextResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteTextHandler(IRepositoryWrapper repository, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _logger = logger;
    }

    public async Task<Result<DeleteTextResponseDto>> Handle(DeleteTextCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var text = await _repositoryWrapper.TextRepository
           .GetFirstOrDefaultAsync(sr => sr.Id == request.Id);

        if (text is null)
        {
            string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(TextEntity).Name,
            request.Id);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        _repositoryWrapper.TextRepository.Delete(text);
        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToDeleteTextError(request);
        }

        var responseDto = new DeleteTextResponseDto(true);

        return Result.Ok(responseDto);
    }

    private Result<DeleteTextResponseDto> FailedToDeleteTextError(DeleteTextRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.DeleteFailed,
            typeof(TextEntity).Name,
            request.Id);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}