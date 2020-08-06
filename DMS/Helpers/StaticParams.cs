using Microsoft.AspNetCore.Antiforgery;
using System;

namespace Helpers
{
    public class StaticParams
    {
        public static int ChangeYear = 0;
        public static DateTime DateTimeNow => DateTime.UtcNow.AddYears(ChangeYear);
        public static DateTime DateTimeMin => DateTime.MinValue;
        public static string ExcelFileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public static string ModuleName = "DMS";
        public static bool EnableExternalService = true;
    }
}
