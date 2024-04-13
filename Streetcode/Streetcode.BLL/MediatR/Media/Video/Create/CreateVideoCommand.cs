﻿using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Video;

namespace Streetcode.BLL.MediatR.Media.Video.Create
{
    public record CreateVideoCommand(CreateVideoRequestDto Request) : IRequest<Result<CreateVideoRequestDto>>;
}