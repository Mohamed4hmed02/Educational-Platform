namespace Educational_Platform.Application.Extensions
{
	public static class PaginationExtensions
	{
		public static IEnumerable<T> GetPageFromList<T>(this IEnumerable<T> lst, int page, int pageSize, int size)
		{
			return lst.Skip(page * pageSize).Take(int.Min(size, pageSize)) ?? [];
		}
		public static IQueryable<T> GetPageFromList<T>(this IQueryable<T> lst, int page, int pageSize, int size)
		{
			return lst.Skip(page * pageSize).Take(int.Min(size, pageSize));
		}
	}
}
