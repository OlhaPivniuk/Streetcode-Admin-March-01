using FluentValidation;
using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.Create
{
    public class CreateStreetcodeRequestDtoValidator : AbstractValidator<CreateStreetcodeRequestDto>
    {
        private const int MAXTITLELENGTH = 100;
        private const int MAXFIRSTNAMELENGTH = 50;
        private const int MAXLASTNAMELENGTH = 50;
        private const int MAXALIASLENGTH = 33;
        private const int MAXNUMBEROFTAGS = 50;
        private const int MAXTEASERLENGTH = 450;
        private const int MAXPARAGRAPHTEASERLENGTH = 400;
        private const int MAXTRANSLITERATIONURLLENGTH = 100;
        private const int MAXACCEPTANCECRITERIALENGTH = 255;

        public CreateStreetcodeRequestDtoValidator()
        {
            RuleFor(dto => dto.StreetcodeType)
                .IsInEnum()
                .WithMessage("Invalid StreetcodeType.");

            RuleFor(dto => dto.Title)
                .NotEmpty()
                .MaximumLength(MAXTITLELENGTH);

            RuleFor(dto => dto.FirstName)
                .NotEmpty()
                .MaximumLength(MAXFIRSTNAMELENGTH);

            RuleFor(dto => dto.LastName)
                .NotEmpty()
                .MaximumLength(MAXLASTNAMELENGTH);

            RuleFor(dto => dto.EventEndOrPersonDeathDate)
                .Must((dto, endDate) => endDate == null || endDate >= dto.EventStartOrPersonBirthDate)
                .WithMessage("EventEndOrPersonDeathDate cannot be before EventStartOrPersonBirthDate.");

            RuleFor(dto => dto.Alias)
                .NotEmpty()
                .MaximumLength(MAXALIASLENGTH);

            RuleFor(dto => dto.TagIds)
                .Must(t => t.Count() <= MAXNUMBEROFTAGS)
                .WithMessage($"Number of tags must not be more than {MAXNUMBEROFTAGS}.");

            RuleFor(dto => dto.Teaser)
                .NotEmpty()
                .Must(teaser => string.IsNullOrEmpty(teaser) || IsValidTeaser(teaser))
                .WithMessage($"Teaser length of streetcode must be less than {MAXTEASERLENGTH} characters or {MAXPARAGRAPHTEASERLENGTH} characters if it contains a paragraph.");

            RuleFor(request => request.ImageIds)
                .NotEmpty()
                .Must(ids => ids.Count() <= 2)
                .Must(ids =>
                {
                    var expectedCount = ids.Count();
                    var actualCount = ids.Distinct().Count();
                    return expectedCount == actualCount;
                });

            RuleFor(dto => dto.InstagramARLink)
                .MaximumLength(MAXACCEPTANCECRITERIALENGTH);

            RuleFor(dto => dto.InvolvedPeople)
                .MaximumLength(MAXACCEPTANCECRITERIALENGTH);

            RuleFor(dto => dto.TransliterationUrl)
                .NotEmpty()
                .MaximumLength(MAXTRANSLITERATIONURLLENGTH)
                .Matches(@"^[a-z0-9\-]+$");
        }

        private static bool IsValidTeaser(string teaser)
        {
            var limit = teaser.Contains('\n') ? MAXPARAGRAPHTEASERLENGTH : MAXTEASERLENGTH;
            return teaser.Length <= limit;
        }
    }
}
