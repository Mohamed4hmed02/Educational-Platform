using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.OperationInterfaces;
using Educational_Platform.Application.Abstractions.UnitInterfaces;
using Educational_Platform.Application.Enums;
using Educational_Platform.Application.Extensions;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Application.Models.QueryModels;
using Educational_Platform.Domain.Enums;
using Educational_Platform.Domain.Exceptions;

namespace Educational_Platform.Application.Services.UnitServices
{
    internal class UnitQueryServices(
        IUnitOfWork unitOfWork,
        ICachingItemService cachingServices) : IUnitQueryServices
    {
        public async ValueTask<object> BelongToCourseAsync(object unitId)
        {
            return await unitOfWork.UnitsRepository.AsNoTracking()
                .ReadAsync(u => u.Id.Equals(unitId), u => u.CourseId);
        }

        public async ValueTask<bool> CanUserAccessUnitAsync(object unitId, object userId)
        {
            var unit = await unitOfWork.UnitsRepository
                .AsNoTracking()
                .ReadAsync(u => u.Id.Equals(unitId));

            return await unitOfWork.UsersCoursesRepository.IsExistAsync(
                uc => uc.CourseId == unit.CourseId &&
                uc.UserId.Equals(userId) &&
                uc.CurrentUnitId >= unit.UnitOrder
            );
        }

        public async ValueTask<object> GetUnitsAsync(object courseId, ModelTypeEnum modelType)
        {
            if (modelType == ModelTypeEnum.Query)
            {
                var models = cachingServices.CacheItemAndGet(CachedItemType.Unit, false) as QueryUnitModel[];
                return models.Where(u => u.CourseId.Equals(courseId));
            }
            else if (modelType == ModelTypeEnum.Command)
                return await GetCommandModles(courseId);
            return Array.Empty<object>();
        }

        private async ValueTask<object> GetCommandModles(object courseId)
        {
            if (!await unitOfWork.CoursesRepository.IsExistAsync(c => c.Id.Equals(courseId)))
                throw new NotExistException("No Such Course For The Given Course Id");

            return await unitOfWork.UnitsRepository.ReadAllAsync(u => u.CourseId.Equals(courseId), u => (CommandUnitModel)u);
        }
    }
}
