using System.Linq.Expressions;

namespace Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.ReposInterfaces
{
	public interface IAsyncRepositoryBase<TEntity> : IAsyncQueries<TEntity> where TEntity : class
	{
		/// <summary>
		/// Query Must Be On The Same Table
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		ValueTask<IEnumerable<TEntity>> RawSqlQueryAsync(string query);

		/// <summary>
		/// If Entity Is Not Exist It Throws NotExistException
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		ValueTask<TEntity> ReadAsync(object id);

		ValueTask<TEntity> CreateAsync(TEntity record);

		ValueTask CreateAsync(IEnumerable<TEntity> records);

		ValueTask UpdateAsync(TEntity record);

		/// <summary>
		/// If Entity Is Not Exist It Throws NotExistException
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		ValueTask<TEntity> DeleteAsync(object id);

		
		ValueTask DeleteAsync(IEnumerable<TEntity> entities);

		/// <summary>
		/// If Entity Is Not Exist It Throws NotExistException
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		ValueTask<TEntity> DeleteAsync(Expression<Func<TEntity, bool>> expression);

		IAsyncQueries<TEntity> AsNoTracking();
	}
}
