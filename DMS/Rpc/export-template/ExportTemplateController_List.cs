using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MExportTemplate;
using DMS.Services.MByte[];

namespace DMS.Rpc.export_template
{
    public partial class ExportTemplateController : RpcController
    {
    }
}

