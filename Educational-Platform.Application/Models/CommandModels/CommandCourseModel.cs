using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Models.CommandModels
{
	public class CommandCourseModel
	{
		public int Id { get; set; }
		public int IntroVideoId { get; set; }
		public required string Name { get; set; }
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public int Duration { get; set; }
		public bool IsVisible { get; set; }
		public float Discount { get; set; }
		public DateOnly DiscountEndTime { get; set; }
		public bool IsDiscountAvailable => DateOnly.FromDateTime(DateTime.UtcNow) <= DiscountEndTime;

		public Course GetCourse(IUnitOfWork unitOfWork)
		{
			return new Course
			{
				Name = Name,
				Description = Description,
				Price = Price,
				Duration = Duration,
				Id = Id,
				IntroTopicId = IntroVideoId,
				IsVisible = IsVisible,
				Discount = Discount,
				DiscountEndTime = DiscountEndTime,
				ImageName = GetImageName(unitOfWork)
			};
		}

		public static explicit operator CommandCourseModel(Course model)
		{
			return new CommandCourseModel
			{
				Name = model.Name,
				Description = model.Description,
				Price = model.Price,
				Duration = model.Duration,
				Id = model.Id,
				IntroVideoId = Convert.ToInt32(model.IntroTopicId),
				IsVisible = model.IsVisible,
				Discount = model.Discount,
				DiscountEndTime = model.DiscountEndTime
			};
		}

		private string? GetImageName(IUnitOfWork unitOfWork)
		{
			try
			{
				return unitOfWork.CoursesRepository
				.AsNoTracking()
				.ReadAsync(c => c.Id == Id, c => c.ImageName)
				.AsTask().Result;
			}
			catch
			{
				return null;
			}

		}
	}
}
