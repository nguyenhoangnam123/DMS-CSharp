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

namespace DMS.Services.MIndirectSalesOrderPromotion
{
    public interface IIndirectSalesOrderPromotionService :  IServiceScoped
    {
        Task<int> Count(IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter);
        Task<List<IndirectSalesOrderPromotion>> List(IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter);
        Task<IndirectSalesOrderPromotion> Get(long Id);
        Task<IndirectSalesOrderPromotion> Create(IndirectSalesOrderPromotion IndirectSalesOrderPromotion);
        Task<IndirectSalesOrderPromotion> Update(IndirectSalesOrderPromotion IndirectSalesOrderPromotion);
        Task<IndirectSalesOrderPromotion> Delete(IndirectSalesOrderPromotion IndirectSalesOrderPromotion);
        Task<List<IndirectSalesOrderPromotion>> BulkDelete(List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions);
        Task<List<IndirectSalesOrderPromotion>> Import(List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions);
        IndirectSalesOrderPromotionFilter ToFilter(IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter);
    }

    public class IndirectSalesOrderPromotionService : BaseService, IIndirectSalesOrderPromotionService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IIndirectSalesOrderPromotionValidator IndirectSalesOrderPromotionValidator;

        public IndirectSalesOrderPromotionService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IIndirectSalesOrderPromotionValidator IndirectSalesOrderPromotionValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.IndirectSalesOrderPromotionValidator = IndirectSalesOrderPromotionValidator;
        }
        public async Task<int> Count(IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter)
        {
            try
            {
                int result = await UOW.IndirectSalesOrderPromotionRepository.Count(IndirectSalesOrderPromotionFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderPromotionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<IndirectSalesOrderPromotion>> List(IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter)
        {
            try
            {
                List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions = await UOW.IndirectSalesOrderPromotionRepository.List(IndirectSalesOrderPromotionFilter);
                return IndirectSalesOrderPromotions;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderPromotionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<IndirectSalesOrderPromotion> Get(long Id)
        {
            IndirectSalesOrderPromotion IndirectSalesOrderPromotion = await UOW.IndirectSalesOrderPromotionRepository.Get(Id);
            if (IndirectSalesOrderPromotion == null)
                return null;
            return IndirectSalesOrderPromotion;
        }
       
        public async Task<IndirectSalesOrderPromotion> Create(IndirectSalesOrderPromotion IndirectSalesOrderPromotion)
        {
            if (!await IndirectSalesOrderPromotionValidator.Create(IndirectSalesOrderPromotion))
                return IndirectSalesOrderPromotion;

            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderPromotionRepository.Create(IndirectSalesOrderPromotion);
                await UOW.Commit();

                await Logging.CreateAuditLog(IndirectSalesOrderPromotion, new { }, nameof(IndirectSalesOrderPromotionService));
                return await UOW.IndirectSalesOrderPromotionRepository.Get(IndirectSalesOrderPromotion.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderPromotionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<IndirectSalesOrderPromotion> Update(IndirectSalesOrderPromotion IndirectSalesOrderPromotion)
        {
            if (!await IndirectSalesOrderPromotionValidator.Update(IndirectSalesOrderPromotion))
                return IndirectSalesOrderPromotion;
            try
            {
                var oldData = await UOW.IndirectSalesOrderPromotionRepository.Get(IndirectSalesOrderPromotion.Id);

                await UOW.Begin();
                await UOW.IndirectSalesOrderPromotionRepository.Update(IndirectSalesOrderPromotion);
                await UOW.Commit();

                var newData = await UOW.IndirectSalesOrderPromotionRepository.Get(IndirectSalesOrderPromotion.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(IndirectSalesOrderPromotionService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderPromotionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<IndirectSalesOrderPromotion> Delete(IndirectSalesOrderPromotion IndirectSalesOrderPromotion)
        {
            if (!await IndirectSalesOrderPromotionValidator.Delete(IndirectSalesOrderPromotion))
                return IndirectSalesOrderPromotion;

            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderPromotionRepository.Delete(IndirectSalesOrderPromotion);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, IndirectSalesOrderPromotion, nameof(IndirectSalesOrderPromotionService));
                return IndirectSalesOrderPromotion;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderPromotionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<IndirectSalesOrderPromotion>> BulkDelete(List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions)
        {
            if (!await IndirectSalesOrderPromotionValidator.BulkDelete(IndirectSalesOrderPromotions))
                return IndirectSalesOrderPromotions;

            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderPromotionRepository.BulkDelete(IndirectSalesOrderPromotions);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, IndirectSalesOrderPromotions, nameof(IndirectSalesOrderPromotionService));
                return IndirectSalesOrderPromotions;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderPromotionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<IndirectSalesOrderPromotion>> Import(List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions)
        {
            if (!await IndirectSalesOrderPromotionValidator.Import(IndirectSalesOrderPromotions))
                return IndirectSalesOrderPromotions;
            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderPromotionRepository.BulkMerge(IndirectSalesOrderPromotions);
                await UOW.Commit();

                await Logging.CreateAuditLog(IndirectSalesOrderPromotions, new { }, nameof(IndirectSalesOrderPromotionService));
                return IndirectSalesOrderPromotions;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderPromotionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public IndirectSalesOrderPromotionFilter ToFilter(IndirectSalesOrderPromotionFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<IndirectSalesOrderPromotionFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                IndirectSalesOrderPromotionFilter subFilter = new IndirectSalesOrderPromotionFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.IndirectSalesOrderId))
                        subFilter.IndirectSalesOrderId = Map(subFilter.IndirectSalesOrderId, FilterPermissionDefinition);
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
