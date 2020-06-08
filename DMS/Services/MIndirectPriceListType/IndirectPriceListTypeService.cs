using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MIndirectPriceListType
{
    public interface IIndirectPriceListTypeService : IServiceScoped
    {
        Task<int> Count(IndirectPriceListTypeFilter IndirectPriceListTypeFilter);
        Task<List<IndirectPriceListType>> List(IndirectPriceListTypeFilter IndirectPriceListTypeFilter);
        Task<IndirectPriceListType> Get(long Id);
        Task<IndirectPriceListType> Create(IndirectPriceListType IndirectPriceListType);
        Task<IndirectPriceListType> Update(IndirectPriceListType IndirectPriceListType);
        Task<IndirectPriceListType> Delete(IndirectPriceListType IndirectPriceListType);
        Task<List<IndirectPriceListType>> BulkDelete(List<IndirectPriceListType> IndirectPriceListTypes);
        Task<List<IndirectPriceListType>> Import(List<IndirectPriceListType> IndirectPriceListTypes);
        IndirectPriceListTypeFilter ToFilter(IndirectPriceListTypeFilter IndirectPriceListTypeFilter);
    }

    public class IndirectPriceListTypeService : BaseService, IIndirectPriceListTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IIndirectPriceListTypeValidator IndirectPriceListTypeValidator;

        public IndirectPriceListTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IIndirectPriceListTypeValidator IndirectPriceListTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.IndirectPriceListTypeValidator = IndirectPriceListTypeValidator;
        }
        public async Task<int> Count(IndirectPriceListTypeFilter IndirectPriceListTypeFilter)
        {
            try
            {
                int result = await UOW.IndirectPriceListTypeRepository.Count(IndirectPriceListTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<IndirectPriceListType>> List(IndirectPriceListTypeFilter IndirectPriceListTypeFilter)
        {
            try
            {
                List<IndirectPriceListType> IndirectPriceListTypes = await UOW.IndirectPriceListTypeRepository.List(IndirectPriceListTypeFilter);
                return IndirectPriceListTypes;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<IndirectPriceListType> Get(long Id)
        {
            IndirectPriceListType IndirectPriceListType = await UOW.IndirectPriceListTypeRepository.Get(Id);
            if (IndirectPriceListType == null)
                return null;
            return IndirectPriceListType;
        }

        public async Task<IndirectPriceListType> Create(IndirectPriceListType IndirectPriceListType)
        {
            if (!await IndirectPriceListTypeValidator.Create(IndirectPriceListType))
                return IndirectPriceListType;

            try
            {
                await UOW.Begin();
                await UOW.IndirectPriceListTypeRepository.Create(IndirectPriceListType);
                await UOW.Commit();

                await Logging.CreateAuditLog(IndirectPriceListType, new { }, nameof(IndirectPriceListTypeService));
                return await UOW.IndirectPriceListTypeRepository.Get(IndirectPriceListType.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<IndirectPriceListType> Update(IndirectPriceListType IndirectPriceListType)
        {
            if (!await IndirectPriceListTypeValidator.Update(IndirectPriceListType))
                return IndirectPriceListType;
            try
            {
                var oldData = await UOW.IndirectPriceListTypeRepository.Get(IndirectPriceListType.Id);

                await UOW.Begin();
                await UOW.IndirectPriceListTypeRepository.Update(IndirectPriceListType);
                await UOW.Commit();

                var newData = await UOW.IndirectPriceListTypeRepository.Get(IndirectPriceListType.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(IndirectPriceListTypeService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<IndirectPriceListType> Delete(IndirectPriceListType IndirectPriceListType)
        {
            if (!await IndirectPriceListTypeValidator.Delete(IndirectPriceListType))
                return IndirectPriceListType;

            try
            {
                await UOW.Begin();
                await UOW.IndirectPriceListTypeRepository.Delete(IndirectPriceListType);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, IndirectPriceListType, nameof(IndirectPriceListTypeService));
                return IndirectPriceListType;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<IndirectPriceListType>> BulkDelete(List<IndirectPriceListType> IndirectPriceListTypes)
        {
            if (!await IndirectPriceListTypeValidator.BulkDelete(IndirectPriceListTypes))
                return IndirectPriceListTypes;

            try
            {
                await UOW.Begin();
                await UOW.IndirectPriceListTypeRepository.BulkDelete(IndirectPriceListTypes);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, IndirectPriceListTypes, nameof(IndirectPriceListTypeService));
                return IndirectPriceListTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<IndirectPriceListType>> Import(List<IndirectPriceListType> IndirectPriceListTypes)
        {
            if (!await IndirectPriceListTypeValidator.Import(IndirectPriceListTypes))
                return IndirectPriceListTypes;
            try
            {
                await UOW.Begin();
                await UOW.IndirectPriceListTypeRepository.BulkMerge(IndirectPriceListTypes);
                await UOW.Commit();

                await Logging.CreateAuditLog(IndirectPriceListTypes, new { }, nameof(IndirectPriceListTypeService));
                return IndirectPriceListTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public IndirectPriceListTypeFilter ToFilter(IndirectPriceListTypeFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<IndirectPriceListTypeFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                IndirectPriceListTypeFilter subFilter = new IndirectPriceListTypeFilter();
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
