using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.ReposInterfaces;
using Educational_Platform.Domain.Exceptions;
using Educational_Platform.Infrastructure.Implementations.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Educational_Platform.Infrastructure.Implementations.Repos
{
	public class AsyncQueries<TEntity>(
		AppDbContext appDbContext,
		ILogger<AsyncRepository<TEntity>> log,
		bool isTracking) : IAsyncQueries<TEntity> where TEntity : class
	{

		private IQueryable<TEntity> _query = appDbContext.Set<TEntity>();
		private readonly AppDbContext _appDbContext = appDbContext;
		private readonly ILogger<AsyncRepository<TEntity>> _log = log;
		private readonly bool _isTracking = isTracking;
		protected readonly string _notExistErrorMessage = $"{typeof(TEntity).Name} Is Not Exist!";
		public IQueryable<TEntity> Query => _query;

		public async ValueTask<bool> IsExistAsync(Expression<Func<TEntity, bool>> expression)
		{
			await CheckDatabaseConnectionAsync();
			MakeNewQuery();

			return _query.Any(expression);
		}

		public async ValueTask<bool> IsExistAsync(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, string>> select, string compareWith, StringComparison comparison)
		{
			await CheckDatabaseConnectionAsync();
			MakeNewQuery();

			var res = _query.Where(expression).Select(select).FirstOrDefault();
			if (res is null)
				return false;

			if (!res.Equals(compareWith, comparison))
				return false;

			return true;
		}

		public bool IsExist(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, string>> select, string compareWith, StringComparison comparison, out TEntity? entity)
		{
			CheckDatabaseConnectionAsync().Wait();
			MakeNewQuery();

			var res = _query.Where(expression);
			var resString = res.Select(select).FirstOrDefault();
			entity = null;
			if (resString is null)
				return false;

			if (!resString.Equals(compareWith, comparison))
				return false;

			entity = res.FirstOrDefault();
			return true;
		}
		public async ValueTask<TEntity> ReadAsync<TProperty>(
			Expression<Func<TEntity, bool>> predicate,
			Expression<Func<TEntity, TProperty>>[]? include = null)
		{
			await CheckDatabaseConnectionAsync();
			MakeNewQuery();

			if (!await IsExistAsync(predicate))
				throw new NotExistException(_notExistErrorMessage);

			var query = _query;
			if (include is not null)
				foreach (var item in include)
					query = query.Include(item);

			return query.First(predicate);
		}

		public async ValueTask<TSelected> ReadAsync<TSelected, TProperty>(
			Expression<Func<TEntity, bool>> expression,
			Expression<Func<TEntity, TSelected>> select,
			Expression<Func<TEntity, TProperty>>[]? include = null)
		{
			await CheckDatabaseConnectionAsync();
			MakeNewQuery();

			if (!await IsExistAsync(expression))
				throw new NotExistException(_notExistErrorMessage);

			var query = _query.Where(expression);
			if (include is not null)
				foreach (var item in include)
					query = query.Include(item);

			var selected = query.Select(select).FirstOrDefault() ?? throw new NotExistException(); ;
			return selected;
		}

		public async ValueTask<TSelected> ReadAsync<TSelected>(
			Expression<Func<TEntity, bool>> expression,
			Expression<Func<TEntity, TSelected>> select)
		{
			await CheckDatabaseConnectionAsync();
			MakeNewQuery();

			if (!await IsExistAsync(expression))
				throw new NotExistException(_notExistErrorMessage);

			var record = _query.Where(expression).Select(select).FirstOrDefault() ?? throw new NotExistException();
			return record;
		}

		public async ValueTask<TEntity> ReadAsync(Expression<Func<TEntity, bool>> expression)
		{
			await CheckDatabaseConnectionAsync();
			MakeNewQuery();

			if (!await IsExistAsync(expression))
				throw new NotExistException(_notExistErrorMessage);

			var record = _query.Where(expression).FirstOrDefault() ?? throw new NotExistException();
			return record;
		}

		public async ValueTask<IEnumerable<TEntity>> ReadAllAsync()
		{
			await CheckDatabaseConnectionAsync();
			MakeNewQuery();

			return _query.ToList();
		}

		public async ValueTask<IEnumerable<TSelected>> ReadAllAsync<TSelected, TProperty>(
			Expression<Func<TEntity, TSelected>> select,
			Expression<Func<TEntity, TProperty>>[]? include = null)
		{
			await CheckDatabaseConnectionAsync();
			MakeNewQuery();

			var query = _query;
			if (include is not null)
				foreach (var item in include)
					query = query.Include(item);

			return query.Select(select).ToList();
		}

		public async ValueTask<IEnumerable<TSelected>> ReadAllAsync<TSelected, TProperty>(
			Expression<Func<TEntity, bool>> expression,
			Expression<Func<TEntity, TSelected>> select,
			Expression<Func<TEntity, TProperty>>[]? include = null)
		{
			await CheckDatabaseConnectionAsync();
			MakeNewQuery();

			var query = _query.Where(expression);
			if (include is not null)
				foreach (var item in include)
					query = query.Include(item);

			return query.Select(select).ToList();
		}

		public async ValueTask<IEnumerable<TSelected>> ReadAllAsync<TSelected>(
			Expression<Func<TEntity, TSelected>> select)
		{
			await CheckDatabaseConnectionAsync();
			MakeNewQuery();

			return _query.Select(select).ToList();
		}

		public async ValueTask<IEnumerable<TEntity>> ReadAllAsync(
			Expression<Func<TEntity, bool>> expression)
		{
			await CheckDatabaseConnectionAsync();
			MakeNewQuery();

			return _query.Where(expression).ToList();
		}

		public async ValueTask<IEnumerable<TSelected>> ReadAllAsync<TSelected>(
			Expression<Func<TEntity, bool>> expression,
			Expression<Func<TEntity, TSelected>> select)
		{
			await CheckDatabaseConnectionAsync();
			MakeNewQuery();

			return _query.Where(expression).Select(select).ToList();
		}

		public async ValueTask<IEnumerable<TEntity>> ReadAllAsync<TProperty>(Expression<Func<TEntity, TProperty>>[] include)
		{
			await CheckDatabaseConnectionAsync();
			MakeNewQuery();

			var query = _query;
			foreach (var item in include)
				query = query.Include(item);

			return query.ToList();
		}

		public async ValueTask<IEnumerable<TEntity>> ReadAllAsync<TProperty>(
			Expression<Func<TEntity, bool>> predicate,
			Expression<Func<TEntity, TProperty>>[]? include = null)
		{
			await CheckDatabaseConnectionAsync();
			MakeNewQuery();

			var query = _query.Where(predicate);
			if (include is not null)
				foreach (var item in include)
					query = query.Include(item);

			return query.ToList();
		}



		private async Task CheckDatabaseConnectionAsync()
		{
			if (!await _appDbContext.Database.CanConnectAsync())
			{
				_log.LogCritical("DataBase Is Down");
				throw new DataBaseDownException("Service Not Available");
			}
		}

		/// <summary>
		/// Asign New IQuerable Object To _query Variable
		/// </summary>
		private void MakeNewQuery()
		{
			if (_isTracking)
				_query = _appDbContext.Set<TEntity>();
			else
				_query = _appDbContext.Set<TEntity>().AsNoTracking();
		}


	}
}
