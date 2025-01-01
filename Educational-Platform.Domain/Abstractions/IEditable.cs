namespace Educational_Platform.Domain.Abstractions
{
	/// <summary>
	/// Contains two methods Create,Update for a specific class type
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IEditable<T> where T : class
	{
		ValueTask CreateAsync(T entity);
		ValueTask UpdateAsync(T entity);
	}
}
