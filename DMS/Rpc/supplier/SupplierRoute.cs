using Common;
using System.Collections.Generic;

namespace DMS.Rpc.supplier
{
    public class SupplierRoute : Root
    {
        public const string Parent = Module + "/partner";
        public const string Master = Module + "/partner/supplier/supplier-master";
        public const string Detail = Module + "/partner/supplier/supplier-detail/*";
        private const string Default = Rpc + Module + "/supplier";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string FilterListStatus = Default + "/filter-list-status";


        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListPersonInCharge = Default + "/single-list-person-in-charge";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListWard = Default + "/single-list-ward";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, Get, 
                FilterListStatus} },
            { "Thêm", new List<string> {
                Parent,
                Master, Count, List, Get, 
                FilterListStatus,
                Detail, Create,
                SingleListDistrict, SingleListPersonInCharge, SingleListProvince, SingleListStatus, SingleListWard, } },
            { "Sửa", new List<string> {
                Parent,
                Master, Count, List, Get, 
                FilterListStatus,
                Detail, Update,
                SingleListDistrict, SingleListPersonInCharge, SingleListProvince, SingleListStatus, SingleListWard, } },
            { "Xoá", new List<string> {
                Parent,
                Master, Count, List, Get, 
                FilterListStatus,
                Detail, Delete,
                SingleListDistrict, SingleListPersonInCharge, SingleListProvince, SingleListStatus, SingleListWard, } },
            { "Xoá nhiều", new List<string> {
                Parent,
                Master, Count, List, Get,
                FilterListStatus,
                BulkDelete } },
        };
    }
}
