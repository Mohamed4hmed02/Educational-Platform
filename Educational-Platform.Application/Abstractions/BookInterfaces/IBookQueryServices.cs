using Educational_Platform.Application.Abstractions.OperationInterfaces;
using Educational_Platform.Application.Models.QueryModels;

namespace Educational_Platform.Application.Abstractions.BookServices
{
	public interface IBookQueryServices : IPagination
	{
		ValueTask<QueryBookModel> GetBookInDetailAsync(object bookId);
	}
}
