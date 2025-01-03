using Educational_Platform.Domain.Abstractions;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Exceptions;
using Educational_Platform.Domain.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Educational_Platform.Domain.Validators
{
    public class UserValidator : AbstractValidator<User>, IEntityValidator<User>
    {
        public UserValidator()
        {
            RuleFor(u => u.Email).EmailAddress().NotEmpty();
            RuleFor(u => u.FirstName).MaximumLength(25).NotEmpty();
            RuleFor(u => u.LastName).MaximumLength(25).NotEmpty();
            RuleFor(u => u.DateOfBirth).NotEmpty();
            RuleFor(u => u.Password).NotEmpty();
        }

        public void ValidateEntity(User entity)
        {
            var res = Validate(entity);
            if (!res.IsValid)
            {
                throw new InvalidDataException(res.Errors.GetAllErrorsMessages())
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);
            }
        }

        public void ValidateEntity(IEnumerable<User> entities)
        {
            foreach (var item in entities)
            {
                ValidateEntity(item);
            }
        }
    }
}
