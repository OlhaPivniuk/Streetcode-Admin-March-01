using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Media.Video;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.Video.GetAll;

public class GetAllVideosHandler : IRequestHandler<GetAllVideosQuery, Result<IEnumerable<VideoDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetAllVideosHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<VideoDto>>> Handle(GetAllVideosQuery request, CancellationToken cancellationToken)
    {
        var videos = await _repositoryWrapper.VideoRepository.GetAllAsync();

        if (videos is null)
        {
            string errorMsg = string.Format(
                ErrorMessages.EntitiesNotFound,
                nameof(DAL.Entities.Media.Video));
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<IEnumerable<VideoDto>>(videos));
    }
}