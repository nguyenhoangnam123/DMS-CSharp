using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.position
{
    public class PositionRoute : Root
    {
        public const string Master = Module + "/position/position-master";
        public const string Detail = Module + "/position/position-detail";
        private const string Default = Rpc + Module + "/position";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";

        public const string FilterListStatus = Default + "/filter-list-status";
        public const string SingleListStatus = Default + "/single-list-status";

        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(PositionFilter.Id), FieldType.ID },
            { nameof(PositionFilter.Code), FieldType.STRING },
            { nameof(PositionFilter.Name), FieldType.STRING },
            { nameof(PositionFilter.StatusId), FieldType.ID },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, FilterListStatus, SingleListStatus, } },
        };
    }
}
