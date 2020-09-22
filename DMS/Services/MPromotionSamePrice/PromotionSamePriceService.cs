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
using DMS.Enums;

namespace DMS.Services.MPromotionSamePrice
{
    public interface IPromotionSamePriceService :  IServiceScoped
    {
        Task<int> Count(PromotionSamePriceFilter PromotionSamePriceFilter);
        Task<List<PromotionSamePrice>> List(PromotionSamePriceFilter PromotionSamePriceFilter);
        Task<PromotionSamePrice> Get(long Id);
        Task<PromotionSamePrice> Create(PromotionSamePrice PromotionSamePrice);
        Task<PromotionSamePrice> Update(PromotionSamePrice PromotionSamePrice);
        Task<PromotionSamePrice> Delete(PromotionSamePrice PromotionSamePrice);
        Task<List<PromotionSamePrice>> BulkDelete(List<PromotionSamePrice> PromotionSamePrices);
        Task<List<PromotionSamePrice>> Import(List<PromotionSamePrice> PromotionSamePrices);
        Task<PromotionSamePriceFilter> ToFilter(PromotionSamePriceFilter PromotionSamePriceFilter);
    }

    public class PromotionSamePriceService : BaseService, IPromotionSamePriceService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPromotionSamePriceValidator PromotionSamePriceValidator;

        public PromotionSamePriceService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPromotionSamePriceValidator PromotionSamePriceValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PromotionSamePriceValidator = PromotionSamePriceValidator;
        }
        public async Task<int> Count(PromotionSamePriceFilter PromotionSamePriceFilter)
        {
            try
            {
                int result = await UOW.PromotionSamePriceRepository.Count(PromotionSamePriceFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionSamePriceService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionSamePriceService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PromotionSamePrice>> List(PromotionSamePriceFilter PromotionSamePriceFilter)
        {
            try
            {
                List<PromotionSamePrice> PromotionSamePrices = await UOW.PromotionSamePriceRepository.List(PromotionSamePriceFilter);
                return PromotionSamePrices;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionSamePriceService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionSamePriceService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<PromotionSamePrice> Get(long Id)
        {
            PromotionSamePrice PromotionSamePrice = await UOW.PromotionSamePriceRepository.Get(Id);
            if (PromotionSamePrice == null)
                return null;
            return PromotionSamePrice;
        }
       
        public async Task<PromotionSamePrice> Create(PromotionSamePrice PromotionSamePrice)
        {
            if (!await PromotionSamePriceValidator.Create(PromotionSamePrice))
                return PromotionSamePrice;

            try
            {
                await UOW.Begin();
                await UOW.PromotionSamePriceRepository.Create(PromotionSamePrice);
                await UOW.Commit();
                PromotionSamePrice = await UOW.PromotionSamePriceRepository.Get(PromotionSamePrice.Id);
                await Logging.CreateAuditLog(PromotionSamePrice, new { }, nameof(PromotionSamePriceService));
                return PromotionSamePrice;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionSamePriceService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionSamePriceService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PromotionSamePrice> Update(PromotionSamePrice PromotionSamePrice)
        {
            if (!await PromotionSamePriceValidator.Update(PromotionSamePrice))
                return PromotionSamePrice;
            try
            {
                var oldData = await UOW.PromotionSamePriceRepository.Get(PromotionSamePrice.Id);

                await UOW.Begin();
                await UOW.PromotionSamePriceRepository.Update(PromotionSamePrice);
                await UOW.Commit();

                PromotionSamePrice = await UOW.PromotionSamePriceRepository.Get(PromotionSamePrice.Id);
                await Logging.CreateAuditLog(PromotionSamePrice, oldData, nameof(PromotionSamePriceService));
                return PromotionSamePrice;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionSamePriceService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionSamePriceService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PromotionSamePrice> Delete(PromotionSamePrice PromotionSamePrice)
        {
            if (!await PromotionSamePriceValidator.Delete(PromotionSamePrice))
                return PromotionSamePrice;

            try
            {
                await UOW.Begin();
                await UOW.PromotionSamePriceRepository.Delete(PromotionSamePrice);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PromotionSamePrice, nameof(PromotionSamePriceService));
                return PromotionSamePrice;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionSamePriceService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionSamePriceService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PromotionSamePrice>> BulkDelete(List<PromotionSamePrice> PromotionSamePrices)
        {
            if (!await PromotionSamePriceValidator.BulkDelete(PromotionSamePrices))
                return PromotionSamePrices;

            try
            {
                await UOW.Begin();
                await UOW.PromotionSamePriceRepository.BulkDelete(PromotionSamePrices);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PromotionSamePrices, nameof(PromotionSamePriceService));
                return PromotionSamePrices;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionSamePriceService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionSamePriceService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        
        public async Task<List<PromotionSamePrice>> Import(List<PromotionSamePrice> PromotionSamePrices)
        {
            if (!await PromotionSamePriceValidator.Import(PromotionSamePrices))
                return PromotionSamePrices;
            try
            {
                await UOW.Begin();
                await UOW.PromotionSamePriceRepository.BulkMerge(PromotionSamePrices);
                await UOW.Commit();

                await Logging.CreateAuditLog(PromotionSamePrices, new { }, nameof(PromotionSamePriceService));
                return PromotionSamePrices;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionSamePriceService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionSamePriceService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }     
        
        public async Task<PromotionSamePriceFilter> ToFilter(PromotionSamePriceFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PromotionSamePriceFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PromotionSamePriceFilter subFilter = new PromotionSamePriceFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.Note))
                        
                        
                        
                        
                        
                        
                        subFilter.Note = FilterBuilder.Merge(subFilter.Note, FilterPermissionDefinition.StringFilter);
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        
                        
                        
                        
                        
                        
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PromotionId))
                        subFilter.PromotionId = FilterBuilder.Merge(subFilter.PromotionId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.Price))
                        
                        
                        subFilter.Price = FilterBuilder.Merge(subFilter.Price, FilterPermissionDefinition.DecimalFilter);
                        
                        
                        
                        
                        
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
