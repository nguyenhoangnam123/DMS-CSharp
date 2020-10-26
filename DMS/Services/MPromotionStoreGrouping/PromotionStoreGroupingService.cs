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

namespace DMS.Services.MPromotionStoreGrouping
{
    public interface IPromotionStoreGroupingService :  IServiceScoped
    {
        Task<int> Count(PromotionStoreGroupingFilter PromotionStoreGroupingFilter);
        Task<List<PromotionStoreGrouping>> List(PromotionStoreGroupingFilter PromotionStoreGroupingFilter);
        Task<PromotionStoreGrouping> Get(long Id);
        Task<PromotionStoreGrouping> Create(PromotionStoreGrouping PromotionStoreGrouping);
        Task<PromotionStoreGrouping> Update(PromotionStoreGrouping PromotionStoreGrouping);
        Task<PromotionStoreGrouping> Delete(PromotionStoreGrouping PromotionStoreGrouping);
        Task<List<PromotionStoreGrouping>> BulkDelete(List<PromotionStoreGrouping> PromotionStoreGroupings);
        Task<List<PromotionStoreGrouping>> Import(List<PromotionStoreGrouping> PromotionStoreGroupings);
        Task<PromotionStoreGroupingFilter> ToFilter(PromotionStoreGroupingFilter PromotionStoreGroupingFilter);
    }

    public class PromotionStoreGroupingService : BaseService, IPromotionStoreGroupingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPromotionStoreGroupingValidator PromotionStoreGroupingValidator;

        public PromotionStoreGroupingService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPromotionStoreGroupingValidator PromotionStoreGroupingValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PromotionStoreGroupingValidator = PromotionStoreGroupingValidator;
        }
        public async Task<int> Count(PromotionStoreGroupingFilter PromotionStoreGroupingFilter)
        {
            try
            {
                int result = await UOW.PromotionStoreGroupingRepository.Count(PromotionStoreGroupingFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionStoreGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionStoreGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PromotionStoreGrouping>> List(PromotionStoreGroupingFilter PromotionStoreGroupingFilter)
        {
            try
            {
                List<PromotionStoreGrouping> PromotionStoreGroupings = await UOW.PromotionStoreGroupingRepository.List(PromotionStoreGroupingFilter);
                return PromotionStoreGroupings;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionStoreGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionStoreGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<PromotionStoreGrouping> Get(long Id)
        {
            PromotionStoreGrouping PromotionStoreGrouping = await UOW.PromotionStoreGroupingRepository.Get(Id);
            if (PromotionStoreGrouping == null)
                return null;
            return PromotionStoreGrouping;
        }
       
        public async Task<PromotionStoreGrouping> Create(PromotionStoreGrouping PromotionStoreGrouping)
        {
            if (!await PromotionStoreGroupingValidator.Create(PromotionStoreGrouping))
                return PromotionStoreGrouping;

            try
            {
                await UOW.Begin();
                await UOW.PromotionStoreGroupingRepository.Create(PromotionStoreGrouping);
                await UOW.Commit();
                PromotionStoreGrouping = await UOW.PromotionStoreGroupingRepository.Get(PromotionStoreGrouping.Id);
                await Logging.CreateAuditLog(PromotionStoreGrouping, new { }, nameof(PromotionStoreGroupingService));
                return PromotionStoreGrouping;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionStoreGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionStoreGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PromotionStoreGrouping> Update(PromotionStoreGrouping PromotionStoreGrouping)
        {
            if (!await PromotionStoreGroupingValidator.Update(PromotionStoreGrouping))
                return PromotionStoreGrouping;
            try
            {
                var oldData = await UOW.PromotionStoreGroupingRepository.Get(PromotionStoreGrouping.Id);

                await UOW.Begin();
                await UOW.PromotionStoreGroupingRepository.Update(PromotionStoreGrouping);
                await UOW.Commit();

                PromotionStoreGrouping = await UOW.PromotionStoreGroupingRepository.Get(PromotionStoreGrouping.Id);
                await Logging.CreateAuditLog(PromotionStoreGrouping, oldData, nameof(PromotionStoreGroupingService));
                return PromotionStoreGrouping;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionStoreGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionStoreGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PromotionStoreGrouping> Delete(PromotionStoreGrouping PromotionStoreGrouping)
        {
            if (!await PromotionStoreGroupingValidator.Delete(PromotionStoreGrouping))
                return PromotionStoreGrouping;

            try
            {
                await UOW.Begin();
                await UOW.PromotionStoreGroupingRepository.Delete(PromotionStoreGrouping);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PromotionStoreGrouping, nameof(PromotionStoreGroupingService));
                return PromotionStoreGrouping;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionStoreGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionStoreGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PromotionStoreGrouping>> BulkDelete(List<PromotionStoreGrouping> PromotionStoreGroupings)
        {
            if (!await PromotionStoreGroupingValidator.BulkDelete(PromotionStoreGroupings))
                return PromotionStoreGroupings;

            try
            {
                await UOW.Begin();
                await UOW.PromotionStoreGroupingRepository.BulkDelete(PromotionStoreGroupings);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PromotionStoreGroupings, nameof(PromotionStoreGroupingService));
                return PromotionStoreGroupings;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionStoreGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionStoreGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        
        public async Task<List<PromotionStoreGrouping>> Import(List<PromotionStoreGrouping> PromotionStoreGroupings)
        {
            if (!await PromotionStoreGroupingValidator.Import(PromotionStoreGroupings))
                return PromotionStoreGroupings;
            try
            {
                await UOW.Begin();
                await UOW.PromotionStoreGroupingRepository.BulkMerge(PromotionStoreGroupings);
                await UOW.Commit();

                await Logging.CreateAuditLog(PromotionStoreGroupings, new { }, nameof(PromotionStoreGroupingService));
                return PromotionStoreGroupings;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionStoreGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionStoreGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }     
        
        public async Task<PromotionStoreGroupingFilter> ToFilter(PromotionStoreGroupingFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PromotionStoreGroupingFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PromotionStoreGroupingFilter subFilter = new PromotionStoreGroupingFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.PromotionPolicyId))
                        subFilter.PromotionPolicyId = FilterBuilder.Merge(subFilter.PromotionPolicyId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.PromotionId))
                        subFilter.PromotionId = FilterBuilder.Merge(subFilter.PromotionId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.Note))
                        
                        
                        
                        
                        
                        
                        subFilter.Note = FilterBuilder.Merge(subFilter.Note, FilterPermissionDefinition.StringFilter);
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.FromValue))
                        
                        
                        subFilter.FromValue = FilterBuilder.Merge(subFilter.FromValue, FilterPermissionDefinition.DecimalFilter);
                        
                        
                        
                        
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ToValue))
                        
                        
                        subFilter.ToValue = FilterBuilder.Merge(subFilter.ToValue, FilterPermissionDefinition.DecimalFilter);
                        
                        
                        
                        
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PromotionDiscountTypeId))
                        subFilter.PromotionDiscountTypeId = FilterBuilder.Merge(subFilter.PromotionDiscountTypeId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.DiscountPercentage))
                        
                        
                        subFilter.DiscountPercentage = FilterBuilder.Merge(subFilter.DiscountPercentage, FilterPermissionDefinition.DecimalFilter);
                        
                        
                        
                        
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DiscountValue))
                        
                        
                        subFilter.DiscountValue = FilterBuilder.Merge(subFilter.DiscountValue, FilterPermissionDefinition.DecimalFilter);
                        
                        
                        
                        
                        
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
