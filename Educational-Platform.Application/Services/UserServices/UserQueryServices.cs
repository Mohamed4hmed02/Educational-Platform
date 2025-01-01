using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.UserInterfaces;
using Educational_Platform.Application.Models.QueryModels;

namespace Educational_Platform.Application.Services.UserServices
{
	public class UserQueryServices(IUnitOfWork unitOfWork) : IUserQueryServices
	{
		public async ValueTask<QueryUserModel> GetUserAsync(object userFullId)
		{
			var user = await unitOfWork.UsersRepository.AsNoTracking().ReadAsync(u => userFullId.Equals(u.FullId));

			return QueryUserModel.GetModel(user, unitOfWork);
		}
	}
}
