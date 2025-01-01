using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Domain.Abstractions;
using Educational_Platform.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Educational_Platform.Application.Abstractions.BookInterfaces
{
    public interface IBookCommandServices : IEditable<CommandBookModel>, IDeleteable<Book>, IDisposable
    {
        ValueTask SetImageAsync(object bookId, IFormFile photo);
        ValueTask UpdateBookQuantityAsync(object bookId, int addQuantity);
        ValueTask UpdateBooksQuantityAsync(IEnumerable<int> booksIds, IEnumerable<int> addQuantities);
    }
}
