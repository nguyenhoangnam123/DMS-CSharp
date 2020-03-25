using System;

namespace Helpers
{
    public class StaticParams
    {
        public static DateTime DateTimeNow => DateTime.Now;
        public static DateTime DateTimeMin => DateTime.MinValue;
        public static string ExcelFileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }
}
