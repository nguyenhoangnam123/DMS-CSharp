using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;
using System.ComponentModel;

namespace DMS.Rpc.mobile
{
    public class Mobile_KpiGeneralContentDTO : DataDTO
    {
        public long Id { get; set; }
        public long KpiGeneralId { get; set; }
        public long KpiCriteriaGeneralId { get; set; }
        public long StatusId { get; set; }
        public Mobile_KpiCriteriaGeneralDTO KpiCriteriaGeneral { get; set; }   
        public Mobile_StatusDTO Status { get; set; }
        public Dictionary<long, decimal?> KpiGeneralContentKpiPeriodMappings { get; set; }
        public Dictionary<long, bool> KpiGeneralContentKpiPeriodMappingEnables { get; set; }


        public Mobile_KpiGeneralContentDTO() {}
        public Mobile_KpiGeneralContentDTO(KpiGeneralContent KpiGeneralContent)
        {
            this.Id = KpiGeneralContent.Id;
            this.KpiGeneralId = KpiGeneralContent.KpiGeneralId;
            this.KpiCriteriaGeneralId = KpiGeneralContent.KpiCriteriaGeneralId;
            this.StatusId = KpiGeneralContent.StatusId;
            this.KpiCriteriaGeneral = KpiGeneralContent.KpiCriteriaGeneral == null ? null : new Mobile_KpiCriteriaGeneralDTO(KpiGeneralContent.KpiCriteriaGeneral);
            this.Status = KpiGeneralContent.Status == null ? null : new Mobile_StatusDTO(KpiGeneralContent.Status);
            this.KpiGeneralContentKpiPeriodMappings = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings?.ToDictionary(x => x.KpiPeriodId, y => y.Value);
        }
    }

    public class Mobile_KpiGeneralContentFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter KpiGeneralId { get; set; }
        
        public IdFilter KpiCriteriaGeneralId { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public KpiGeneralContentOrder OrderBy { get; set; }
    }
}