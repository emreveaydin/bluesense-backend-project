using Core.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class LocalStorageService : IStorageService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public LocalStorageService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string containerName)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is not selected or empty.");
        }

        var containerPath = Path.Combine(_webHostEnvironment.WebRootPath, containerName);
        if (!Directory.Exists(containerPath))
        {
            Directory.CreateDirectory(containerPath);
        }

        var fileExtension = Path.GetExtension(file.FileName);
        var newFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(containerPath, newFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return the relative path to be stored in the database
        return Path.Combine(containerName, newFileName).Replace("\\", "/");
    }
    
    public Task DeleteFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        return Task.CompletedTask;
    }
} 