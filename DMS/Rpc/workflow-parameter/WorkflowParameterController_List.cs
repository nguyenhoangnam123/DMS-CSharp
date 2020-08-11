using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MWorkflow;

namespace DMS.Rpc.workflow_parameter
{
    public partial class WorkflowParameterController : RpcController
    {
        [Route(WorkflowParameterRoute.FilterListWorkflowParameterType), HttpPost]
        public async Task<List<WorkflowParameter_WorkflowParameterTypeDTO>> FilterListWorkflowParameterType()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowParameterTypeFilter WorkflowParameterTypeFilter = new WorkflowParameterTypeFilter();
            WorkflowParameterTypeFilter.Skip = 0;
            WorkflowParameterTypeFilter.Take = 20;
            WorkflowParameterTypeFilter.OrderBy = WorkflowParameterTypeOrder.Id;
            WorkflowParameterTypeFilter.OrderType = OrderType.ASC;
            WorkflowParameterTypeFilter.Selects = WorkflowParameterTypeSelect.ALL;

            List<WorkflowParameterType> WorkflowParameterTypes = await WorkflowParameterTypeService.List(WorkflowParameterTypeFilter);
            List<WorkflowParameter_WorkflowParameterTypeDTO> WorkflowParameter_WorkflowParameterTypeDTOs = WorkflowParameterTypes
                .Select(x => new WorkflowParameter_WorkflowParameterTypeDTO(x)).ToList();
            return WorkflowParameter_WorkflowParameterTypeDTOs;
        }
    }
}

