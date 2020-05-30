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

namespace DMS.Services.MKpiPeriod
{
    public interface IKpiPeriodService :  IServiceScoped
    {
        Task<int> Count(KpiPeriodFilter KpiPeriodFilter);
        Task<List<KpiPeriod>> List(KpiPeriodFilter KpiPeriodFilter);
        Task<KpiPeriod> Get(long Id);
        Task<KpiPeriod> Create(KpiPeriod KpiPeriod);
        Task<KpiPeriod> Update(KpiPeriod KpiPeriod);
        Task<KpiPeriod> Delete(KpiPeriod KpiPeriod);
        Task<List<KpiPeriod>> BulkDelete(List<KpiPeriod> KpiPeriods);
        Task<List<KpiPeriod>> Import(List<KpiPeriod> KpiPeriods);
        KpiPeriodFilter ToFilter(KpiPeriodFilter KpiPeriodFilter);
    }

    public class KpiPeriodService : BaseService, IKpiPeriodService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IKpiPeriodValidator KpiPeriodValidator;

        public KpiPeriodService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IKpiPeriodValidator KpiPeriodValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.KpiPeriodValidator = KpiPeriodValidator;
        }
        public async Task<int> Count(KpiPeriodFilter KpiPeriodFilter)
        {
            try
            {
                int result = await UOW.KpiPeriodRepository.Count(KpiPeriodFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiPeriodService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<KpiPeriod>> List(KpiPeriodFilter KpiPeriodFilter)
        {
            try
            {
                List<KpiPeriod> KpiPeriods = await UOW.KpiPeriodRepository.List(KpiPeriodFilter);
                return KpiPeriods;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiPeriodService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<KpiPeriod> Get(long Id)
        {
            KpiPeriod KpiPeriod = await UOW.KpiPeriodRepository.Get(Id);
            if (KpiPeriod == null)
                return null;
            return KpiPeriod;
        }
       
        public async Task<KpiPeriod> Create(KpiPeriod KpiPeriod)
        {
            if (!await KpiPeriodValidator.Create(KpiPeriod))
                return KpiPeriod;

            try
            {
                await UOW.Begin();
                await UOW.KpiPeriodRepository.Create(KpiPeriod);
                await UOW.Commit();

                await Logging.CreateAuditLog(KpiPeriod, new { }, nameof(KpiPeriodService));
                return await UOW.KpiPeriodRepository.Get(KpiPeriod.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiPeriodService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<KpiPeriod> Update(KpiPeriod KpiPeriod)
        {
            if (!await KpiPeriodValidator.Update(KpiPeriod))
                return KpiPeriod;
            try
            {
                var oldData = await UOW.KpiPeriodRepository.Get(KpiPeriod.Id);

                await UOW.Begin();
                await UOW.KpiPeriodRepository.Update(KpiPeriod);
                await UOW.Commit();

                var newData = await UOW.KpiPeriodRepository.Get(KpiPeriod.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(KpiPeriodService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiPeriodService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<KpiPeriod> Delete(KpiPeriod KpiPeriod)
        {
            if (!await KpiPeriodValidator.Delete(KpiPeriod))
                return KpiPeriod;

            try
            {
                await UOW.Begin();
                await UOW.KpiPeriodRepository.Delete(KpiPeriod);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, KpiPeriod, nameof(KpiPeriodService));
                return KpiPeriod;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiPeriodService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<KpiPeriod>> BulkDelete(List<KpiPeriod> KpiPeriods)
        {
            if (!await KpiPeriodValidator.BulkDelete(KpiPeriods))
                return KpiPeriods;

            try
            {
                await UOW.Begin();
                await UOW.KpiPeriodRepository.BulkDelete(KpiPeriods);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, KpiPeriods, nameof(KpiPeriodService));
                return KpiPeriods;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiPeriodService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<KpiPeriod>> Import(List<KpiPeriod> KpiPeriods)
        {
            if (!await KpiPeriodValidator.Import(KpiPeriods))
                return KpiPeriods;
            try
            {
                await UOW.Begin();
                await UOW.KpiPeriodRepository.BulkMerge(KpiPeriods);
                await UOW.Commit();

                await Logging.CreateAuditLog(KpiPeriods, new { }, nameof(KpiPeriodService));
                return KpiPeriods;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiPeriodService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public KpiPeriodFilter ToFilter(KpiPeriodFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<KpiPeriodFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                KpiPeriodFilter subFilter = new KpiPeriodFilter();
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
