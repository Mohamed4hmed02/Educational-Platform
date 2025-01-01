using FluentValidation;
using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Domain.Implementaions.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(b => b.Price).NotEmpty();
            RuleFor(b => b.Code).NotEmpty();
            RuleFor(b => b.Quantity).NotEmpty();
        }
    }
}
