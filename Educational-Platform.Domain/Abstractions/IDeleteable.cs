namespace Educational_Platform.Domain.Abstractions
{
	/// <summary>
	/// Make a child delete able 
	/// </summary>
	/// <typeparam name="TType"></typeparam>
	public interface IDeleteable<TType> : IDeleteable
	{
		ValueTask DeleteAsync(object Id);
	}

	/// <summary>
	/// Mark up interface
	/// </summary>
	public interface IDeleteable
	{
	}
}
