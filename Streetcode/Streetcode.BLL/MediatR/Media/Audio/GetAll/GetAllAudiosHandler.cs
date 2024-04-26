using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources.Errors;

namespace Streetcode.BLL.MediatR.Media.Audio.GetAll;

public class GetAllAudiosHandler : IRequestHandler<GetAllAudiosQuery, Result<IEnumerable<AudioDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IBlobService _blobService;
    private readonly ILoggerService _logger;

    public GetAllAudiosHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IBlobService blobService, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _blobService = blobService;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<AudioDto>>> Handle(GetAllAudiosQuery request, CancellationToken cancellationToken)
    {
        var audios = await _repositoryWrapper.AudioRepository.GetAllAsync();

        if (audios is null)
        {
            string errorMsg = string.Format(
                ErrorMessages.EntitiesNotFound,
                nameof(DAL.Entities.Media.Audio));
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var audioDtos = _mapper.Map<IEnumerable<AudioDto>>(audios);

        foreach (var audio in audioDtos)
        {
            try
            {
                audio.Base64 = _blobService.FindFileInStorageAsBase64(audio.BlobName);
            }
            catch (Azure.RequestFailedException ex)
            {
                if (ex.ErrorCode == "BlobNotFound")
                {
                    continue;
                }

                throw;
            }
        }

        return Result.Ok(audioDtos.Where(x => !string.IsNullOrEmpty(x.Base64)).AsEnumerable());
    }
}