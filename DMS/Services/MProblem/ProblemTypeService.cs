using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MProblem
{
    public interface IProblemTypeService : IServiceScoped
    {
        Task<int> Count(ProblemTypeFilter ProblemTypeFilter);
        Task<List<ProblemType>> List(ProblemTypeFilter ProblemTypeFilter);
    }

    public class ProblemTypeService : BaseService, IProblemTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public ProblemTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(ProblemTypeFilter ProblemTypeFilter)
        {
            try
            {
                int result = await UOW.ProblemTypeRepository.Count(ProblemTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProblemTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<ProblemType>> List(ProblemTypeFilter ProblemTypeFilter)
        {
            try
            {
                List<ProblemType> ProblemTypes = await UOW.ProblemTypeRepository.List(ProblemTypeFilter);
                return ProblemTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProblemTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
    }
}
