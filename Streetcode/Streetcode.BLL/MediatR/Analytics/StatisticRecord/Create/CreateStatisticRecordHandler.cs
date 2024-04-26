using System.Security.Cryptography;
using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Analytics.StatisticRecord;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using StatisticRecordEntity = Streetcode.DAL.Entities.Analytics.StatisticRecord;

namespace Streetcode.BLL.MediatR.Analytics.StatisticRecord.Create;

public class CreateStatisticRecordHandler :
    IRequestHandler<CreateStatisticRecordCommand, Result<CreateStatisticRecordResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;
    private readonly RandomNumberGenerator _random = RandomNumberGenerator.Create();

    public CreateStatisticRecordHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CreateStatisticRecordResponseDto>> Handle(CreateStatisticRecordCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (!await IsStreetcodeCoordinateUniqueAsync(request.StreetcodeCoordinateId))
        {
            return StreetcodeCoordinateIsNotUniqueError(request);
        }

        if (!await IsStreetcodeExistAsync(request.StreetcodeId))
        {
            return StreetcodeNotFoundError(request);
        }

        if (!await IsStreetcodeCoordinateExistAsync(request.StreetcodeCoordinateId))
        {
            return StreetcodeCoordinateNotFoundError(request);
        }

        var statisticRecordToCreate = _mapper.Map<StatisticRecordEntity>(request);
        statisticRecordToCreate.QrId = await GetUniqueQrId();

        var statisticRecord = _repositoryWrapper.StatisticRecordRepository.Create(statisticRecordToCreate);

        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToCreateStatisticRecordError(request);
        }

        var responseDto = _mapper.Map<CreateStatisticRecordResponseDto>(statisticRecord);

        return Result.Ok(responseDto);
    }

    private async Task<bool> IsStreetcodeExistAsync(int streetcodeId)
    {
        var streetcode = await _repositoryWrapper.StreetcodeRepository
            .GetFirstOrDefaultAsync(s => s.Id == streetcodeId);

        return streetcode is not null;
    }

    private Result<CreateStatisticRecordResponseDto> StreetcodeNotFoundError(CreateStatisticRecordRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StreetcodeContent).Name,
            request.StreetcodeId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private async Task<bool> IsStreetcodeCoordinateExistAsync(int streetcodeCoordinateId)
    {
        var streetcodeCoordinate = await _repositoryWrapper.StreetcodeCoordinateRepository
            .GetFirstOrDefaultAsync(s => s.Id == streetcodeCoordinateId);

        return streetcodeCoordinate is not null;
    }

    private Result<CreateStatisticRecordResponseDto> StreetcodeCoordinateNotFoundError(CreateStatisticRecordRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StreetcodeCoordinate).Name,
            request.StreetcodeCoordinateId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private async Task<bool> IsStreetcodeCoordinateUniqueAsync(int streetcodeCoordinateId)
    {
        var statisticRecord = await _repositoryWrapper.StatisticRecordRepository
            .GetFirstOrDefaultAsync(sr => sr.StreetcodeCoordinateId == streetcodeCoordinateId);

        return statisticRecord is null;
    }

    private Result<CreateStatisticRecordResponseDto> StreetcodeCoordinateIsNotUniqueError(CreateStatisticRecordRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.PotencialPrimaryKeyIsNotUnique,
            typeof(StreetcodeCoordinate).Name,
            request.StreetcodeCoordinateId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<CreateStatisticRecordResponseDto> FailedToCreateStatisticRecordError(CreateStatisticRecordRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.CreateFailed,
            typeof(StatisticRecordEntity).Name);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private async Task<int> GetUniqueQrId()
    {
        int qrId = 0;
        byte[] bytes = new byte[32];
        bool isNewKey = false;

        while (!isNewKey)
        {
            _random.GetBytes(bytes);
            qrId = Math.Abs((int)BitConverter.ToUInt32(bytes) / 2);
            var statisticRecord = await _repositoryWrapper.StatisticRecordRepository.GetFirstOrDefaultAsync(sr => sr.QrId == qrId);
            if (statisticRecord is null)
            {
                isNewKey = true;
            }
        }

        return qrId;
    }
}
