using Educational_Platform.Application.Abstractions.Infrastructure.SpecialRepos;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.ReposInterfaces;
using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Abstractions.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryBase<Admin> AdminsRepository { get; }
        IRepositoryBase<Payment> PaymentsRepository { get; }
        IRepositoryBase<Order> OrdersRepository { get; }
        IRepositoryBase<OrderDetail> OrderDetailsRepository { get; }
        IBookAsyncRepository BooksRepository { get; }
        IRepositoryBase<Course> CoursesRepository { get; }
        ICartAsyncRepository CartsRepository { get; }
        ICartDetailAsyncRepository CartDetailsRepository { get; }
        IRepositoryBase<User> UsersRepository { get; }
        IOldUserCoursesAsyncRepository CoursesUserTakesRepository { get; }
        IUserCourseAsyncRepository UsersCoursesRepository { get; }
        ITopicAsyncRepository TopicsRepository { get; }
        IUnitAsyncRepository UnitsRepository { get; }
        ValueTask SaveChangesAsync();
        ValueTask<IEnumerable<TResult>> RawSqlQueryAsync<TResult>(string query, params object[] parameters);
        ValueTask RawSqlCommandAsync(string command);
    }
}
