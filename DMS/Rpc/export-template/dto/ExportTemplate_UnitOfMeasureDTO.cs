using DMS.Common;
using DMS.Entities;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DMS.Rpc.export_template.dto
{
    public class ExportTemplate_UnitOfMeasureDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}