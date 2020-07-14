using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MDirectPriceList
{
    public interface IDirectPriceListTypeService : IServiceScoped
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DirectPriceListTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListTypeService));
                    throw new MessageException(ex.InnerException);
                }
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DirectPriceListTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DirectPriceListTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
