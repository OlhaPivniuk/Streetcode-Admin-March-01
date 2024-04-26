using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;

using TimelineEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete
{
    public class DeleteTimelineItemHandler : IRequestHandler<DeleteTimelineItemCommand, Result<Unit>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public DeleteTimelineItemHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(DeleteTimelineItemCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;
            var timeline = await _repositoryWrapper.TimelineRepository.GetFirstOrDefaultAsync(x => x.Id == id);

            if (timeline is null)
            {
                string errorMsg = string.Format(
                    ErrorMessages.EntityByIdNotFound,
                    nameof(TimelineEntity),
                    request.Id);

                _logger.LogError(request, errorMsg);

                return Result.Fail(errorMsg);
            }

            _repositoryWrapper.TimelineRepository.Delete(timeline);

            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (resultIsSuccess)
            {
                return Result.Ok(Unit.Value);
            }
            else
            {
                string errorMsg = string.Format(
                    ErrorMessages.DeleteFailed,
                    nameof(TimelineEntity),
                    request.Id);

                _logger.LogError(request, errorMsg);

                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
