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

namespace DMS.Services.MWorkflow
{
    public interface IWorkflowParameterService :  IServiceScoped
    {
        Task<int> Count(WorkflowParameterFilter WorkflowParameterFilter);
        Task<List<WorkflowParameter>> List(WorkflowParameterFilter WorkflowParameterFilter);
    }

    public class WorkflowParameterService : BaseService, IWorkflowParameterService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public WorkflowParameterService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(WorkflowParameterFilter WorkflowParameterFilter)
        {
            try
            {
                int result = await UOW.WorkflowParameterRepository.Count(WorkflowParameterFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WorkflowParameterService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowParameterService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<WorkflowParameter>> List(WorkflowParameterFilter WorkflowParameterFilter)
        {
            try
            {
                List<WorkflowParameter> WorkflowParameters = await UOW.WorkflowParameterRepository.List(WorkflowParameterFilter);
                return WorkflowParameters;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WorkflowParameterService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowParameterService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
