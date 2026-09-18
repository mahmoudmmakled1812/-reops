namespace CinemaDashboard.Services;

public class ImageService(IWebHostEnvironment environment) : IImageService
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSize = 5 * 1024 * 1024;

    public async Task<string?> SaveAsync(IFormFile? image, string folder, CancellationToken cancellationToken = default)
    {
        if (image is null || image.Length == 0)
            return null;

        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension) || image.Length > MaxFileSize)
        {
            throw new InvalidOperationException(
                "Use JPG, PNG, or WEBP images smaller than 5 MB."
            );
        }

        var relativeDirectory = Path.Combine("uploads", folder);
        var physicalDirectory = Path.Combine(environment.WebRootPath, relativeDirectory);
        Directory.CreateDirectory(physicalDirectory);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(physicalDirectory, fileName);

        await using var stream = File.Create(filePath);
        await image.CopyToAsync(stream, cancellationToken);

        return "/" + Path.Combine(relativeDirectory, fileName).Replace('\\', '/');
    }

    public void Delete(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return;

        var relative = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var path = Path.GetFullPath(Path.Combine(environment.WebRootPath, relative));

        var webRootPath = Path.GetFullPath(environment.WebRootPath);
        if (path.StartsWith(webRootPath, StringComparison.OrdinalIgnoreCase) && File.Exists(path))
        {
            File.Delete(path);
        }
    }
}