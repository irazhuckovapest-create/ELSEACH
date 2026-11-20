using EF_DILesson.Contracts;
using FluentValidation;

namespace EF_DILesson.Validators
{
    public class FilmFilterRequestValidator : AbstractValidator<FilmFilterRequest>
    {
        public FilmFilterRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id має бути більше 0")
                .When(x => x.Id.HasValue);

            RuleFor(x => x.Year)
                .InclusiveBetween(1888, DateTime.Now.Year + 1)
                .WithMessage("Рік випуску має бути від 1888 до " + (DateTime.Now.Year + 1))
                .When(x => x.Year.HasValue);

            RuleFor(x => x.Title).MaximumLength(100);
            RuleFor(x => x.Genre).MaximumLength(50);
            RuleFor(x => x.Country).MaximumLength(50);
            RuleFor(x => x.Keyword).MaximumLength(100);
        }
    }
}