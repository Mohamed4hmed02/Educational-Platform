namespace Educational_Platform.Application.Extensions
{
    public static class EnumerablesExtensions
    {
        /// <summary>
        /// Check wheather an array is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }
    }
}
