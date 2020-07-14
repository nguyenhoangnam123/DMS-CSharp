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

namespace DMS.Services.MKpiYear
{
    public interface IKpiYearService :  IServiceScoped
    {
        Task<int> Count(KpiYearFilter KpiYearFilter);
        Task<List<KpiYear>> List(KpiYearFilter KpiYearFilter);
    }

    public class KpiYearService : BaseService, IKpiYearService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IKpiYearValidator KpiYearValidator;

        public KpiYearService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IKpiYearValidator KpiYearValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.KpiYearValidator = KpiYearValidator;
        }
        public async Task<int> Count(KpiYearFilter KpiYearFilter)
        {
            try
            {
                int result = await UOW.KpiYearRepository.Count(KpiYearFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiYearService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiYearService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<KpiYear>> List(KpiYearFilter KpiYearFilter)
        {
            try
            {
                List<KpiYear> KpiYears = await UOW.KpiYearRepository.List(KpiYearFilter);
                return KpiYears;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiYearService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiYearService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
