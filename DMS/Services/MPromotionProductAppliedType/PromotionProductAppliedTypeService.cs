using DMS.Common;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;
using DMS.Enums;

namespace DMS.Services.MPromotionProductAppliedType
{
    public interface IPromotionProductAppliedTypeService :  IServiceScoped
    {
        Task<int> Count(PromotionProductAppliedTypeFilter PromotionProductAppliedTypeFilter);
        Task<List<PromotionProductAppliedType>> List(PromotionProductAppliedTypeFilter PromotionProductAppliedTypeFilter);
    }

    public class PromotionProductAppliedTypeService : BaseService, IPromotionProductAppliedTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPromotionProductAppliedTypeValidator PromotionProductAppliedTypeValidator;

        public PromotionProductAppliedTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPromotionProductAppliedTypeValidator PromotionProductAppliedTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PromotionProductAppliedTypeValidator = PromotionProductAppliedTypeValidator;
        }
        public async Task<int> Count(PromotionProductAppliedTypeFilter PromotionProductAppliedTypeFilter)
        {
            try
            {
                int result = await UOW.PromotionProductAppliedTypeRepository.Count(PromotionProductAppliedTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionProductAppliedTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionProductAppliedTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PromotionProductAppliedType>> List(PromotionProductAppliedTypeFilter PromotionProductAppliedTypeFilter)
        {
            try
            {
                List<PromotionProductAppliedType> PromotionProductAppliedTypes = await UOW.PromotionProductAppliedTypeRepository.List(PromotionProductAppliedTypeFilter);
                return PromotionProductAppliedTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionProductAppliedTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionProductAppliedTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
