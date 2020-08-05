using Common;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class WorkflowParameterType :DataEntity
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
