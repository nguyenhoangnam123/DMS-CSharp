using DMS.Common;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;
using DMS.Enums;

namespace DMS.Services.MShowingWarehouse
{
    public interface IShowingWarehouseService :  IServiceScoped
    {
        Task<int> Count(ShowingWarehouseFilter ShowingWarehouseFilter);
        Task<List<ShowingWarehouse>> List(ShowingWarehouseFilter ShowingWarehouseFilter);
        Task<ShowingWarehouse> Get(long Id);
        Task<ShowingWarehouse> Create(ShowingWarehouse ShowingWarehouse);
        Task<ShowingWarehouse> Update(ShowingWarehouse ShowingWarehouse);
        Task<ShowingWarehouse> Delete(ShowingWarehouse ShowingWarehouse);
        Task<List<ShowingWarehouse>> BulkDelete(List<ShowingWarehouse> ShowingWarehouses);
        Task<List<ShowingWarehouse>> Import(List<ShowingWarehouse> ShowingWarehouses);
        Task<ShowingWarehouseFilter> ToFilter(ShowingWarehouseFilter ShowingWarehouseFilter);
    }

    public class ShowingWarehouseService : BaseService, IShowingWarehouseService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IShowingWarehouseValidator ShowingWarehouseValidator;

        public ShowingWarehouseService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IShowingWarehouseValidator ShowingWarehouseValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ShowingWarehouseValidator = ShowingWarehouseValidator;
        }
        public async Task<int> Count(ShowingWarehouseFilter ShowingWarehouseFilter)
        {
            try
            {
                int result = await UOW.ShowingWarehouseRepository.Count(ShowingWarehouseFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingWarehouseService));
            }
            return 0;
        }

        public async Task<List<ShowingWarehouse>> List(ShowingWarehouseFilter ShowingWarehouseFilter)
        {
            try
            {
                List<ShowingWarehouse> ShowingWarehouses = await UOW.ShowingWarehouseRepository.List(ShowingWarehouseFilter);
                return ShowingWarehouses;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingWarehouseService));
            }
            return null;
        }
        
        public async Task<ShowingWarehouse> Get(long Id)
        {
            ShowingWarehouse ShowingWarehouse = await UOW.ShowingWarehouseRepository.Get(Id);
            if (ShowingWarehouse == null)
                return null;
            return ShowingWarehouse;
        }
        public async Task<ShowingWarehouse> Create(ShowingWarehouse ShowingWarehouse)
        {
            if (!await ShowingWarehouseValidator.Create(ShowingWarehouse))
                return ShowingWarehouse;

            try
            {
                await UOW.ShowingWarehouseRepository.Create(ShowingWarehouse);
                ShowingWarehouse = await UOW.ShowingWarehouseRepository.Get(ShowingWarehouse.Id);
                await Logging.CreateAuditLog(ShowingWarehouse, new { }, nameof(ShowingWarehouseService));
                return ShowingWarehouse;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingWarehouseService));
            }
            return null;
        }

        public async Task<ShowingWarehouse> Update(ShowingWarehouse ShowingWarehouse)
        {
            if (!await ShowingWarehouseValidator.Update(ShowingWarehouse))
                return ShowingWarehouse;
            try
            {
                var oldData = await UOW.ShowingWarehouseRepository.Get(ShowingWarehouse.Id);

                await UOW.ShowingWarehouseRepository.Update(ShowingWarehouse);

                ShowingWarehouse = await UOW.ShowingWarehouseRepository.Get(ShowingWarehouse.Id);
                await Logging.CreateAuditLog(ShowingWarehouse, oldData, nameof(ShowingWarehouseService));
                return ShowingWarehouse;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingWarehouseService));
            }
            return null;
        }

        public async Task<ShowingWarehouse> Delete(ShowingWarehouse ShowingWarehouse)
        {
            if (!await ShowingWarehouseValidator.Delete(ShowingWarehouse))
                return ShowingWarehouse;

            try
            {
                await UOW.ShowingWarehouseRepository.Delete(ShowingWarehouse);
                await Logging.CreateAuditLog(new { }, ShowingWarehouse, nameof(ShowingWarehouseService));
                return ShowingWarehouse;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingWarehouseService));
            }
            return null;
        }

        public async Task<List<ShowingWarehouse>> BulkDelete(List<ShowingWarehouse> ShowingWarehouses)
        {
            if (!await ShowingWarehouseValidator.BulkDelete(ShowingWarehouses))
                return ShowingWarehouses;

            try
            {
                await UOW.ShowingWarehouseRepository.BulkDelete(ShowingWarehouses);
                await Logging.CreateAuditLog(new { }, ShowingWarehouses, nameof(ShowingWarehouseService));
                return ShowingWarehouses;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingWarehouseService));
            }
            return null;

        }
        
        public async Task<List<ShowingWarehouse>> Import(List<ShowingWarehouse> ShowingWarehouses)
        {
            if (!await ShowingWarehouseValidator.Import(ShowingWarehouses))
                return ShowingWarehouses;
            try
            {
                await UOW.ShowingWarehouseRepository.BulkMerge(ShowingWarehouses);

                await Logging.CreateAuditLog(ShowingWarehouses, new { }, nameof(ShowingWarehouseService));
                return ShowingWarehouses;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingWarehouseService));
            }
            return null;
        }     
        
        public async Task<ShowingWarehouseFilter> ToFilter(ShowingWarehouseFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ShowingWarehouseFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ShowingWarehouseFilter subFilter = new ShowingWarehouseFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Address))
                        subFilter.Address = FilterBuilder.Merge(subFilter.Address, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = FilterBuilder.Merge(subFilter.OrganizationId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ProvinceId))
                        subFilter.ProvinceId = FilterBuilder.Merge(subFilter.ProvinceId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DistrictId))
                        subFilter.DistrictId = FilterBuilder.Merge(subFilter.DistrictId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.WardId))
                        subFilter.WardId = FilterBuilder.Merge(subFilter.WardId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }
    }
}
