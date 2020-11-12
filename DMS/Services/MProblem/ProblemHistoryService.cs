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

namespace DMS.Services.MProblem
{
    public interface IProblemHistoryService :  IServiceScoped
    {
        Task<int> Count(ProblemHistoryFilter ProblemHistoryFilter);
        Task<List<ProblemHistory>> List(ProblemHistoryFilter ProblemHistoryFilter);
        Task<ProblemHistory> Get(long Id);
        ProblemHistoryFilter ToFilter(ProblemHistoryFilter ProblemHistoryFilter);
    }

    public class ProblemHistoryService : BaseService, IProblemHistoryService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public ProblemHistoryService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(ProblemHistoryFilter ProblemHistoryFilter)
        {
            try
            {
                int result = await UOW.ProblemHistoryRepository.Count(ProblemHistoryFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProblemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<ProblemHistory>> List(ProblemHistoryFilter ProblemHistoryFilter)
        {
            try
            {
                List<ProblemHistory> ProblemHistorys = await UOW.ProblemHistoryRepository.List(ProblemHistoryFilter);
                return ProblemHistorys;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProblemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<ProblemHistory> Get(long Id)
        {
            ProblemHistory ProblemHistory = await UOW.ProblemHistoryRepository.Get(Id);
            if (ProblemHistory == null)
                return null;
            return ProblemHistory;
        } 
        
        public ProblemHistoryFilter ToFilter(ProblemHistoryFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ProblemHistoryFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ProblemHistoryFilter subFilter = new ProblemHistoryFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                }
            }
            return filter;
        }
    }
}
