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
    public class WorkflowParameterRoute : Root
    {
        public const string Master = Module + "/workflow-parameter/workflow-parameter-master";
        public const string Detail = Module + "/workflow-parameter/workflow-parameter-detail";
        private const string Default = Rpc + Module + "/workflow-parameter";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        
        public const string FilterListWorkflowParameterType = Default + "/filter-list-workflow-parameter-type";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(WorkflowParameterFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(WorkflowParameterFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(WorkflowParameterFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(WorkflowParameterFilter.WorkflowTypeId), FieldTypeEnum.ID.Id },
            { nameof(WorkflowParameterFilter.WorkflowParameterTypeId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List,
                FilterListWorkflowParameterType, } },
        };
    }
}
