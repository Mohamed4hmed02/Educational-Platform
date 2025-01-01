namespace Educational_Platform.Domain.Entities
{
	public class User
	{
		public int Id { get; set; }
		public string FullId { set; get; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateOnly DateOfBirth { get; set; }
		public string? Phone { get; set; }
		public string? Region { get; set; }
		public string? RefreshToken { get; set; }
		public bool IsMale { get; set; }
		public string? Language { get; set; }
		public string? EducationQualification { get; set; }
		public string? Specialization { get; set; }
		public string? MasterDegree { get; set; }
		public DateTime? RefreshTokenExpireDate { get; set; }

		//Just Query Properties
		public bool IsActive => !IsTokenExpired;
		public bool IsTokenExpired => RefreshToken is null || RefreshTokenExpireDate is null || RefreshTokenExpireDate <= DateTime.UtcNow;
		
		//Navigation Properties
		public IEnumerable<Cart>? Carts { get; set; }
		public IEnumerable<UserCourse>? Courses { get; set; }
		public IEnumerable<OldUserCourse> CoursesTakes { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}
