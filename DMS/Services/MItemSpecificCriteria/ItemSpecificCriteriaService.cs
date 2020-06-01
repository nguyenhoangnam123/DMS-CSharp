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

namespace DMS.Services.MItemSpecificCriteria
{
    public interface IItemSpecificCriteriaService :  IServiceScoped
    {
        Task<int> Count(ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter);
        Task<List<ItemSpecificCriteria>> List(ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter);
        Task<ItemSpecificCriteria> Get(long Id);
        Task<ItemSpecificCriteria> Create(ItemSpecificCriteria ItemSpecificCriteria);
        Task<ItemSpecificCriteria> Update(ItemSpecificCriteria ItemSpecificCriteria);
        Task<ItemSpecificCriteria> Delete(ItemSpecificCriteria ItemSpecificCriteria);
        Task<List<ItemSpecificCriteria>> BulkDelete(List<ItemSpecificCriteria> ItemSpecificCriterias);
        Task<List<ItemSpecificCriteria>> Import(List<ItemSpecificCriteria> ItemSpecificCriterias);
        ItemSpecificCriteriaFilter ToFilter(ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter);
    }

    public class ItemSpecificCriteriaService : BaseService, IItemSpecificCriteriaService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IItemSpecificCriteriaValidator ItemSpecificCriteriaValidator;

        public ItemSpecificCriteriaService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IItemSpecificCriteriaValidator ItemSpecificCriteriaValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ItemSpecificCriteriaValidator = ItemSpecificCriteriaValidator;
        }
        public async Task<int> Count(ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter)
        {
            try
            {
                int result = await UOW.ItemSpecificCriteriaRepository.Count(ItemSpecificCriteriaFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ItemSpecificCriteria>> List(ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter)
        {
            try
            {
                List<ItemSpecificCriteria> ItemSpecificCriterias = await UOW.ItemSpecificCriteriaRepository.List(ItemSpecificCriteriaFilter);
                return ItemSpecificCriterias;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<ItemSpecificCriteria> Get(long Id)
        {
            ItemSpecificCriteria ItemSpecificCriteria = await UOW.ItemSpecificCriteriaRepository.Get(Id);
            if (ItemSpecificCriteria == null)
                return null;
            return ItemSpecificCriteria;
        }
       
        public async Task<ItemSpecificCriteria> Create(ItemSpecificCriteria ItemSpecificCriteria)
        {
            if (!await ItemSpecificCriteriaValidator.Create(ItemSpecificCriteria))
                return ItemSpecificCriteria;

            try
            {
                await UOW.Begin();
                await UOW.ItemSpecificCriteriaRepository.Create(ItemSpecificCriteria);
                await UOW.Commit();

                await Logging.CreateAuditLog(ItemSpecificCriteria, new { }, nameof(ItemSpecificCriteriaService));
                return await UOW.ItemSpecificCriteriaRepository.Get(ItemSpecificCriteria.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ItemSpecificCriteria> Update(ItemSpecificCriteria ItemSpecificCriteria)
        {
            if (!await ItemSpecificCriteriaValidator.Update(ItemSpecificCriteria))
                return ItemSpecificCriteria;
            try
            {
                var oldData = await UOW.ItemSpecificCriteriaRepository.Get(ItemSpecificCriteria.Id);

                await UOW.Begin();
                await UOW.ItemSpecificCriteriaRepository.Update(ItemSpecificCriteria);
                await UOW.Commit();

                var newData = await UOW.ItemSpecificCriteriaRepository.Get(ItemSpecificCriteria.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ItemSpecificCriteriaService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ItemSpecificCriteria> Delete(ItemSpecificCriteria ItemSpecificCriteria)
        {
            if (!await ItemSpecificCriteriaValidator.Delete(ItemSpecificCriteria))
                return ItemSpecificCriteria;

            try
            {
                await UOW.Begin();
                await UOW.ItemSpecificCriteriaRepository.Delete(ItemSpecificCriteria);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ItemSpecificCriteria, nameof(ItemSpecificCriteriaService));
                return ItemSpecificCriteria;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ItemSpecificCriteria>> BulkDelete(List<ItemSpecificCriteria> ItemSpecificCriterias)
        {
            if (!await ItemSpecificCriteriaValidator.BulkDelete(ItemSpecificCriterias))
                return ItemSpecificCriterias;

            try
            {
                await UOW.Begin();
                await UOW.ItemSpecificCriteriaRepository.BulkDelete(ItemSpecificCriterias);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ItemSpecificCriterias, nameof(ItemSpecificCriteriaService));
                return ItemSpecificCriterias;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<ItemSpecificCriteria>> Import(List<ItemSpecificCriteria> ItemSpecificCriterias)
        {
            if (!await ItemSpecificCriteriaValidator.Import(ItemSpecificCriterias))
                return ItemSpecificCriterias;
            try
            {
                await UOW.Begin();
                await UOW.ItemSpecificCriteriaRepository.BulkMerge(ItemSpecificCriterias);
                await UOW.Commit();

                await Logging.CreateAuditLog(ItemSpecificCriterias, new { }, nameof(ItemSpecificCriteriaService));
                return ItemSpecificCriterias;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public ItemSpecificCriteriaFilter ToFilter(ItemSpecificCriteriaFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ItemSpecificCriteriaFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ItemSpecificCriteriaFilter subFilter = new ItemSpecificCriteriaFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = Map(subFilter.Code, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = Map(subFilter.Name, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
