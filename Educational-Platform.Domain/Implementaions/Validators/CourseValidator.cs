using FluentValidation;
using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Domain.Implementaions.Validators
{
    public class CourseValidator : AbstractValidator<Course>
    {
        public CourseValidator()
        {
            RuleFor(c => c.Price).NotEmpty();
            RuleFor(c => c.Duration).NotEmpty();
            RuleFor(c => c.Name).NotEmpty();
        }
    }
}
