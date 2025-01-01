namespace Educational_Platform.Domain.Exceptions
{
	public class NotExistException : Exception
	{
        public NotExistException(string message):base(message) 
        {
            
        }
        public NotExistException()
        {
            
        }
    }
}
