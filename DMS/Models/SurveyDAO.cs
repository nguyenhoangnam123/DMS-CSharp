using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class SurveyDAO
    {
        public SurveyDAO()
        {
            SurveyQuestions = new HashSet<SurveyQuestionDAO>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<SurveyQuestionDAO> SurveyQuestions { get; set; }
    }
}
