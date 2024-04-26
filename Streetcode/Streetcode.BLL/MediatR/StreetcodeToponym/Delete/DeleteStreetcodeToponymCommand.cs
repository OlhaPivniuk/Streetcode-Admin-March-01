using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.StreetcodeToponym;

namespace Streetcode.BLL.MediatR.StreetcodeToponym.Delete;

public sealed record DeleteStreetcodeToponymCommand(DeleteStreetcodeToponymRequestDto Request) :
    IRequest<Result<DeleteStreetcodeToponymResponseDto>>;