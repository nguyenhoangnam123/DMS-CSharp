using Common;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;

namespace DMS.Services.MItemSpecificKpi
{
    public interface IItemSpecificKpiService :  IServiceScoped
    {
        Task<int> Count(ItemSpecificKpiFilter ItemSpecificKpiFilter);
        Task<List<ItemSpecificKpi>> List(ItemSpecificKpiFilter ItemSpecificKpiFilter);
        Task<ItemSpecificKpi> Get(long Id);
        Task<ItemSpecificKpi> Create(ItemSpecificKpi ItemSpecificKpi);
        Task<ItemSpecificKpi> Update(ItemSpecificKpi ItemSpecificKpi);
        Task<ItemSpecificKpi> Delete(ItemSpecificKpi ItemSpecificKpi);
        Task<List<ItemSpecificKpi>> BulkDelete(List<ItemSpecificKpi> ItemSpecificKpis);
        Task<List<ItemSpecificKpi>> Import(List<ItemSpecificKpi> ItemSpecificKpis);
        ItemSpecificKpiFilter ToFilter(ItemSpecificKpiFilter ItemSpecificKpiFilter);
    }

    public class ItemSpecificKpiService : BaseService, IItemSpecificKpiService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IItemSpecificKpiValidator ItemSpecificKpiValidator;

        public ItemSpecificKpiService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IItemSpecificKpiValidator ItemSpecificKpiValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ItemSpecificKpiValidator = ItemSpecificKpiValidator;
        }
        public async Task<int> Count(ItemSpecificKpiFilter ItemSpecificKpiFilter)
        {
            try
            {
                int result = await UOW.ItemSpecificKpiRepository.Count(ItemSpecificKpiFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ItemSpecificKpi>> List(ItemSpecificKpiFilter ItemSpecificKpiFilter)
        {
            try
            {
                List<ItemSpecificKpi> ItemSpecificKpis = await UOW.ItemSpecificKpiRepository.List(ItemSpecificKpiFilter);
                return ItemSpecificKpis;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<ItemSpecificKpi> Get(long Id)
        {
            ItemSpecificKpi ItemSpecificKpi = await UOW.ItemSpecificKpiRepository.Get(Id);
            if (ItemSpecificKpi == null)
                return null;
            return ItemSpecificKpi;
        }
       
        public async Task<ItemSpecificKpi> Create(ItemSpecificKpi ItemSpecificKpi)
        {
            if (!await ItemSpecificKpiValidator.Create(ItemSpecificKpi))
                return ItemSpecificKpi;

            try
            {
                await UOW.Begin();
                await UOW.ItemSpecificKpiRepository.Create(ItemSpecificKpi);
                await UOW.Commit();

                await Logging.CreateAuditLog(ItemSpecificKpi, new { }, nameof(ItemSpecificKpiService));
                return await UOW.ItemSpecificKpiRepository.Get(ItemSpecificKpi.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ItemSpecificKpi> Update(ItemSpecificKpi ItemSpecificKpi)
        {
            if (!await ItemSpecificKpiValidator.Update(ItemSpecificKpi))
                return ItemSpecificKpi;
            try
            {
                var oldData = await UOW.ItemSpecificKpiRepository.Get(ItemSpecificKpi.Id);

                await UOW.Begin();
                await UOW.ItemSpecificKpiRepository.Update(ItemSpecificKpi);
                await UOW.Commit();

                var newData = await UOW.ItemSpecificKpiRepository.Get(ItemSpecificKpi.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ItemSpecificKpiService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ItemSpecificKpi> Delete(ItemSpecificKpi ItemSpecificKpi)
        {
            if (!await ItemSpecificKpiValidator.Delete(ItemSpecificKpi))
                return ItemSpecificKpi;

            try
            {
                await UOW.Begin();
                await UOW.ItemSpecificKpiRepository.Delete(ItemSpecificKpi);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ItemSpecificKpi, nameof(ItemSpecificKpiService));
                return ItemSpecificKpi;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ItemSpecificKpi>> BulkDelete(List<ItemSpecificKpi> ItemSpecificKpis)
        {
            if (!await ItemSpecificKpiValidator.BulkDelete(ItemSpecificKpis))
                return ItemSpecificKpis;

            try
            {
                await UOW.Begin();
                await UOW.ItemSpecificKpiRepository.BulkDelete(ItemSpecificKpis);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ItemSpecificKpis, nameof(ItemSpecificKpiService));
                return ItemSpecificKpis;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<ItemSpecificKpi>> Import(List<ItemSpecificKpi> ItemSpecificKpis)
        {
            if (!await ItemSpecificKpiValidator.Import(ItemSpecificKpis))
                return ItemSpecificKpis;
            try
            {
                await UOW.Begin();
                await UOW.ItemSpecificKpiRepository.BulkMerge(ItemSpecificKpis);
                await UOW.Commit();

                await Logging.CreateAuditLog(ItemSpecificKpis, new { }, nameof(ItemSpecificKpiService));
                return ItemSpecificKpis;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public ItemSpecificKpiFilter ToFilter(ItemSpecificKpiFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ItemSpecificKpiFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ItemSpecificKpiFilter subFilter = new ItemSpecificKpiFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = Map(subFilter.OrganizationId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.KpiPeriodId))
                        subFilter.KpiPeriodId = Map(subFilter.KpiPeriodId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = Map(subFilter.StatusId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.EmployeeId))
                        subFilter.EmployeeId = Map(subFilter.EmployeeId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CreatorId))
                        subFilter.CreatorId = Map(subFilter.CreatorId, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
