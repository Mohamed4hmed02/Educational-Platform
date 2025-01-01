namespace Educational_Platform.Domain.Exceptions
{
	public class DataBaseDownException : Exception
	{
		public DataBaseDownException(string message) : base(message)
		{

		}
        public DataBaseDownException()
        {
            
        }
    }
}
