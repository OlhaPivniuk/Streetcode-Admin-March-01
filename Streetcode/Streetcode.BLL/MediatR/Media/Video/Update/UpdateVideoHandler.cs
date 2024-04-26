using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;

using VideoEntity = Streetcode.DAL.Entities.Media.Video;

namespace Streetcode.BLL.MediatR.Media.Video.Update
{
    public class UpdateVideoHandler : IRequestHandler<UpdateVideoCommand, Result<UpdateVideoResponseDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public UpdateVideoHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<UpdateVideoResponseDto>> Handle(UpdateVideoCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var videoToUpdate = await _repositoryWrapper.VideoRepository.GetFirstOrDefaultAsync(v => v.Id == request.Id);

            if (videoToUpdate == null)
            {
                return VideoNotFoundError(request);
            }

            videoToUpdate.Title = request.Title;
            videoToUpdate.Description = request.Description;
            videoToUpdate.Url = request.Url;

            _repositoryWrapper.VideoRepository.Update(videoToUpdate);

            bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (!resultIsSuccess)
            {
                return FailedToUpdateVideoError(request);
            }

            var responseDto = _mapper.Map<UpdateVideoResponseDto>(videoToUpdate);

            return Result.Ok(responseDto);
        }

        private Result<UpdateVideoResponseDto> VideoNotFoundError(UpdateVideoRequestDto request)
        {
            string errorMsg = string.Format(
                ErrorMessages.EntityByIdNotFound,
                typeof(VideoEntity).Name,
                request.Id);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        private Result<UpdateVideoResponseDto> FailedToUpdateVideoError(UpdateVideoRequestDto request)
        {
            string errorMsg = string.Format(
                ErrorMessages.UpdateFailed,
                typeof(VideoEntity).Name,
                request.Id);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }
    }
}
