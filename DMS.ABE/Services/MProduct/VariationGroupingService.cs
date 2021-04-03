using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Repositories;
using DMS.ABE.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Services.MProduct
{
    public interface IVariationGroupingService : IServiceScoped
    {
        Task<int> Count(VariationGroupingFilter VariationGroupingFilter);
        Task<List<VariationGrouping>> List(VariationGroupingFilter VariationGroupingFilter);
        Task<VariationGrouping> Get(long Id);
    }

    public class VariationGroupingService : BaseService, IVariationGroupingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IVariationGroupingValidator VariationGroupingValidator;

        public VariationGroupingService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IVariationGroupingValidator VariationGroupingValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.VariationGroupingValidator = VariationGroupingValidator;
        }
        public async Task<int> Count(VariationGroupingFilter VariationGroupingFilter)
        {
            try
            {
                int result = await UOW.VariationGroupingRepository.Count(VariationGroupingFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(VariationGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(VariationGroupingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<VariationGrouping>> List(VariationGroupingFilter VariationGroupingFilter)
        {
            try
            {
                List<VariationGrouping> VariationGroupings = await UOW.VariationGroupingRepository.List(VariationGroupingFilter);
                return VariationGroupings;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(VariationGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(VariationGroupingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        public async Task<VariationGrouping> Get(long Id)
        {
            VariationGrouping VariationGrouping = await UOW.VariationGroupingRepository.Get(Id);
            if (VariationGrouping == null)
                return null;
            return VariationGrouping;
        }

    }
}
