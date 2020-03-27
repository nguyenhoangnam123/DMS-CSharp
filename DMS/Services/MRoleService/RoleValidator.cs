using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MRole
{
    public interface IRoleValidator : IServiceScoped
    {
        Task<bool> Create(Role Role);
        Task<bool> Update(Role Role);
        Task<bool> Delete(Role Role);
        Task<bool> BulkDelete(List<Role> Roles);
        Task<bool> Import(List<Role> Roles);
    }

    public class RoleValidator : IRoleValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            CodeExisted,
            NameExisted,
            StatusNotExisted,
            PageNotExisted,
            FieldNotExisted
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public RoleValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Role Role)
        {
            RoleFilter RoleFilter = new RoleFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Role.Id },
                Selects = RoleSelect.Id
            };

            int count = await UOW.RoleRepository.Count(RoleFilter);
            if (count == 0)
                Role.AddError(nameof(RoleValidator), nameof(Role.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> ValidateCode(Role Role)
        {
            RoleFilter RoleFilter = new RoleFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { NotEqual = Role.Id },
                Code = new StringFilter { Equal = Role.Code },
                Selects = RoleSelect.Code
            };

            int count = await UOW.RoleRepository.Count(RoleFilter);
            if (count != 0)
                Role.AddError(nameof(RoleValidator), nameof(Role.Code), ErrorCode.CodeExisted);
            return count == 1;
        }

        public async Task<bool> ValidateStatus(Role Role)
        {
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Role.StatusId },
                Selects = StatusSelect.Id
            };
            int count = await UOW.StatusRepository.Count(StatusFilter);
            if (count == 0)
                Role.AddError(nameof(RoleValidator), nameof(Role.Status), ErrorCode.StatusNotExisted);
            return count != 0;
        }

        public async Task<bool> ValidatePermission(Role Role)
        {
            foreach (var Permission in Role.Permissions)
            {
                if (!await ValidatePermissionCode(Permission)) return false;
                if (!await ValidatePermissionStatus(Permission)) return false;
            }
            return true;
        }
        public async Task<bool> ValidatePermissionId(Permission Permission)
        {
            PermissionFilter PermissionFilter = new PermissionFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Permission.Id },
                Selects = PermissionSelect.Id
            };

            int count = await UOW.PermissionRepository.Count(PermissionFilter);
            if (count == 0)
                Permission.AddError(nameof(RoleValidator), nameof(Permission.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }
        public async Task<bool> ValidatePermissionCode(Permission Permission)
        {
            PermissionFilter PermissionFilter = new PermissionFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { NotEqual = Permission.Id },
                Code = new StringFilter { Equal = Permission.Code },
                Selects = PermissionSelect.Code
            };

            int count = await UOW.PermissionRepository.Count(PermissionFilter);
            if (count != 0)
                Permission.AddError(nameof(RoleValidator), nameof(Permission.Code), ErrorCode.CodeExisted);
            return count == 1;
        }

        public async Task<bool> ValidatePermissionStatus(Permission Permission)
        {
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Permission.StatusId },
                Selects = StatusSelect.Id
            };
            int count = await UOW.StatusRepository.Count(StatusFilter);
            if (count == 0)
                Permission.AddError(nameof(RoleValidator), nameof(Permission.Status), ErrorCode.StatusNotExisted);
            return count != 0;
        }
        public async Task<bool> ValidateMenu(Role Role)
        {
            foreach (var Permission in Role.Permissions)
            {
                if (!await ValidateMenuCode(Permission.Menu)) return false;
            }
            return true;
        }
        public async Task<bool> ValidateMenuId(Menu Menu)
        {
            MenuFilter MenuFilter = new MenuFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Menu.Id },
                Selects = MenuSelect.Id
            };

            int count = await UOW.MenuRepository.Count(MenuFilter);
            if (count == 0)
                Menu.AddError(nameof(RoleValidator), nameof(Menu.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> ValidateMenuCode(Menu Menu)
        {
            MenuFilter MenuFilter = new MenuFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { NotEqual = Menu.Id },
                Code = new StringFilter { Equal = Menu.Code },
                Selects = MenuSelect.Code
            };

            int count = await UOW.MenuRepository.Count(MenuFilter);
            if (count != 0)
                Menu.AddError(nameof(RoleValidator), nameof(Menu.Code), ErrorCode.CodeExisted);
            return count == 1;
        }
        public async Task<bool> Create(Role Role)
        {
            await ValidateCode(Role);
            await ValidateStatus(Role);
            return Role.IsValidated;
        }

        public async Task<bool> Update(Role Role)
        {
            if (await ValidateId(Role))
            {
                await ValidateCode(Role);
                await ValidateStatus(Role);
            }
            return Role.IsValidated;
        }

        public async Task<bool> Delete(Role Role)
        {
            if (await ValidateId(Role))
            {
            }
            return Role.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Role> Roles)
        {
            RoleFilter RoleFilter = new RoleFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { In = Roles.Select(a => a.Id).ToList() },
                Selects = RoleSelect.Id
            };

            var listInDB = await UOW.RoleRepository.List(RoleFilter);
            var listExcept = Roles.Except(listInDB);
            if (listExcept == null || listExcept.Count() == 0) return true;
            foreach (var Role in listExcept)
            {
                Role.AddError(nameof(RoleValidator), nameof(Role.Id), ErrorCode.IdNotExisted);
            }
            return false;
        }

        public async Task<bool> Import(List<Role> Roles)
        {
            var listCodeInDB = (await UOW.RoleRepository.List(new RoleFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = RoleSelect.Code
            })).Select(e => e.Code);

            foreach (var Role in Roles)
            {
                ;
                if (!await ValidateStatus(Role)) return false;
                if (!await ValidatePermission(Role)) return false;
                if (!await ValidateMenu(Role)) return false;
                if (listCodeInDB.Contains(Role.Code))
                {
                    Role.AddError(nameof(RoleValidator), nameof(Role.Code), ErrorCode.CodeExisted);
                    return false;
                }
            }
            return true;
        }
    }
}
