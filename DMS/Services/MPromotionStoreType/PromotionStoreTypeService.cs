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

namespace DMS.Services.MPromotionStoreType
{
    public interface IPromotionStoreTypeService :  IServiceScoped
    {
        Task<int> Count(PromotionStoreTypeFilter PromotionStoreTypeFilter);
        Task<List<PromotionStoreType>> List(PromotionStoreTypeFilter PromotionStoreTypeFilter);
        Task<PromotionStoreType> Get(long Id);
        Task<PromotionStoreType> Create(PromotionStoreType PromotionStoreType);
        Task<PromotionStoreType> Update(PromotionStoreType PromotionStoreType);
        Task<PromotionStoreType> Delete(PromotionStoreType PromotionStoreType);
        Task<List<PromotionStoreType>> BulkDelete(List<PromotionStoreType> PromotionStoreTypes);
        Task<List<PromotionStoreType>> Import(List<PromotionStoreType> PromotionStoreTypes);
        Task<PromotionStoreTypeFilter> ToFilter(PromotionStoreTypeFilter PromotionStoreTypeFilter);
    }

    public class PromotionStoreTypeService : BaseService, IPromotionStoreTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPromotionStoreTypeValidator PromotionStoreTypeValidator;

        public PromotionStoreTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPromotionStoreTypeValidator PromotionStoreTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PromotionStoreTypeValidator = PromotionStoreTypeValidator;
        }
        public async Task<int> Count(PromotionStoreTypeFilter PromotionStoreTypeFilter)
        {
            try
            {
                int result = await UOW.PromotionStoreTypeRepository.Count(PromotionStoreTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionStoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionStoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PromotionStoreType>> List(PromotionStoreTypeFilter PromotionStoreTypeFilter)
        {
            try
            {
                List<PromotionStoreType> PromotionStoreTypes = await UOW.PromotionStoreTypeRepository.List(PromotionStoreTypeFilter);
                return PromotionStoreTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionStoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionStoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<PromotionStoreType> Get(long Id)
        {
            PromotionStoreType PromotionStoreType = await UOW.PromotionStoreTypeRepository.Get(Id);
            if (PromotionStoreType == null)
                return null;
            return PromotionStoreType;
        }
       
        public async Task<PromotionStoreType> Create(PromotionStoreType PromotionStoreType)
        {
            if (!await PromotionStoreTypeValidator.Create(PromotionStoreType))
                return PromotionStoreType;

            try
            {
                await UOW.Begin();
                await UOW.PromotionStoreTypeRepository.Create(PromotionStoreType);
                await UOW.Commit();
                PromotionStoreType = await UOW.PromotionStoreTypeRepository.Get(PromotionStoreType.Id);
                await Logging.CreateAuditLog(PromotionStoreType, new { }, nameof(PromotionStoreTypeService));
                return PromotionStoreType;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionStoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionStoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PromotionStoreType> Update(PromotionStoreType PromotionStoreType)
        {
            if (!await PromotionStoreTypeValidator.Update(PromotionStoreType))
                return PromotionStoreType;
            try
            {
                var oldData = await UOW.PromotionStoreTypeRepository.Get(PromotionStoreType.Id);

                await UOW.Begin();
                await UOW.PromotionStoreTypeRepository.Update(PromotionStoreType);
                await UOW.Commit();

                PromotionStoreType = await UOW.PromotionStoreTypeRepository.Get(PromotionStoreType.Id);
                await Logging.CreateAuditLog(PromotionStoreType, oldData, nameof(PromotionStoreTypeService));
                return PromotionStoreType;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionStoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionStoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PromotionStoreType> Delete(PromotionStoreType PromotionStoreType)
        {
            if (!await PromotionStoreTypeValidator.Delete(PromotionStoreType))
                return PromotionStoreType;

            try
            {
                await UOW.Begin();
                await UOW.PromotionStoreTypeRepository.Delete(PromotionStoreType);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PromotionStoreType, nameof(PromotionStoreTypeService));
                return PromotionStoreType;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionStoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionStoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PromotionStoreType>> BulkDelete(List<PromotionStoreType> PromotionStoreTypes)
        {
            if (!await PromotionStoreTypeValidator.BulkDelete(PromotionStoreTypes))
                return PromotionStoreTypes;

            try
            {
                await UOW.Begin();
                await UOW.PromotionStoreTypeRepository.BulkDelete(PromotionStoreTypes);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PromotionStoreTypes, nameof(PromotionStoreTypeService));
                return PromotionStoreTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionStoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionStoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        
        public async Task<List<PromotionStoreType>> Import(List<PromotionStoreType> PromotionStoreTypes)
        {
            if (!await PromotionStoreTypeValidator.Import(PromotionStoreTypes))
                return PromotionStoreTypes;
            try
            {
                await UOW.Begin();
                await UOW.PromotionStoreTypeRepository.BulkMerge(PromotionStoreTypes);
                await UOW.Commit();

                await Logging.CreateAuditLog(PromotionStoreTypes, new { }, nameof(PromotionStoreTypeService));
                return PromotionStoreTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionStoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionStoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }     
        
        public async Task<PromotionStoreTypeFilter> ToFilter(PromotionStoreTypeFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PromotionStoreTypeFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PromotionStoreTypeFilter subFilter = new PromotionStoreTypeFilter();
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
