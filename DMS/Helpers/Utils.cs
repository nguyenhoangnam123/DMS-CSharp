using Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Helpers
{
    public static class Utils
    {
        public static string ConvertDateTimeToString(DateTime date)
        {
            return "" + date.Year.ToString() + date.Month + date.Day + date.Hour + date.Minute + date.Second;
        }
        public static string rootFolderImport = Directory.GetCurrentDirectory() + "\\FileImport";

        public static async Task<FileInfo> CreateFileImport(IFormFile file)
        {
            var rootFolder = rootFolderImport;
            var fileName = ConvertDateTimeToString(DateTime.Now) + file.FileName;
            var filePath = Path.Combine(rootFolder, fileName);
            var fileLocation = new FileInfo(filePath);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            if (file.Length <= 0)
                return null; 
            return fileLocation; 
        }
    }
}
