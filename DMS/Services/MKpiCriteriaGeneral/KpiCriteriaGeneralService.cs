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

namespace DMS.Services.MKpiCriteriaGeneral
{
    public interface IKpiCriteriaGeneralService :  IServiceScoped
    {
        Task<int> Count(KpiCriteriaGeneralFilter KpiCriteriaGeneralFilter);
        Task<List<KpiCriteriaGeneral>> List(KpiCriteriaGeneralFilter KpiCriteriaGeneralFilter);
    }

    public class KpiCriteriaGeneralService : BaseService, IKpiCriteriaGeneralService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IKpiCriteriaGeneralValidator KpiCriteriaGeneralValidator;

        public KpiCriteriaGeneralService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IKpiCriteriaGeneralValidator KpiCriteriaGeneralValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.KpiCriteriaGeneralValidator = KpiCriteriaGeneralValidator;
        }
        public async Task<int> Count(KpiCriteriaGeneralFilter KpiCriteriaGeneralFilter)
        {
            try
            {
                int result = await UOW.KpiCriteriaGeneralRepository.Count(KpiCriteriaGeneralFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiCriteriaGeneralService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<KpiCriteriaGeneral>> List(KpiCriteriaGeneralFilter KpiCriteriaGeneralFilter)
        {
            try
            {
                List<KpiCriteriaGeneral> KpiCriteriaGenerals = await UOW.KpiCriteriaGeneralRepository.List(KpiCriteriaGeneralFilter);
                return KpiCriteriaGenerals;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiCriteriaGeneralService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
    }
}
