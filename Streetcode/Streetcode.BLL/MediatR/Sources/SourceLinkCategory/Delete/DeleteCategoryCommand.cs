using MediatR;
using FluentResults;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete;

public record DeleteCategoryCommand(int Id) : IRequest<Result<Unit>>;