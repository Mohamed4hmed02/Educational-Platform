using Educational_Platform.Application.Abstractions.BookInterfaces;
using Educational_Platform.Application.Abstractions.BookServices;
using Educational_Platform.Application.Enums;
using Educational_Platform.Application.Models.CommandModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.API.Controllers
{
	//[Authorize(Roles = "Admin")]
	//[ApiController]
	//[Route("api/v1/[controller]/")]
	//public class BooksController(
	//	IBookCommandServices bookCommandServices,
	//	IBookQueryServices bookQueryServices) : ControllerBase
	//{
	//	[AllowAnonymous]
	//	[HttpGet("{Id}")]
	//	public async ValueTask<IActionResult> GetBook(int Id)
	//	{
	//		return Ok(await bookQueryServices.GetBookInDetailAsync(Id));
	//	}
	//
	//	[HttpPost]
	//	public async ValueTask AddBook(CommandBookModel book)
	//	{
	//		await bookCommandServices.CreateAsync(book);
	//	}
	//
	//	[HttpPut]
	//	public async ValueTask UpdateBook(CommandBookModel book)
	//	{
	//		await bookCommandServices.UpdateAsync(book);
	//	}
	//
	//	[HttpDelete("{Id}")]
	//	public async ValueTask DeleteBook(int Id)
	//	{
	//		await bookCommandServices.DeleteAsync(Id);
	//	}
	//
	//	[AllowAnonymous]
	//	[HttpGet]
	//	public async ValueTask<IActionResult> GetAll(int page, int size, decimal? price, int comparison, string? title)
	//	{
	//		return Ok(await bookQueryServices.GetPageAsync(page, size, ModelTypeEnum.Query));
	//	}
	//
	//	[HttpPatch("{bookId}/Image")]
	//	public async ValueTask SetPhoto(int bookId, IFormFile image)
	//	{
	//		await bookCommandServices.SetImageAsync(bookId, image);
	//	}
	//
	//}
}
