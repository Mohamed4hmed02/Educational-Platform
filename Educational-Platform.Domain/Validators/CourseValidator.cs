using Educational_Platform.Domain.Abstractions;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Exceptions;
using Educational_Platform.Domain.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Educational_Platform.Domain.Validators
{
    public class CourseValidator : AbstractValidator<Course>, IEntityValidator<Course>
    {
        public CourseValidator()
        {
            RuleFor(c => c.Price).NotEmpty();
            RuleFor(c => c.Duration).NotEmpty();
            RuleFor(c => c.Name).NotEmpty();
        }

        public void ValidateEntity(Course entity)
        {
            var res = Validate(entity);
            if (!res.IsValid)
            {
                throw new InvalidDataException(res.Errors.GetAllErrorsMessages())
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);
            }
        }

        public void ValidateEntity(IEnumerable<Course> entities)
        {
            foreach (var item in entities)
            {
                ValidateEntity(item);
            }
        }
    }
}
