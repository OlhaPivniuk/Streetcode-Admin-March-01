using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.Sources;
using Streetcode.BLL.Resources.Errors;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Dto.Sources;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;

public class UpdateCategoryContentHandler : IRequestHandler<UpdateCategoryContentCommand, Result<StreetcodeCategoryContentDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;
    public UpdateCategoryContentHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<StreetcodeCategoryContentDto>> Handle(UpdateCategoryContentCommand command, CancellationToken cancellationToken)
    {
        CategoryContentUpdateDto request = command.CategoryContentUpdateDto;

        if (!await IsStreetcodeExistAsync(request.StreetcodeId))
        {
            return StreetcodeNotFoundError(request);
        }

        StreetcodeCategoryContent content = _mapper.Map<StreetcodeCategoryContent>(request);

        _repositoryWrapper.StreetcodeCategoryContentRepository.Update(content);

        bool isSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!isSuccess)
        {
            return UpdateFailedError(request);
        }

        return Result.Ok(_mapper.Map<StreetcodeCategoryContentDto>(content));
    }

    private async Task<bool> IsStreetcodeExistAsync(int streetcodeId)
    {
        var streetcode = await _repositoryWrapper.StreetcodeRepository.GetFirstOrDefaultAsync(x => x.Id == streetcodeId);

        return streetcode is not null;
    }

    private Result<StreetcodeCategoryContentDto> StreetcodeNotFoundError(CategoryContentUpdateDto request)
    {
        string errorMessage = string.Format(
            ErrorMessages.EntityByCategoryIdNotFound,
            nameof(StreetcodeContent),
            request.SourceLinkCategoryId);

        _logger.LogError(request, errorMessage);

        return Result.Fail(errorMessage);
    }

    private Result<StreetcodeCategoryContentDto> UpdateFailedError(CategoryContentUpdateDto request)
    {
        string errorMessage = string.Format(
            ErrorMessages.UpdateFailed,
            nameof(StreetcodeCategoryContent),
            request.SourceLinkCategoryId);

        _logger.LogError(request, errorMessage);

        return Result.Fail(errorMessage);
    }
}