﻿using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.export_template.dto
{
    public class ExportTemplate_RequestStateDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
