using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MPriceList
{
    public interface IPriceListValidator : IServiceScoped
    {
        Task<bool> Create(PriceList PriceList);
        Task<bool> Update(PriceList PriceList);
        Task<bool> Delete(PriceList PriceList);
        Task<bool> BulkDelete(List<PriceList> PriceLists);
        Task<bool> Import(List<PriceList> PriceLists);
    }

    public class PriceListValidator : IPriceListValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            CodeEmpty,
            CodeHasSpecialCharacter,
            CodeExisted,
            NameEmpty,
            NameOverLength,
            OrganizationEmpty,
            OrganizationNotExisted,
            StatusNotExisted
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public PriceListValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(PriceList PriceList)
        {
            PriceListFilter PriceListFilter = new PriceListFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = PriceList.Id },
                Selects = PriceListSelect.Id
            };

            int count = await UOW.PriceListRepository.Count(PriceListFilter);
            if (count == 0)
                PriceList.AddError(nameof(PriceListValidator), nameof(PriceList.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        private async Task<bool> ValidateCode(PriceList PriceList)
        {
            if (string.IsNullOrWhiteSpace(PriceList.Code))
            {
                PriceList.AddError(nameof(PriceListValidator), nameof(PriceList.Code), ErrorCode.CodeEmpty);
            }
            else
            {
                var Code = PriceList.Code;
                if (PriceList.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(PriceList.Code))
                {
                    PriceList.AddError(nameof(PriceListValidator), nameof(PriceList.Code), ErrorCode.CodeHasSpecialCharacter);
                }

                PriceListFilter PriceListFilter = new PriceListFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = PriceList.Id },
                    Code = new StringFilter { Equal = PriceList.Code },
                    Selects = PriceListSelect.Code
                };

                int count = await UOW.PriceListRepository.Count(PriceListFilter);
                if (count != 0)
                    PriceList.AddError(nameof(PriceListValidator), nameof(PriceList.Code), ErrorCode.CodeExisted);
            }

            return PriceList.IsValidated;
        }

        private async Task<bool> ValidateName(PriceList PriceList)
        {
            if (string.IsNullOrWhiteSpace(PriceList.Name))
            {
                PriceList.AddError(nameof(PriceListValidator), nameof(PriceList.Name), ErrorCode.NameEmpty);
            }
            else if (PriceList.Name.Length > 255)
            {
                PriceList.AddError(nameof(PriceListValidator), nameof(PriceList.Name), ErrorCode.NameOverLength);
            }
            return PriceList.IsValidated;
        }

        private async Task<bool> ValidateOrganization(PriceList PriceList)
        {
            if (PriceList.OrganizationId == 0)
            {
                PriceList.AddError(nameof(PriceListValidator), nameof(PriceList.Organization), ErrorCode.OrganizationEmpty);
            }
            else
            {
                OrganizationFilter OrganizationFilter = new OrganizationFilter
                {
                    Id = new IdFilter { Equal = PriceList.OrganizationId }
                };

                var count = await UOW.OrganizationRepository.Count(OrganizationFilter);
                if (count == 0)
                    PriceList.AddError(nameof(PriceListValidator), nameof(PriceList.Organization), ErrorCode.OrganizationNotExisted);
            }
            return PriceList.IsValidated;
        }

        private async Task<bool> ValidateStatus(PriceList PriceList)
        {
            if (StatusEnum.ACTIVE.Id != PriceList.StatusId && StatusEnum.INACTIVE.Id != PriceList.StatusId)
                PriceList.AddError(nameof(PriceListValidator), nameof(PriceList.Status), ErrorCode.StatusNotExisted);
            return PriceList.IsValidated;
        }

        public async Task<bool> Create(PriceList PriceList)
        {
            await ValidateCode(PriceList);
            await ValidateName(PriceList);
            await ValidateOrganization(PriceList);
            await ValidateStatus(PriceList);
            return PriceList.IsValidated;
        }

        public async Task<bool> Update(PriceList PriceList)
        {
            if (await ValidateId(PriceList))
            {
            }
            return PriceList.IsValidated;
        }

        public async Task<bool> Delete(PriceList PriceList)
        {
            if (await ValidateId(PriceList))
            {
            }
            return PriceList.IsValidated;
        }

        public async Task<bool> BulkDelete(List<PriceList> PriceLists)
        {
            return true;
        }

        public async Task<bool> Import(List<PriceList> PriceLists)
        {
            return true;
        }
    }
}
