using Educational_Platform.Application.Models.CommonModels;
using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Models.CommandModels
{
	public class RegisterUserModel
	{
		public required string Email { get; set; }
		public required string Password { get; set; }
		public bool IsPasswordChanged { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public DateOnly DateOfBirth { get; set; }
		public string? Phone { get; set; }
		public string? Region { get; set; }
		public bool IsMale { get; set; }
		public string? Language { get; set; }
		public string? EducationQualification { get; set; }
		public string? Specialisation { get; set; }
		public string? MasterDegree { get; set; }
		public IEnumerable<OldUserCourseModel>? OldUserCourseModels { get; set; }

        public static implicit operator User(RegisterUserModel model)
		{
			return new User
			{
				Email = model.Email,
				Password = model.Password,
				FirstName = model.FirstName,
				LastName = model.LastName,
				Phone = model.Phone,
				Region = model.Region,
				DateOfBirth = model.DateOfBirth,
				IsMale = model.IsMale,
				Language = model.Language,
				EducationQualification = model.EducationQualification,
				MasterDegree = model.MasterDegree,
				Specialization = model.Specialisation
			};
		}
	}
}
