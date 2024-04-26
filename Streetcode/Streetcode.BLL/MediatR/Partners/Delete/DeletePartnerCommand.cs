using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Partners.Delete;

namespace Streetcode.BLL.MediatR.Partners.Delete;

public sealed record DeletePartnerCommand(DeletePartnerRequestDto Request) :
    IRequest<Result<DeletePartnerResponseDto>>;