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

namespace DMS.Services.MResellerType
{
    public interface IResellerTypeService :  IServiceScoped
    {
        Task<int> Count(ResellerTypeFilter ResellerTypeFilter);
        Task<List<ResellerType>> List(ResellerTypeFilter ResellerTypeFilter);
        Task<ResellerType> Get(long Id);
        Task<ResellerType> Create(ResellerType ResellerType);
        Task<ResellerType> Update(ResellerType ResellerType);
        Task<ResellerType> Delete(ResellerType ResellerType);
        Task<List<ResellerType>> BulkDelete(List<ResellerType> ResellerTypes);
        Task<List<ResellerType>> Import(List<ResellerType> ResellerTypes);
        ResellerTypeFilter ToFilter(ResellerTypeFilter ResellerTypeFilter);
    }

    public class ResellerTypeService : BaseService, IResellerTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IResellerTypeValidator ResellerTypeValidator;

        public ResellerTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IResellerTypeValidator ResellerTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ResellerTypeValidator = ResellerTypeValidator;
        }
        public async Task<int> Count(ResellerTypeFilter ResellerTypeFilter)
        {
            try
            {
                int result = await UOW.ResellerTypeRepository.Count(ResellerTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ResellerType>> List(ResellerTypeFilter ResellerTypeFilter)
        {
            try
            {
                List<ResellerType> ResellerTypes = await UOW.ResellerTypeRepository.List(ResellerTypeFilter);
                return ResellerTypes;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<ResellerType> Get(long Id)
        {
            ResellerType ResellerType = await UOW.ResellerTypeRepository.Get(Id);
            if (ResellerType == null)
                return null;
            return ResellerType;
        }
       
        public async Task<ResellerType> Create(ResellerType ResellerType)
        {
            if (!await ResellerTypeValidator.Create(ResellerType))
                return ResellerType;

            try
            {
                await UOW.Begin();
                await UOW.ResellerTypeRepository.Create(ResellerType);
                await UOW.Commit();

                await Logging.CreateAuditLog(ResellerType, new { }, nameof(ResellerTypeService));
                return await UOW.ResellerTypeRepository.Get(ResellerType.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ResellerType> Update(ResellerType ResellerType)
        {
            if (!await ResellerTypeValidator.Update(ResellerType))
                return ResellerType;
            try
            {
                var oldData = await UOW.ResellerTypeRepository.Get(ResellerType.Id);

                await UOW.Begin();
                await UOW.ResellerTypeRepository.Update(ResellerType);
                await UOW.Commit();

                var newData = await UOW.ResellerTypeRepository.Get(ResellerType.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ResellerTypeService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ResellerType> Delete(ResellerType ResellerType)
        {
            if (!await ResellerTypeValidator.Delete(ResellerType))
                return ResellerType;

            try
            {
                await UOW.Begin();
                await UOW.ResellerTypeRepository.Delete(ResellerType);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ResellerType, nameof(ResellerTypeService));
                return ResellerType;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ResellerType>> BulkDelete(List<ResellerType> ResellerTypes)
        {
            if (!await ResellerTypeValidator.BulkDelete(ResellerTypes))
                return ResellerTypes;

            try
            {
                await UOW.Begin();
                await UOW.ResellerTypeRepository.BulkDelete(ResellerTypes);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ResellerTypes, nameof(ResellerTypeService));
                return ResellerTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<ResellerType>> Import(List<ResellerType> ResellerTypes)
        {
            if (!await ResellerTypeValidator.Import(ResellerTypes))
                return ResellerTypes;
            try
            {
                await UOW.Begin();
                await UOW.ResellerTypeRepository.BulkMerge(ResellerTypes);
                await UOW.Commit();

                await Logging.CreateAuditLog(ResellerTypes, new { }, nameof(ResellerTypeService));
                return ResellerTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public ResellerTypeFilter ToFilter(ResellerTypeFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ResellerTypeFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ResellerTypeFilter subFilter = new ResellerTypeFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Code))
                    subFilter.Code = Map(subFilter.Code, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.StatusId))
                    subFilter.StatusId = Map(subFilter.StatusId, currentFilter.Value);
            }
            return filter;
        }
    }
}