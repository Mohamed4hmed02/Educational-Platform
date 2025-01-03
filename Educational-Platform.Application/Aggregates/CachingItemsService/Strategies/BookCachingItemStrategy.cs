using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Enums;
using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Application.Aggregates.CachingItemsService.Strategies
{
    public class BookCachingItemStrategy(
        IUnitOfWork unitOfWork,
        ICacheServices cacheServices,
        IStorageService storageServices
        ) : ICachingItemStrategy
    {
        public async Task<object> GetItemAndCacheAsync(bool isModified)
        {
            if (!isModified)
            {
                var item = cacheServices.GetValue(CachedItemType.Book);
                if (item is not null)
                    return item;
            }

            var books = await unitOfWork.BooksRepository.AsNoTracking().ReadAllAsync(b => new QueryBookModel
            {
                Id = b.Id,
                Price = b.Price,
                Quantity = b.Quantity,
                Title = b.Title,
                ImagePath = b.ImageName,
                Code = b.Code,
                Type = ProductTypes.Book
            });

            foreach (var book in books)
            {
                var imageName = book.ImagePath;
                book.ImagePath = storageServices.GetFileViewPath(imageName, FileTypeEnum.Image);
            }
            cacheServices.Cache(CachedItemType.Book, books, TimeSpan.FromMinutes(8));
            return new();
        }
    }
}
