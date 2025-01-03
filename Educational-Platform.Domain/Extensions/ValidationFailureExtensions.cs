using FluentValidation.Results;

namespace Educational_Platform.Domain.Extensions
{
    public static class ValidationFailureExtensions
    {
        public static string GetAllErrorsMessages(this IList<ValidationFailure> failures)
        {
            return string.Join(',', failures.Select(x => $"{x.ErrorMessage}"));
        }
    }
}
