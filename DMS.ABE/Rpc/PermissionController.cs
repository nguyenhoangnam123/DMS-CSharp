using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using DMS.ABE.Common;
using DMS.ABE.Rpc;
using DMS.ABE.Services.MRole;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMS.ABE.Rpc
{
    public class PermissionController : RpcSimpleController
    {
        private IPermissionService PermissionService;
        private ICurrentContext CurrentContext;
        public PermissionController(IPermissionService PermissionService, ICurrentContext CurrentContext)
        {
            this.PermissionService = PermissionService;
            this.CurrentContext = CurrentContext;
        }

        [HttpPost, Route("rpc/ams/permission/list-path")]
        public async Task<List<string>> ListPath()
        {
            return await PermissionService.ListPath(CurrentContext.UserId);
        }
    }
}
