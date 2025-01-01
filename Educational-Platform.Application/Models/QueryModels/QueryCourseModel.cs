using Educational_Platform.Application.Abstractions.UnitInterfaces;
using Educational_Platform.Application.Enums;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Application.Models.QueryModels
{
    public class QueryCourseModel
    {
        public int Id { get; set; }
        public int? IntroVideoId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal PriceBeforeDiscount { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public int Duration { get; set; }
        public string? ImagePath { get; set; }
        public ProductTypes Type { get; set; }
        public bool HasDiscount { get; set; }
        public float DiscountValue { get; set; }

        public static QueryCourseModel GetModel(Course course, IStorageService storageServices)
        {
            return new QueryCourseModel
            {
                Type = ProductTypes.Course,
                Description = course.Description,
                Duration = course.Duration,
                Id = course.Id,
                Name = course.Name,
                PriceBeforeDiscount = course.Price,
                PriceAfterDiscount = course.IsDiscountAvailable ?
                Math.Round(course.Price - course.Price * ((decimal)course.Discount) / 100, 2) : course.Price,
                IntroVideoId = course.IntroTopicId,
                ImagePath = storageServices.GetFileViewPath(course.ImageName, FileTypeEnum.Image),
                HasDiscount = course.IsDiscountAvailable,
                DiscountValue = course.Discount
            };
        }

        public QueryCourseDetailsModel GetDetailedQueryModel(IVideoHostServices hostServices, IUnitQueryServices unitQueryServices)
        {
            string? introVideoUrl = default;
            try
            {
                introVideoUrl = hostServices.GetVideoUrlPlayerAsync(Convert.ToInt32(IntroVideoId)).Result;
            }
            catch
            {

            }
            return new QueryCourseDetailsModel
            {
                Description = Description,
                Duration = Duration,
                Id = Id,
                ImagePath = ImagePath,
                Name = Name,
                Price = PriceAfterDiscount,
                Type = Type,
                IntroVideoUrl = introVideoUrl,
                Units = (IEnumerable<QueryUnitModel>)unitQueryServices.GetUnitsAsync(Id, ModelTypeEnum.Query).AsTask().Result
            };
        }
    }
}
