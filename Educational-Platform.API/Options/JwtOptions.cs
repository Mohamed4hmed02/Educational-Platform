namespace Educational_Platform.Application.Options
{
	public class JwtOptions
	{
		public string Issuer { get; set; }
		public string Audience { get; set; }
		public double LifeTime { get; set; }
		public string SigningKey { get; set; }
	}
}
