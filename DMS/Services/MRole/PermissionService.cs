using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
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
        private ILogging Logging;
        public PermissionService(IUOW UOW, ILogging Logging)
        {
            this.UOW = UOW;
            this.Logging = Logging;
        }

        public async Task<Permission> Create(Permission Permission)
        {
            try
            {
                await UOW.PermissionRepository.Create(Permission);
                return await UOW.PermissionRepository.Get(Permission.Id);
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(RoleService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
    }
}
