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

namespace DMS.Services.MTransactionType
{
    public interface ITransactionTypeService :  IServiceScoped
    {
        Task<int> Count(TransactionTypeFilter TransactionTypeFilter);
        Task<List<TransactionType>> List(TransactionTypeFilter TransactionTypeFilter);
        Task<TransactionType> Get(long Id);
    }

    public class TransactionTypeService : BaseService, ITransactionTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ITransactionTypeValidator TransactionTypeValidator;

        public TransactionTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ITransactionTypeValidator TransactionTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.TransactionTypeValidator = TransactionTypeValidator;
        }
        public async Task<int> Count(TransactionTypeFilter TransactionTypeFilter)
        {
            try
            {
                int result = await UOW.TransactionTypeRepository.Count(TransactionTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(TransactionTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(TransactionTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<TransactionType>> List(TransactionTypeFilter TransactionTypeFilter)
        {
            try
            {
                List<TransactionType> TransactionTypes = await UOW.TransactionTypeRepository.List(TransactionTypeFilter);
                return TransactionTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(TransactionTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(TransactionTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        
        public async Task<TransactionType> Get(long Id)
        {
            TransactionType TransactionType = await UOW.TransactionTypeRepository.Get(Id);
            if (TransactionType == null)
                return null;
            return TransactionType;
        }
    }
}
