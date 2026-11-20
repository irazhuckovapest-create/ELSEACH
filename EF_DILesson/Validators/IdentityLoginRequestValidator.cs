using EF_DILesson.Contracts;
using FluentValidation;

namespace EF_DILesson.Validators
{
    public class IdentityLoginRequestValidator : AbstractValidator<IdentityLoginRequest>
    {
        public IdentityLoginRequestValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(100);
        }
    }
}