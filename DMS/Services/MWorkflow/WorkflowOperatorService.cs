using DMS.Common;
using DMS.Entities;
using DMS.Repositories;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MWorkflow
{
    public interface IWorkflowOperatorService : IServiceScoped
    {
        Task<int> Count(WorkflowOperatorFilter WorkflowOperatorFilter);
        Task<List<WorkflowOperator>> List(WorkflowOperatorFilter WorkflowOperatorFilter);
    }

    public class WorkflowOperatorService : BaseService, IWorkflowOperatorService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public WorkflowOperatorService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(WorkflowOperatorFilter WorkflowOperatorFilter)
        {
            try
            {
                int result = await UOW.WorkflowOperatorRepository.Count(WorkflowOperatorFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WorkflowOperatorService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowOperatorService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<WorkflowOperator>> List(WorkflowOperatorFilter WorkflowOperatorFilter)
        {
            try
            {
                List<WorkflowOperator> WorkflowOperators = await UOW.WorkflowOperatorRepository.List(WorkflowOperatorFilter);
                return WorkflowOperators;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WorkflowOperatorService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowOperatorService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
