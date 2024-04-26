using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using StreetcodeCoordinateEntity = Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate;

namespace Streetcode.BLL.MediatR.AdditionalContent.StreetcodeCoordinate.Delete;

public class DeleteStreetcodeCoordinateHandler :
    IRequestHandler<DeleteStreetcodeCoordinateCommand, Result<DeleteStreetcodeCoordinateResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteStreetcodeCoordinateHandler(IRepositoryWrapper repository, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _logger = logger;
    }

    public async Task<Result<DeleteStreetcodeCoordinateResponseDto>> Handle(DeleteStreetcodeCoordinateCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var streetcodeCoordinate = await _repositoryWrapper.StreetcodeCoordinateRepository
           .GetFirstOrDefaultAsync(sc => sc.Id == request.Id);

        if (streetcodeCoordinate is null)
        {
            string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StreetcodeCoordinateEntity).Name,
            request.Id);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        _repositoryWrapper.StreetcodeCoordinateRepository.Delete(streetcodeCoordinate);
        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToDeleteStreetcodeCoordinateError(request);
        }

        var responseDto = new DeleteStreetcodeCoordinateResponseDto(true);

        return Result.Ok(responseDto);
    }

    private Result<DeleteStreetcodeCoordinateResponseDto> FailedToDeleteStreetcodeCoordinateError(DeleteStreetcodeCoordinateRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.DeleteFailed,
            typeof(StreetcodeCoordinateEntity).Name,
            request.Id);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}
