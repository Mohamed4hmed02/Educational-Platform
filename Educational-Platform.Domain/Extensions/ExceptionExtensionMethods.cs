namespace Educational_Platform.Domain.Extensions
{
    public static class ExceptionExtensionMethods
    {
        public static Exception AddToExceptionData(
            this Exception ex,
            object key,
            object value)
        {
            ex.Data.Add(key, value);
            return ex;
        }
    }
}
