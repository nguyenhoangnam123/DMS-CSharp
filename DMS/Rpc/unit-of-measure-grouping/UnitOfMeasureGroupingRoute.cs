using Common;
using DMS.Entities;
using System.Collections.Generic;

namespace DMS.Rpc.unit_of_measure_grouping
{
    public class UnitOfMeasureGroupingRoute : Root
    {
        public const string Master = Module + "/unit-of-measure-grouping/unit-of-measure-grouping-master";
        public const string Detail = Module + "/unit-of-measure-grouping/unit-of-measure-grouping-detail";
        private const string Default = Rpc + Module + "/unit-of-measure-grouping";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(UnitOfMeasureGroupingFilter.Code), FieldType.STRING },
            { nameof(UnitOfMeasureGroupingFilter.Name), FieldType.STRING },
            { nameof(UnitOfMeasureGroupingFilter.Description), FieldType.STRING },
            { nameof(UnitOfMeasureGroupingFilter.UnitOfMeasureId), FieldType.ID },
            { nameof(UnitOfMeasureGroupingFilter.StatusId), FieldType.ID },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get } },
            { "Thêm", new List<string> { 
                Master, Count, List, Get,
                Detail, Create, 
                SingleListStatus, SingleListUnitOfMeasure, } },
            { "Sửa", new List<string> {
                Master, Count, List, Get, 
                Detail, Update,
                SingleListStatus, SingleListUnitOfMeasure, } },
            { "Xoá", new List<string> {
                Master, Count, List, Get, 
                Detail, Delete,  
                SingleListStatus, SingleListUnitOfMeasure, } },
            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, 
                BulkDelete } },
        };
    }
}
