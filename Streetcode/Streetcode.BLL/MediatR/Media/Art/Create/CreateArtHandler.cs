using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.Art.Create;

public class CreateArtHandler : IRequestHandler<CreateArtCommand, Result<ArtDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public CreateArtHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ArtDto>> Handle(CreateArtCommand command, CancellationToken cancellationToken)
    {
        var request = command.ArtDto;

        if (!await ImageExistsAsync(request.ImageId))
        {
            return ImageNotFoundError(request);
        }

        if (!await StreetcodeExistsAsync(request.StreetcodeId))
        {
            return StreetcodeNotFoundError(request);
        }

        var newArt = _mapper.Map<DAL.Entities.Media.Images.Art>(request);

        _repositoryWrapper.ArtRepository.Create(newArt);

        await _repositoryWrapper.SaveChangesAsync();

        var streetcodeArt = new DAL.Entities.Streetcode.StreetcodeArt()
        {
            StreetcodeId = request.StreetcodeId,
            ArtId = newArt.Id,
        };

        newArt.StreetcodeArts.Add(streetcodeArt);

        var isSuccessful = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (isSuccessful)
        {
            var artDto = _mapper.Map<ArtDto>(newArt);

            return Result.Ok(artDto);
        }
        else
        {
            return FailedToCreateArtError(request);
        }
    }

    private async Task<bool> StreetcodeExistsAsync(int streetcodeId)
    {
        var streetcode = await _repositoryWrapper.StreetcodeRepository
            .GetFirstOrDefaultAsync(s => s.Id == streetcodeId);

        return streetcode is not null;
    }

    private async Task<bool> ImageExistsAsync(int imageId)
    {
        var image = await _repositoryWrapper.ImageRepository
            .GetFirstOrDefaultAsync(i => i.Id == imageId);

        return image is not null;
    }

    private Result<ArtDto> ImageNotFoundError(CreateArtDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            nameof(Image),
            request.ImageId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<ArtDto> StreetcodeNotFoundError(CreateArtDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            nameof(StreetcodeContent),
            request.StreetcodeId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<ArtDto> FailedToCreateArtError(CreateArtDto request)
    {
        string errorMsg = string.Format(
                ErrorMessages.CreateFailed,
                nameof(Art));
        _logger.LogError(request, errorMsg);

        return Result.Fail(errorMsg);
    }
}
