using System.Data;
using FluentValidation;
using Educational_Platform.Application.Abstractions.CourseInterfaces;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.OperationInterfaces;
using Educational_Platform.Application.Abstractions.UnitInterfaces;
using Educational_Platform.Application.Extensions;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Enums;
using Educational_Platform.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Application.Services.CourseServices
{
    public class CourseCommandServices(
            IUnitOfWork unitOfWork,
            IUnitCommandServices unitServices,
            IStorageService storageServices,
            ICachingItemService caching,
            IValidator<Course> validator,
            ILogger<CourseCommandServices> log) : ICourseCommandServices
    {
        public async ValueTask CreateAsync(CommandCourseModel entity)
        {
            if (entity.Id != 0)
                throw new InvalidDataException("Id Must Be Zero When Adding A New Course")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);

            await ValidateCourseAsync(entity);

            var course = await unitOfWork.CoursesRepository.CreateAsync(entity.GetCourse(unitOfWork));

            await unitOfWork.SaveChangesAsync();

            var newCourse = (CommandCourseModel)course;

            log.LogInformation("A New Course {@Course} Is Added", newCourse);
        }

        public async ValueTask UpdateAsync(CommandCourseModel entity)
        {
            if (!await unitOfWork.CoursesRepository.IsExistAsync(c => c.Id == entity.Id))
                throw new NotExistException("Course Is Not Exist");

            await ValidateCourseAsync(entity);

            await unitOfWork.CoursesRepository.UpdateAsync(entity.GetCourse(unitOfWork));
            await unitOfWork.SaveChangesAsync();

            log.LogInformation("A Course Was Updated To {@Course}", entity);
        }

        public async ValueTask DeleteAsync(object entityId)
        {
            var course = await unitOfWork.CoursesRepository.ReadAsync(entityId);

            var unitsIds = await unitOfWork.UnitsRepository.ReadAllAsync(
                v => v.CourseId == course.Id,
                v => v.Id
            );

            await unitServices.DeleteUnitsAsync(unitsIds);

            await unitOfWork.CoursesRepository.DeleteAsync(course.Id);

            var details = await unitOfWork.CartDetailsRepository
                .ReadAllAsync(
                pd => pd.ProductType == ProductTypes.Course &&
                pd.ProductId == course.Id);

            await unitOfWork.CartDetailsRepository.DeleteAsync(details);

            await unitOfWork.SaveChangesAsync();

            log.LogInformation("A Course {@Course} Was Deleted", (CommandCourseModel)course);
        }

        public async ValueTask SetTopicIntroAsync(object courseId, object topicId)
        {
            var course = await unitOfWork.CoursesRepository.ReadAsync(courseId);
            if (!await unitOfWork.TopicsRepository.IsExistAsync(t => t.Id.Equals(topicId)))
                throw new NotExistException("Topic Is Not Exist");

            course.IntroTopicId = (int)topicId;
            await unitOfWork.SaveChangesAsync();
        }

        public async ValueTask SetImageAsync(object courseId, IFormFile image)
        {
            if (!image.ContentType.Contains("image", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("The File Is Not An Image File")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status415UnsupportedMediaType);

            var course = await unitOfWork.CoursesRepository.ReadAsync(courseId);
            var path = await storageServices.SaveFileAsync(image, $"Course_{course.Id}", FileTypeEnum.Image);

            course.ImageName = path;
            await unitOfWork.SaveChangesAsync();

            log.LogInformation("Image Was Set For Course {courseName}", course.Name);
        }

        public async ValueTask SetVisibiltyAsync(object courseId, bool visibilty)
        {
            var course = await unitOfWork.CoursesRepository.ReadAsync(courseId);
            course.IsVisible = visibilty;
            await unitOfWork.SaveChangesAsync();
        }

        public void Dispose()
        {
            UpdateCachedCourses();
            GC.SuppressFinalize(this);
        }

        private void UpdateCachedCourses()
        {
            caching.CacheItemAndGet(CachedItemType.Course, true);
        }

        private async ValueTask ValidateCourseAsync(CommandCourseModel model)
        {
            var result = validator.Validate(model.GetCourse(unitOfWork));
            if (!result.IsValid)
            {
                throw new InvalidDataException(result.Errors.GetAllErrorsMessages())
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);
            }

            if (await unitOfWork.CoursesRepository.IsExistAsync(c => c.Name == model.Name && c.Id != model.Id))
                throw new DuplicateNameException("A Course With Same Name Is Exist")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status409Conflict);

            if (model.IntroVideoId != 0)
                if (!await unitOfWork.TopicsRepository.IsExistAsync(t => t.Id == model.IntroVideoId))
                    throw new NotExistException("No Such A Topic With The Given Id");
        }
    }
}
