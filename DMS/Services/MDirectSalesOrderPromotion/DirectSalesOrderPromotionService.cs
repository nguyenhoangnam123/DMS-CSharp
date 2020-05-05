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

namespace DMS.Services.MDirectSalesOrderPromotion
{
    public interface IDirectSalesOrderPromotionService :  IServiceScoped
    {
        Task<int> Count(DirectSalesOrderPromotionFilter DirectSalesOrderPromotionFilter);
        Task<List<DirectSalesOrderPromotion>> List(DirectSalesOrderPromotionFilter DirectSalesOrderPromotionFilter);
        Task<DirectSalesOrderPromotion> Get(long Id);
        Task<DirectSalesOrderPromotion> Create(DirectSalesOrderPromotion DirectSalesOrderPromotion);
        Task<DirectSalesOrderPromotion> Update(DirectSalesOrderPromotion DirectSalesOrderPromotion);
        Task<DirectSalesOrderPromotion> Delete(DirectSalesOrderPromotion DirectSalesOrderPromotion);
        Task<List<DirectSalesOrderPromotion>> BulkDelete(List<DirectSalesOrderPromotion> DirectSalesOrderPromotions);
        Task<List<DirectSalesOrderPromotion>> Import(List<DirectSalesOrderPromotion> DirectSalesOrderPromotions);
        DirectSalesOrderPromotionFilter ToFilter(DirectSalesOrderPromotionFilter DirectSalesOrderPromotionFilter);
    }

    public class DirectSalesOrderPromotionService : BaseService, IDirectSalesOrderPromotionService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IDirectSalesOrderPromotionValidator DirectSalesOrderPromotionValidator;

        public DirectSalesOrderPromotionService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IDirectSalesOrderPromotionValidator DirectSalesOrderPromotionValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.DirectSalesOrderPromotionValidator = DirectSalesOrderPromotionValidator;
        }
        public async Task<int> Count(DirectSalesOrderPromotionFilter DirectSalesOrderPromotionFilter)
        {
            try
            {
                int result = await UOW.DirectSalesOrderPromotionRepository.Count(DirectSalesOrderPromotionFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderPromotionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<DirectSalesOrderPromotion>> List(DirectSalesOrderPromotionFilter DirectSalesOrderPromotionFilter)
        {
            try
            {
                List<DirectSalesOrderPromotion> DirectSalesOrderPromotions = await UOW.DirectSalesOrderPromotionRepository.List(DirectSalesOrderPromotionFilter);
                return DirectSalesOrderPromotions;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderPromotionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<DirectSalesOrderPromotion> Get(long Id)
        {
            DirectSalesOrderPromotion DirectSalesOrderPromotion = await UOW.DirectSalesOrderPromotionRepository.Get(Id);
            if (DirectSalesOrderPromotion == null)
                return null;
            return DirectSalesOrderPromotion;
        }
       
        public async Task<DirectSalesOrderPromotion> Create(DirectSalesOrderPromotion DirectSalesOrderPromotion)
        {
            if (!await DirectSalesOrderPromotionValidator.Create(DirectSalesOrderPromotion))
                return DirectSalesOrderPromotion;

            try
            {
                await UOW.Begin();
                await UOW.DirectSalesOrderPromotionRepository.Create(DirectSalesOrderPromotion);
                await UOW.Commit();

                await Logging.CreateAuditLog(DirectSalesOrderPromotion, new { }, nameof(DirectSalesOrderPromotionService));
                return await UOW.DirectSalesOrderPromotionRepository.Get(DirectSalesOrderPromotion.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderPromotionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DirectSalesOrderPromotion> Update(DirectSalesOrderPromotion DirectSalesOrderPromotion)
        {
            if (!await DirectSalesOrderPromotionValidator.Update(DirectSalesOrderPromotion))
                return DirectSalesOrderPromotion;
            try
            {
                var oldData = await UOW.DirectSalesOrderPromotionRepository.Get(DirectSalesOrderPromotion.Id);

                await UOW.Begin();
                await UOW.DirectSalesOrderPromotionRepository.Update(DirectSalesOrderPromotion);
                await UOW.Commit();

                var newData = await UOW.DirectSalesOrderPromotionRepository.Get(DirectSalesOrderPromotion.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(DirectSalesOrderPromotionService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderPromotionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DirectSalesOrderPromotion> Delete(DirectSalesOrderPromotion DirectSalesOrderPromotion)
        {
            if (!await DirectSalesOrderPromotionValidator.Delete(DirectSalesOrderPromotion))
                return DirectSalesOrderPromotion;

            try
            {
                await UOW.Begin();
                await UOW.DirectSalesOrderPromotionRepository.Delete(DirectSalesOrderPromotion);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, DirectSalesOrderPromotion, nameof(DirectSalesOrderPromotionService));
                return DirectSalesOrderPromotion;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderPromotionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<DirectSalesOrderPromotion>> BulkDelete(List<DirectSalesOrderPromotion> DirectSalesOrderPromotions)
        {
            if (!await DirectSalesOrderPromotionValidator.BulkDelete(DirectSalesOrderPromotions))
                return DirectSalesOrderPromotions;

            try
            {
                await UOW.Begin();
                await UOW.DirectSalesOrderPromotionRepository.BulkDelete(DirectSalesOrderPromotions);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, DirectSalesOrderPromotions, nameof(DirectSalesOrderPromotionService));
                return DirectSalesOrderPromotions;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderPromotionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<DirectSalesOrderPromotion>> Import(List<DirectSalesOrderPromotion> DirectSalesOrderPromotions)
        {
            if (!await DirectSalesOrderPromotionValidator.Import(DirectSalesOrderPromotions))
                return DirectSalesOrderPromotions;
            try
            {
                await UOW.Begin();
                await UOW.DirectSalesOrderPromotionRepository.BulkMerge(DirectSalesOrderPromotions);
                await UOW.Commit();

                await Logging.CreateAuditLog(DirectSalesOrderPromotions, new { }, nameof(DirectSalesOrderPromotionService));
                return DirectSalesOrderPromotions;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderPromotionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public DirectSalesOrderPromotionFilter ToFilter(DirectSalesOrderPromotionFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<DirectSalesOrderPromotionFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                DirectSalesOrderPromotionFilter subFilter = new DirectSalesOrderPromotionFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DirectSalesOrderId))
                        subFilter.DirectSalesOrderId = Map(subFilter.DirectSalesOrderId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ItemId))
                        subFilter.ItemId = Map(subFilter.ItemId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.UnitOfMeasureId))
                        subFilter.UnitOfMeasureId = Map(subFilter.UnitOfMeasureId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Quantity))
                        subFilter.Quantity = Map(subFilter.Quantity, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PrimaryUnitOfMeasureId))
                        subFilter.PrimaryUnitOfMeasureId = Map(subFilter.PrimaryUnitOfMeasureId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.RequestedQuantity))
                        subFilter.RequestedQuantity = Map(subFilter.RequestedQuantity, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Note))
                        subFilter.Note = Map(subFilter.Note, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
