using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Services.MRole;
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
    }
}
