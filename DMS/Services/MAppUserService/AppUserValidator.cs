using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MAppUser
{
    public interface IAppUserValidator : IServiceScoped
    {
        Task<bool> Create(AppUser AppUser);
        Task<bool> Update(AppUser AppUser);
        Task<bool> Delete(AppUser AppUser);
        Task<bool> BulkDelete(List<AppUser> AppUsers);
        Task<bool> Import(List<AppUser> AppUsers);
    }

    public class AppUserValidator : IAppUserValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            UsernameExisted,
            DisplayNameEmpty,
            DisplayNameOverLength,
            EmailExisted,
            PhoneEmpty,
            PhoneOverLength,
            AddressOverLength,
            StatusNotExisted
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public AppUserValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(AppUser AppUser)
        {
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = AppUser.Id },
                Selects = AppUserSelect.Id
            };

            int count = await UOW.AppUserRepository.Count(AppUserFilter);
            if (count == 0)
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> ValidateUsername(AppUser AppUser)
        {
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { NotEqual = AppUser.Id },
                Username = new StringFilter { Equal = AppUser.Username },
                Selects = AppUserSelect.Username
            };

            int count = await UOW.AppUserRepository.Count(AppUserFilter);
            if (count == 0)
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.UsernameExisted);
            return count == 1;
        }

        public async Task<bool> ValidateDisplayName(AppUser AppUser)
        {
            if (string.IsNullOrEmpty(AppUser.DisplayName))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.DisplayName), ErrorCode.DisplayNameEmpty);
            }
            if (AppUser.DisplayName.Length > 255)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.DisplayName), ErrorCode.DisplayNameOverLength);
            }
            return true;
        }

        public async Task<bool> ValidateEmail(AppUser AppUser)
        {
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { NotEqual = AppUser.Id },
                Email = new StringFilter { Equal = AppUser.Email },
                Selects = AppUserSelect.Email
            };

            int count = await UOW.AppUserRepository.Count(AppUserFilter);
            if (count == 0)
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailExisted);
            return count == 1;
        }

        public async Task<bool> ValidatePhone(AppUser AppUser)
        {
            if (string.IsNullOrEmpty(AppUser.Phone))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Phone), ErrorCode.PhoneEmpty);
            }
            if (AppUser.Phone.Length > 255)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Phone), ErrorCode.PhoneOverLength);
            }
            return true;
        }

        public async Task<bool> ValidateAddress(AppUser AppUser)
        {
            if (!string.IsNullOrEmpty(AppUser.Address) && AppUser.Address.Length > 255)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Address), ErrorCode.AddressOverLength);
            }
            return true;
        }

        public async Task<bool> ValidateStatus(AppUser AppUser)
        {
            if (StatusEnum.ACTIVE.Id != AppUser.StatusId && StatusEnum.INACTIVE.Id != AppUser.StatusId)
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Status), ErrorCode.StatusNotExisted);
            return true;
        }

        public async Task<bool> Create(AppUser AppUser)
        {
            await ValidateUsername(AppUser);
            await ValidateDisplayName(AppUser);
            await ValidateEmail(AppUser);
            await ValidatePhone(AppUser);
            await ValidateAddress(AppUser);
            await ValidateStatus(AppUser);
            return AppUser.IsValidated;
        }

        public async Task<bool> Update(AppUser AppUser)
        {
            if (await ValidateId(AppUser))
            {
                await ValidateUsername(AppUser);
                await ValidateDisplayName(AppUser);
                await ValidateEmail(AppUser);
                await ValidatePhone(AppUser);
                await ValidateAddress(AppUser);
                await ValidateStatus(AppUser);
            }
            return AppUser.IsValidated;
        }

        public async Task<bool> Delete(AppUser AppUser)
        {
            await ValidateId(AppUser);
            return AppUser.IsValidated;
        }

        public async Task<bool> BulkDelete(List<AppUser> AppUsers)
        {
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { In = AppUsers.Select(a => a.Id).ToList() },
                Selects = AppUserSelect.Id
            };

            var listInDB = await UOW.AppUserRepository.List(AppUserFilter);
            var listExcept = AppUsers.Except(listInDB);
            if (listExcept == null || listExcept.Count() == 0) return true;
            foreach (var AppUser in listExcept)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Id), ErrorCode.IdNotExisted);
            }
            return false;
        }

        public async Task<bool> Import(List<AppUser> AppUsers)
        {
            var listEmailInDB = (await UOW.AppUserRepository.List(new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Email
            })).Select(e => e.Email);
            var listUserNameInDB = (await UOW.AppUserRepository.List(new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Username
            })).Select(e => e.Username);

            foreach (var AppUser in AppUsers)
            {
                await ValidateDisplayName(AppUser);
                await ValidatePhone(AppUser);
                await ValidateAddress(AppUser);
                await ValidateStatus(AppUser);
                if (!AppUser.IsValidated) return false;
                if (listEmailInDB.Contains(AppUser.Email))
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailExisted);
                    return false;
                }
                if (listUserNameInDB.Contains(AppUser.Username))
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.UsernameExisted);
                    return false;
                }
            }
            return true;
        }
    }
}
