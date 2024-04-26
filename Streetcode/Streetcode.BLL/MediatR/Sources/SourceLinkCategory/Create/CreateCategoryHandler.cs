using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.Sources;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<SourceLinkCategoryDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public CreateCategoryHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<SourceLinkCategoryDto>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var request = command.Category;

        if (!await ImageExistsAsync(request.ImageId))
        {
            return ImageNotFoundError(request);
        }

        if (!await StreetcodeExistsAsync(request.StreetcodeId))
        {
            return StreetcodeNotFoundError(request);
        }

        var newCategory = _mapper.Map<DAL.Entities.Sources.SourceLinkCategory>(request);

        _repositoryWrapper.SourceCategoryRepository.Create(newCategory);

        await _repositoryWrapper.SaveChangesAsync();

        var streetcodeCategoryContent = new StreetcodeCategoryContent()
        {
            StreetcodeId = request.StreetcodeId,
            SourceLinkCategoryId = newCategory.Id,
            Text = request.Text
        };

        newCategory.StreetcodeCategoryContents.Add(streetcodeCategoryContent);

        var isSuccessful = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (isSuccessful)
        {
            var categoryDto = _mapper.Map<SourceLinkCategoryDto>(newCategory);

            return Result.Ok(categoryDto);
        }
        else
        {
            return FailedToCreateCategoryError(request);
        }
    }

    private Result<SourceLinkCategoryDto> FailedToCreateCategoryError(CreateCategoryRequestDto request)
    {
        string errorMsg = string.Format(
                ErrorMessages.CreateFailed,
                nameof(SourceLinkCategory));
        _logger.LogError(request, errorMsg);

        return Result.Fail(errorMsg);
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

    private Result<SourceLinkCategoryDto> ImageNotFoundError(CreateCategoryRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            nameof(Image),
            request.ImageId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<SourceLinkCategoryDto> StreetcodeNotFoundError(CreateCategoryRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            nameof(StreetcodeContent),
            request.StreetcodeId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}
