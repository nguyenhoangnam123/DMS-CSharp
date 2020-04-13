using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MReseller
{
    public interface IResellerValidator : IServiceScoped
    {
        Task<bool> Create(Reseller Reseller);
        Task<bool> Update(Reseller Reseller);
        Task<bool> Delete(Reseller Reseller);
        Task<bool> BulkDelete(List<Reseller> Resellers);
        Task<bool> Import(List<Reseller> Resellers);
    }

    public class ResellerValidator : IResellerValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            CodeExisted,
            CodeEmpty,
            CodeOverLength,
            NameEmpty,
            NameOverLength,
            AddressOverLength,
            EmailOverLength,
            TaxCodeOverLength,
            CompanyNameOverLength,
            DeputyNameOverLength,
            PhoneEmpty,
            PhoneOverLength,
            DescriptionOverLength,
            StatusNotExisted,
            StaffNotExisted,
            OrganizationNotExisted,
            ResellerTypeNotExisted,
            ResellerStatusNotExisted
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ResellerValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Reseller Reseller)
        {
            ResellerFilter ResellerFilter = new ResellerFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Reseller.Id },
                Selects = ResellerSelect.Id
            };

            int count = await UOW.ResellerRepository.Count(ResellerFilter);
            if (count == 0)
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> ValidateCode(Reseller Reseller)
        {
            if (string.IsNullOrEmpty(Reseller.Code))
            {
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Code), ErrorCode.CodeEmpty);
            }
            else if (Reseller.Code.Length > 255)
            {
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Code), ErrorCode.CodeOverLength);
            }

            ResellerFilter ResellerFilter = new ResellerFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { NotEqual = Reseller.Id },
                Code = new StringFilter { Equal = Reseller.Code },
                Selects = ResellerSelect.Code
            };

            int count = await UOW.ResellerRepository.Count(ResellerFilter);
            if (count != 0)
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Code), ErrorCode.CodeExisted);
            return Reseller.IsValidated;
        }

        public async Task<bool> ValidateName(Reseller Reseller)
        {
            if (string.IsNullOrEmpty(Reseller.Name))
            {
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Name), ErrorCode.NameEmpty);
            }
            else if (Reseller.Name.Length > 255)
            {
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Name), ErrorCode.NameOverLength);
            }
            return Reseller.IsValidated;
        }

        public async Task<bool> ValidateAddress(Reseller Reseller)
        {
            if (!string.IsNullOrEmpty(Reseller.Address) && Reseller.Address.Length > 255)
            {
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Address), ErrorCode.AddressOverLength);
            }
            return Reseller.IsValidated;
        }

        public async Task<bool> ValidateEmail(Reseller Reseller)
        {
            if (!string.IsNullOrEmpty(Reseller.Email) && Reseller.Email.Length > 255)
            {
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Email), ErrorCode.EmailOverLength);
            }
            return Reseller.IsValidated;
        }

        public async Task<bool> ValidateTaxCode(Reseller Reseller)
        {
            if (!string.IsNullOrEmpty(Reseller.TaxCode) && Reseller.TaxCode.Length > 255)
            {
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.TaxCode), ErrorCode.TaxCodeOverLength);
            }
            return Reseller.IsValidated;
        }

        public async Task<bool> ValidateCompanyName(Reseller Reseller)
        {
            if (!string.IsNullOrEmpty(Reseller.CompanyName) && Reseller.CompanyName.Length > 255)
            {
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.CompanyName), ErrorCode.CompanyNameOverLength);
            }
            return Reseller.IsValidated;
        }

        public async Task<bool> ValidateDeputyName(Reseller Reseller)
        {
            if (!string.IsNullOrEmpty(Reseller.DeputyName) && Reseller.DeputyName.Length > 255)
            {
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.DeputyName), ErrorCode.DeputyNameOverLength);
            }
            return Reseller.IsValidated;
        }

        public async Task<bool> ValidatePhone(Reseller Reseller)
        {
            if (string.IsNullOrEmpty(Reseller.Phone))
            {
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Phone), ErrorCode.PhoneEmpty);
            }
            else if (Reseller.Phone.Length > 255)
            {
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Phone), ErrorCode.PhoneOverLength);
            }
            return true;
        }

        public async Task<bool> ValidateDescription(Reseller Reseller)
        {
            if (!string.IsNullOrEmpty(Reseller.Description) && Reseller.Description.Length > 1000)
            {
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Description), ErrorCode.DescriptionOverLength);
            }
            return Reseller.IsValidated;
        }

        public async Task<bool> ValidateAppUser(Reseller Reseller)
        {
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Reseller.StaffId },
                Selects = AppUserSelect.Id
            };
            int count = await UOW.AppUserRepository.Count(AppUserFilter);
            if (count == 0)
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Staff), ErrorCode.StaffNotExisted);
            return Reseller.IsValidated;
        }

        public async Task<bool> ValidateOrganization(Reseller Reseller)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Reseller.OrganizationId },
                Selects = OrganizationSelect.Id
            };
            int count = await UOW.OrganizationRepository.Count(OrganizationFilter);
            if (count == 0)
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Organization), ErrorCode.OrganizationNotExisted);
            return Reseller.IsValidated;
        }

        public async Task<bool> ValidateResellerType(Reseller Reseller)
        {
            ResellerTypeFilter ResellerTypeFilter = new ResellerTypeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Reseller.ResellerTypeId },
                Selects = ResellerTypeSelect.Id
            };
            int count = await UOW.ResellerTypeRepository.Count(ResellerTypeFilter);
            if (count == 0)
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.ResellerType), ErrorCode.ResellerTypeNotExisted);
            return Reseller.IsValidated;
        }

        public async Task<bool> ValidateResellerStatus(Reseller Reseller)
        {
            ResellerStatusFilter ResellerStatusFilter = new ResellerStatusFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Reseller.ResellerStatusId },
                Selects = ResellerStatusSelect.Id
            };
            int count = await UOW.ResellerStatusRepository.Count(ResellerStatusFilter);
            if (count == 0)
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.ResellerStatus), ErrorCode.ResellerStatusNotExisted);
            return Reseller.IsValidated;
        }

        public async Task<bool> ValidateStatus(Reseller Reseller)
        {
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Reseller.StatusId },
                Selects = StatusSelect.Id
            };
            int count = await UOW.StatusRepository.Count(StatusFilter);
            if (count == 0)
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Status), ErrorCode.StatusNotExisted);
            return Reseller.IsValidated;
        }

        public async Task<bool>Create(Reseller Reseller)
        {
            await ValidateCode(Reseller);
            await ValidateName(Reseller);
            await ValidateAddress(Reseller);
            await ValidateEmail(Reseller);
            await ValidateTaxCode(Reseller);
            await ValidateCompanyName(Reseller);
            await ValidateDeputyName(Reseller);
            await ValidatePhone(Reseller);
            await ValidateDescription(Reseller);
            await ValidateAppUser(Reseller);
            await ValidateOrganization(Reseller);
            await ValidateResellerType(Reseller);
            await ValidateResellerStatus(Reseller);
            await ValidateStatus(Reseller);
            return Reseller.IsValidated;
        }

        public async Task<bool> Update(Reseller Reseller)
        {
            if (await ValidateId(Reseller))
            {
                await ValidateCode(Reseller);
                await ValidateName(Reseller);
                await ValidateAddress(Reseller);
                await ValidateEmail(Reseller);
                await ValidateTaxCode(Reseller);
                await ValidateCompanyName(Reseller);
                await ValidateDeputyName(Reseller);
                await ValidatePhone(Reseller);
                await ValidateDescription(Reseller);
                await ValidateAppUser(Reseller);
                await ValidateOrganization(Reseller);
                await ValidateResellerType(Reseller);
                await ValidateResellerStatus(Reseller);
                await ValidateStatus(Reseller);
            }
            return Reseller.IsValidated;
        }

        public async Task<bool> Delete(Reseller Reseller)
        {
            if (await ValidateId(Reseller))
            {
            }
            return Reseller.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Reseller> Resellers)
        {
            ResellerFilter ResellerFilter = new ResellerFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { In = Resellers.Select(a => a.Id).ToList() },
                Selects = ResellerSelect.Id
            };

            var listInDB = await UOW.ResellerRepository.List(ResellerFilter);
            var listExcept = Resellers.Except(listInDB);
            if (listExcept == null || listExcept.Count() == 0) return true;
            foreach (var Reseller in listExcept)
            {
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Id), ErrorCode.IdNotExisted);
            }
            return false;
        }
        
        public async Task<bool> Import(List<Reseller> Resellers)
        {
            var listCodeInDB = (await UOW.ResellerRepository.List(new ResellerFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ResellerSelect.Code
            })).Select(e => e.Code);

            var listOrganizationCodeInDB = (await UOW.OrganizationRepository.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Code
            })).Select(e => e.Code);

            var listResellerTypeCodeInDB = (await UOW.ResellerTypeRepository.List(new ResellerTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ResellerTypeSelect.Code
            })).Select(e => e.Code);

            var listResellerStatusCodeInDB = (await UOW.ResellerStatusRepository.List(new ResellerStatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ResellerStatusSelect.Code
            })).Select(e => e.Code);

            foreach (var Reseller in Resellers)
            {
                if (listCodeInDB.Contains(Reseller.Code))
                {
                    Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Code), ErrorCode.CodeExisted);
                }
                if (!listOrganizationCodeInDB.Contains(Reseller.Organization.Code))
                {
                    Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Organization), ErrorCode.OrganizationNotExisted);
                }
                if (!listResellerTypeCodeInDB.Contains(Reseller.ResellerType.Code))
                {
                    Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.ResellerType), ErrorCode.ResellerTypeNotExisted);
                }
                if (!listResellerStatusCodeInDB.Contains(Reseller.ResellerStatus.Code))
                {
                    Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.ResellerStatus), ErrorCode.ResellerStatusNotExisted);
                }
                await ValidateCode(Reseller);
                await ValidateName(Reseller);
                await ValidateAddress(Reseller);
                await ValidateEmail(Reseller);
                await ValidateTaxCode(Reseller);
                await ValidateCompanyName(Reseller);
                await ValidateDeputyName(Reseller);
                await ValidatePhone(Reseller);
                await ValidateDescription(Reseller);
            }
            return Resellers.Any(s => !s.IsValidated) ? false : true;
        }
    }
}
