using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class SurveyQuestionDAO
    {
        public SurveyQuestionDAO()
        {
            SurveyOptions = new HashSet<SurveyOptionDAO>();
        }

        public long Id { get; set; }
        public long SurveyId { get; set; }
        public string Content { get; set; }
        public long SurveyQuestionTypeId { get; set; }
        public bool IsMandatory { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual SurveyDAO Survey { get; set; }
        public virtual SurveyQuestionTypeDAO SurveyQuestionType { get; set; }
        public virtual ICollection<SurveyOptionDAO> SurveyOptions { get; set; }
    }
}
