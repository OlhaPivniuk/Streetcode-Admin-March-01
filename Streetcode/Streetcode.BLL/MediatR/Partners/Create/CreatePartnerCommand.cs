using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Partners.Create;

namespace Streetcode.BLL.MediatR.Partners.Create;

public sealed record CreatePartnerCommand(CreatePartnerRequestDto Request) :
    IRequest<Result<CreatePartnerResponseDto>>;