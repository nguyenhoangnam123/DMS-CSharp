using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class SurveyResultCellDAO
    {
        public long Id { get; set; }
        public long SurveyQuestionId { get; set; }
        public long AppUserId { get; set; }
        public long RowOptionId { get; set; }
        public long ColumnOptionId { get; set; }
        public long StoreId { get; set; }
        public DateTime Time { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual SurveyOptionDAO ColumnOption { get; set; }
        public virtual SurveyOptionDAO RowOption { get; set; }
        public virtual StoreDAO Store { get; set; }
        public virtual SurveyQuestionDAO SurveyQuestion { get; set; }
    }
}
