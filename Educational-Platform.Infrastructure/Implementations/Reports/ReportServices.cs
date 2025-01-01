using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.OperationInterfaces;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NPOI.XSSF.UserModel;
using System.Net.Mail;


namespace Educational_Platform.Infrastructure.Implementations.Reports
{
	public class ReportServices(
		IUnitOfWork unitOfWork,
		IEmailSenderServices emailSender,
		IConfiguration configuration,
		ILogger<ReportServices> log) : IReportServices
	{
		private class TempClass
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public decimal Price { get; set; }
			public float Discount { get; set; }
			public int NumberOfSales { get; set; }
		}
		private readonly string _to = configuration["Report:To"] ?? "";

		public async Task SendReportAsync(string? To = null)
		{
			UserCourse userCourse = new();
			Course course = new();
			TempClass temp = new();

			string query =
				$"""
				select c.{nameof(course.Id)},c.{nameof(course.Name)},c.{nameof(course.Price)},
				case
				when datediff(day,getdate(),c.{nameof(course.DiscountEndTime)}) >= 1 then c.{nameof(course.Discount)}
				else 0
				end as {nameof(temp.Discount)},
				(
				select isnull(count(*),0)
				from dbo.UsersCourses as uc
				where c.{nameof(course.Id)} = uc.{nameof(userCourse.CourseId)}
				and datediff(day,uc.{nameof(userCourse.StartDate)},getdate()) <=30
				) as {nameof(temp.NumberOfSales)}
				from dbo.Courses as c
				""";

			var result = await unitOfWork.RawSqlQueryAsync<TempClass>(query);

			using (var stream = new MemoryStream())
			{
				ExportToExcel(result.ToList(), stream);

				var attachment = new Attachment(stream, "Report.xlsx");
				if (To == null)
					await emailSender.SendMailAsync("Here Is Your Monthly Report:-", _to, attachment: attachment);
				else
					await emailSender.SendMailAsync("Here Is Your Monthly Report:-", To, attachment: attachment);

				stream.Dispose();
			}
			log.LogInformation("A Report Has Been Sent");
		}

		private void ExportToExcel(List<TempClass> items, Stream stream)
		{
			using var workbook = new XSSFWorkbook();
			var sheet = workbook.CreateSheet("Products");

			// Create header row
			var headerRow = sheet.CreateRow(0);
			headerRow.CreateCell(0).SetCellValue("Id");
			headerRow.CreateCell(1).SetCellValue("Type");
			headerRow.CreateCell(2).SetCellValue("Name");
			headerRow.CreateCell(3).SetCellValue("Price");
			headerRow.CreateCell(4).SetCellValue("Discount");
			headerRow.CreateCell(5).SetCellValue("Number of sales");

			// Add data rows
			int rowNumber = 1;
			foreach (var person in items)
			{
				var row = sheet.CreateRow(rowNumber++);
				row.CreateCell(0).SetCellValue(person.Id);
				row.CreateCell(1).SetCellValue(nameof(ProductTypes.Course));
				row.CreateCell(2).SetCellValue(person.Name);
				row.CreateCell(3).SetCellValue(Convert.ToDouble(person.Price));
				row.CreateCell(4).SetCellValue(person.Discount);
				row.CreateCell(5).SetCellValue(person.NumberOfSales);
			}

			// Auto size columns
			for (int i = 0; i < items.Count; i++)
			{
				sheet.AutoSizeColumn(i);
			}

			// Save the file
			workbook.Write(stream, true);
		}
	}
}
