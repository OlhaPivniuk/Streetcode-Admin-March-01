using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;
using ImageEntity = Streetcode.DAL.Entities.Media.Images.Image;
using StreetcodeArtEntity = Streetcode.DAL.Entities.Streetcode.StreetcodeArt;

namespace Streetcode.BLL.MediatR.Media.Art.Create;

public class CreateArtHandler : IRequestHandler<CreateArtCommand, Result<CreateArtResponseDto>>
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

    public async Task<Result<CreateArtResponseDto>> Handle(CreateArtCommand command, CancellationToken cancellationToken)
    {
        using var transaction = _repositoryWrapper.BeginTransaction();

        var request = command.CreateArtRequestDto;

        if (!await ImageExistsAsync(request.ImageId))
        {
            return ImageNotFoundError(request);
        }

        if (await ArtWithThisImageAlreadyExists(request.ImageId))
        {
            return ArtExistsError(request);
        }

        if (!await StreetcodeExistsAsync(request.StreetcodeId))
        {
            return StreetcodeNotFoundError(request);
        }

        var newArt = _mapper.Map<ArtEntity>(request);

        _repositoryWrapper.ArtRepository.Create(newArt);

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return FailedToCreateArtError(request);
        }

        var streetcodeArt = new StreetcodeArtEntity()
        {
            StreetcodeId = request.StreetcodeId,
            ArtId = newArt.Id,
            Index = await CalculateIndexAsync(request.StreetcodeId),
        };

        if (!await IsPrimarKeyUnique(request.StreetcodeId, newArt.Id))
        {
            return PrimaryKeyIsNotUniqueError(request);
        }

        _repositoryWrapper.StreetcodeArtRepository.Create(streetcodeArt);

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return FailedToCreateStreetcodeArtError(request);
        }

        transaction.Complete();

        var artResponseDto = new CreateArtResponseDto(
            Id: newArt.Id,
            Description: request.Description,
            Title: request.Title,
            ImageId: request.ImageId,
            StreetcodeId: request.StreetcodeId);

        return Result.Ok(artResponseDto);
    }

    private async Task<bool> ArtWithThisImageAlreadyExists(int imageId)
    {
        var art = await _repositoryWrapper.ArtRepository
            .GetFirstOrDefaultAsync(a => a.ImageId == imageId);

        return art != null;
    }

    private async Task<int> CalculateIndexAsync(int streetcodeId)
    {
        var streetcodeArts = await _repositoryWrapper.StreetcodeArtRepository.GetAllAsync(
            predicate: streetcodeArt => streetcodeArt.StreetcodeId == streetcodeId);

        return streetcodeArts != null ?
            streetcodeArts.Max(sa => sa.Index) + 1 :
            1;
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

    private async Task<bool> IsPrimarKeyUnique(int streetcodeId, int artId)
    {
        var streetcodeArt = await _repositoryWrapper.StreetcodeArtRepository
            .GetFirstOrDefaultAsync(st => st.StreetcodeId == streetcodeId && st.ArtId == artId);

        return streetcodeArt is null;
    }

    private Result<CreateArtResponseDto> PrimaryKeyIsNotUniqueError(CreateArtRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.PrimaryKeyIsNotUnique,
            typeof(StreetcodeArtEntity).Name);
        _logger.LogError(request, errorMsg);

        return Result.Fail(errorMsg);
    }

    private Result<CreateArtResponseDto> ImageNotFoundError(CreateArtRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(ImageEntity).Name,
            request.ImageId);
        _logger.LogError(request, errorMsg);

        return Result.Fail(errorMsg);
    }

    private Result<CreateArtResponseDto> StreetcodeNotFoundError(CreateArtRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StreetcodeContent).Name,
            request.StreetcodeId);
        _logger.LogError(request, errorMsg);

        return Result.Fail(errorMsg);
    }

    private Result<CreateArtResponseDto> FailedToCreateArtError(CreateArtRequestDto request)
    {
        string errorMsg = string.Format(
                ErrorMessages.CreateFailed,
                typeof(ArtEntity).Name);
        _logger.LogError(request, errorMsg);

        return Result.Fail(errorMsg);
    }

    private Result<CreateArtResponseDto> FailedToCreateStreetcodeArtError(CreateArtRequestDto request)
    {
        string errorMsg = string.Format(
                ErrorMessages.CreateFailed,
                typeof(StreetcodeArtEntity).Name);
        _logger.LogError(request, errorMsg);

        return Result.Fail(errorMsg);
    }

    private Result<CreateArtResponseDto> ArtExistsError(CreateArtRequestDto request)
    {
        string errorMsg = $"Art with image (Id: {request.ImageId}) already exists.";
        _logger.LogError(request, errorMsg);

        return Result.Fail(errorMsg);
    }
}
