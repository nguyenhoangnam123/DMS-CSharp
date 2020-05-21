﻿using Common;
using DMS.Entities;

namespace DMS.Rpc.workflow_definition
{
    public class WorkflowDefinition_StatusDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public WorkflowDefinition_StatusDTO() { }
        public WorkflowDefinition_StatusDTO(Status Status)
        {

            this.Id = Status.Id;

            this.Code = Status.Code;

            this.Name = Status.Name;

        }
    }

    public class WorkflowDefinition_StatusFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public StatusOrder OrderBy { get; set; }
    }
}
