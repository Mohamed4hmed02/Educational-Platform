using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Models.CommonModels;
using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Models.QueryModels
{
	public class QueryUserModel
	{
		public required string FullId { set; get; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public required string Email { get; set; }
		public DateOnly DateOfBirth { get; set; }
		public string? Phone { get; set; }
		public string? Region { get; set; }
		public bool IsMale { get; set; }
		public string? Language { get; set; }
		public string? EducationQualification { get; set; }
		public string? Specialization { get; set; }
		public string? MasterDegree { get; set; }
		public IEnumerable<OldUserCourseModel>? OldUserCourseModels { get; set; }

		public static QueryUserModel GetModel(User user, IUnitOfWork unitOfWork)
		{
			return new QueryUserModel
			{
				Email = user.Email,
				Phone = user.Phone,
				FullId = user.FullId,
				FirstName = user.FirstName,
				LastName = user.LastName,
				IsMale = user.IsMale,
				Language = user.Language,
				Specialization = user.Specialization,
				MasterDegree = user.MasterDegree,
				EducationQualification = user.EducationQualification,
				Region = user.Region,
				DateOfBirth = user.DateOfBirth,
				OldUserCourseModels = unitOfWork.CoursesUserTakesRepository.AsNoTracking().ReadAllAsync(c => c.UserId == user.Id).AsTask().Result.Select(c => (OldUserCourseModel)c)
			};
		}
	}
}
