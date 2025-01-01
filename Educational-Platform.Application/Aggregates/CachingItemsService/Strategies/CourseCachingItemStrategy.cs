using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Application.Aggregates.CachingItemsService.Strategies
{
    public class CourseCachingItemStrategy(
        IUnitOfWork unitOfWork,
        ICacheServices cacheServices,
        IStorageService storageServices) : ICachingItemStrategy
    {
        public async Task<object> GetItemAndCacheAsync(bool isModified)
        {
            if (!isModified)
            {
                var item = cacheServices.GetValue(CachedItemType.Course);
                if (item is not null)
                    return item;
            }

            var courses = (await unitOfWork.CoursesRepository
                .AsNoTracking()
                .ReadAllAsync(c => c.IsVisible))
                .Select(c => QueryCourseModel.GetModel(c, storageServices));

            cacheServices.Cache(CachedItemType.Course, courses, TimeSpan.FromMinutes(8));
            return courses;
        }
    }
}
