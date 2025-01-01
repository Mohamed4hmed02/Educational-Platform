using Educational_Platform.Application.Abstractions.SecurityInterfaces;
using System.Security.Cryptography;

namespace Educational_Platform.Application.Aggregates
{
	public class HashingServices : IHashingServices
	{
		private readonly int saltSize = 16;
		private readonly int hashSize = 32;
		private readonly int iterations = 100000;
		private readonly HashAlgorithmName algorithm = HashAlgorithmName.SHA512;
		
		public string Hash(string value)
		{
			byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
			byte[] hash = Rfc2898DeriveBytes.Pbkdf2(value, salt, iterations, algorithm, hashSize);
			return Convert.ToHexString(hash) + "-" + Convert.ToHexString(salt);
		}

		public bool Verify(string value, string hashedValue)
		{
			string[] parts = hashedValue.Split('-');
			byte[] hash = Convert.FromHexString(parts[0]);
			byte[] salt = Convert.FromHexString(parts[1]);

			byte[] valueHash = Rfc2898DeriveBytes.Pbkdf2(value, salt, iterations, algorithm, hashSize);

			return CryptographicOperations.FixedTimeEquals(valueHash, hash);
		}
	}
}
