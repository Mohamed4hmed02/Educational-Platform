using System.Data;
using FluentValidation;
using Educational_Platform.Application.Abstractions.BookInterfaces;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.OperationInterfaces;
using Educational_Platform.Application.Extensions;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Enums;
using Educational_Platform.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Application.Services.BookServices
{
    public partial class BookCommandServices(
        IUnitOfWork unitOfWork,
        IStorageService storageServices,
        ICachingItemService caching,
        IValidator<Book> validator,
        ILogger<BookCommandServices> log) : IBookCommandServices
    {
        public async ValueTask CreateAsync(CommandBookModel entity)
        {
            if (await unitOfWork.BooksRepository.IsExistAsync(b => b.Code == entity.Code))
                throw new DuplicateNameException("Book With Same Code Is Exist Try Update Instead")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status409Conflict);

            if (entity.Id != 0)
                throw new InvalidDataException("Id Must Be Zero When Adding A New Book")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);

            ValidateBook(entity);

            Book book = await unitOfWork.BooksRepository.CreateAsync(entity);
            await unitOfWork.SaveChangesAsync();

            log.LogInformation("New Book {@Book} Was Created", (CommandBookModel)book);
        }

        public async ValueTask UpdateAsync(CommandBookModel entity)
        {
            if (!await unitOfWork.BooksRepository.IsExistAsync(b => b.Id == entity.Id))
                throw new NotExistException("Book With Same Id Is Not Exist Try Add Instead");

            ValidateBook(entity);
            Book book = entity;

            await unitOfWork.BooksRepository.UpdateAsync(book);
            await unitOfWork.SaveChangesAsync();

            log.LogInformation("A Book Was Updated To {@Book}", (CommandBookModel)book);
        }

        public async ValueTask DeleteAsync(object entityId)
        {
            var book = await unitOfWork.BooksRepository.DeleteAsync(entityId);

            if (book.ImageName != null)
                await DeleteBookImageAsync(book.ImageName);

            var details = await unitOfWork.CartDetailsRepository
                .ReadAllAsync(pd => pd.ProductType == ProductTypes.Book && pd.ProductId == book.Id);

            await unitOfWork.CartDetailsRepository.DeleteAsync(details);

            await unitOfWork.SaveChangesAsync();

            log.LogInformation("A Book {@Book} Was Deleted", (CommandBookModel)book);
        }

        public async ValueTask SetImageAsync(object bookId, IFormFile photo)
        {
            if (!photo.ContentType.Contains("image", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("The File Is Not An Image File")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status415UnsupportedMediaType);

            var book = await unitOfWork.BooksRepository.ReadAsync(bookId);
            var path = await storageServices.SaveFileAsync(photo, $"Book_{book.Id}", FileTypeEnum.Image);

            book.ImageName = path;
            await unitOfWork.SaveChangesAsync();

            log.LogInformation("Image Was Set For Book {@Book}", (CommandBookModel)book);
        }

        public async ValueTask UpdateBookQuantityAsync(object bookId, int addQuantity)
        {
            var book = await unitOfWork.BooksRepository.ReadAsync(bookId);
            var oldQuantity = book.Quantity;

            book.Quantity += addQuantity;
            await unitOfWork.SaveChangesAsync();

            log.LogInformation("A Book {Book} Qauntity Was Updated,Old Quantity {oldQuantity}", (CommandBookModel)book, oldQuantity);
        }

        public async ValueTask UpdateBooksQuantityAsync(IEnumerable<int> booksIds, IEnumerable<int> addQuantities)
        {
            if (booksIds.Count() != addQuantities.Count())
                throw new InvalidDataException("Quantity Values Is Not Same As Ids Values")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);

            if (!booksIds.Any())
                return;

            var books = (await unitOfWork.BooksRepository.GetBooksAsync(booksIds)).ToArray();

            if (books.Length != booksIds.Count())
                throw new NotExistException("Some Book Id Is Not Exist");

            for (int i = 0; i < booksIds.Count(); i++)
                if (books[i].Quantity >= addQuantities.ElementAt(i))
                    books[i].Quantity -= addQuantities.ElementAt(i);
                else
                {
                    log.LogError("Not Enough Book Quantity For {@Book}", (CommandBookModel)books[i]);
                    throw new NotEnoughQuantityException($"Not Enough Book Quantity For Book With Id = {books[i].Id}")
                        .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);
                }

            await unitOfWork.SaveChangesAsync();


            foreach (var book in books)
                log.LogInformation("Book Quantity Updated To {@Book}", (CommandBookModel)book);
        }

        public void Dispose()
        {
            UpdateCachedBooks();
            GC.SuppressFinalize(this);
        }
    }
    public partial class BookCommandServices
    {
        private async ValueTask DeleteBookImageAsync(string imageName)
        {
            try
            {
                await storageServices.DeleteFileAsync(imageName, FileTypeEnum.Image);
            }
            catch
            {

            }
        }

        private void UpdateCachedBooks()
        {
            caching.CacheItemAndGet(CachedItemType.Book, true);
        }

        private async void ValidateBook(Book book)
        {
            var result = validator.Validate(book);
            if (!result.IsValid)
            {
                throw new InvalidDataException(result.Errors.GetAllErrorsMessages())
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);
            }

            if (await unitOfWork.BooksRepository.IsExistAsync(b => b.Code == book.Code && b.Id != book.Id))
                throw new DuplicateNameException("Book New Code Is Already Exist")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status409Conflict);
        }
    }
}
