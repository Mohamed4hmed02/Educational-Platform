using System.Data;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.TopicInterfaces;
using Educational_Platform.Application.Abstractions.UnitInterfaces;
using Educational_Platform.Application.Enums;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Educational_Platform.Domain.Exceptions;
using Educational_Platform.Domain.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Application.Services.UnitServices
{
    public class UnitCommandServices(
        IUnitOfWork unitOfWork,
        ICacheServices cacheServices,
        ITopicCommandServices topicCommandServices,
        ILogger<UnitCommandServices> log) : IUnitCommandServices
    {
        public async ValueTask CreateAsync(CommandUnitModel entity)
        {
            if (entity.Id != 0)
                throw new InvalidDataException("Id Must Equal Zero")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);

            if (await unitOfWork.UnitsRepository.IsExistAsync(u => u.Name == entity.Name && u.CourseId == entity.CourseId))
                throw new DuplicateNameException("There Is A Unit With Same Name For The Same Course")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status409Conflict);

            if (!await unitOfWork.CoursesRepository.IsExistAsync(c => c.Id == entity.CourseId))
                throw new NotExistException("No Such Course For The Given Course Id");

            int order = unitOfWork.UnitsRepository
                .Query
                .Where(u => u.CourseId == entity.CourseId)
                .Count() + 1;

            entity.UnitOrder = order;
            var unit = await unitOfWork.UnitsRepository.CreateAsync(await entity.GetUnitAsync(unitOfWork));
            await unitOfWork.SaveChangesAsync();

            log.LogInformation("A New Unit {@Unit} Is Added", unit);
        }

        public async ValueTask UpdateAsync(CommandUnitModel entity)
        {
            if (await unitOfWork.UnitsRepository.IsExistAsync(u => u.Name == entity.Name && u.CourseId == entity.CourseId && u.Id != entity.Id))
                throw new DuplicateNameException("The Update Will Lead To Same Name Unit For Course")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status409Conflict);

            if (!await unitOfWork.CoursesRepository.IsExistAsync(c => c.Id == entity.CourseId))
                throw new NotExistException("No Such Course For The Given Course Id");

            // check if order is want to be changed
            if (await unitOfWork.UnitsRepository
                .IsExistAsync(u => u.Id == entity.Id && u.UnitOrder != entity.UnitOrder))
            {
                var maxOrder = unitOfWork.UnitsRepository
                    .Query
                    .Where(u => u.CourseId == entity.CourseId)
                    .Count();

                if (entity.UnitOrder > maxOrder || entity.UnitOrder < 0)
                    throw new InvalidDataException($"Invalid Order Input Max Order Is:{maxOrder}")
                        .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);

                var unit2 = await unitOfWork.UnitsRepository
                    .ReadAsync(u => u.CourseId == entity.CourseId && u.UnitOrder == entity.UnitOrder);

                var entityOrder = await unitOfWork.UnitsRepository
                    .ReadAsync(u => u.Id == entity.Id, u => u.UnitOrder);

                unit2.UnitOrder = entityOrder;
            }

            await unitOfWork.UnitsRepository.UpdateAsync(await entity.GetUnitAsync(unitOfWork));
            await unitOfWork.SaveChangesAsync();
        }

        public async ValueTask DeleteAsync(object Id)
        {
            var unit = await unitOfWork.UnitsRepository.DeleteAsync(Id);
            await DeleteTopicsAsync(Id);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteUnitsAsync(IEnumerable<int> unitsIds)
        {
            if (!unitsIds.Any())
                return;

            var queryResult = await unitOfWork.UnitsRepository.GetTopicsIdAsync(unitsIds);

            var unit = await unitOfWork.UnitsRepository
                .AsNoTracking()
                .ReadAsync(u => u.Id == unitsIds.ElementAt(0));

            await topicCommandServices.DeleteTopicsAsync(queryResult);
            await unitOfWork.UnitsRepository.DeleteAsync(unitsIds);

            log.LogInformation("All Topics With Id In {@Ids} Are Deleted", queryResult);
        }

        private async ValueTask DeleteTopicsAsync(object unitId)
        {
            var topicsIds = await unitOfWork.TopicsRepository.ReadAllAsync(t => t.UnitId.Equals(unitId), t => t.Id);
            await topicCommandServices.DeleteTopicsAsync(topicsIds);
        }

        private void RemoveCachedUnits()
        {
            cacheServices.Remove(CachedItemType.Unit);
        }

        public void Dispose()
        {
            RemoveCachedUnits();
            GC.SuppressFinalize(this);
        }
    }
}
