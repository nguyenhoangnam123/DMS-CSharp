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

namespace DMS.ABE.Services.MTaxType
{
    public interface ITaxTypeService : IServiceScoped
    {
        Task<int> Count(TaxTypeFilter TaxTypeFilter);
        Task<List<TaxType>> List(TaxTypeFilter TaxTypeFilter);
        Task<TaxType> Get(long Id);

    }

    public class TaxTypeService : BaseService, ITaxTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ITaxTypeValidator TaxTypeValidator;

        public TaxTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ITaxTypeValidator TaxTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.TaxTypeValidator = TaxTypeValidator;
        }
        public async Task<int> Count(TaxTypeFilter TaxTypeFilter)
        {
            try
            {
                int result = await UOW.TaxTypeRepository.Count(TaxTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(TaxTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(TaxTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<TaxType>> List(TaxTypeFilter TaxTypeFilter)
        {
            try
            {
                List<TaxType> TaxTypes = await UOW.TaxTypeRepository.List(TaxTypeFilter);
                return TaxTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(TaxTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(TaxTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<TaxType> Get(long Id)
        {
            TaxType TaxType = await UOW.TaxTypeRepository.Get(Id);
            if (TaxType == null)
                return null;
            return TaxType;
        }

    }
}
