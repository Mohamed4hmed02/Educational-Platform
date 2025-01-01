namespace Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions
{
	public interface ISaveLogsFile
	{
		/// <summary>
		/// Save The Logs File
		/// </summary>
		/// <param name="fileStream"></param>
		/// <param name="name">Name Must Be With Extension Type</param>
		/// <returns></returns>
		ValueTask SaveLogsFileAsync(Stream fileStream, string name);
	}
}
