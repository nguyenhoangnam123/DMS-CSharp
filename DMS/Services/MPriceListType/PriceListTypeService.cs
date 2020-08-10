using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MPriceListType
{
    public interface IPriceListTypeService : IServiceScoped
    {
        Task<int> Count(PriceListTypeFilter PriceListTypeFilter);
        Task<List<PriceListType>> List(PriceListTypeFilter PriceListTypeFilter);
        Task<PriceListType> Get(long Id);
        Task<PriceListType> Create(PriceListType PriceListType);
        Task<PriceListType> Update(PriceListType PriceListType);
        Task<PriceListType> Delete(PriceListType PriceListType);
        Task<List<PriceListType>> BulkDelete(List<PriceListType> PriceListTypes);
        Task<List<PriceListType>> Import(List<PriceListType> PriceListTypes);
        PriceListTypeFilter ToFilter(PriceListTypeFilter PriceListTypeFilter);
    }

    public class PriceListTypeService : BaseService, IPriceListTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPriceListTypeValidator PriceListTypeValidator;

        public PriceListTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPriceListTypeValidator PriceListTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PriceListTypeValidator = PriceListTypeValidator;
        }
        public async Task<int> Count(PriceListTypeFilter PriceListTypeFilter)
        {
            try
            {
                int result = await UOW.PriceListTypeRepository.Count(PriceListTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PriceListType>> List(PriceListTypeFilter PriceListTypeFilter)
        {
            try
            {
                List<PriceListType> PriceListTypes = await UOW.PriceListTypeRepository.List(PriceListTypeFilter);
                return PriceListTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<PriceListType> Get(long Id)
        {
            PriceListType PriceListType = await UOW.PriceListTypeRepository.Get(Id);
            if (PriceListType == null)
                return null;
            return PriceListType;
        }

        public async Task<PriceListType> Create(PriceListType PriceListType)
        {
            if (!await PriceListTypeValidator.Create(PriceListType))
                return PriceListType;

            try
            {
                await UOW.Begin();
                await UOW.PriceListTypeRepository.Create(PriceListType);
                await UOW.Commit();

                await Logging.CreateAuditLog(PriceListType, new { }, nameof(PriceListTypeService));
                return await UOW.PriceListTypeRepository.Get(PriceListType.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PriceListType> Update(PriceListType PriceListType)
        {
            if (!await PriceListTypeValidator.Update(PriceListType))
                return PriceListType;
            try
            {
                var oldData = await UOW.PriceListTypeRepository.Get(PriceListType.Id);

                await UOW.Begin();
                await UOW.PriceListTypeRepository.Update(PriceListType);
                await UOW.Commit();

                var newData = await UOW.PriceListTypeRepository.Get(PriceListType.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(PriceListTypeService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PriceListType> Delete(PriceListType PriceListType)
        {
            if (!await PriceListTypeValidator.Delete(PriceListType))
                return PriceListType;

            try
            {
                await UOW.Begin();
                await UOW.PriceListTypeRepository.Delete(PriceListType);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PriceListType, nameof(PriceListTypeService));
                return PriceListType;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PriceListType>> BulkDelete(List<PriceListType> PriceListTypes)
        {
            if (!await PriceListTypeValidator.BulkDelete(PriceListTypes))
                return PriceListTypes;

            try
            {
                await UOW.Begin();
                await UOW.PriceListTypeRepository.BulkDelete(PriceListTypes);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PriceListTypes, nameof(PriceListTypeService));
                return PriceListTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PriceListType>> Import(List<PriceListType> PriceListTypes)
        {
            if (!await PriceListTypeValidator.Import(PriceListTypes))
                return PriceListTypes;
            try
            {
                await UOW.Begin();
                await UOW.PriceListTypeRepository.BulkMerge(PriceListTypes);
                await UOW.Commit();

                await Logging.CreateAuditLog(PriceListTypes, new { }, nameof(PriceListTypeService));
                return PriceListTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public PriceListTypeFilter ToFilter(PriceListTypeFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PriceListTypeFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PriceListTypeFilter subFilter = new PriceListTypeFilter();
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
