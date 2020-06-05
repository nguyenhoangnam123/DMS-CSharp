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

namespace DMS.Services.MDirectPriceList
{
    public interface IDirectPriceListService :  IServiceScoped
    {
        Task<int> Count(DirectPriceListFilter DirectPriceListFilter);
        Task<List<DirectPriceList>> List(DirectPriceListFilter DirectPriceListFilter);
        Task<DirectPriceList> Get(long Id);
        Task<DirectPriceList> Create(DirectPriceList DirectPriceList);
        Task<DirectPriceList> Update(DirectPriceList DirectPriceList);
        Task<DirectPriceList> Delete(DirectPriceList DirectPriceList);
        Task<List<DirectPriceList>> BulkDelete(List<DirectPriceList> DirectPriceLists);
        Task<List<DirectPriceList>> Import(List<DirectPriceList> DirectPriceLists);
        DirectPriceListFilter ToFilter(DirectPriceListFilter DirectPriceListFilter);
    }

    public class DirectPriceListService : BaseService, IDirectPriceListService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IDirectPriceListValidator DirectPriceListValidator;

        public DirectPriceListService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IDirectPriceListValidator DirectPriceListValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.DirectPriceListValidator = DirectPriceListValidator;
        }
        public async Task<int> Count(DirectPriceListFilter DirectPriceListFilter)
        {
            try
            {
                int result = await UOW.DirectPriceListRepository.Count(DirectPriceListFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<DirectPriceList>> List(DirectPriceListFilter DirectPriceListFilter)
        {
            try
            {
                List<DirectPriceList> DirectPriceLists = await UOW.DirectPriceListRepository.List(DirectPriceListFilter);
                return DirectPriceLists;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<DirectPriceList> Get(long Id)
        {
            DirectPriceList DirectPriceList = await UOW.DirectPriceListRepository.Get(Id);
            if (DirectPriceList == null)
                return null;
            return DirectPriceList;
        }
       
        public async Task<DirectPriceList> Create(DirectPriceList DirectPriceList)
        {
            if (!await DirectPriceListValidator.Create(DirectPriceList))
                return DirectPriceList;

            try
            {
                await UOW.Begin();
                await UOW.DirectPriceListRepository.Create(DirectPriceList);
                await UOW.Commit();

                await Logging.CreateAuditLog(DirectPriceList, new { }, nameof(DirectPriceListService));
                return await UOW.DirectPriceListRepository.Get(DirectPriceList.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DirectPriceList> Update(DirectPriceList DirectPriceList)
        {
            if (!await DirectPriceListValidator.Update(DirectPriceList))
                return DirectPriceList;
            try
            {
                var oldData = await UOW.DirectPriceListRepository.Get(DirectPriceList.Id);

                await UOW.Begin();
                await UOW.DirectPriceListRepository.Update(DirectPriceList);
                await UOW.Commit();

                var newData = await UOW.DirectPriceListRepository.Get(DirectPriceList.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(DirectPriceListService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DirectPriceList> Delete(DirectPriceList DirectPriceList)
        {
            if (!await DirectPriceListValidator.Delete(DirectPriceList))
                return DirectPriceList;

            try
            {
                await UOW.Begin();
                await UOW.DirectPriceListRepository.Delete(DirectPriceList);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, DirectPriceList, nameof(DirectPriceListService));
                return DirectPriceList;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<DirectPriceList>> BulkDelete(List<DirectPriceList> DirectPriceLists)
        {
            if (!await DirectPriceListValidator.BulkDelete(DirectPriceLists))
                return DirectPriceLists;

            try
            {
                await UOW.Begin();
                await UOW.DirectPriceListRepository.BulkDelete(DirectPriceLists);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, DirectPriceLists, nameof(DirectPriceListService));
                return DirectPriceLists;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<DirectPriceList>> Import(List<DirectPriceList> DirectPriceLists)
        {
            if (!await DirectPriceListValidator.Import(DirectPriceLists))
                return DirectPriceLists;
            try
            {
                await UOW.Begin();
                await UOW.DirectPriceListRepository.BulkMerge(DirectPriceLists);
                await UOW.Commit();

                await Logging.CreateAuditLog(DirectPriceLists, new { }, nameof(DirectPriceListService));
                return DirectPriceLists;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public DirectPriceListFilter ToFilter(DirectPriceListFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<DirectPriceListFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                DirectPriceListFilter subFilter = new DirectPriceListFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    
                }
            }
            return filter;
        }
    }
}
