using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MKpiItem
{
    public interface IKpiItemValidator : IServiceScoped
    {
        Task<bool> Create(KpiItem KpiItem);
        Task<bool> Update(KpiItem KpiItem);
        Task<bool> Delete(KpiItem KpiItem);
        Task<bool> BulkDelete(List<KpiItem> KpiItems);
        Task<bool> Import(List<KpiItem> KpiItems);
    }

    public class KpiItemValidator : IKpiItemValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            OrganizationIdNotExisted,
            OrganizationEmpty,
            EmployeeIdsEmpty,
            StatusNotExisted,
            KpiPeriodIdNotExisted,
            KpiItemContentsEmpty,
            ItemIdNotExisted
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public KpiItemValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(KpiItem KpiItem)
        {
            KpiItemFilter KpiItemFilter = new KpiItemFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = KpiItem.Id },
                Selects = KpiItemSelect.Id
            };

            int count = await UOW.KpiItemRepository.Count(KpiItemFilter);
            if (count == 0)
                KpiItem.AddError(nameof(KpiItemValidator), nameof(KpiItem.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        private async Task<bool> ValidateOrganization(KpiItem KpiItem)
        {
            if(KpiItem.OrganizationId == 0)
            {
                KpiItem.AddError(nameof(KpiItemValidator), nameof(KpiItem.Organization), ErrorCode.OrganizationEmpty);
            }
            else
            {
                OrganizationFilter OrganizationFilter = new OrganizationFilter
                {
                    Id = new IdFilter { Equal = KpiItem.OrganizationId }
                };

                var count = await UOW.OrganizationRepository.Count(OrganizationFilter);
                if (count == 0)
                    KpiItem.AddError(nameof(KpiItemValidator), nameof(KpiItem.Organization), ErrorCode.OrganizationIdNotExisted);
            }
            
            return KpiItem.IsValidated;
        }

        private async Task<bool> ValidateEmployees(KpiItem KpiItem)
        {
            if (KpiItem.EmployeeIds == null || !KpiItem.EmployeeIds.Any())
                KpiItem.AddError(nameof(KpiItemValidator), nameof(KpiItem.EmployeeIds), ErrorCode.EmployeeIdsEmpty);
            else
            {
                AppUserFilter AppUserFilter = new AppUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = KpiItem.EmployeeIds },
                    OrganizationId = new IdFilter(),
                    Selects = AppUserSelect.Id
                };

                var EmployeeIdsInDB = (await UOW.AppUserRepository.List(AppUserFilter)).Select(x => x.Id).ToList();
                var listIdsNotExisted = KpiItem.EmployeeIds.Except(EmployeeIdsInDB).ToList();

                if (listIdsNotExisted != null && listIdsNotExisted.Any())
                {
                    foreach (var Id in listIdsNotExisted)
                    {
                        KpiItem.AddError(nameof(KpiItemValidator), nameof(KpiItem.EmployeeIds), ErrorCode.IdNotExisted);
                    }
                }

            }
            return KpiItem.IsValidated;
        }

        private async Task<bool> ValidateStatus(KpiItem KpiItem)
        {
            if (StatusEnum.ACTIVE.Id != KpiItem.StatusId && StatusEnum.INACTIVE.Id != KpiItem.StatusId)
                KpiItem.AddError(nameof(KpiItemValidator), nameof(KpiItem.Status), ErrorCode.StatusNotExisted);
            return KpiItem.IsValidated;
        }

        private async Task<bool> ValidateKpiPeriod(KpiItem KpiItem)
        {
            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter
            {
                Id = new IdFilter { Equal = KpiItem.KpiPeriodId }
            };

            int count = await UOW.KpiPeriodRepository.Count(KpiPeriodFilter);
            if (count == 0)
                KpiItem.AddError(nameof(KpiItemValidator), nameof(KpiItem.KpiPeriod), ErrorCode.KpiPeriodIdNotExisted);
            return KpiItem.IsValidated;
        }

        private async Task<bool> ValidateItem(KpiItem KpiItem)
        {
            if (KpiItem.KpiItemContents == null || !KpiItem.KpiItemContents.Any())
                KpiItem.AddError(nameof(KpiItemValidator), nameof(KpiItem.KpiItemContents), ErrorCode.KpiItemContentsEmpty);
            else
            {
                var ItemIds = KpiItem.KpiItemContents.Select(x => x.ItemId).ToList();
                ItemFilter ItemFilter = new ItemFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = ItemSelect.Id,
                    Id = new IdFilter { In = ItemIds }
                };

                var ItemIdsInDB = (await UOW.ItemRepository.List(ItemFilter)).Select(x => x.Id).ToList();
                var ItemIdsNotExisted = ItemIds.Except(ItemIdsInDB).ToList();
                if (ItemIdsNotExisted != null && ItemIdsNotExisted.Any())
                {
                    foreach (var Id in ItemIdsNotExisted)
                    {
                        KpiItem.AddError(nameof(KpiItemValidator), nameof(KpiItem.KpiItemContents), ErrorCode.ItemIdNotExisted);
                    }
                }
            }
            return KpiItem.IsValidated;
        }

        public async Task<bool> Create(KpiItem KpiItem)
        {
            await ValidateOrganization(KpiItem);
            await ValidateEmployees(KpiItem);
            await ValidateStatus(KpiItem);
            await ValidateKpiPeriod(KpiItem);
            return KpiItem.IsValidated;
        }

        public async Task<bool> Update(KpiItem KpiItem)
        {
            if (await ValidateId(KpiItem))
            {
                await ValidateOrganization(KpiItem);
                await ValidateStatus(KpiItem);
                await ValidateKpiPeriod(KpiItem);
            }
            return KpiItem.IsValidated;
        }

        public async Task<bool> Delete(KpiItem KpiItem)
        {
            if (await ValidateId(KpiItem))
            {
            }
            return KpiItem.IsValidated;
        }

        public async Task<bool> BulkDelete(List<KpiItem> KpiItems)
        {
            return true;
        }

        public async Task<bool> Import(List<KpiItem> KpiItems)
        {
            return true;
        }
    }
}
