namespace CinemaDashboard.Services;

public interface IImageService



{ Task<string?> SaveAsync(IFormFile? image, string folder, CancellationToken cancellationToken = default); void Delete(string? relativePath); }