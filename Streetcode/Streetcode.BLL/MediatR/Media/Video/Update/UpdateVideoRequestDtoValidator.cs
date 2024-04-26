using FluentValidation;
using Streetcode.BLL.DTO.Media.Video;

namespace Streetcode.BLL.MediatR.Media.Video.Update
{
    public class UpdateVideoRequestDtoValidator : AbstractValidator<UpdateVideoRequestDto>
    {
        private const int MAXTITLELENGTH = 50;
        private const int MAXDESCRPTIONLENGTH = 500;

        public UpdateVideoRequestDtoValidator()
        {
            RuleFor(dto => dto.Id)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(dto => dto.Title)
                .MaximumLength(MAXTITLELENGTH);

            RuleFor(dto => dto.Description)
                .MaximumLength(MAXDESCRPTIONLENGTH);

            RuleFor(dto => dto.Url)
                .Must(IsValidYoutubeUrl)
                .WithMessage("Only youtube.com links are accepted.");
        }

        private bool IsValidYoutubeUrl(string url)
        {
            return url.Contains("youtube.com");
        }
    }
}
