using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class SurveyOptionDAO
    {
        public long Id { get; set; }
        public long SurveyQuestionId { get; set; }
        public long SurveyOptionTypeId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual SurveyOptionTypeDAO SurveyOptionType { get; set; }
        public virtual SurveyQuestionDAO SurveyQuestion { get; set; }
    }
}
