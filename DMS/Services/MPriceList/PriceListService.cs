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

namespace DMS.Services.MPriceList
{
    public interface IPriceListService :  IServiceScoped
    {
        Task<int> Count(PriceListFilter PriceListFilter);
        Task<List<PriceList>> List(PriceListFilter PriceListFilter);
        Task<PriceList> Get(long Id);
        Task<PriceList> Create(PriceList PriceList);
        Task<PriceList> Update(PriceList PriceList);
        Task<PriceList> Delete(PriceList PriceList);
        Task<List<PriceList>> BulkDelete(List<PriceList> PriceLists);
        Task<List<PriceList>> Import(List<PriceList> PriceLists);
        PriceListFilter ToFilter(PriceListFilter PriceListFilter);
    }

    public class PriceListService : BaseService, IPriceListService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPriceListValidator PriceListValidator;

        public PriceListService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPriceListValidator PriceListValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PriceListValidator = PriceListValidator;
        }
        public async Task<int> Count(PriceListFilter PriceListFilter)
        {
            try
            {
                int result = await UOW.PriceListRepository.Count(PriceListFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<PriceList>> List(PriceListFilter PriceListFilter)
        {
            try
            {
                List<PriceList> PriceLists = await UOW.PriceListRepository.List(PriceListFilter);
                return PriceLists;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<PriceList> Get(long Id)
        {
            PriceList PriceList = await UOW.PriceListRepository.Get(Id);
            if (PriceList == null)
                return null;
            return PriceList;
        }
       
        public async Task<PriceList> Create(PriceList PriceList)
        {
            if (!await PriceListValidator.Create(PriceList))
                return PriceList;

            try
            {
                await UOW.Begin();
                await UOW.PriceListRepository.Create(PriceList);
                await UOW.Commit();

                await Logging.CreateAuditLog(PriceList, new { }, nameof(PriceListService));
                return await UOW.PriceListRepository.Get(PriceList.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<PriceList> Update(PriceList PriceList)
        {
            if (!await PriceListValidator.Update(PriceList))
                return PriceList;
            try
            {
                var oldData = await UOW.PriceListRepository.Get(PriceList.Id);

                await UOW.Begin();
                await UOW.PriceListRepository.Update(PriceList);
                await UOW.Commit();

                var newData = await UOW.PriceListRepository.Get(PriceList.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(PriceListService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<PriceList> Delete(PriceList PriceList)
        {
            if (!await PriceListValidator.Delete(PriceList))
                return PriceList;

            try
            {
                await UOW.Begin();
                await UOW.PriceListRepository.Delete(PriceList);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PriceList, nameof(PriceListService));
                return PriceList;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<PriceList>> BulkDelete(List<PriceList> PriceLists)
        {
            if (!await PriceListValidator.BulkDelete(PriceLists))
                return PriceLists;

            try
            {
                await UOW.Begin();
                await UOW.PriceListRepository.BulkDelete(PriceLists);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PriceLists, nameof(PriceListService));
                return PriceLists;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<PriceList>> Import(List<PriceList> PriceLists)
        {
            if (!await PriceListValidator.Import(PriceLists))
                return PriceLists;
            try
            {
                await UOW.Begin();
                await UOW.PriceListRepository.BulkMerge(PriceLists);
                await UOW.Commit();

                await Logging.CreateAuditLog(PriceLists, new { }, nameof(PriceListService));
                return PriceLists;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public PriceListFilter ToFilter(PriceListFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PriceListFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PriceListFilter subFilter = new PriceListFilter();
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
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PriceListTypeId))
                        subFilter.PriceListTypeId = Map(subFilter.PriceListTypeId, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
