using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Entities
{
    public class SurveyResultText
    {
        public long SurveyResultId { get; set; }
        public long SurveyQuestionId { get; set; }
        public string Content { get; set; }

        public virtual SurveyQuestion SurveyQuestion { get; set; }
        public virtual SurveyResult SurveyResult { get; set; }
    }
}
