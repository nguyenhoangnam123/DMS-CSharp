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

namespace DMS.Services.MUsedVariation
{
    public interface IUsedVariationService :  IServiceScoped
    {
        Task<int> Count(UsedVariationFilter UsedVariationFilter);
        Task<List<UsedVariation>> List(UsedVariationFilter UsedVariationFilter);
    }

    public class UsedVariationService : BaseService, IUsedVariationService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IUsedVariationValidator UsedVariationValidator;

        public UsedVariationService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IUsedVariationValidator UsedVariationValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.UsedVariationValidator = UsedVariationValidator;
        }
        public async Task<int> Count(UsedVariationFilter UsedVariationFilter)
        {
            try
            {
                int result = await UOW.UsedVariationRepository.Count(UsedVariationFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(UsedVariationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<UsedVariation>> List(UsedVariationFilter UsedVariationFilter)
        {
            try
            {
                List<UsedVariation> UsedVariations = await UOW.UsedVariationRepository.List(UsedVariationFilter);
                return UsedVariations;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(UsedVariationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
    }
}
