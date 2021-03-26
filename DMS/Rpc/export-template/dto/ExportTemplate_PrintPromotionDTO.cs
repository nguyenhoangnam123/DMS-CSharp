using DMS.Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.export_template.dto
{
    public class ExportTemplate_PrintPromotionDTO : DataDTO
    {
        public long STT { get; set; }
        public long Id { get; set; }
        public string QuantityString { get; set; }
        public long RequestedQuantity { get; set; }
        public string RequestedQuantityString { get; set; }
        public ExportTemplate_ItemDTO Item { get; set; }
        public ExportTemplate_UnitOfMeasureDTO PrimaryUnitOfMeasure { get; set; }
        public ExportTemplate_UnitOfMeasureDTO UnitOfMeasure { get; set; }
    }
}
