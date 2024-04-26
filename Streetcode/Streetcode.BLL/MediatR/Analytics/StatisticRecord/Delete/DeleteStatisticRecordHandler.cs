using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Analytics.StatisticRecord;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using StatisticRecordEntity = Streetcode.DAL.Entities.Analytics.StatisticRecord;

namespace Streetcode.BLL.MediatR.Analytics.StatisticRecord.Delete;

public class DeleteStatisticRecordHandler :
    IRequestHandler<DeleteStatisticRecordCommand, Result<DeleteStatisticRecordResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteStatisticRecordHandler(IRepositoryWrapper repository, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _logger = logger;
    }

    public async Task<Result<DeleteStatisticRecordResponseDto>> Handle(DeleteStatisticRecordCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var statisticRecord = await _repositoryWrapper.StatisticRecordRepository
           .GetFirstOrDefaultAsync(sr => sr.Id == request.Id);

        if (statisticRecord is null)
        {
            string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StatisticRecordEntity).Name,
            request.Id);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        _repositoryWrapper.StatisticRecordRepository.Delete(statisticRecord);
        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToDeleteStatisticRecordError(request);
        }

        var responseDto = new DeleteStatisticRecordResponseDto(true);

        return Result.Ok(responseDto);
    }

    private Result<DeleteStatisticRecordResponseDto> FailedToDeleteStatisticRecordError(DeleteStatisticRecordRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.DeleteFailed,
            typeof(StatisticRecordEntity).Name,
            request.Id);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}