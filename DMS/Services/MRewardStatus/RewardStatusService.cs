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

namespace DMS.Services.MRewardStatus
{
    public interface IRewardStatusService :  IServiceScoped
    {
        Task<int> Count(RewardStatusFilter RewardStatusFilter);
        Task<List<RewardStatus>> List(RewardStatusFilter RewardStatusFilter);
    }

    public class RewardStatusService : BaseService, IRewardStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IRewardStatusValidator RewardStatusValidator;

        public RewardStatusService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IRewardStatusValidator RewardStatusValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.RewardStatusValidator = RewardStatusValidator;
        }
        public async Task<int> Count(RewardStatusFilter RewardStatusFilter)
        {
            try
            {
                int result = await UOW.RewardStatusRepository.Count(RewardStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(RewardStatusService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(RewardStatusService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<RewardStatus>> List(RewardStatusFilter RewardStatusFilter)
        {
            try
            {
                List<RewardStatus> RewardStatuss = await UOW.RewardStatusRepository.List(RewardStatusFilter);
                return RewardStatuss;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(RewardStatusService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(RewardStatusService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
