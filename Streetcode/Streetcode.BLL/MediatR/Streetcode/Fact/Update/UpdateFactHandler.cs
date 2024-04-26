using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using AutoMapper;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Media.Images;

using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Update
{
    public class UpdateFactHandler : IRequestHandler<UpdateFactCommand, Result<FactDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        public UpdateFactHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger, IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<FactDto>> Handle(UpdateFactCommand command, CancellationToken cancellationToken)
        {
            UpdateFactDto request = command.UpdateRequest;

            var fact = await _repositoryWrapper.FactRepository.GetFirstOrDefaultAsync(x => x.Id == request.Id);

            if (fact is null)
            {
                return FactNotFoundError(request);
            }

            if (!await IsImageExistAsync(request.ImageId))
            {
                return ImageNotFoundError(request);
            }

            if (!await IsStreetcodeExistAsync(request.StreetcodeId))
            {
                return StreetcodeNotFoundError(request);
            }

            FactEntity factEntity = _mapper.Map<FactEntity>(request);

            factEntity.Number = fact.Number;

            _repositoryWrapper.FactRepository.Update(factEntity);

            bool isSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (!isSuccess)
            {
                return UpdateFailedError(request);
            }

            return Result.Ok(_mapper.Map<FactDto>(factEntity));
        }

        private Result<FactDto> FactNotFoundError(UpdateFactDto request)
        {
            string errorMessage = string.Format(
                ErrorMessages.EntityByIdNotFound,
                nameof(FactEntity),
                request.Id);

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        private async Task<bool> IsImageExistAsync(int imageId)
        {
            var image = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(i => i.Id == imageId);

            return image is not null;
        }

        private Result<FactDto> ImageNotFoundError(UpdateFactDto request)
        {
            string errorMessage = string.Format(
                ErrorMessages.EntityByIdNotFound,
                nameof(Image),
                request.ImageId);

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        private async Task<bool> IsStreetcodeExistAsync(int streetcodeId)
        {
            var streetcode = await _repositoryWrapper.StreetcodeRepository.GetFirstOrDefaultAsync(s => s.Id == streetcodeId);

            return streetcode is not null;
        }

        private Result<FactDto> StreetcodeNotFoundError(UpdateFactDto request)
        {
            string errorMessage = string.Format(
                ErrorMessages.EntityByIdNotFound,
                nameof(StreetcodeContent),
                request.StreetcodeId);

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        private Result<FactDto> UpdateFailedError(UpdateFactDto request)
        {
            string errorMessage = string.Format(
                ErrorMessages.UpdateFailed,
                nameof(FactEntity),
                request.Id);

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }
    }
}
