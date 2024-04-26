using FluentValidation;
using Streetcode.BLL.DTO.Media.Video;

namespace Streetcode.BLL.MediatR.Media.Video.Delete;

public class DeleteVideoRequestDtoValidator : AbstractValidator<DeleteVideoRequestDto>
{
    public DeleteVideoRequestDtoValidator()
    {
        RuleFor(dto => dto.Id)
            .GreaterThan(0);
    }
}