using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Repositories;
using DMS.ABE.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.ABE.Services.MRole
{
    public interface IPermissionService : IServiceScoped
    {
        Task<int> Count(PermissionFilter PermissionFilter);
        Task<List<Permission>> List(PermissionFilter PermissionFilter);
        Task<Permission> Get(long Id);
        Task<List<string>> ListPath(long AppUserId);
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
            try
            {
                return await UOW.PermissionRepository.Count(PermissionFilter);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PermissionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Permission>> List(PermissionFilter PermissionFilter)
        {
            try
            {
                List<Permission> Permissions = await UOW.PermissionRepository.List(PermissionFilter);
                return Permissions;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PermissionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionService));
                    throw new MessageException(ex.InnerException);
                };
            }
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
                    await Logging.CreateSystemLog(ex, nameof(PermissionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<string>> ListPath(long AppUserId)
        {
            try
            {
                List<string> Paths = await UOW.PermissionRepository.ListPath(AppUserId);
                return Paths;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PermissionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
