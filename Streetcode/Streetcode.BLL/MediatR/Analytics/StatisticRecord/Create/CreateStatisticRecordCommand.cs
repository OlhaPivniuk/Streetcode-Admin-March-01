using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Analytics.StatisticRecord;

namespace Streetcode.BLL.MediatR.Analytics.StatisticRecord.Create;

public sealed record CreateStatisticRecordCommand(CreateStatisticRecordRequestDto Request) :
    IRequest<Result<CreateStatisticRecordResponseDto>>;
