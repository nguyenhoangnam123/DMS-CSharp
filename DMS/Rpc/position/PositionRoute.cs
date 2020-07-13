using Common;
using System.Collections.Generic;

namespace DMS.Rpc.position
{
    public class PositionRoute : Root
    {
        public const string Master = Module + "/account/position/position-master";
        public const string Detail = Module + "/account/position/position-detail/*";
        private const string Default = Rpc + Module + "/position";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";

        public const string FilterListStatus = Default + "/filter-list-status";
        public const string SingleListStatus = Default + "/single-list-status";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, FilterListStatus, SingleListStatus, } },
        };
    }
}
