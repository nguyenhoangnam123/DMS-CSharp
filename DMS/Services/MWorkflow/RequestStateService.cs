using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MWorkflow
{
    public interface IRequestStateService : IServiceScoped
    {
        Task<int> Count(RequestStateFilter RequestStateFilter);
        Task<List<RequestState>> List(RequestStateFilter RequestStateFilter);
    }

    public class RequestStateService : BaseService, IRequestStateService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public RequestStateService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(RequestStateFilter RequestStateFilter)
        {
            try
            {
                int result = await UOW.RequestStateRepository.Count(RequestStateFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(RequestStateService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<RequestState>> List(RequestStateFilter RequestStateFilter)
        {
            try
            {
                List<RequestState> RequestStates = await UOW.RequestStateRepository.List(RequestStateFilter);
                return RequestStates;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(RequestStateService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
    }
}
