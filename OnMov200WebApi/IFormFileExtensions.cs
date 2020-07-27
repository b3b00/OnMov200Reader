using Microsoft.AspNetCore.Http;

namespace OnMov200WebApi
{
    public static class IFormFileExtensions
    {
        public static string NameWithoutExtension(this IFormFile file)
        {
            string name = file.FileName;
            string nameWithoutExt = name.Substring(0, name.Length() - 4);
            return nameWithoutExt;
        }
        
        public static string Extension(this IFormFile file)
        {
            string name = file.FileName;
            int index = name.LastIndexOf(".");
            string extension = name.Substring(index);
            return extension;
        }
    }
}