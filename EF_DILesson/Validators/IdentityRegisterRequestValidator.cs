using EF_DILesson.Contracts;
using FluentValidation;

namespace EF_DILesson.Validators
{
    public class IdentityRegisterRequestValidator : AbstractValidator<IdentityRegisterRequest>
    {
        public IdentityRegisterRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty()
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");
        }
    }
}