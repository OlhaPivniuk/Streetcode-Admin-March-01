using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Streetcode.BLL.Constants;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Update
{
    public class UpdateFactHandler : IRequestHandler<UpdateFactCommand, Result<UpdateFactDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;

        public UpdateFactHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger, IHttpContextAccessor httpContext, IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
            _httpContext = httpContext;
            _mapper = mapper;
        }

        public async Task<Result<UpdateFactDto>> Handle(UpdateFactCommand command, CancellationToken cancellationToken)
        {
            UpdateFactDto request = command.UpdateRequest;

            if (_httpContext.HttpContext!.Items[GeneralConstants.ENTITY] is not FactEntity fact)
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

            _repositoryWrapper.FactRepository.Update(factEntity);

            bool isSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (!isSuccess)
            {
                return UpdateFailedError(request);
            }

            return Result.Ok(_mapper.Map<UpdateFactDto>(factEntity));
        }

        private Result<UpdateFactDto> FactNotFoundError(UpdateFactDto request)
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

        private Result<UpdateFactDto> ImageNotFoundError(UpdateFactDto request)
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

        private Result<UpdateFactDto> StreetcodeNotFoundError(UpdateFactDto request)
        {
            string errorMessage = string.Format(
                ErrorMessages.EntityByIdNotFound,
                nameof(StreetcodeContent),
                request.StreetcodeId);

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        private Result<UpdateFactDto> UpdateFailedError(UpdateFactDto request)
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
