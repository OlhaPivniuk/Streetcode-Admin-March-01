using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Update
{
    public class UpdateFactHandler : IRequestHandler<UpdateFactCommand, Result<UpdateFactDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;
        public UpdateFactHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<UpdateFactDto>> Handle(UpdateFactCommand query, CancellationToken cancellationToken)
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
                string errorMsg = string.Format(
                ErrorMessages.EntityByIdNotFound,
                nameof(Image),
                request.ImageId);
                _logger.LogError(query, errorMsg);
                return Result.Fail(errorMsg);
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
                string errorMsg = string.Format(
                ErrorMessages.UpdateFailed,
                nameof(Fact),
                request.ImageId);

                _logger.LogError(query, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(request);
        }
    }
}
