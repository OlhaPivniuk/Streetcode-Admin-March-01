using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using StreetcodeCoordinateEntity = Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate;

namespace Streetcode.BLL.MediatR.AdditionalContent.StreetcodeCoordinate.Create;

public class CreateStreetcodeCoordinateHandler :
    IRequestHandler<CreateStreetcodeCoordinateCommand, Result<CreateStreetcodeCoordinateResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public CreateStreetcodeCoordinateHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CreateStreetcodeCoordinateResponseDto>> Handle(CreateStreetcodeCoordinateCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (!await IsStreetcodeExistAsync(request.StreetcodeId))
        {
            return StreetcodeNotFoundError(request);
        }

        var streetcodeCoordinateToCreate = _mapper.Map<StreetcodeCoordinateEntity>(request);

        var streetcodeCoordinate = _repositoryWrapper.StreetcodeCoordinateRepository.Create(streetcodeCoordinateToCreate);

        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToCreateStreetcodeCoordinateError(request);
        }

        var responseDto = _mapper.Map<CreateStreetcodeCoordinateResponseDto>(streetcodeCoordinate);

        return Result.Ok(responseDto);
    }

    private async Task<bool> IsStreetcodeExistAsync(int streetcodeId)
    {
        var streetcode = await _repositoryWrapper.StreetcodeRepository
            .GetFirstOrDefaultAsync(s => s.Id == streetcodeId);

        return streetcode is not null;
    }

    private Result<CreateStreetcodeCoordinateResponseDto> StreetcodeNotFoundError(CreateStreetcodeCoordinateRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StreetcodeContent).Name,
            request.StreetcodeId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<CreateStreetcodeCoordinateResponseDto> FailedToCreateStreetcodeCoordinateError(CreateStreetcodeCoordinateRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.CreateFailed,
            typeof(StreetcodeCoordinateEntity).Name);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}
