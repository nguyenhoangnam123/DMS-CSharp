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

namespace DMS.Services.MPromotionDiscountType
{
    public interface IPromotionDiscountTypeService :  IServiceScoped
    {
        Task<int> Count(PromotionDiscountTypeFilter PromotionDiscountTypeFilter);
        Task<List<PromotionDiscountType>> List(PromotionDiscountTypeFilter PromotionDiscountTypeFilter);
    }

    public class PromotionDiscountTypeService : BaseService, IPromotionDiscountTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPromotionDiscountTypeValidator PromotionDiscountTypeValidator;

        public PromotionDiscountTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPromotionDiscountTypeValidator PromotionDiscountTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PromotionDiscountTypeValidator = PromotionDiscountTypeValidator;
        }
        public async Task<int> Count(PromotionDiscountTypeFilter PromotionDiscountTypeFilter)
        {
            try
            {
                int result = await UOW.PromotionDiscountTypeRepository.Count(PromotionDiscountTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionDiscountTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionDiscountTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PromotionDiscountType>> List(PromotionDiscountTypeFilter PromotionDiscountTypeFilter)
        {
            try
            {
                List<PromotionDiscountType> PromotionDiscountTypes = await UOW.PromotionDiscountTypeRepository.List(PromotionDiscountTypeFilter);
                return PromotionDiscountTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionDiscountTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionDiscountTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
