using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Repositories;
using DMS.ABE.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DMS.ABE.Services.MNation
{
    public interface INationService : IServiceScoped
    {
        Task<int> Count(NationFilter NationFilter);
        Task<List<Nation>> List(NationFilter NationFilter);
        Task<Nation> Get(long Id);
    }

    public class NationService : BaseService, INationService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private INationValidator NationValidator;

        public NationService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            INationValidator NationValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.NationValidator = NationValidator;
        }
        public async Task<int> Count(NationFilter NationFilter)
        {
            try
            {
                int result = await UOW.NationRepository.Count(NationFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(NationService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(NationService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Nation>> List(NationFilter NationFilter)
        {
            try
            {
                List<Nation> Nations = await UOW.NationRepository.List(NationFilter);
                return Nations;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(NationService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(NationService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        public async Task<Nation> Get(long Id)
        {
            Nation Nation = await UOW.NationRepository.Get(Id);
            if (Nation == null)
                return null;
            return Nation;
        }

    }
}
