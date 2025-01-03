using Educational_Platform.Application.Enums;

namespace Educational_Platform.Application.Abstractions.OperationInterfaces
{
    public interface ICachingItemService
	{
		/// <summary>
		/// Caching Items Within Database And Return These Items As Object, If Items Is Already Cached It Won't Read It From Database Again,
		/// No Mutual Exclusion Will Happen
		/// </summary>
		/// <param name="itemType">The Type Of Item Within Database</param>
		/// <param name="isModified">Select To Updating Cached Items If Existing</param>
		/// <returns></returns>
		object CacheItemAndGet(CachedItemType itemType, bool isModified);
	}
}
