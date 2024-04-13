﻿using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Update;

public class UpdateTextRequestDtoValidator : AbstractValidator<UpdateTextRequestDto>
{
    private const int MAXTITLE = 50;
    private const int MAXTEXTCONTENT = 15000;
    private const int MAXADDITIONALTEXT = 200;

    public UpdateTextRequestDtoValidator()
    {
        RuleFor(dto => dto.Title)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(MAXTITLE);

        RuleFor(dto => dto.TextContent)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(MAXTEXTCONTENT);

        RuleFor(dto => dto.AdditionalText)
            .MaximumLength(MAXADDITIONALTEXT);
    }
}