using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Models.CommandModels
{
	public class CommandBookModel
	{
		public int Id { get; set; }
		public required string Code { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }

		public static implicit operator Book(CommandBookModel model)
		{
			return new Book
			{
				Code = model.Code,
				Title = model.Title,
				Description = model.Description,
				Quantity = model.Quantity,
				Price = model.Price,
				Id = model.Id
			};
		}

		public static explicit operator CommandBookModel(Book book)
		{
			return new CommandBookModel
			{
				Code = book.Code,
				Title = book.Title,
				Description = book.Description,
				Quantity = book.Quantity,
				Price = book.Price,
				Id = book.Id
			};
		}
	}
}
