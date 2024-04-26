using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using static Streetcode.DAL.Specifications.TimelineSpecifications.HistoricalContextSpecs;
using static Streetcode.DAL.Specifications.TimelineSpecifications.HistoricalContextTimelineSpecs;
using static Streetcode.DAL.Specifications.TimelineSpecifications.TimelineItemSpecs;
using HistoricalContextEntity = Streetcode.DAL.Entities.Timeline.HistoricalContext;
using HistoricalContextTimelineEntity = Streetcode.DAL.Entities.Timeline.HistoricalContextTimeline;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;

public class UpdateTimelineItemHandler : IRequestHandler<UpdateTimelineItemCommand, Result<TimelineItemDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public UpdateTimelineItemHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<TimelineItemDto>> Handle(UpdateTimelineItemCommand command, CancellationToken cancellationToken)
    {
        using var transactionScope = _repositoryWrapper.BeginTransaction();

        var request = command.UpdateTimelineItemDto;
        TimelineItemEntity timelineItem = _mapper.Map<TimelineItemEntity>(request);
        TimelineItemEntity? updatedTimelineItem;
        TimelineItemDto responseDto;

        if (!string.IsNullOrEmpty(request.HistoricalContext))
        {
            HistoricalContextTimelineEntity? historicalContextTimeline;
            var historicalContext = await GetHistoricalContextByTitleAsync(request);

            if (historicalContext == null)
            {
                historicalContext = await CreateHistoricalContextAsync(request);
                historicalContextTimeline = CreateHistoricalContextTimeline(request, historicalContext);
            }
            else
            {
                historicalContextTimeline = await GetHistoricalContextTimelineAsync(request, historicalContext);

                historicalContextTimeline ??= CreateHistoricalContextTimeline(request, historicalContext);
            }

            _repositoryWrapper.TimelineRepository.Update(timelineItem!);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return UpdateFailedError(request);
            }

            updatedTimelineItem = await GetUpdatedTimelineItemAsync(request);

            responseDto = _mapper.Map<TimelineItemDto>(updatedTimelineItem);

            IEnumerable<HistoricalContextDto>? historicalContextDtos = await GetHistoricalContextDtosAsync(request, historicalContextTimeline);

            if (historicalContextDtos is not null)
            {
                responseDto.HistoricalContexts = historicalContextDtos;
            }

            transactionScope.Complete();

            return Result.Ok(responseDto);
        }
        else
        {
            _repositoryWrapper.TimelineRepository.Update(timelineItem!);

            if(await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return UpdateFailedError(request);
            }

            updatedTimelineItem = await GetUpdatedTimelineItemAsync(request);

            responseDto = _mapper.Map<TimelineItemDto>(updatedTimelineItem);

            transactionScope.Complete();

            return Result.Ok(responseDto);
        }
    }

    private async Task<HistoricalContextEntity?> GetHistoricalContextByTitleAsync(UpdateTimelineItemRequestDto request)
    {
        return await _repositoryWrapper.HistoricalContextRepository
            .GetItemBySpecAsync(new GetByTitleWithHistoricalContextTimelines(request.HistoricalContext!));
    }

    private async Task<IEnumerable<HistoricalContextDto>?> GetHistoricalContextDtosAsync(UpdateTimelineItemRequestDto request, HistoricalContextTimelineEntity? historicalContextTimeline)
    {
        var historicalContexts = await _repositoryWrapper.HistoricalContextTimelineRepository
            .GetItemsBySpecAsync(new GetAllByHistoricalContextIdAndTimelineId(
                historicalContextTimeline!.HistoricalContextId,
                request.Id));

        if (historicalContexts is null)
        {
            return null;
        }

        return historicalContexts
            .Select(pair => new HistoricalContextDto
                {
                    Id = pair.HistoricalContextId,
                    Title = request.HistoricalContext!
                });
    }

    private async Task<HistoricalContextTimelineEntity?> GetHistoricalContextTimelineAsync(UpdateTimelineItemRequestDto request, HistoricalContextEntity historicalContext)
    {
        return await _repositoryWrapper.HistoricalContextTimelineRepository
            .GetItemBySpecAsync(new GetByHistoricalContextIdAndTimelineId(historicalContext.Id, request.Id));
    }

    private HistoricalContextTimelineEntity CreateHistoricalContextTimeline(UpdateTimelineItemRequestDto request, HistoricalContextEntity historicalContext)
    {
        HistoricalContextTimelineEntity historicalContextTimeline = new()
        {
            TimelineId = request.Id,
            HistoricalContextId = historicalContext.Id,
        };
        _repositoryWrapper.HistoricalContextTimelineRepository.Create(historicalContextTimeline);

        return historicalContextTimeline;
    }

    private async Task<HistoricalContextEntity> CreateHistoricalContextAsync(UpdateTimelineItemRequestDto request)
    {
        HistoricalContextEntity? historicalContext = new ()
        {
            Title = request.HistoricalContext,
        };
        _repositoryWrapper.HistoricalContextRepository.Create(historicalContext);
        await _repositoryWrapper.SaveChangesAsync();

        return historicalContext;
    }

    private Result<TimelineItemDto> UpdateFailedError(UpdateTimelineItemRequestDto request)
    {
        string errorMessage = string.Format(
            ErrorMessages.UpdateFailed,
            typeof(TimelineItemEntity).Name,
            request.Id);

        _logger.LogError(request, errorMessage);

        return Result.Fail(errorMessage);
    }

    private async Task<TimelineItemEntity?> GetUpdatedTimelineItemAsync(UpdateTimelineItemRequestDto request)
    {
        return await _repositoryWrapper.TimelineRepository
            .GetItemBySpecAsync(new GetByIdWithHistoricalContextTimelinesAndHistoricalContext(request.Id));
    }
}
