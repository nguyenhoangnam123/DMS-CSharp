﻿using Common;
using DMS.Entities;

namespace DMS.Rpc.kpi_tracking.kpi_item_report
{
    public class KpiItemReport_ItemDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public KpiItemReport_ItemDTO(Item item)
        {
            this.Id = item.Id;
            this.Code = item.Code;
            this.Name = item.Name;
        }
    }

    public class KpiItemReport_ItemFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
    }
}
