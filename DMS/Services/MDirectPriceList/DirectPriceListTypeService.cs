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

namespace DMS.Services.MDirectPriceList
{
    public interface IDirectPriceListTypeService :  IServiceScoped
    {
        Task<int> Count(DirectPriceListTypeFilter DirectPriceListTypeFilter);
        Task<List<DirectPriceListType>> List(DirectPriceListTypeFilter DirectPriceListTypeFilter);
    }

    public class DirectPriceListTypeService : BaseService, IDirectPriceListTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public DirectPriceListTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(DirectPriceListTypeFilter DirectPriceListTypeFilter)
        {
            try
            {
                int result = await UOW.DirectPriceListTypeRepository.Count(DirectPriceListTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<DirectPriceListType>> List(DirectPriceListTypeFilter DirectPriceListTypeFilter)
        {
            try
            {
                List<DirectPriceListType> DirectPriceListTypes = await UOW.DirectPriceListTypeRepository.List(DirectPriceListTypeFilter);
                return DirectPriceListTypes;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
    }
}
