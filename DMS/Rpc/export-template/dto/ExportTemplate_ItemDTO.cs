using DMS.Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.export_template.dto
{
    public class ExportTemplate_ItemDTO : DataDTO
    {

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public ExportTemplate_ProductDTO Product { get; set; }
    }
}