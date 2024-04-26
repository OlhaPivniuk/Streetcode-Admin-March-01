using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Analytics.StatisticRecord;

namespace Streetcode.BLL.MediatR.Analytics.StatisticRecord.Delete;

public sealed record DeleteStatisticRecordCommand(DeleteStatisticRecordRequestDto Request) :
    IRequest<Result<DeleteStatisticRecordResponseDto>>;