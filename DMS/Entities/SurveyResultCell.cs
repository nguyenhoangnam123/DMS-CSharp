using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class SurveyResultCell : DataEntity,  IEquatable<SurveyResultCell>
    {
        public long Id { get; set; }
        public long SurveyQuestionId { get; set; }
        public long AppUserId { get; set; }
        public long RowOptionId { get; set; }
        public long ColumnOptionId { get; set; }
        public DateTime Time { get; set; }
        public AppUser AppUser { get; set; }
        public SurveyOption ColumnOption { get; set; }
        public SurveyOption RowOption { get; set; }
        public SurveyQuestion SurveyQuestion { get; set; }

        public bool Equals(SurveyResultCell other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class SurveyResultCellFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter SurveyQuestionId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter RowOptionId { get; set; }
        public IdFilter ColumnOptionId { get; set; }
        public DateFilter Time { get; set; }
        public List<SurveyResultCellFilter> OrFilter { get; set; }
        public SurveyResultCellOrder OrderBy {get; set;}
        public SurveyResultCellSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SurveyResultCellOrder
    {
        Id = 0,
        SurveyQuestion = 1,
        AppUser = 2,
        RowOption = 3,
        ColumnOption = 4,
        Time = 5,
    }

    [Flags]
    public enum SurveyResultCellSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        SurveyQuestion = E._1,
        AppUser = E._2,
        RowOption = E._3,
        ColumnOption = E._4,
        Time = E._5,
    }
}
