using Microsoft.AspNetCore.Http;

namespace Core.Application.Interfaces;

public interface IStorageService
{
    Task<string> UploadFileAsync(IFormFile file, string containerName);
    Task DeleteFileAsync(string filePath);
} 