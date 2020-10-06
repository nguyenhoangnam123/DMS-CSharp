using Common;
using DMS.Entities;
using DMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_RequestWorkflowStepMappingDTO : DataDTO
    {
        public Guid RequestId { get; set; }
        public long WorkflowStepId { get; set; }
        public long WorkflowStateId { get; set; }
        public long? AppUserId { get; set; }
        public MobileSync_AppUserDTO AppUser { get; set; }
        public List<MobileSync_AppUserDTO> NextApprovers { get; set; }
        public MobileSync_WorkflowStateDTO WorkflowState { get; set; }
        public MobileSync_WorkflowStepDTO WorkflowStep { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public MobileSync_RequestWorkflowStepMappingDTO() { }
        public MobileSync_RequestWorkflowStepMappingDTO(RequestWorkflowStepMappingDAO RequestWorkflowStepMappingDAO)
        {
            this.RequestId = RequestWorkflowStepMappingDAO.RequestId;
            this.WorkflowStepId = RequestWorkflowStepMappingDAO.WorkflowStepId;
            this.WorkflowStateId = RequestWorkflowStepMappingDAO.WorkflowStateId;
            this.AppUserId = RequestWorkflowStepMappingDAO.AppUserId;
            this.CreatedAt = RequestWorkflowStepMappingDAO.CreatedAt;
            this.UpdatedAt = RequestWorkflowStepMappingDAO.UpdatedAt;
            this.AppUser = RequestWorkflowStepMappingDAO.AppUser == null ? null : new MobileSync_AppUserDTO(RequestWorkflowStepMappingDAO.AppUser);
            this.WorkflowState = RequestWorkflowStepMappingDAO.WorkflowState == null ? null : new MobileSync_WorkflowStateDTO(RequestWorkflowStepMappingDAO.WorkflowState);
            this.WorkflowStep = RequestWorkflowStepMappingDAO.WorkflowStep == null ? null : new MobileSync_WorkflowStepDTO(RequestWorkflowStepMappingDAO.WorkflowStep);
        }
    }
}
