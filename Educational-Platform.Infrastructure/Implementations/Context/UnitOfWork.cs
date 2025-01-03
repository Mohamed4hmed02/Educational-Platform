using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.Infrastructure.SpecialRepos;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.ReposInterfaces;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Exceptions;
using Educational_Platform.Domain.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Infrastructure.Implementations.Context
{
    public class UnitOfWork(
		AppDbContext dbContext,
		IRepositoryBase<Admin> adminRepo,
		IBookAsyncRepository bookRepo,
		IRepositoryBase<Course> courseRepo,
		ICartAsyncRepository cartRepo,
		ICartDetailAsyncRepository cartDetailRepo,
		IRepositoryBase<User> userRepo,
		IUserCourseAsyncRepository userCourseRepo,
		ITopicAsyncRepository topicRepo,
		IOldUserCoursesAsyncRepository oldUserCoursesRepo,
		IUnitAsyncRepository unitRepo,
        IRepositoryBase<Payment> paymentRepo,
        IRepositoryBase<Order> orderRepo,
        IRepositoryBase<OrderDetail> orderDetailRepo,
        ILogger<UnitOfWork> log) : IUnitOfWork
	{
		public IRepositoryBase<Admin> AdminsRepository { get; } = adminRepo;
		public IRepositoryBase<Payment> PaymentsRepository { get; } = paymentRepo;

        public IBookAsyncRepository BooksRepository { get; } = bookRepo;

		public IRepositoryBase<Course> CoursesRepository { get; } = courseRepo;

		public ICartAsyncRepository CartsRepository { get; } = cartRepo;

		public ICartDetailAsyncRepository CartDetailsRepository { get; } = cartDetailRepo;

		public IRepositoryBase<User> UsersRepository { get; } = userRepo;

		public IUserCourseAsyncRepository UsersCoursesRepository { get; } = userCourseRepo;

		public ITopicAsyncRepository TopicsRepository { get; } = topicRepo;

		public IOldUserCoursesAsyncRepository CoursesUserTakesRepository { get; } = oldUserCoursesRepo;

		public IUnitAsyncRepository UnitsRepository { get; } = unitRepo;

        public IRepositoryBase<Order> OrdersRepository => orderRepo;

        public IRepositoryBase<OrderDetail> OrderDetailsRepository => orderDetailRepo;

        public void Dispose()
		{
			dbContext.Dispose();
			GC.SuppressFinalize(this);
		}

		public async ValueTask SaveChangesAsync()
		{
			await CheckDatabaseConnectionAsync();
			dbContext.SaveChanges();
		}

		public async ValueTask<IEnumerable<TResult>> RawSqlQueryAsync<TResult>(string query, params object[] parameters)
		{
			await CheckDatabaseConnectionAsync();
			try
			{
				return dbContext.Database.SqlQueryRaw<TResult>(query, parameters).ToList();
			}
			catch (Exception ex)
			{
				log.LogError("Error While Performing Sql Query: {Message}", ex.Message);
				throw new Exception()
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status500InternalServerError);
			}
		}

		public async ValueTask RawSqlCommandAsync(string command)
		{
			await CheckDatabaseConnectionAsync();
			try
			{
				dbContext.Database.ExecuteSqlRaw(command);
			}
			catch (Exception ex)
			{
				log.LogError("Error While Performing Sql Command: {Message}", ex.Message);
				throw new Exception()
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status500InternalServerError);
			}
		}

		private async Task CheckDatabaseConnectionAsync()
		{
			if (!await dbContext.Database.CanConnectAsync())
			{
				log.LogCritical("DataBase Is Down");
				throw new DataBaseDownException("Service Not Available");
			}
		}

	}
}
