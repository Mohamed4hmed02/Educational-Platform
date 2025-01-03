namespace Educational_Platform.Domain.Abstractions
{
	public interface IEntityValidator<TEntity> where TEntity : class
	{
		/// <summary>
		/// Throws Exception If Not Valid
		/// </summary>
		/// <param name="entity"></param>
		void ValidateEntity(TEntity entity);
		/// <summary>
		/// Throws Exception If Not Valid
		/// </summary>
		/// <param name="entities"></param>
		void ValidateEntity(IEnumerable<TEntity> entities);
	}
}
