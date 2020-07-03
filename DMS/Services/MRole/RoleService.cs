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
    public interface IRoleService : IServiceScoped
    {
        Task<int> Count(RoleFilter RoleFilter);
        Task<List<Role>> List(RoleFilter RoleFilter);
        Task<Role> Get(long Id);
        Task<Role> Create(Role Role);
        Task<Role> Clone(long Id);
        Task<Role> Update(Role Role);
        Task<Role> AssignAppUser(Role Role);
        Task<Role> Delete(Role Role);
        Task<List<Role>> BulkDelete(List<Role> Roles);
        Task<List<Role>> Import(List<Role> Roles);
        RoleFilter ToFilter(RoleFilter RoleFilter);
    }

    public class RoleService : BaseService, IRoleService
    {
        private IUOW UOW;
        private ILogging Logging;
        private IPermissionRepository PermissionRepository;
        private ICurrentContext CurrentContext;
        private IRoleValidator RoleValidator;

        public RoleService(
            IUOW UOW,
            ILogging Logging,
            IPermissionRepository PermissionRepository,
            ICurrentContext CurrentContext,
            IRoleValidator RoleValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.PermissionRepository = PermissionRepository;
            this.CurrentContext = CurrentContext;
            this.RoleValidator = RoleValidator;
        }
        public async Task<int> Count(RoleFilter RoleFilter)
        {
            try
            {
                int result = await UOW.RoleRepository.Count(RoleFilter);
                return result;
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

        public async Task<List<Role>> List(RoleFilter RoleFilter)
        {
            try
            {
                List<Role> Roles = await UOW.RoleRepository.List(RoleFilter);
                return Roles;
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
        public async Task<Role> Get(long Id)
        {
            Role Role = await UOW.RoleRepository.Get(Id);
            if (Role == null)
                return null;
            return Role;
        }

        public async Task<Role> Clone(long Id)
        {
            try
            {
                Role Role = await UOW.RoleRepository.Get(Id);
                var listRolesInDb = await UOW.RoleRepository.List(new RoleFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Code = new StringFilter { StartWith = Role.Code + "_Clone" },
                    Name = new StringFilter { StartWith = Role.Name + "_Clone" },
                    Selects = RoleSelect.ALL
                });

                var listPermissionsInDb = await UOW.PermissionRepository.List(new PermissionFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    RoleId = new IdFilter { Equal = Role .Id},
                    Selects = PermissionSelect.ALL
                });
               
                for (int index = 1; index < 1000; index++)
                {
                    if (listRolesInDb.Any(x => x.Code == Role.Code + "_Clone" + index.ToString() && x.Name == Role.Name + "_Clone" + index.ToString()))
                        continue;
                    Role.Code = Role.Code + "_Clone" + index.ToString();
                    Role.Name = Role.Name + "_Clone" + index.ToString();
                    break;
                }
                Role.Id = 0;
                await UOW.Begin();
                await UOW.RoleRepository.Create(Role);
                foreach(Permission Permission in listPermissionsInDb)
                {
                    Permission.RoleId = Role.Id;
                }
                await PermissionRepository.BulkMerge(listPermissionsInDb);
                await UOW.Commit();

                await Logging.CreateAuditLog(Role, new { }, nameof(RoleService));
                return await UOW.RoleRepository.Get(Role.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
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
        public async Task<Role> Create(Role Role)
        {
            if (!await RoleValidator.Create(Role))
                return Role;

            try
            {
                Role.Id = 0;
                await UOW.Begin();
                await UOW.RoleRepository.Create(Role);
                await UOW.Commit();

                await Logging.CreateAuditLog(Role, new { }, nameof(RoleService));
                return await UOW.RoleRepository.Get(Role.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
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

        public async Task<Role> Update(Role Role)
        {
            if (!await RoleValidator.Update(Role))
                return Role;
            try
            {
                var oldData = await UOW.RoleRepository.Get(Role.Id);

                await UOW.Begin();
                Role.AppUserRoleMappings = oldData.AppUserRoleMappings;
                await UOW.RoleRepository.Update(Role);
                await UOW.Commit();

                var newData = await UOW.RoleRepository.Get(Role.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(RoleService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
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

        public async Task<Role> AssignAppUser(Role Role)
        {
            if (!await RoleValidator.AssignAppUser(Role))
                return Role;
            try
            {
                var oldData = await UOW.RoleRepository.Get(Role.Id);
                oldData.AppUserRoleMappings = Role.AppUserRoleMappings;
                await UOW.Begin();
                await UOW.RoleRepository.Update(oldData);
                await UOW.Commit();

                var newData = await UOW.RoleRepository.Get(Role.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(RoleService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
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

        public async Task<Role> Delete(Role Role)
        {
            if (!await RoleValidator.Delete(Role))
                return Role;

            try
            {
                await UOW.Begin();
                await UOW.RoleRepository.Delete(Role);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Role, nameof(RoleService));
                return Role;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
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

        public async Task<List<Role>> BulkDelete(List<Role> Roles)
        {
            if (!await RoleValidator.BulkDelete(Roles))
                return Roles;

            try
            {
                await UOW.Begin();
                await UOW.RoleRepository.BulkDelete(Roles);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Roles, nameof(RoleService));
                return Roles;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
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

        public async Task<List<Role>> Import(List<Role> Roles)
        {
            if (!await RoleValidator.Import(Roles))
                return Roles;

            try
            {
                Roles.ForEach(r => r.Id = 0);
                await UOW.Begin();
                await UOW.RoleRepository.BulkMerge(Roles);
                var listRolesInDb = await UOW.RoleRepository.List(new RoleFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = RoleSelect.ALL
                });

                List<Permission> Permissions = new List<Permission>();
                foreach (var Role in Roles)
                {
                    var r = listRolesInDb.Where(r => r.Code == Role.Code).FirstOrDefault();
                    Role.Id = r != null ? r.Id : 0;
                    if (Role.Permissions.Any())
                    {
                        foreach (var Permission in Role.Permissions)
                        {
                            Permission.RoleId = Role.Id;
                            Permissions.Add(Permission);
                        }
                    }
                }
                var listPermissionInDB = Roles.SelectMany(r => r.Permissions).ToList();
                await UOW.PermissionRepository.BulkDelete(listPermissionInDB);
                await UOW.PermissionRepository.BulkMerge(Permissions);
                await UOW.Commit();

                await Logging.CreateAuditLog(Roles, new { }, nameof(RoleService));
                return Roles;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
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

        public RoleFilter ToFilter(RoleFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<RoleFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                RoleFilter subFilter = new RoleFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                }
            }
            return filter;
        }
    }
}
