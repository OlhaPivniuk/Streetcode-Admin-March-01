using Streetcode.BLL.DTO.Media.Video;
using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;

using VideoEntity = Streetcode.DAL.Entities.Media.Video;

namespace Streetcode.BLL.MediatR.Media.Video.Create
{
    public class CreateVideoHandler : IRequestHandler<CreateVideoCommand, Result<CreateVideoRequestDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public CreateVideoHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CreateVideoRequestDto>> Handle(CreateVideoCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var video = _repositoryWrapper.VideoRepository
                .Create(_mapper.Map<VideoEntity>(request));

            bool isSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (!isSuccess)
            {
                var errorMsg = string.Format(
                ErrorMessages.CreateFailed,
                typeof(VideoEntity).Name);
                _logger.LogError(request, errorMsg);

                return Result.Fail(errorMsg);
            }

            return Result.Ok(_mapper.Map<VideoEntity, CreateVideoRequestDto>(video));
        }
    }
}
