using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Application.Models.QueryModels
{
    public class QueryBookModel
	{
		public int Id { get; set; }
		public required string Code { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public string? ImagePath { get; set; }
		public ProductTypes Type { get; set; }

		public static explicit operator QueryBookModel(Book book)
		{
			return new QueryBookModel
			{
				Code = book.Code,
				Description = book.Description,
				Id = book.Id,
				Title = book.Title,
				ImagePath = book.ImageName,
				Price = book.Price,
				Quantity = book.Quantity,
				Type = ProductTypes.Book
			};
		}
	}
}
