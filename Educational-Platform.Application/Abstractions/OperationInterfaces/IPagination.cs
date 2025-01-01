using Educational_Platform.Application.Enums;

namespace Educational_Platform.Application.Abstractions.OperationInterfaces
{
	public interface IPagination
	{
		/// <summary>
		/// Retrieve List Of Models As Object Based On <paramref name="modelType"/> Of Size Based On Pagination
		/// </summary>
		/// <param name="page">page number start from 0</param>
		/// <param name="size">Max is defined in settings file</param>
		/// <param name="modelType">The type of model to get</param>
		/// <returns></returns>
		Task<object> GetPageAsync(int page, int size, ModelTypeEnum modelType);
	}
}
