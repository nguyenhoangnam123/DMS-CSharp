using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;
using DMS.Enums;

namespace DMS.Services.MItemSpecificKpi
{
    public interface IItemSpecificKpiValidator : IServiceScoped
    {
        Task<bool> Create(ItemSpecificKpi ItemSpecificKpi);
        Task<bool> Update(ItemSpecificKpi ItemSpecificKpi);
        Task<bool> Delete(ItemSpecificKpi ItemSpecificKpi);
        Task<bool> BulkDelete(List<ItemSpecificKpi> ItemSpecificKpis);
        Task<bool> Import(List<ItemSpecificKpi> ItemSpecificKpis);
    }

    public class ItemSpecificKpiValidator : IItemSpecificKpiValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            OrganizationIdNotExisted,
            EmployeeIdsEmpty,
            StatusNotExisted,
            KpiPeriodIdNotExisted,
            ItemSpecificKpiContentsEmpty,
            ItemIdNotExisted
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ItemSpecificKpiValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ItemSpecificKpi ItemSpecificKpi)
        {
            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ItemSpecificKpi.Id },
                Selects = ItemSpecificKpiSelect.Id
            };

            int count = await UOW.ItemSpecificKpiRepository.Count(ItemSpecificKpiFilter);
            if (count == 0)
                ItemSpecificKpi.AddError(nameof(ItemSpecificKpiValidator), nameof(ItemSpecificKpi.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        private async Task<bool> ValidateOrganization(ItemSpecificKpi ItemSpecificKpi)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter
            {
                Id = new IdFilter { Equal = ItemSpecificKpi.OrganizationId }
            };

            var count = await UOW.OrganizationRepository.Count(OrganizationFilter);
            if (count == 0)
                ItemSpecificKpi.AddError(nameof(ItemSpecificKpiValidator), nameof(ItemSpecificKpi.Organization), ErrorCode.OrganizationIdNotExisted);
            return ItemSpecificKpi.IsValidated;
        }

        private async Task<bool> ValidateEmployees(ItemSpecificKpi ItemSpecificKpi)
        {
            if (ItemSpecificKpi.EmployeeIds == null || !ItemSpecificKpi.EmployeeIds.Any())
                ItemSpecificKpi.AddError(nameof(ItemSpecificKpiValidator), nameof(ItemSpecificKpi.EmployeeIds), ErrorCode.EmployeeIdsEmpty);
            else
            {
                AppUserFilter AppUserFilter = new AppUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = ItemSpecificKpi.EmployeeIds },
                    OrganizationId = new IdFilter(),
                    Selects = AppUserSelect.Id
                };

                var EmployeeIdsInDB = (await UOW.AppUserRepository.List(AppUserFilter)).Select(x => x.Id).ToList();
                var listIdsNotExisted = ItemSpecificKpi.EmployeeIds.Except(EmployeeIdsInDB).ToList();

                if (listIdsNotExisted != null && listIdsNotExisted.Any())
                {
                    foreach (var Id in listIdsNotExisted)
                    {
                        ItemSpecificKpi.AddError(nameof(ItemSpecificKpiValidator), nameof(ItemSpecificKpi.EmployeeIds), ErrorCode.IdNotExisted);
                    }
                }

            }
            return ItemSpecificKpi.IsValidated;
        }

        private async Task<bool> ValidateStatus(ItemSpecificKpi ItemSpecificKpi)
        {
            if (StatusEnum.ACTIVE.Id != ItemSpecificKpi.StatusId && StatusEnum.INACTIVE.Id != ItemSpecificKpi.StatusId)
                ItemSpecificKpi.AddError(nameof(ItemSpecificKpiValidator), nameof(ItemSpecificKpi.Status), ErrorCode.StatusNotExisted);
            return ItemSpecificKpi.IsValidated;
        }

        private async Task<bool> ValidateKpiPeriod(ItemSpecificKpi ItemSpecificKpi)
        {
            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter
            {
                Id = new IdFilter { Equal = ItemSpecificKpi.KpiPeriodId }
            };

            int count = await UOW.KpiPeriodRepository.Count(KpiPeriodFilter);
            if (count == 0)
                ItemSpecificKpi.AddError(nameof(ItemSpecificKpiValidator), nameof(ItemSpecificKpi.KpiPeriod), ErrorCode.KpiPeriodIdNotExisted);
            return ItemSpecificKpi.IsValidated;
        }

        private async Task<bool> ValidateItem(ItemSpecificKpi ItemSpecificKpi)
        {
            if(ItemSpecificKpi.ItemSpecificKpiContents == null || !ItemSpecificKpi.ItemSpecificKpiContents.Any())
                ItemSpecificKpi.AddError(nameof(ItemSpecificKpiValidator), nameof(ItemSpecificKpi.ItemSpecificKpiContents), ErrorCode.ItemSpecificKpiContentsEmpty);
            else
            {
                var ItemIds = ItemSpecificKpi.ItemSpecificKpiContents.Select(x => x.ItemId).ToList();
                ItemFilter ItemFilter = new ItemFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = ItemSelect.Id,
                    Id = new IdFilter { In = ItemIds }
                };

                var ItemIdsInDB = (await UOW.ItemRepository.List(ItemFilter)).Select(x => x.Id).ToList();
                var ItemIdsNotExisted = ItemIds.Except(ItemIdsInDB).ToList();
                if(ItemIdsNotExisted != null && ItemIdsNotExisted.Any())
                {
                    foreach (var Id in ItemIdsNotExisted)
                    {
                        ItemSpecificKpi.AddError(nameof(ItemSpecificKpiValidator), nameof(ItemSpecificKpi.ItemSpecificKpiContents), ErrorCode.ItemIdNotExisted);
                    }
                }
            }
            return ItemSpecificKpi.IsValidated;
        }

        public async Task<bool>Create(ItemSpecificKpi ItemSpecificKpi)
        {
            await ValidateOrganization(ItemSpecificKpi);
            await ValidateEmployees(ItemSpecificKpi);
            await ValidateStatus(ItemSpecificKpi);
            await ValidateKpiPeriod(ItemSpecificKpi);
            return ItemSpecificKpi.IsValidated;
        }

        public async Task<bool> Update(ItemSpecificKpi ItemSpecificKpi)
        {
            if (await ValidateId(ItemSpecificKpi))
            {
                await ValidateOrganization(ItemSpecificKpi);
                await ValidateStatus(ItemSpecificKpi);
                await ValidateKpiPeriod(ItemSpecificKpi);
            }
            return ItemSpecificKpi.IsValidated;
        }

        public async Task<bool> Delete(ItemSpecificKpi ItemSpecificKpi)
        {
            if (await ValidateId(ItemSpecificKpi))
            {
            }
            return ItemSpecificKpi.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ItemSpecificKpi> ItemSpecificKpis)
        {
            return true;
        }
        
        public async Task<bool> Import(List<ItemSpecificKpi> ItemSpecificKpis)
        {
            return true;
        }
    }
}
