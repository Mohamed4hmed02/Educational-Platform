using Educational_Platform.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions
{
	public interface IStorageService
	{
		/// <summary>
		/// Write The Entire File Into Target Storage
		/// </summary>
		/// <param name="file"></param>
		/// <param name="fileName"></param>
		/// <param name="fileType"></param>
		/// <returns>FileName + FileExtension</returns>
		ValueTask<string> SaveFileAsync(IFormFile file, string fileName, FileTypeEnum fileType);
		/// <summary>
		/// Write The Entire File Into Target Storage
		/// </summary>
		/// <param name="fileStream"></param>
		/// <param name="fileFullName">Must be file name + file extension</param>
		/// <param name="fileType"></param>
		/// <returns></returns>
		ValueTask<string> SaveFileAsync(Stream fileStream, string fileFullName, FileTypeEnum fileType);
		Task DeleteFileAsync(string? fileName, FileTypeEnum fileType);
		/// <summary>
		/// Retrun The File Path That Is Ready To Embed , If The File Is Not Found
		/// It Will Return null
		/// </summary>
		/// <param fileName="file"></param>
		/// <param fileName="fileName"></param>
		/// <param fileName="isCourse"></param>
		/// <returns>The Appropriate File Path</returns>
		string? GetFilePhysicalPath(string? fileName, FileTypeEnum fileType);
		/// <summary>
		/// Retrun An Array Of File Path That Is Ready To Embed , If The File Is Not Found
		/// It Will Return null
		/// </summary>
		/// <param fileName="file"></param>
		/// <param fileName="fileName"></param>
		/// <param fileName="isCourse"></param>
		/// <returns>The Appropriate File Path</returns>
		string[]? GetFilesPath(string[] filesName, FileTypeEnum fileType);

		/// <summary>
		/// Retrun A File Path That Is Ready To Embed , If The File Is Not Found
		/// It Will Return null
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="fileType"></param>
		/// <returns></returns>
		string? GetFileViewPath(string? fileName, FileTypeEnum fileType);
	}
}
