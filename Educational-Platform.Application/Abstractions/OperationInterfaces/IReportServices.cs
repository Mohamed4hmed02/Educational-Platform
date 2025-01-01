namespace Educational_Platform.Application.Abstractions.OperationInterfaces
{
	public interface IReportServices
	{
		Task SendReportAsync(string? to=null);
	}
}
