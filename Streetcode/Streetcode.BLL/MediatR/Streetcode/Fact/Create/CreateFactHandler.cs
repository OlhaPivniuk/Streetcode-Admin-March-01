using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;

using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Create
{
    public class CreateFactHandler : IRequestHandler<CreateFactCommand, Result<CreateFactDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public CreateFactHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CreateFactDto>> Handle(CreateFactCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (!await IsImageExistAsync(request.ImageId))
            {
                return ImageNotFoundError(request);
            }

            if (!await IsStreetcodeExistAsync(request.StreetcodeId))
            {
                return StreetcodeNotFoundError(request);
            }

            int latestFactNumber = await GetLatestFactNumberAsync(request.StreetcodeId);

            var factToCreate = _mapper.Map<FactEntity>(request);

            factToCreate.Number = latestFactNumber + 1;

            var fact = _repositoryWrapper.FactRepository.Create(factToCreate);

            bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (!resultIsSuccess)
            {
                return FailedToCreateFactError(request);
            }

            return Result.Ok(_mapper.Map<FactEntity, CreateFactDto>(fact));
        }

        private async Task<bool> IsImageExistAsync(int imageId)
        {
            var image = await _repositoryWrapper.ImageRepository
                .GetFirstOrDefaultAsync(i => i.Id == imageId);

            return image is not null;
        }

        private Result<CreateFactDto> ImageNotFoundError(CreateFactDto request)
        {
            string errorMsg = string.Format(
                ErrorMessages.EntityByIdNotFound,
                nameof(Image),
                request.ImageId);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        private async Task<bool> IsStreetcodeExistAsync(int streetcodeId)
        {
            var streetcode = await _repositoryWrapper.StreetcodeRepository
                .GetFirstOrDefaultAsync(s => s.Id == streetcodeId);

            return streetcode is not null;
        }

        private Result<CreateFactDto> StreetcodeNotFoundError(CreateFactDto request)
        {
            string errorMsg = string.Format(
                ErrorMessages.EntityByIdNotFound,
                nameof(StreetcodeContent),
                request.StreetcodeId);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        private Task<int> GetLatestFactNumberAsync(int streetcodeId)
        {
            return _repositoryWrapper.FactRepository.GetMaxNumberAsync(streetcodeId);
        }

        private Result<CreateFactDto> FailedToCreateFactError(CreateFactDto request)
        {
            string errorMsg = string.Format(
                ErrorMessages.CreateFailed,
                nameof(Fact));
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }
    }
}
