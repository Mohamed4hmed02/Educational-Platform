namespace Educational_Platform.Domain.Exceptions
{
	public class NotEnoughQuantityException : Exception
	{
		public NotEnoughQuantityException() : base()
		{

		}
		public NotEnoughQuantityException(string message) : base(message)
		{

		}
	}
}
