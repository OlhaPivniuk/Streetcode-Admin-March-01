using MediatR;
using Streetcode.BLL.Dto.Sources;
using FluentResults;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;

public record CreateCategoryCommand(CreateCategoryRequestDto Category) : IRequest<Result<SourceLinkCategoryDto>>;