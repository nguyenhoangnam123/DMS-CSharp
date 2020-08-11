using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Services.MRole;
using GleamTech.DocumentUltimate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Rpc
{
    public class PermissionController : SimpleController
    {
        private IPermissionService PermissionService;
        private ICurrentContext CurrentContext;
        public PermissionController(IPermissionService PermissionService, ICurrentContext CurrentContext)
        {
            this.PermissionService = PermissionService;
            this.CurrentContext = CurrentContext;
        }

        [HttpPost, Route("rpc/dms/permission/list-path")]
        public async Task<List<string>> ListPath()
        {
            return await PermissionService.ListPath(CurrentContext.UserId);
        }

        [HttpGet, Route("rpc/dms/permission/test-export")]
        public async Task<ActionResult> Export()
        {
            MemoryStream MemoryStream = new MemoryStream();
            var documentConverter = new DocumentConverter("Templates/Store_Export.xlsx");

            // Convert "InputFile.docx" to Pdf written to outputStream
            documentConverter.ConvertTo(MemoryStream, DocumentFormat.Pdf);
            return File(MemoryStream.ToArray(), "application/octet-stream", "Store_Export.pdf");

        }
    }
}
