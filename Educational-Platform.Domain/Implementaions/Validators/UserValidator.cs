using FluentValidation;
using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Domain.Implementaions.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(u => u.Email).EmailAddress().NotEmpty();
            RuleFor(u => u.FirstName).MaximumLength(25).NotEmpty();
            RuleFor(u => u.LastName).MaximumLength(25).NotEmpty();
            RuleFor(u => u.DateOfBirth).NotEmpty();
            RuleFor(u => u.Password).NotEmpty();
        }
    }
}
