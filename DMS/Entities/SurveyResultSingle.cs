using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class SurveyResultSingle : DataEntity,  IEquatable<SurveyResultSingle>
    {
        public long Id { get; set; }
        public long SurveyQuestionId { get; set; }
        public long SurveyOptionId { get; set; }
        public long AppUserId { get; set; }
        public DateTime Time { get; set; }
        public AppUser AppUser { get; set; }
        public SurveyOption SurveyOption { get; set; }
        public SurveyQuestion SurveyQuestion { get; set; }

        public bool Equals(SurveyResultSingle other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class SurveyResultSingleFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter SurveyQuestionId { get; set; }
        public IdFilter SurveyOptionId { get; set; }
        public IdFilter AppUserId { get; set; }
        public DateFilter Time { get; set; }
        public List<SurveyResultSingleFilter> OrFilter { get; set; }
        public SurveyResultSingleOrder OrderBy {get; set;}
        public SurveyResultSingleSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SurveyResultSingleOrder
    {
        Id = 0,
        SurveyQuestion = 1,
        SurveyOption = 2,
        AppUser = 3,
        Time = 4,
    }

    [Flags]
    public enum SurveyResultSingleSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        SurveyQuestion = E._1,
        SurveyOption = E._2,
        AppUser = E._3,
        Time = E._4,
    }
}
