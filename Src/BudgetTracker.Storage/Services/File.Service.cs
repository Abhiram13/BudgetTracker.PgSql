using Microsoft.AspNetCore.StaticFiles;

namespace BudgetTracker.Storage.Services;

public class FileService
{
    public string GetExtension(string mimeType)
    {
        FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
        string extension = provider.Mappings.FirstOrDefault(e => e.Value.Equals(mimeType, StringComparison.OrdinalIgnoreCase)).Key;
        
        return extension;
    }
}