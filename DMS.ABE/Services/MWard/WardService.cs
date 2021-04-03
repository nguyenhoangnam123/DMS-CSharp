using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Repositories;
using DMS.ABE.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DMS.ABE.Services.MWard
{
    public interface IWardService : IServiceScoped
    {
        Task<int> Count(WardFilter WardFilter);
        Task<List<Ward>> List(WardFilter WardFilter);
        Task<Ward> Get(long Id);
    }

    public class WardService : BaseService, IWardService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IWardValidator WardValidator;

        public WardService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IWardValidator WardValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.WardValidator = WardValidator;
        }
        public async Task<int> Count(WardFilter WardFilter)
        {
            try
            {
                int result = await UOW.WardRepository.Count(WardFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WardService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WardService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Ward>> List(WardFilter WardFilter)
        {
            try
            {
                List<Ward> Wards = await UOW.WardRepository.List(WardFilter);
                return Wards;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WardService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WardService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<Ward> Get(long Id)
        {
            Ward Ward = await UOW.WardRepository.Get(Id);
            if (Ward == null)
                return null;
            return Ward;
        }

    }
}
