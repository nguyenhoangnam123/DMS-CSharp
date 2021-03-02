using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile.general_mobile
{
    public class GeneralMobile_StoreReportDTO : DataDTO
    {
        public long ProblemCounter { get; set; }
        public long ImageCounter { get; set; }
        public long SurveyResultCounter { get; set; }
    }

    public class GeneralMobile_StoreReportFilterDTO : FilterDTO 
    {
        public IdFilter StoreId { get; set; }
        public DateFilter Date { get; set; }
    }
}
