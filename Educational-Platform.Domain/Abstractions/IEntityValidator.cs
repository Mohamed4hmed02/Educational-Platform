namespace Educational_Platform.Domain.Abstractions
{
	public interface IEntityValidator<TEntity> where TEntity : class
	{
		/// <summary>
		/// Throws Exception If Not Valid
		/// </summary>
		/// <param name="entity"></param>
		void Validate(TEntity entity);
		/// <summary>
		/// Throws Exception If Not Valid
		/// </summary>
		/// <param name="entities"></param>
		void Validate(IEnumerable<TEntity> entities);
	}
}
