using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.ReposInterfaces;
using Educational_Platform.Domain.Exceptions;
using Educational_Platform.Domain.Extensions;
using Educational_Platform.Infrastructure.Implementations.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Educational_Platform.Infrastructure.Implementations.Repos
{
    public class AsyncRepository<TEntity>(
		AppDbContext appDbContext,
		ILogger<AsyncRepository<TEntity>> log) :
		AsyncQueries<TEntity>(appDbContext, log, true), IRepositoryBase<TEntity> where TEntity : class
	{
		private readonly DbSet<TEntity> _entity = appDbContext.Set<TEntity>();
		protected readonly AppDbContext _appDbContext = appDbContext;
		private readonly ILogger<AsyncRepository<TEntity>> log = log;

		public IQueries<TEntity> AsNoTracking()
		{
			return new AsyncQueries<TEntity>(_appDbContext, log, false);
		}

		public async ValueTask<TEntity> CreateAsync(TEntity record)
		{
			await CheckDatabaseConnectionAsync();
			await _entity.AddAsync(record);
			return record;
		}

		public async ValueTask<TEntity> DeleteAsync(object id)
		{
			await CheckDatabaseConnectionAsync();
			var record = await ReadAsync(id);
			_entity.Remove(record);
			return record;
		}

		public async ValueTask DeleteAsync(IEnumerable<TEntity> entities)
		{
			await CheckDatabaseConnectionAsync();
			try
			{
				_entity.RemoveRange(entities);
			}
			catch (Exception ex)
			{
				log.LogError("Error occurred while deleting some rows:{errorMessage}", ex.Message);
				throw;
			}

		}

		public async ValueTask<TEntity> DeleteAsync(Expression<Func<TEntity, bool>> expression)
		{
			await CheckDatabaseConnectionAsync();
			var record = await base.ReadAsync(expression);
			_entity.Remove(record);
			return record;
		}

		public async ValueTask UpdateAsync(TEntity record)
		{
			await CheckDatabaseConnectionAsync();
			_entity.Update(record);
		}

		public async ValueTask<TEntity> ReadAsync(object id)
		{
			await CheckDatabaseConnectionAsync();

			var record = _entity.Find(id) ?? throw new NotExistException(_notExistErrorMessage);
			return record;
		}

		public async ValueTask<IEnumerable<TEntity>> RawSqlQueryAsync(string query)
		{
			await CheckDatabaseConnectionAsync();
			return _entity.FromSqlRaw(query).ToArray();
		}

		public async ValueTask CreateAsync(IEnumerable<TEntity> records)
		{
			await CheckDatabaseConnectionAsync();
			_entity.AddRange(records);
		}

		protected async ValueTask CheckDatabaseConnectionAsync()
		{
			if (!await _appDbContext.Database.CanConnectAsync())
			{
				log.LogCritical("DataBase Is Down");
				throw new DataBaseDownException("Service Not Available");
			}
		}

		protected async ValueTask<IEnumerable<TResult>> RawSqlQueryAsync<TResult>(string query, params object[] parameters)
		{
			await CheckDatabaseConnectionAsync();
			try
			{
				return _appDbContext.Database.SqlQueryRaw<TResult>(query, parameters).ToList();
			}
			catch (Exception ex)
			{
				log.LogError("Error While Performing Sql Query: {Message}", ex.Message);
				throw new Exception()
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status500InternalServerError);
			}
		}

		protected async ValueTask RawSqlCommandAsync(string command)
		{
			await CheckDatabaseConnectionAsync();
			try
			{
				_appDbContext.Database.ExecuteSqlRaw(command);
			}
			catch (Exception ex)
			{
				log.LogError("Error While Performing Sql Command: {Message}", ex.Message);
				throw new Exception()
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status500InternalServerError);
			}
		}


	}
}
