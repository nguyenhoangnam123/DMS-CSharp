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

namespace DMS.Services.MIndirectSalesOrderStatus
{
    public interface IIndirectSalesOrderStatusService :  IServiceScoped
    {
        Task<int> Count(IndirectSalesOrderStatusFilter IndirectSalesOrderStatusFilter);
        Task<List<IndirectSalesOrderStatus>> List(IndirectSalesOrderStatusFilter IndirectSalesOrderStatusFilter);
    }

    public class IndirectSalesOrderStatusService : BaseService, IIndirectSalesOrderStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IIndirectSalesOrderStatusValidator IndirectSalesOrderStatusValidator;

        public IndirectSalesOrderStatusService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IIndirectSalesOrderStatusValidator IndirectSalesOrderStatusValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.IndirectSalesOrderStatusValidator = IndirectSalesOrderStatusValidator;
        }
        public async Task<int> Count(IndirectSalesOrderStatusFilter IndirectSalesOrderStatusFilter)
        {
            try
            {
                int result = await UOW.IndirectSalesOrderStatusRepository.Count(IndirectSalesOrderStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<IndirectSalesOrderStatus>> List(IndirectSalesOrderStatusFilter IndirectSalesOrderStatusFilter)
        {
            try
            {
                List<IndirectSalesOrderStatus> IndirectSalesOrderStatuss = await UOW.IndirectSalesOrderStatusRepository.List(IndirectSalesOrderStatusFilter);
                return IndirectSalesOrderStatuss;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
    }
}
