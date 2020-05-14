using System;

namespace Helpers
{
    public class StaticParams
    {
        public static DateTime DateTimeNow => DateTime.UtcNow.AddHours(7);
        public static DateTime DateTimeMin => DateTime.MinValue;
        public static string ExcelFileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public static string ModuleName = "DMS";
    }
}
