using Educational_Platform.Domain.Abstractions;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Exceptions;
using Educational_Platform.Domain.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Educational_Platform.Domain.Validators
{
    public class BookValidator : AbstractValidator<Book>, IEntityValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(b => b.Price).NotEmpty();
            RuleFor(b => b.Code).NotEmpty();
            RuleFor(b => b.Quantity).NotEmpty();
        }

        public void ValidateEntity(Book entity)
        {
            var res = Validate(entity);
            if (!res.IsValid)
            {
                throw new InvalidDataException(res.Errors.GetAllErrorsMessages())
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);
            }
        }

        public void ValidateEntity(IEnumerable<Book> entities)
        {
            foreach (var item in entities)
            {
                ValidateEntity(item);
            }
        }
    }
}
