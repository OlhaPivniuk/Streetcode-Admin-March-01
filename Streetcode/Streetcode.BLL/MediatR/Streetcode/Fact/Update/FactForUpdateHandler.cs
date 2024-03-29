using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Update
{
    public class FactForUpdateHandler : IRequestHandler<FactForUpdateCommand, Result<FactForUpdateDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;
        public FactForUpdateHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<FactForUpdateDto>> Handle(FactForUpdateCommand query, CancellationToken cancellationToken)
        {
            var request = query.UpdateRequest;

            var fact = await _repositoryWrapper.FactRepository.GetFirstOrDefaultAsync(x => x.Id == request.Id);

            var image = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(i => i.Id == request.ImageId);

            if (image is not null)
            {
                fact.ImageId = request.ImageId;
            }
            else
            {
                string error = "Failed to find image";
                _logger.LogError(query, error);
                return Result.Fail(error);
            }

            if (request.Title is not null)
            {
                fact.Title = request.Title;
            }

            if (request.FactContent is not null)
            {
                fact.FactContent = request.FactContent;
            }

            _repositoryWrapper.FactRepository.Update(fact);

            var isSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (!isSuccess)
            {
                string error = "Failed update fact";
                _logger.LogError(query, error);
                return Result.Fail(new Error(error));
            }

            return Result.Ok(request);
        }
    }
}
