 namespace TechMove.Web.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<(string filePath, string fileName)> SaveFileAsync(IFormFile file)
        {
            if (!IsValidPdf(file))
                throw new InvalidOperationException("Only PDF files are allowed.");

            // UUID naming to prevent overwrites
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            // Create folder if it doesn't exist
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (filePath, uniqueFileName);
        }

        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public bool IsValidPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return extension == ".pdf";
        }
    }
}
