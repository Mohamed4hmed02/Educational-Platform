using Educational_Platform.Application.Abstractions.BookServices;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.OperationInterfaces;
using Educational_Platform.Application.Enums;
using Educational_Platform.Application.Extensions;
using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Application.Options;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Educational_Platform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Educational_Platform.Application.Services.BookServices
{
    public class BookQueryServices(
        IUnitOfWork unitOfWork,
        IStorageService storageServices,
        ICachingItemService cache,
        IOptionsMonitor<PaginationOptions> options) : IBookQueryServices
    {

        public async ValueTask<QueryBookModel> GetBookInDetailAsync(object entityId)
        {
            var book = await unitOfWork.BooksRepository.AsNoTracking().ReadAsync(b => b.Id.Equals(entityId),
                b => (QueryBookModel)b);

            book.ImagePath = storageServices.GetFileViewPath(book.ImagePath, FileTypeEnum.Image);
            return book;
        }

        public async Task<object> GetPageAsync(int page, int size, ModelTypeEnum modelType)
        {
            int pageSize = options.CurrentValue.PageSize;
            if (modelType == ModelTypeEnum.Query)
                return ((IEnumerable<QueryBookModel>)cache.CacheItemAndGet(CachedItemType.Book, false))
                    .GetPageFromList(page, pageSize, size);
            else if (modelType == ModelTypeEnum.Command)
                return await unitOfWork.BooksRepository
                .Query
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .GetPageFromList(page, pageSize, size)
                .ToArrayAsync();

            return new object();
        }
    }
}
