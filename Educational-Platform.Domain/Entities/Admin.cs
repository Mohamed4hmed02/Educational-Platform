namespace Educational_Platform.Domain.Entities
{
	public class Admin
	{
		public required string UserName { get; set; }
		public required string Password { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime? RefreshTokenExpireDate { get; set; }
		public bool IsTokenExpired => RefreshToken is null || RefreshTokenExpireDate is null || RefreshTokenExpireDate <= DateTime.UtcNow;
	}
}
