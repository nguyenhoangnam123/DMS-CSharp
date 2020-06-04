using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class SurveyResultDAO
    {
        public SurveyResultDAO()
        {
            SurveyResultCells = new HashSet<SurveyResultCellDAO>();
            SurveyResultSingles = new HashSet<SurveyResultSingleDAO>();
        }

        public long Id { get; set; }
        public long SurveyId { get; set; }
        public long AppUserId { get; set; }
        public long StoreId { get; set; }
        public DateTime Time { get; set; }
        public Guid RowId { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual StoreDAO Store { get; set; }
        public virtual SurveyDAO Survey { get; set; }
        public virtual ICollection<SurveyResultCellDAO> SurveyResultCells { get; set; }
        public virtual ICollection<SurveyResultSingleDAO> SurveyResultSingles { get; set; }
    }
}
