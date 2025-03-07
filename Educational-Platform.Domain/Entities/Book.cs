﻿namespace Educational_Platform.Domain.Entities
{
	public class Book
	{
		public int Id { get; set; }
		public required string Code { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
        public string? ImageName { get; set; }
    }
}
