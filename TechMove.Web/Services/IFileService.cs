namespace TechMove.Web.Services
{
    public interface IFileService
    {
        Task<(string filePath, string fileName)> SaveFileAsync(IFormFile file);
        void DeleteFile(string filePath);
        bool IsValidPdf(IFormFile file);
    }
}
