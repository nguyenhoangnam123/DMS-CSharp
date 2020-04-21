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

namespace DMS.Services.MResellerStatus
{
    public interface IResellerStatusService :  IServiceScoped
    {
        Task<int> Count(ResellerStatusFilter ResellerStatusFilter);
        Task<List<ResellerStatus>> List(ResellerStatusFilter ResellerStatusFilter);
        Task<ResellerStatus> Get(long Id);
    }

    public class ResellerStatusService : BaseService, IResellerStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IResellerStatusValidator ResellerStatusValidator;

        public ResellerStatusService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IResellerStatusValidator ResellerStatusValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ResellerStatusValidator = ResellerStatusValidator;
        }
        public async Task<int> Count(ResellerStatusFilter ResellerStatusFilter)
        {
            try
            {
                int result = await UOW.ResellerStatusRepository.Count(ResellerStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ResellerStatus>> List(ResellerStatusFilter ResellerStatusFilter)
        {
            try
            {
                List<ResellerStatus> ResellerStatuss = await UOW.ResellerStatusRepository.List(ResellerStatusFilter);
                return ResellerStatuss;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<ResellerStatus> Get(long Id)
        {
            ResellerStatus ResellerStatus = await UOW.ResellerStatusRepository.Get(Id);
            if (ResellerStatus == null)
                return null;
            return ResellerStatus;
        }
    }
}
