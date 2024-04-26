using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;

using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;

namespace Streetcode.BLL.MediatR.Media.Art.Delete;

public class DeleteArtHandler : IRequestHandler<DeleteArtCommand, Result<DeleteArtResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    public DeleteArtHandler(IRepositoryWrapper repository, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _logger = logger;
    }

    public async Task<Result<DeleteArtResponseDto>> Handle(DeleteArtCommand command, CancellationToken cancellationToken)
    {
        var request = command.DeleteArtRequestDto;

        var art = await _repositoryWrapper.ArtRepository.GetFirstOrDefaultAsync(x => x.Id == request.Id);

        if (art is null)
        {
            return ArtNotFoundError(request.Id);
        }

        _repositoryWrapper.ArtRepository.Delete(art);

        bool isSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!isSuccess)
        {
            return DeleteFailed(request.Id);
        }

        var response = new DeleteArtResponseDto(true);

        return Result.Ok(response);
    }

    private Result<DeleteArtResponseDto> ArtNotFoundError(int id)
    {
        string errorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(ArtEntity).Name,
            id);

        _logger.LogError(id, errorMessage);

        return Result.Fail(errorMessage);
    }

    private Result<DeleteArtResponseDto> DeleteFailed(int id)
    {
        string errorMessage = string.Format(
            ErrorMessages.DeleteFailed,
            typeof(ArtEntity).Name,
            id);

        _logger.LogError(id, errorMessage);

        return Result.Fail(errorMessage);
    }
}
