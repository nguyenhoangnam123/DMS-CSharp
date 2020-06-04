using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class SurveyResult : DataEntity, IEquatable<SurveyResult>
    {
        public long Id { get; set; }
        public long SurveyId { get; set; }
        public long AppUserId { get; set; }
        public long StoreId { get; set; }
        public DateTime Time { get; set; }
        public Guid RowId { get; set; }

        public AppUser AppUser { get; set; }
        public Store Store { get; set; }
        public Survey Survey { get; set; }
        public List<SurveyResultCell> SurveyResultCells { get; set; }
        public List<SurveyResultSingle> SurveyResultSingles { get; set; }
        public bool Equals(SurveyResult other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class SurveyResultFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter SurveyId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter StoreId { get; set; }
        public DateFilter Time { get; set; }
        public List<SurveyFilter> OrFilter { get; set; }
        public SurveyResultOrder OrderBy { get; set; }
        public SurveyResultSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SurveyResultOrder
    {
        Id = 0,
        Survey = 1,
        AppUser = 2,
        Store = 3,
        Time = 4,
    }

    [Flags]
    public enum SurveyResultSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Survey = E._1,
        AppUser = E._2,
        Store = E._3,
        Time = E._4,
    }
}
