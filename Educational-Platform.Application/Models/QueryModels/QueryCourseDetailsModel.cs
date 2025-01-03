using Educational_Platform.Application.Abstractions.UnitInterfaces;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Application.Models.QueryModels
{
    public class QueryCourseDetailsModel
	{
		public int Id { get; set; }
		public string? IntroVideoUrl { get; set; }
		public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
		public decimal Price { get; set; }
		public int Duration { get; set; }
		public string? ImagePath { get; set; }
		public ProductTypes Type { get; set; }
		public IEnumerable<QueryUnitModel>? Units { get; set; }
	}
}
