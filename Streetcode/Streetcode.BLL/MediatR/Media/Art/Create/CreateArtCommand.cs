using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Media.Art;

namespace Streetcode.BLL.MediatR.Media.Art.Create;

public record CreateArtCommand(CreateArtDto ArtDto) : IRequest<Result<ArtDto>>;