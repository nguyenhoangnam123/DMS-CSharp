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
using DMS.Enums;

namespace DMS.Services.MPromotionType
{
    public interface IPromotionTypeService :  IServiceScoped
    {
        Task<int> Count(PromotionTypeFilter PromotionTypeFilter);
        Task<List<PromotionType>> List(PromotionTypeFilter PromotionTypeFilter);
    }

    public class PromotionTypeService : BaseService, IPromotionTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPromotionTypeValidator PromotionTypeValidator;

        public PromotionTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPromotionTypeValidator PromotionTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PromotionTypeValidator = PromotionTypeValidator;
        }
        public async Task<int> Count(PromotionTypeFilter PromotionTypeFilter)
        {
            try
            {
                int result = await UOW.PromotionTypeRepository.Count(PromotionTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PromotionType>> List(PromotionTypeFilter PromotionTypeFilter)
        {
            try
            {
                List<PromotionType> PromotionTypes = await UOW.PromotionTypeRepository.List(PromotionTypeFilter);
                return PromotionTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
