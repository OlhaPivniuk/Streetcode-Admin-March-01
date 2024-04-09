﻿using Streetcode.BLL.Dto.Timeline;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.DTO.Timeline;

public sealed record CreateTimelineItemRequestDto(string Title, string? Description, DateTime Date, DateViewPattern DateViewPattern, IEnumerable<HistoricalContextDto> HistoricalContexts, int StreetcodeId);