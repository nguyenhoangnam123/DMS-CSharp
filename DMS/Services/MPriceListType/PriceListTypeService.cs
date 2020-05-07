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

namespace DMS.Services.MPriceListType
{
    public interface IPriceListTypeService :  IServiceScoped
    {
        Task<int> Count(PriceListTypeFilter PriceListTypeFilter);
        Task<List<PriceListType>> List(PriceListTypeFilter PriceListTypeFilter);
    }

    public class PriceListTypeService : BaseService, IPriceListTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPriceListTypeValidator PriceListTypeValidator;

        public PriceListTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPriceListTypeValidator PriceListTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PriceListTypeValidator = PriceListTypeValidator;
        }
        public async Task<int> Count(PriceListTypeFilter PriceListTypeFilter)
        {
            try
            {
                int result = await UOW.PriceListTypeRepository.Count(PriceListTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<PriceListType>> List(PriceListTypeFilter PriceListTypeFilter)
        {
            try
            {
                List<PriceListType> PriceListTypes = await UOW.PriceListTypeRepository.List(PriceListTypeFilter);
                return PriceListTypes;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
    }
}
