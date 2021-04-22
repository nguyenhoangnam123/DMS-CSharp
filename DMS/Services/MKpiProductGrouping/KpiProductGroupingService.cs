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

namespace DMS.Services.MKpiProductGrouping
{
    public interface IKpiProductGroupingService :  IServiceScoped
    {
        Task<int> Count(KpiProductGroupingFilter KpiProductGroupingFilter);
        Task<List<KpiProductGrouping>> List(KpiProductGroupingFilter KpiProductGroupingFilter);
        Task<KpiProductGrouping> Get(long Id);
        Task<KpiProductGrouping> Create(KpiProductGrouping KpiProductGrouping);
        Task<KpiProductGrouping> Update(KpiProductGrouping KpiProductGrouping);
        Task<KpiProductGrouping> Delete(KpiProductGrouping KpiProductGrouping);
        Task<List<KpiProductGrouping>> BulkDelete(List<KpiProductGrouping> KpiProductGroupings);
        Task<List<KpiProductGrouping>> Import(List<KpiProductGrouping> KpiProductGroupings);
        Task<KpiProductGroupingFilter> ToFilter(KpiProductGroupingFilter KpiProductGroupingFilter);
    }

    public class KpiProductGroupingService : BaseService, IKpiProductGroupingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IKpiProductGroupingValidator KpiProductGroupingValidator;

        public KpiProductGroupingService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IKpiProductGroupingValidator KpiProductGroupingValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.KpiProductGroupingValidator = KpiProductGroupingValidator;
        }
        public async Task<int> Count(KpiProductGroupingFilter KpiProductGroupingFilter)
        {
            try
            {
                int result = await UOW.KpiProductGroupingRepository.Count(KpiProductGroupingFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(KpiProductGroupingService));
            }
            return 0;
        }

        public async Task<List<KpiProductGrouping>> List(KpiProductGroupingFilter KpiProductGroupingFilter)
        {
            try
            {
                List<KpiProductGrouping> KpiProductGroupings = await UOW.KpiProductGroupingRepository.List(KpiProductGroupingFilter);
                return KpiProductGroupings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(KpiProductGroupingService));
            }
            return null;
        }
        
        public async Task<KpiProductGrouping> Get(long Id)
        {
            KpiProductGrouping KpiProductGrouping = await UOW.KpiProductGroupingRepository.Get(Id);
            if (KpiProductGrouping == null)
                return null;
            return KpiProductGrouping;
        }
        public async Task<KpiProductGrouping> Create(KpiProductGrouping KpiProductGrouping)
        {
            if (!await KpiProductGroupingValidator.Create(KpiProductGrouping))
                return KpiProductGrouping;

            try
            {
                await UOW.KpiProductGroupingRepository.Create(KpiProductGrouping);
                KpiProductGrouping = await UOW.KpiProductGroupingRepository.Get(KpiProductGrouping.Id);
                await Logging.CreateAuditLog(KpiProductGrouping, new { }, nameof(KpiProductGroupingService));
                return KpiProductGrouping;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(KpiProductGroupingService));
            }
            return null;
        }

        public async Task<KpiProductGrouping> Update(KpiProductGrouping KpiProductGrouping)
        {
            if (!await KpiProductGroupingValidator.Update(KpiProductGrouping))
                return KpiProductGrouping;
            try
            {
                var oldData = await UOW.KpiProductGroupingRepository.Get(KpiProductGrouping.Id);

                await UOW.KpiProductGroupingRepository.Update(KpiProductGrouping);

                KpiProductGrouping = await UOW.KpiProductGroupingRepository.Get(KpiProductGrouping.Id);
                await Logging.CreateAuditLog(KpiProductGrouping, oldData, nameof(KpiProductGroupingService));
                return KpiProductGrouping;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(KpiProductGroupingService));
            }
            return null;
        }

        public async Task<KpiProductGrouping> Delete(KpiProductGrouping KpiProductGrouping)
        {
            if (!await KpiProductGroupingValidator.Delete(KpiProductGrouping))
                return KpiProductGrouping;

            try
            {
                await UOW.KpiProductGroupingRepository.Delete(KpiProductGrouping);
                await Logging.CreateAuditLog(new { }, KpiProductGrouping, nameof(KpiProductGroupingService));
                return KpiProductGrouping;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(KpiProductGroupingService));
            }
            return null;
        }

        public async Task<List<KpiProductGrouping>> BulkDelete(List<KpiProductGrouping> KpiProductGroupings)
        {
            if (!await KpiProductGroupingValidator.BulkDelete(KpiProductGroupings))
                return KpiProductGroupings;

            try
            {
                await UOW.KpiProductGroupingRepository.BulkDelete(KpiProductGroupings);
                await Logging.CreateAuditLog(new { }, KpiProductGroupings, nameof(KpiProductGroupingService));
                return KpiProductGroupings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(KpiProductGroupingService));
            }
            return null;

        }
        
        public async Task<List<KpiProductGrouping>> Import(List<KpiProductGrouping> KpiProductGroupings)
        {
            if (!await KpiProductGroupingValidator.Import(KpiProductGroupings))
                return KpiProductGroupings;
            try
            {
                await UOW.KpiProductGroupingRepository.BulkMerge(KpiProductGroupings);

                await Logging.CreateAuditLog(KpiProductGroupings, new { }, nameof(KpiProductGroupingService));
                return KpiProductGroupings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(KpiProductGroupingService));
            }
            return null;
        }     
        
        public async Task<KpiProductGroupingFilter> ToFilter(KpiProductGroupingFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<KpiProductGroupingFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                KpiProductGroupingFilter subFilter = new KpiProductGroupingFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = FilterBuilder.Merge(subFilter.OrganizationId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.KpiYearId))
                        subFilter.KpiYearId = FilterBuilder.Merge(subFilter.KpiYearId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.KpiPeriodId))
                        subFilter.KpiPeriodId = FilterBuilder.Merge(subFilter.KpiPeriodId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.KpiProductGroupingTypeId))
                        subFilter.KpiProductGroupingTypeId = FilterBuilder.Merge(subFilter.KpiProductGroupingTypeId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.EmployeeId))
                        subFilter.EmployeeId = FilterBuilder.Merge(subFilter.EmployeeId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CreatorId))
                        subFilter.CreatorId = FilterBuilder.Merge(subFilter.CreatorId, FilterPermissionDefinition.IdFilter);
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
