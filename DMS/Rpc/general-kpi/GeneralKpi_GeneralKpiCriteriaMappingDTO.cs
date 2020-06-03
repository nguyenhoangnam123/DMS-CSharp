﻿using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.general_kpi
{
    public class GeneralKpi_GeneralKpiCriteriaMappingDTO : DataDTO
    {
        public long GeneralKpiId { get; set; }
        public long GeneralCriteriaId { get; set; }
        public long? M01 { get; set; }
        public long? M02 { get; set; }
        public long? M03 { get; set; }
        public long? M04 { get; set; }
        public long? M05 { get; set; }
        public long? M06 { get; set; }
        public long? M07 { get; set; }
        public long? M08 { get; set; }
        public long? M09 { get; set; }
        public long? M10 { get; set; }
        public long? M11 { get; set; }
        public long? M12 { get; set; }
        public long? Q01 { get; set; }
        public long? Q02 { get; set; }
        public long? Q03 { get; set; }
        public long? Q04 { get; set; }
        public long? Y01 { get; set; }
        public long StatusId { get; set; }
        public GeneralKpi_GeneralCriteriaDTO GeneralCriteria { get; set; }
        public GeneralKpi_GeneralKpiDTO GeneralKpi { get; set; }
        public GeneralKpi_StatusDTO Status { get; set; }
        public GeneralKpi_GeneralKpiCriteriaMappingDTO() { }
        public GeneralKpi_GeneralKpiCriteriaMappingDTO(GeneralKpiCriteriaMapping GeneralKpiCriteriaMapping)
        {
            this.GeneralKpiId = GeneralKpiCriteriaMapping.GeneralKpiId;
            this.GeneralCriteriaId = GeneralKpiCriteriaMapping.GeneralCriteriaId;
            this.M01 = GeneralKpiCriteriaMapping.M01;
            this.M02 = GeneralKpiCriteriaMapping.M02;
            this.M03 = GeneralKpiCriteriaMapping.M03;
            this.M04 = GeneralKpiCriteriaMapping.M04;
            this.M05 = GeneralKpiCriteriaMapping.M05;
            this.M06 = GeneralKpiCriteriaMapping.M06;
            this.M07 = GeneralKpiCriteriaMapping.M07;
            this.M08 = GeneralKpiCriteriaMapping.M08;
            this.M09 = GeneralKpiCriteriaMapping.M09;
            this.M10 = GeneralKpiCriteriaMapping.M10;
            this.M11 = GeneralKpiCriteriaMapping.M11;
            this.M12 = GeneralKpiCriteriaMapping.M12;
            this.Q01 = GeneralKpiCriteriaMapping.Q01;
            this.Q02 = GeneralKpiCriteriaMapping.Q02;
            this.Q03 = GeneralKpiCriteriaMapping.Q03;
            this.Q04 = GeneralKpiCriteriaMapping.Q04;
            this.Y01 = GeneralKpiCriteriaMapping.Y01;
            this.StatusId = GeneralKpiCriteriaMapping.StatusId;
            this.GeneralCriteria = GeneralKpiCriteriaMapping.GeneralCriteria == null ? null : new GeneralKpi_GeneralCriteriaDTO(GeneralKpiCriteriaMapping.GeneralCriteria);
            this.GeneralKpi = GeneralKpiCriteriaMapping.GeneralKpi == null ? null : new GeneralKpi_GeneralKpiDTO(GeneralKpiCriteriaMapping.GeneralKpi);
            this.Status = GeneralKpiCriteriaMapping.Status == null ? null : new GeneralKpi_StatusDTO(GeneralKpiCriteriaMapping.Status);
        }
    }
}