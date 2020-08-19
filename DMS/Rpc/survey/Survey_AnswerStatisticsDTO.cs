using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.survey
{
    public class Survey_AnswerStatisticsDTO : DataDTO
    {
        public long StoreCounter { get; set; }
        public long StoreScoutingCounter { get; set; }
        public long OtherCounter { get; set; }
        public List<Survey_StoreResultStatisticsDTO> StoreResults { get; set; }
        public List<Survey_StoreScoutingResultStatisticsDTO> StoreScoutingResults { get; set; }
        public List<Survey_OtherStatisticsDTO> OtherResults { get; set; }

    }

    public class Survey_StoreResultStatisticsDTO : DataDTO
    {
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string OrganizationName { get; set; }
    }

    public class Survey_StoreScoutingResultStatisticsDTO : DataDTO
    {
        public string StoreScoutingCode { get; set; }
        public string StoreScoutingName { get; set; }
        public string OrganizationName { get; set; }
    }

    public class Survey_OtherStatisticsDTO : DataDTO
    {
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}
