using Common;
using System.Collections.Generic;

namespace DMS.Rpc.system_configuration
{
    public class SystemConfigurationRoute : Root
    {
        public const string Master = Module + "/configuration/system-configuration/system-configuration-master";
        private const string Default = Rpc + Module + "/system-configuration";
        public const string Get = Default + "/get";
        public const string Update = Default + "/update";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Quản trị", new List<string> {
                Master, Get, Update} },
        };
    }
}
