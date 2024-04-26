using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Sources;
using Streetcode.BLL.DTO.Sources;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;

public record class UpdateCategoryContentCommand(CategoryContentUpdateDto CategoryContentUpdateDto) : IRequest<Result<StreetcodeCategoryContentDto>>;
