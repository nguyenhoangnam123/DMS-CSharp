using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MKpiCriteriaTotal
{
    public interface IKpiCriteriaTotalService : IServiceScoped
    {
        Task<int> Count(KpiCriteriaTotalFilter KpiCriteriaTotalFilter);
        Task<List<KpiCriteriaTotal>> List(KpiCriteriaTotalFilter KpiCriteriaTotalFilter);
        Task<KpiCriteriaTotal> Get(long Id);
    }

    public class KpiCriteriaTotalService : BaseService, IKpiCriteriaTotalService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IKpiCriteriaTotalValidator KpiCriteriaTotalValidator;

        public KpiCriteriaTotalService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IKpiCriteriaTotalValidator KpiCriteriaTotalValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.KpiCriteriaTotalValidator = KpiCriteriaTotalValidator;
        }
        public async Task<int> Count(KpiCriteriaTotalFilter KpiCriteriaTotalFilter)
        {
            try
            {
                int result = await UOW.KpiCriteriaTotalRepository.Count(KpiCriteriaTotalFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiCriteriaTotalService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<KpiCriteriaTotal>> List(KpiCriteriaTotalFilter KpiCriteriaTotalFilter)
        {
            try
            {
                List<KpiCriteriaTotal> KpiCriteriaTotals = await UOW.KpiCriteriaTotalRepository.List(KpiCriteriaTotalFilter);
                return KpiCriteriaTotals;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiCriteriaTotalService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<KpiCriteriaTotal> Get(long Id)
        {
            KpiCriteriaTotal KpiCriteriaTotal = await UOW.KpiCriteriaTotalRepository.Get(Id);
            if (KpiCriteriaTotal == null)
                return null;
            return KpiCriteriaTotal;
        }
    }
}
