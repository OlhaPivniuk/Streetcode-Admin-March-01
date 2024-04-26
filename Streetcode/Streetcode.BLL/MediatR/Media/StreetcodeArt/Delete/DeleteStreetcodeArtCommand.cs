using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Art;

namespace Streetcode.BLL.MediatR.Media.StreetcodeArt.Delete;

public record DeleteStreetcodeArtCommand(DeleteStreetcodeArtRequestDto DeleteStreetcodeArtRequestDto) : IRequest<Result<DeleteStreetcodeArtResponeDto>>;