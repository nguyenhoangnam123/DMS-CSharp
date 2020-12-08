using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using DMS.Common;
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
            byte[] arr = System.IO.File.ReadAllBytes("Templates/DHGT.docx");
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            var documentConverter = new DocumentConverter(input, DocumentFormat.Docx);

            // Convert "InputFile.docx" to Pdf written to outputStream
            documentConverter.ConvertTo(output, DocumentFormat.Pdf);
            ContentDisposition cd = new ContentDisposition
            {
                FileName = "DHGT.pdf",
                Inline = true,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return File(output.ToArray(), "application/pdf;charset=utf-8");
        }
    }
}
