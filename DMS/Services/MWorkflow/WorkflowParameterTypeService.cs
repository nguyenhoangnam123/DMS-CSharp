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
using DMS.Enums;

namespace DMS.Services.MWorkflow
{
    public interface IWorkflowParameterTypeService :  IServiceScoped
    {
        Task<int> Count(WorkflowParameterTypeFilter WorkflowParameterTypeFilter);
        Task<List<WorkflowParameterType>> List(WorkflowParameterTypeFilter WorkflowParameterTypeFilter);
    }

    public class WorkflowParameterTypeService : BaseService, IWorkflowParameterTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public WorkflowParameterTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(WorkflowParameterTypeFilter WorkflowParameterTypeFilter)
        {
            try
            {
                int result = await UOW.WorkflowParameterTypeRepository.Count(WorkflowParameterTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WorkflowParameterTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowParameterTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<WorkflowParameterType>> List(WorkflowParameterTypeFilter WorkflowParameterTypeFilter)
        {
            try
            {
                List<WorkflowParameterType> WorkflowParameterTypes = await UOW.WorkflowParameterTypeRepository.List(WorkflowParameterTypeFilter);
                return WorkflowParameterTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WorkflowParameterTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowParameterTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
