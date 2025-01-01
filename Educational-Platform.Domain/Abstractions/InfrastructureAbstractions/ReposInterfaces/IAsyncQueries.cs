using System.Linq.Expressions;

namespace Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.ReposInterfaces
{
	public interface IAsyncQueries<TEntity> where TEntity : class
	{
		public IQueryable<TEntity> Query { get; }

		/// <summary>
		/// If Entity Is Not Exist It Throws NotExistException
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		ValueTask<TEntity> ReadAsync(Expression<Func<TEntity, bool>> expression);

		/// <summary>
		/// If Entity Is Not Exist It Throws NotExistException
		/// </summary>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="expression"></param>
		/// <param name="include"></param>
		/// <returns></returns>
		ValueTask<TEntity> ReadAsync<TProperty>(
			Expression<Func<TEntity, bool>> expression,
			Expression<Func<TEntity, TProperty>>[]? include = null);

		/// <summary>
		/// If Entity Is Not Exist It Throws NotExistException
		/// </summary>
		/// <typeparam name="TSelected"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="expression"></param>
		/// <param name="select"></param>
		/// <param name="include"></param>
		/// <returns></returns>
		ValueTask<TSelected> ReadAsync<TSelected, TProperty>(
			Expression<Func<TEntity, bool>> expression,
			Expression<Func<TEntity, TSelected>> select,
			Expression<Func<TEntity, TProperty>>[]? include = null);

		/// <summary>
		/// If Entity Is Not Exist It Throws NotExistException
		/// </summary>
		/// <typeparam name="TSelected"></typeparam>
		/// <param name="expression"></param>
		/// <param name="select"></param>
		/// <returns></returns>
		ValueTask<TSelected> ReadAsync<TSelected>(
			Expression<Func<TEntity, bool>> expression,
			Expression<Func<TEntity, TSelected>> select);

		ValueTask<bool> IsExistAsync(Expression<Func<TEntity, bool>> expression);

		/// <summary>
		/// Return False If Value Not Exist In Database Orr When Compare It Return False
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="select"></param>
		/// <param name="compareWith"></param>
		/// <param name="comparison"></param>
		/// <returns></returns>
		ValueTask<bool> IsExistAsync(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, string>> select, string compareWith, StringComparison comparison);

		/// <summary>
		/// Return False If Value Not Exist In Database Orr When Compare It Return False
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="select"></param>
		/// <param name="compareWith"></param>
		/// <param name="comparison"></param>
		/// <param name="entity"></param>
		/// <returns></returns>
		bool IsExist(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, string>> select, string compareWith, StringComparison comparison ,out TEntity? entity);

		ValueTask<IEnumerable<TEntity>> ReadAllAsync();

		ValueTask<IEnumerable<TEntity>> ReadAllAsync<TProperty>(
			Expression<Func<TEntity, TProperty>>[] include);

		ValueTask<IEnumerable<TEntity>> ReadAllAsync(
			Expression<Func<TEntity, bool>> expression);

		ValueTask<IEnumerable<TSelected>> ReadAllAsync<TSelected>(
			Expression<Func<TEntity, TSelected>> select);

		ValueTask<IEnumerable<TSelected>> ReadAllAsync<TSelected>(
			Expression<Func<TEntity, bool>> expression,
			Expression<Func<TEntity, TSelected>> select);

		ValueTask<IEnumerable<TSelected>> ReadAllAsync<TSelected, TProperty>(
			Expression<Func<TEntity, TSelected>> select,
			Expression<Func<TEntity, TProperty>>[]? include = null);

		ValueTask<IEnumerable<TEntity>> ReadAllAsync<TProperty>(
			Expression<Func<TEntity, bool>> expression,
			Expression<Func<TEntity, TProperty>>[]? include = null);

		ValueTask<IEnumerable<TSelected>> ReadAllAsync<TSelected, TProperty>(
			Expression<Func<TEntity, bool>> expression,
			Expression<Func<TEntity, TSelected>> select,
			Expression<Func<TEntity, TProperty>>[]? include = null);
	}
}
