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

namespace DMS.Services.MIndirectPriceList
{
    public interface IIndirectPriceListService :  IServiceScoped
    {
        Task<int> Count(IndirectPriceListFilter IndirectPriceListFilter);
        Task<List<IndirectPriceList>> List(IndirectPriceListFilter IndirectPriceListFilter);
        Task<IndirectPriceList> Get(long Id);
        Task<IndirectPriceList> Create(IndirectPriceList IndirectPriceList);
        Task<IndirectPriceList> Update(IndirectPriceList IndirectPriceList);
        Task<IndirectPriceList> Delete(IndirectPriceList IndirectPriceList);
        Task<List<IndirectPriceList>> BulkDelete(List<IndirectPriceList> IndirectPriceLists);
        Task<List<IndirectPriceList>> Import(List<IndirectPriceList> IndirectPriceLists);
        IndirectPriceListFilter ToFilter(IndirectPriceListFilter IndirectPriceListFilter);
    }

    public class IndirectPriceListService : BaseService, IIndirectPriceListService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IIndirectPriceListValidator IndirectPriceListValidator;

        public IndirectPriceListService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IIndirectPriceListValidator IndirectPriceListValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.IndirectPriceListValidator = IndirectPriceListValidator;
        }
        public async Task<int> Count(IndirectPriceListFilter IndirectPriceListFilter)
        {
            try
            {
                int result = await UOW.IndirectPriceListRepository.Count(IndirectPriceListFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectPriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<IndirectPriceList>> List(IndirectPriceListFilter IndirectPriceListFilter)
        {
            try
            {
                List<IndirectPriceList> IndirectPriceLists = await UOW.IndirectPriceListRepository.List(IndirectPriceListFilter);
                return IndirectPriceLists;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectPriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<IndirectPriceList> Get(long Id)
        {
            IndirectPriceList IndirectPriceList = await UOW.IndirectPriceListRepository.Get(Id);
            if (IndirectPriceList == null)
                return null;
            return IndirectPriceList;
        }
       
        public async Task<IndirectPriceList> Create(IndirectPriceList IndirectPriceList)
        {
            if (!await IndirectPriceListValidator.Create(IndirectPriceList))
                return IndirectPriceList;

            try
            {
                await UOW.Begin();
                await UOW.IndirectPriceListRepository.Create(IndirectPriceList);
                await UOW.Commit();

                await Logging.CreateAuditLog(IndirectPriceList, new { }, nameof(IndirectPriceListService));
                return await UOW.IndirectPriceListRepository.Get(IndirectPriceList.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectPriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<IndirectPriceList> Update(IndirectPriceList IndirectPriceList)
        {
            if (!await IndirectPriceListValidator.Update(IndirectPriceList))
                return IndirectPriceList;
            try
            {
                var oldData = await UOW.IndirectPriceListRepository.Get(IndirectPriceList.Id);

                await UOW.Begin();
                await UOW.IndirectPriceListRepository.Update(IndirectPriceList);
                await UOW.Commit();

                var newData = await UOW.IndirectPriceListRepository.Get(IndirectPriceList.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(IndirectPriceListService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectPriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<IndirectPriceList> Delete(IndirectPriceList IndirectPriceList)
        {
            if (!await IndirectPriceListValidator.Delete(IndirectPriceList))
                return IndirectPriceList;

            try
            {
                await UOW.Begin();
                await UOW.IndirectPriceListRepository.Delete(IndirectPriceList);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, IndirectPriceList, nameof(IndirectPriceListService));
                return IndirectPriceList;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectPriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<IndirectPriceList>> BulkDelete(List<IndirectPriceList> IndirectPriceLists)
        {
            if (!await IndirectPriceListValidator.BulkDelete(IndirectPriceLists))
                return IndirectPriceLists;

            try
            {
                await UOW.Begin();
                await UOW.IndirectPriceListRepository.BulkDelete(IndirectPriceLists);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, IndirectPriceLists, nameof(IndirectPriceListService));
                return IndirectPriceLists;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectPriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<IndirectPriceList>> Import(List<IndirectPriceList> IndirectPriceLists)
        {
            if (!await IndirectPriceListValidator.Import(IndirectPriceLists))
                return IndirectPriceLists;
            try
            {
                await UOW.Begin();
                await UOW.IndirectPriceListRepository.BulkMerge(IndirectPriceLists);
                await UOW.Commit();

                await Logging.CreateAuditLog(IndirectPriceLists, new { }, nameof(IndirectPriceListService));
                return IndirectPriceLists;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectPriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public IndirectPriceListFilter ToFilter(IndirectPriceListFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<IndirectPriceListFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                IndirectPriceListFilter subFilter = new IndirectPriceListFilter();
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
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = Map(subFilter.StatusId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = Map(subFilter.OrganizationId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.IndirectPriceListTypeId))
                        subFilter.IndirectPriceListTypeId = Map(subFilter.IndirectPriceListTypeId, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
