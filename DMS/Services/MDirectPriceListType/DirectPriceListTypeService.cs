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

namespace DMS.Services.MDirectPriceListType
{
    public interface IDirectPriceListTypeService :  IServiceScoped
    {
        Task<int> Count(DirectPriceListTypeFilter DirectPriceListTypeFilter);
        Task<List<DirectPriceListType>> List(DirectPriceListTypeFilter DirectPriceListTypeFilter);
        Task<DirectPriceListType> Get(long Id);
        Task<DirectPriceListType> Create(DirectPriceListType DirectPriceListType);
        Task<DirectPriceListType> Update(DirectPriceListType DirectPriceListType);
        Task<DirectPriceListType> Delete(DirectPriceListType DirectPriceListType);
        Task<List<DirectPriceListType>> BulkDelete(List<DirectPriceListType> DirectPriceListTypes);
        Task<List<DirectPriceListType>> Import(List<DirectPriceListType> DirectPriceListTypes);
        DirectPriceListTypeFilter ToFilter(DirectPriceListTypeFilter DirectPriceListTypeFilter);
    }

    public class DirectPriceListTypeService : BaseService, IDirectPriceListTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IDirectPriceListTypeValidator DirectPriceListTypeValidator;

        public DirectPriceListTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IDirectPriceListTypeValidator DirectPriceListTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.DirectPriceListTypeValidator = DirectPriceListTypeValidator;
        }
        public async Task<int> Count(DirectPriceListTypeFilter DirectPriceListTypeFilter)
        {
            try
            {
                int result = await UOW.DirectPriceListTypeRepository.Count(DirectPriceListTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<DirectPriceListType>> List(DirectPriceListTypeFilter DirectPriceListTypeFilter)
        {
            try
            {
                List<DirectPriceListType> DirectPriceListTypes = await UOW.DirectPriceListTypeRepository.List(DirectPriceListTypeFilter);
                return DirectPriceListTypes;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<DirectPriceListType> Get(long Id)
        {
            DirectPriceListType DirectPriceListType = await UOW.DirectPriceListTypeRepository.Get(Id);
            if (DirectPriceListType == null)
                return null;
            return DirectPriceListType;
        }
       
        public async Task<DirectPriceListType> Create(DirectPriceListType DirectPriceListType)
        {
            if (!await DirectPriceListTypeValidator.Create(DirectPriceListType))
                return DirectPriceListType;

            try
            {
                await UOW.Begin();
                await UOW.DirectPriceListTypeRepository.Create(DirectPriceListType);
                await UOW.Commit();

                await Logging.CreateAuditLog(DirectPriceListType, new { }, nameof(DirectPriceListTypeService));
                return await UOW.DirectPriceListTypeRepository.Get(DirectPriceListType.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DirectPriceListType> Update(DirectPriceListType DirectPriceListType)
        {
            if (!await DirectPriceListTypeValidator.Update(DirectPriceListType))
                return DirectPriceListType;
            try
            {
                var oldData = await UOW.DirectPriceListTypeRepository.Get(DirectPriceListType.Id);

                await UOW.Begin();
                await UOW.DirectPriceListTypeRepository.Update(DirectPriceListType);
                await UOW.Commit();

                var newData = await UOW.DirectPriceListTypeRepository.Get(DirectPriceListType.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(DirectPriceListTypeService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DirectPriceListType> Delete(DirectPriceListType DirectPriceListType)
        {
            if (!await DirectPriceListTypeValidator.Delete(DirectPriceListType))
                return DirectPriceListType;

            try
            {
                await UOW.Begin();
                await UOW.DirectPriceListTypeRepository.Delete(DirectPriceListType);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, DirectPriceListType, nameof(DirectPriceListTypeService));
                return DirectPriceListType;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<DirectPriceListType>> BulkDelete(List<DirectPriceListType> DirectPriceListTypes)
        {
            if (!await DirectPriceListTypeValidator.BulkDelete(DirectPriceListTypes))
                return DirectPriceListTypes;

            try
            {
                await UOW.Begin();
                await UOW.DirectPriceListTypeRepository.BulkDelete(DirectPriceListTypes);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, DirectPriceListTypes, nameof(DirectPriceListTypeService));
                return DirectPriceListTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<DirectPriceListType>> Import(List<DirectPriceListType> DirectPriceListTypes)
        {
            if (!await DirectPriceListTypeValidator.Import(DirectPriceListTypes))
                return DirectPriceListTypes;
            try
            {
                await UOW.Begin();
                await UOW.DirectPriceListTypeRepository.BulkMerge(DirectPriceListTypes);
                await UOW.Commit();

                await Logging.CreateAuditLog(DirectPriceListTypes, new { }, nameof(DirectPriceListTypeService));
                return DirectPriceListTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public DirectPriceListTypeFilter ToFilter(DirectPriceListTypeFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<DirectPriceListTypeFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                DirectPriceListTypeFilter subFilter = new DirectPriceListTypeFilter();
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
