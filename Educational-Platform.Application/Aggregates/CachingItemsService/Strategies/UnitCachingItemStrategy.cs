using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Enums;
using Educational_Platform.Application.Extensions;
using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Application.Aggregates.CachingItemsService.Strategies
{
    public class UnitCachingItemStrategy(
        IUnitOfWork unitOfWork,
        ICacheServices cacheServices,
        IStorageService storageServices) : ICachingItemStrategy
    {
        public async Task<object> GetItemAndCacheAsync(bool isModified)
        {
            if (cacheServices.GetValue(CachedItemType.Unit) is QueryUnitModel[] units)
            {
                return units;
            }

            units = (await unitOfWork.UnitsRepository.GetUnitsModelAsync()).ToArray();
            for (int i = 0; i < units.Length; i++)
            {
                var res = GetPathsForTopics(units[i].Topics).ToArray();
                if (res.IsNullOrEmpty())
                    continue;
                units[i].Topics = res;
                var s = units[i];
                units[i] = s;
            }

            cacheServices.Cache(CachedItemType.Unit, units, TimeSpan.FromMinutes(5));
            return units;
        }

        private IEnumerable<QueryTopicModel> GetPathsForTopics(IEnumerable<QueryTopicModel>? topics)
        {
            if (topics.IsNullOrEmpty())
                return [];

            if (topics is null)
                return [];

            var paths = storageServices.GetFilesPath(topics.Select(t => t.ReferencePath ?? "").ToArray(), FileTypeEnum.Pdf);
            for (int i = 0; i < paths?.Length; i++)
                topics.ElementAt(i).ReferencePath = paths[i];

            return topics;
        }
    }
}
