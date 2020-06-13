using Common;
using System.Collections.Generic;

namespace DMS.Rpc.mobile
{
    public class MobileRoute : Root
    {
        public const string Banner = Module + "/mobile/mobile-banner";
        private const string Default = Rpc + Module + "/mobile";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Banner", new List<string>
            {
                Banner,
                Count,List,Get, } 
            },
        };
    }
}
