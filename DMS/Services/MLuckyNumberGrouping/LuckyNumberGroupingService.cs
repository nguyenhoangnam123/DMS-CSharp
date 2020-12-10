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

namespace DMS.Services.MLuckyNumberGrouping
{
    public interface ILuckyNumberGroupingService :  IServiceScoped
    {
        Task<int> Count(LuckyNumberGroupingFilter LuckyNumberGroupingFilter);
        Task<List<LuckyNumberGrouping>> List(LuckyNumberGroupingFilter LuckyNumberGroupingFilter);
        Task<LuckyNumberGrouping> Get(long Id);
        Task<LuckyNumberGrouping> Create(LuckyNumberGrouping LuckyNumberGrouping);
        Task<LuckyNumberGrouping> Update(LuckyNumberGrouping LuckyNumberGrouping);
        Task<LuckyNumberGrouping> Delete(LuckyNumberGrouping LuckyNumberGrouping);
        Task<List<LuckyNumberGrouping>> BulkDelete(List<LuckyNumberGrouping> LuckyNumberGroupings);
        Task<List<LuckyNumberGrouping>> Import(List<LuckyNumberGrouping> LuckyNumberGroupings);
        Task<LuckyNumberGroupingFilter> ToFilter(LuckyNumberGroupingFilter LuckyNumberGroupingFilter);
    }

    public class LuckyNumberGroupingService : BaseService, ILuckyNumberGroupingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ILuckyNumberGroupingValidator LuckyNumberGroupingValidator;

        public LuckyNumberGroupingService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ILuckyNumberGroupingValidator LuckyNumberGroupingValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.LuckyNumberGroupingValidator = LuckyNumberGroupingValidator;
        }
        public async Task<int> Count(LuckyNumberGroupingFilter LuckyNumberGroupingFilter)
        {
            try
            {
                int result = await UOW.LuckyNumberGroupingRepository.Count(LuckyNumberGroupingFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(LuckyNumberGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(LuckyNumberGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<LuckyNumberGrouping>> List(LuckyNumberGroupingFilter LuckyNumberGroupingFilter)
        {
            try
            {
                List<LuckyNumberGrouping> LuckyNumberGroupings = await UOW.LuckyNumberGroupingRepository.List(LuckyNumberGroupingFilter);
                return LuckyNumberGroupings;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(LuckyNumberGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(LuckyNumberGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<LuckyNumberGrouping> Get(long Id)
        {
            LuckyNumberGrouping LuckyNumberGrouping = await UOW.LuckyNumberGroupingRepository.Get(Id);
            if (LuckyNumberGrouping == null)
                return null;
            return LuckyNumberGrouping;
        }
       
        public async Task<LuckyNumberGrouping> Create(LuckyNumberGrouping LuckyNumberGrouping)
        {
            if (!await LuckyNumberGroupingValidator.Create(LuckyNumberGrouping))
                return LuckyNumberGrouping;

            try
            {
                await UOW.Begin();
                await UOW.LuckyNumberGroupingRepository.Create(LuckyNumberGrouping);
                await UOW.Commit();
                LuckyNumberGrouping = await UOW.LuckyNumberGroupingRepository.Get(LuckyNumberGrouping.Id);
                await Logging.CreateAuditLog(LuckyNumberGrouping, new { }, nameof(LuckyNumberGroupingService));
                return LuckyNumberGrouping;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(LuckyNumberGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(LuckyNumberGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<LuckyNumberGrouping> Update(LuckyNumberGrouping LuckyNumberGrouping)
        {
            if (!await LuckyNumberGroupingValidator.Update(LuckyNumberGrouping))
                return LuckyNumberGrouping;
            try
            {
                var oldData = await UOW.LuckyNumberGroupingRepository.Get(LuckyNumberGrouping.Id);

                await UOW.Begin();
                await UOW.LuckyNumberGroupingRepository.Update(LuckyNumberGrouping);
                await UOW.Commit();

                LuckyNumberGrouping = await UOW.LuckyNumberGroupingRepository.Get(LuckyNumberGrouping.Id);
                await Logging.CreateAuditLog(LuckyNumberGrouping, oldData, nameof(LuckyNumberGroupingService));
                return LuckyNumberGrouping;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(LuckyNumberGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(LuckyNumberGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<LuckyNumberGrouping> Delete(LuckyNumberGrouping LuckyNumberGrouping)
        {
            if (!await LuckyNumberGroupingValidator.Delete(LuckyNumberGrouping))
                return LuckyNumberGrouping;

            try
            {
                await UOW.Begin();
                await UOW.LuckyNumberGroupingRepository.Delete(LuckyNumberGrouping);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, LuckyNumberGrouping, nameof(LuckyNumberGroupingService));
                return LuckyNumberGrouping;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(LuckyNumberGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(LuckyNumberGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<LuckyNumberGrouping>> BulkDelete(List<LuckyNumberGrouping> LuckyNumberGroupings)
        {
            if (!await LuckyNumberGroupingValidator.BulkDelete(LuckyNumberGroupings))
                return LuckyNumberGroupings;

            try
            {
                await UOW.Begin();
                await UOW.LuckyNumberGroupingRepository.BulkDelete(LuckyNumberGroupings);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, LuckyNumberGroupings, nameof(LuckyNumberGroupingService));
                return LuckyNumberGroupings;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(LuckyNumberGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(LuckyNumberGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        
        public async Task<List<LuckyNumberGrouping>> Import(List<LuckyNumberGrouping> LuckyNumberGroupings)
        {
            if (!await LuckyNumberGroupingValidator.Import(LuckyNumberGroupings))
                return LuckyNumberGroupings;
            try
            {
                await UOW.Begin();
                await UOW.LuckyNumberGroupingRepository.BulkMerge(LuckyNumberGroupings);
                await UOW.Commit();

                await Logging.CreateAuditLog(LuckyNumberGroupings, new { }, nameof(LuckyNumberGroupingService));
                return LuckyNumberGroupings;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(LuckyNumberGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(LuckyNumberGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }     
        
        public async Task<LuckyNumberGroupingFilter> ToFilter(LuckyNumberGroupingFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<LuckyNumberGroupingFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                LuckyNumberGroupingFilter subFilter = new LuckyNumberGroupingFilter();
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
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = FilterBuilder.Merge(subFilter.OrganizationId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StartDate))
                        subFilter.StartDate = FilterBuilder.Merge(subFilter.StartDate, FilterPermissionDefinition.DateFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.EndDate))
                        subFilter.EndDate = FilterBuilder.Merge(subFilter.EndDate, FilterPermissionDefinition.DateFilter);
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
