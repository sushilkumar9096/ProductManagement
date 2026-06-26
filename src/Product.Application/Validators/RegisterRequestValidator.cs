using FluentValidation;
using Product.Application.DTOs;

namespace Product.Application.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters.");
            
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
            
            RuleFor(x => x.Role)
                .Must(role => role == "Administrator" || role == "User")
                .WithMessage("Role must be either 'Administrator' or 'User'.");
        }
    }
}
