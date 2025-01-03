using Educational_Platform.Application.Abstractions.CourseInterfaces;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.OperationInterfaces;
using Educational_Platform.Application.Abstractions.UnitInterfaces;
using Educational_Platform.Application.Enums;
using Educational_Platform.Application.Extensions;
using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Application.Options;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Educational_Platform.Application.Services.CourseServices
{
    public class CourseQueryServices(
		IUnitOfWork unitOfWork,
		ICachingItemService cache,
		IStorageService storageServices,
		IVideoHostServices hostServices,
		IUnitQueryServices unitQueryServices,
		IOptionsMonitor<PaginationOptions> options) : ICourseQueryServices
	{
		public async ValueTask<QueryCourseDetailsModel> GetCourseInDtailAsync(object courseId)
		{
			var course = await unitOfWork.CoursesRepository
				.AsNoTracking()
				.ReadAsync(c => c.Id.Equals(courseId));

			var courseModel = QueryCourseModel.GetModel(course, storageServices);

			var courseDetailModel = courseModel.GetDetailedQueryModel(hostServices, unitQueryServices);

			return courseDetailModel;
		}

		public async Task<object> GetPageAsync(int page, int size, ModelTypeEnum modelType)
		{
			int pageSize = options.CurrentValue.PageSize;
			if (modelType == ModelTypeEnum.Query)
				return ((IEnumerable<QueryCourseModel>)cache.CacheItemAndGet(CachedItemType.Course, false))
				.GetPageFromList(page, pageSize, size);

			else if (modelType == ModelTypeEnum.Command)
				return await unitOfWork.CoursesRepository
				.Query
				.AsNoTracking()
				.OrderBy(c => c.Id)
				.GetPageFromList(page, pageSize, size)
				.ToArrayAsync();

			return Array.Empty<object>();
		}

		public async ValueTask<int> GetCountAsync(ModelTypeEnum modelType)
		{
			if (modelType == ModelTypeEnum.Query)
				return await unitOfWork.CoursesRepository
				.Query
				.Where(c => c.IsVisible)
				.CountAsync();

			else if (modelType == ModelTypeEnum.Command)
				return await unitOfWork.CoursesRepository
				.Query
				.CountAsync();

			return 0;
		}
	}
}
