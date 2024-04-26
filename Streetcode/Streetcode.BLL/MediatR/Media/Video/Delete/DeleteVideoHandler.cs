using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Video.Delete;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using VideoEntity = Streetcode.DAL.Entities.Media.Video;

namespace Streetcode.BLL.MediatR.Video.Delete;

public class DeleteVideoHandler :
    IRequestHandler<DeleteVideoCommand, Result<DeleteVideoResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteVideoHandler(IRepositoryWrapper repository, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _logger = logger;
    }

    public async Task<Result<DeleteVideoResponseDto>> Handle(DeleteVideoCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var video = await _repositoryWrapper.VideoRepository
           .GetFirstOrDefaultAsync(sr => sr.Id == request.Id);

        if (video is null)
        {
            string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(VideoEntity).Name,
            request.Id);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        _repositoryWrapper.VideoRepository.Delete(video);
        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToDeleteVideoError(request);
        }

        var responseDto = new DeleteVideoResponseDto(true);

        return Result.Ok(responseDto);
    }

    private Result<DeleteVideoResponseDto> FailedToDeleteVideoError(DeleteVideoRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.DeleteFailed,
            typeof(VideoEntity).Name,
            request.Id);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}