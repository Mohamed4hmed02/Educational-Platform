using Educational_Platform.Application.Extensions;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.StorageAbstractions;
using Educational_Platform.Domain.Enums;
using Educational_Platform.Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Educational_Platform.Infrastructure.Implementations.Storages
{
    public class HostStorageServices : IStorageService
	{
		private readonly string _imagesWritePath;
		private readonly string _referencesWritePath;
		private readonly string _jsonsWritePath;
		private readonly string _imagesViewPath;
		private readonly string _referencesViewPath;
		private readonly string _jsonsViewPath;
		public HostStorageServices(IOptionsMonitor<HostStorageOptions> options)
		{
			_imagesWritePath = options.CurrentValue.RootPath + options.CurrentValue.ImagesFolderPath;
			_referencesWritePath = options.CurrentValue.RootPath + options.CurrentValue.ReferencesFolderPath;
			_jsonsWritePath = options.CurrentValue.RootPath + options.CurrentValue.JsonsFolderPath;
			_imagesViewPath = options.CurrentValue.DomainName + options.CurrentValue.ImagesFolderPath.Replace('\\', '/');
			_referencesViewPath = options.CurrentValue.DomainName + options.CurrentValue.ReferencesFolderPath.Replace('\\', '/');
			_jsonsViewPath = options.CurrentValue.DomainName + options.CurrentValue.JsonsFolderPath.Replace('\\', '/');

			if (!Directory.Exists(_imagesWritePath))
				Directory.CreateDirectory(_imagesWritePath);

			if (!Directory.Exists(_referencesWritePath))
				Directory.CreateDirectory(_referencesWritePath);

			if (!Directory.Exists(_jsonsWritePath))
				Directory.CreateDirectory(_jsonsWritePath);
		}

		public Task DeleteFileAsync(string? fileName, FileTypeEnum fileType)
		{
			if (fileName == null)
				return Task.CompletedTask;

			if (fileType == FileTypeEnum.Image && File.Exists(_imagesWritePath + fileName))
				File.Delete(_imagesWritePath + fileName);

			else if (fileType == FileTypeEnum.Pdf && File.Exists(_referencesWritePath + fileName))
				File.Delete(_referencesWritePath + fileName);

			else if (fileType == FileTypeEnum.Json && File.Exists(_jsonsWritePath + fileName))
				File.Delete(_jsonsWritePath + fileName);

			return Task.CompletedTask;
		}

		public string? GetFilePhysicalPath(string? fileName, FileTypeEnum fileType)
		{
			if (fileName == null)
				return null;

			if (fileType == FileTypeEnum.Image && File.Exists(_imagesWritePath + fileName))
				return _imagesWritePath + fileName;
			else if (fileType == FileTypeEnum.Pdf && File.Exists(_referencesWritePath + fileName))
				return _referencesWritePath + fileName;
			else if (fileType == FileTypeEnum.Json && File.Exists(_jsonsWritePath + fileName))
				return _jsonsWritePath + fileName;

			return null;
		}

		public string? GetFileViewPath(string? fileName, FileTypeEnum fileType)
		{
			if (fileName == null)
				return null;

			if (fileType == FileTypeEnum.Image && File.Exists(_imagesWritePath + fileName))
				return _imagesViewPath + fileName;
			else if (fileType == FileTypeEnum.Pdf && File.Exists(_referencesWritePath + fileName))
				return _referencesViewPath + fileName;
			else if (fileType == FileTypeEnum.Json && File.Exists(_jsonsWritePath + fileName))
				return _jsonsViewPath + fileName;

			return null;
		}

		public string[]? GetFilesPath(string[] filesName, FileTypeEnum fileType)
		{
			if (filesName.IsNullOrEmpty())
				return null;

			string?[] paths = new string[filesName.Length];
			for (int i = 0; i < filesName.Length; i++)
				paths[i] = GetFileViewPath(filesName[i], fileType);

			return paths;
		}

		public async ValueTask<string> SaveFileAsync(IFormFile file, string fileName, FileTypeEnum fileType)
		{
			string extenion = "." + file.ContentType.Split('/')[1];
			await DeleteFileAsync(fileName + extenion, fileType);

			var stream = file.OpenReadStream();
			byte[] bytes = new byte[stream.Length];
			stream.Read(bytes);

			if (fileType == FileTypeEnum.Image)
				await File.WriteAllBytesAsync(_imagesWritePath + fileName + extenion, bytes);
			else if (fileType == FileTypeEnum.Pdf)
				await File.WriteAllBytesAsync(_referencesWritePath + fileName + extenion, bytes);
			else if (fileType == FileTypeEnum.Json)
				await File.WriteAllBytesAsync(_jsonsWritePath + fileName + extenion, bytes);

			return fileName + extenion;
		}

		public async ValueTask<string> SaveFileAsync(Stream fileStream, string fileFullName, FileTypeEnum fileType)
		{
			if (Path.GetExtension(fileFullName) == null)
				return "File has no extension";

			byte[] bytes = new byte[fileStream.Length];
			await fileStream.ReadAsync(bytes);

			if (fileType == FileTypeEnum.Image)
				await File.WriteAllBytesAsync(_imagesWritePath + fileFullName, bytes);
			else if (fileType == FileTypeEnum.Pdf)
				await File.WriteAllBytesAsync(_referencesWritePath + fileFullName, bytes);
			else if (fileType == FileTypeEnum.Json)
				await File.WriteAllBytesAsync(_jsonsWritePath + fileFullName, bytes);

			return fileFullName;
		}
	}
}
