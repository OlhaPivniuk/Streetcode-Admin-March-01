namespace Streetcode.BLL.MediatR.Streetcode.Fact.Reorder;
using FluentResults;
using global::MediatR;
using global::Streetcode.BLL.Dto.Streetcode.TextContent.Fact;

public sealed record ReorderFactCommand(ReorderFactRequestDto Request) : IRequest<Result<ReorderFactResponseDto>>;