using DMS.ABE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DMS.ABE.Entities
{
    public class SurveyQuestionType : DataEntity, IEquatable<SurveyQuestionType>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public bool Equals(SurveyQuestionType other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class SurveyQuestionTypeFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<SurveyQuestionTypeFilter> OrFilter { get; set; }
        public SurveyQuestionTypeOrder OrderBy { get; set; }
        public SurveyQuestionTypeSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SurveyQuestionTypeOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
    }

    [Flags]
    public enum SurveyQuestionTypeSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
    }
}
