using Common;
using DMS.Entities;
using DMS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MRole
{
    public interface IPermissionService : IServiceScoped
    {
        Task<Permission> Create(Permission Permission);
    }
    public class PermissionService : IPermissionService
    {
        private IUOW UOW;
        public PermissionService(IUOW UOW)
        {
            this.UOW = UOW;
        }

        public async Task<Permission> Create(Permission Permission)
        {
            await UOW.PermissionRepository.Create(Permission);
            return await UOW.PermissionRepository.Get(Permission.Id);
        }
    }
}
