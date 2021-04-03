using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Repositories;
using DMS.ABE.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.ABE.Services.MSex
{
    public interface ISexService : IServiceScoped
    {
        Task<int> Count(SexFilter SexFilter);
        Task<List<Sex>> List(SexFilter SexFilter);
    }

    public class SexService : BaseService, ISexService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ISexValidator SexValidator;

        public SexService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ISexValidator SexValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.SexValidator = SexValidator;
        }
        public async Task<int> Count(SexFilter SexFilter)
        {
            try
            {
                int result = await UOW.SexRepository.Count(SexFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SexService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SexService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Sex>> List(SexFilter SexFilter)
        {
            try
            {
                List<Sex> Sexs = await UOW.SexRepository.List(SexFilter);
                return Sexs;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SexService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SexService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
    }
}
