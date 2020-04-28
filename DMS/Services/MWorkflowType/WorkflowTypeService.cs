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

namespace DMS.Services.MWorkflowType
{
    public interface IWorkflowTypeService :  IServiceScoped
    {
        Task<int> Count(WorkflowTypeFilter WorkflowTypeFilter);
        Task<List<WorkflowType>> List(WorkflowTypeFilter WorkflowTypeFilter);
        Task<WorkflowType> Get(long Id);
    }

    public class WorkflowTypeService : BaseService, IWorkflowTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IWorkflowTypeValidator WorkflowTypeValidator;

        public WorkflowTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IWorkflowTypeValidator WorkflowTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.WorkflowTypeValidator = WorkflowTypeValidator;
        }
        public async Task<int> Count(WorkflowTypeFilter WorkflowTypeFilter)
        {
            try
            {
                int result = await UOW.WorkflowTypeRepository.Count(WorkflowTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<WorkflowType>> List(WorkflowTypeFilter WorkflowTypeFilter)
        {
            try
            {
                List<WorkflowType> WorkflowTypes = await UOW.WorkflowTypeRepository.List(WorkflowTypeFilter);
                return WorkflowTypes;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<WorkflowType> Get(long Id)
        {
            WorkflowType WorkflowType = await UOW.WorkflowTypeRepository.Get(Id);
            if (WorkflowType == null)
                return null;
            return WorkflowType;
        }
      
    }
}
