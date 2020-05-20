using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class Survey : DataEntity,  IEquatable<Survey>
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public long StatusId { get; set; }
        public List<SurveyQuestion> SurveyQuestions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool Equals(Survey other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class SurveyFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Title { get; set; }
        public StringFilter Description { get; set; }
        public DateFilter StartAt { get; set; }
        public DateFilter EndAt { get; set; }
        public IdFilter StatusId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<SurveyFilter> OrFilter { get; set; }
        public SurveyOrder OrderBy {get; set;}
        public SurveySelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SurveyOrder
    {
        Id = 0,
        Title = 1,
        Description = 2,
        StartAt = 3,
        EndAt = 4,
        Status = 5,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum SurveySelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Title = E._1,
        Description = E._2,
        StartAt = E._3,
        EndAt = E._4,
        Status = E._5,
    }
}
