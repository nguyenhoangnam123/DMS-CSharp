using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MRole
{
    public interface IPermissionService : IServiceScoped
    {
        Task<int> Count(PermissionFilter PermissionFilter);
        Task<List<Permission>> List(PermissionFilter PermissionFilter);
        Task<Permission> Get(long Id);
        Task<Permission> Create(Permission Permission);
        Task<Permission> Update(Permission Permission);
        Task<Permission> Delete(Permission Permission);
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

        public async Task<int> Count(PermissionFilter PermissionFilter)
        {
            return await UOW.PermissionRepository.Count(PermissionFilter);
        }

        public async Task<List<Permission>> List(PermissionFilter PermissionFilter)
        {
            List<Permission> Permissions = await UOW.PermissionRepository.List(PermissionFilter);
            return Permissions;
        }
        public async Task<Permission> Get(long Id)
        {
            try
            {
                Permission Permission = await UOW.PermissionRepository.Get(Id);
                return Permission;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(RoleService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(RoleService));
                    throw new MessageException(ex.InnerException);
                }
            }
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(RoleService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(RoleService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Permission> Update(Permission Permission)
        {
            try
            {
                await UOW.PermissionRepository.Update(Permission);
                return await UOW.PermissionRepository.Get(Permission.Id);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(RoleService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(RoleService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Permission> Delete(Permission Permission)
        {
            try
            {
                await UOW.PermissionRepository.Delete(Permission);
                return await UOW.PermissionRepository.Get(Permission.Id);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(RoleService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(RoleService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }


    }
}
