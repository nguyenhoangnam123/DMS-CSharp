using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class GeneralKpiCriteriaMapping : DataEntity,  IEquatable<GeneralKpiCriteriaMapping>
    {
        public long GeneralKpiId { get; set; }
        public long GeneralCriteriaId { get; set; }
        public long? M01 { get; set; }
        public long? M02 { get; set; }
        public long? M03 { get; set; }
        public long? M04 { get; set; }
        public long? M05 { get; set; }
        public long? M06 { get; set; }
        public long? M07 { get; set; }
        public long? M08 { get; set; }
        public long? M09 { get; set; }
        public long? M10 { get; set; }
        public long? M11 { get; set; }
        public long? M12 { get; set; }
        public long? Q01 { get; set; }
        public long? Q02 { get; set; }
        public long? Q03 { get; set; }
        public long? Q04 { get; set; }
        public long? Y01 { get; set; }
        public long StatusId { get; set; }
        public GeneralCriteria GeneralCriteria { get; set; }
        public GeneralKpi GeneralKpi { get; set; }
        public Status Status { get; set; }

        public bool Equals(GeneralKpiCriteriaMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class GeneralKpiCriteriaMappingFilter : FilterEntity
    {
        public IdFilter GeneralKpiId { get; set; }
        public IdFilter GeneralCriteriaId { get; set; }
        public LongFilter M01 { get; set; }
        public LongFilter M02 { get; set; }
        public LongFilter M03 { get; set; }
        public LongFilter M04 { get; set; }
        public LongFilter M05 { get; set; }
        public LongFilter M06 { get; set; }
        public LongFilter M07 { get; set; }
        public LongFilter M08 { get; set; }
        public LongFilter M09 { get; set; }
        public LongFilter M10 { get; set; }
        public LongFilter M11 { get; set; }
        public LongFilter M12 { get; set; }
        public LongFilter Q01 { get; set; }
        public LongFilter Q02 { get; set; }
        public LongFilter Q03 { get; set; }
        public LongFilter Q04 { get; set; }
        public LongFilter Y01 { get; set; }
        public IdFilter StatusId { get; set; }
        public List<GeneralKpiCriteriaMappingFilter> OrFilter { get; set; }
        public GeneralKpiCriteriaMappingOrder OrderBy {get; set;}
        public GeneralKpiCriteriaMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GeneralKpiCriteriaMappingOrder
    {
        GeneralKpi = 0,
        GeneralCriteria = 1,
        M01 = 2,
        M02 = 3,
        M03 = 4,
        M04 = 5,
        M05 = 6,
        M06 = 7,
        M07 = 8,
        M08 = 9,
        M09 = 10,
        M10 = 11,
        M11 = 12,
        M12 = 13,
        Q01 = 14,
        Q02 = 15,
        Q03 = 16,
        Q04 = 17,
        Y01 = 18,
        Status = 19,
    }

    [Flags]
    public enum GeneralKpiCriteriaMappingSelect:long
    {
        ALL = E.ALL,
        GeneralKpi = E._0,
        GeneralCriteria = E._1,
        M01 = E._2,
        M02 = E._3,
        M03 = E._4,
        M04 = E._5,
        M05 = E._6,
        M06 = E._7,
        M07 = E._8,
        M08 = E._9,
        M09 = E._10,
        M10 = E._11,
        M11 = E._12,
        M12 = E._13,
        Q01 = E._14,
        Q02 = E._15,
        Q03 = E._16,
        Q04 = E._17,
        Y01 = E._18,
        Status = E._19,
    }
}
