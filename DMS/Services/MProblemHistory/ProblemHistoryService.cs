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

namespace DMS.Services.MProblemHistory
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
        private IProblemHistoryValidator ProblemHistoryValidator;

        public ProblemHistoryService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IProblemHistoryValidator ProblemHistoryValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ProblemHistoryValidator = ProblemHistoryValidator;
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemHistoryService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemHistoryService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
