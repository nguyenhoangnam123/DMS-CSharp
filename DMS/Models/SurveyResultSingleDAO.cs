using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class SurveyResultSingleDAO
    {
        public long Id { get; set; }
        public long SurveyQuestionId { get; set; }
        public long SurveyOptionId { get; set; }
        public long AppUserId { get; set; }
        public long StoreId { get; set; }
        public DateTime Time { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual StoreDAO Store { get; set; }
        public virtual SurveyOptionDAO SurveyOption { get; set; }
        public virtual SurveyQuestionDAO SurveyQuestion { get; set; }
    }
}
