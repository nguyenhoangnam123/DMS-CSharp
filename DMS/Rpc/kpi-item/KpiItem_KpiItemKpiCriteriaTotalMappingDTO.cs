﻿using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_item
{
    public class KpiItem_KpiItemKpiCriteriaTotalMappingDTO : DataDTO
    {
        public long KpiItemId { get; set; }
        public long KpiCriteriaTotalId { get; set; }
        public long Value { get; set; }

        public KpiItem_KpiItemKpiCriteriaTotalMappingDTO() { }
        public KpiItem_KpiItemKpiCriteriaTotalMappingDTO(KpiItemKpiCriteriaTotalMapping KpiItemKpiCriteriaTotalMapping)
        {
            this.KpiItemId = KpiItemKpiCriteriaTotalMapping.KpiItemId;
            this.KpiCriteriaTotalId = KpiItemKpiCriteriaTotalMapping.KpiCriteriaTotalId;
            this.Value = KpiItemKpiCriteriaTotalMapping.Value;
        }
    }
}