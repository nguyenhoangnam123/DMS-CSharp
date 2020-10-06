using Common;
using DMS.Entities;
using DMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_ProblemDTO : DataDTO
    {
        public long STT { get; set; }
        public long Id { get; set; }
        public string Code { get; set; }
        public long? StoreCheckingId { get; set; }
        public long StoreId { get; set; }
        public long CreatorId { get; set; }
        public long? OrganizationId { get; set; }
        public long ProblemTypeId { get; set; }
        public DateTime NoteAt { get; set; }
        public string NoteAtDisplay => NoteAt.ToString("dd-MM-yyyy");
        public DateTime? CompletedAt { get; set; }
        public string Content { get; set; }
        public long ProblemStatusId { get; set; }
        public Guid RowId { get; set; }
        public MobileSync_AppUserDTO Creator { get; set; }
        public MobileSync_OrganizationDTO Organization { get; set; }
        public MobileSync_ProblemStatusDTO ProblemStatus { get; set; }
        public MobileSync_ProblemTypeDTO ProblemType { get; set; }
        public MobileSync_StoreDTO Store { get; set; }
        public MobileSync_StoreCheckingDTO StoreChecking { get; set; }
        public List<MobileSync_ProblemImageMappingDTO> ProblemImageMappings { get; set; }
        public List<MobileSync_ProblemHistoryDTO> ProblemHistorys { get; set; }
        public MobileSync_ProblemDTO() { }
        public MobileSync_ProblemDTO(ProblemDAO ItemImageMappingDAO)
        {
            this.Id = ItemImageMappingDAO.Id;
            this.Code = ItemImageMappingDAO.Code;
            this.StoreCheckingId = ItemImageMappingDAO.StoreCheckingId;
            this.StoreId = ItemImageMappingDAO.StoreId;
            this.CreatorId = ItemImageMappingDAO.CreatorId;
            this.OrganizationId = ItemImageMappingDAO.Creator.OrganizationId;
            this.ProblemTypeId = ItemImageMappingDAO.ProblemTypeId;
            this.NoteAt = ItemImageMappingDAO.NoteAt;
            this.CompletedAt = ItemImageMappingDAO.CompletedAt;
            this.Content = ItemImageMappingDAO.Content;
            this.ProblemStatusId = ItemImageMappingDAO.ProblemStatusId;
            this.RowId = ItemImageMappingDAO.RowId;
            this.Creator = ItemImageMappingDAO.Creator == null ? null : new MobileSync_AppUserDTO(ItemImageMappingDAO.Creator);
            this.Organization = ItemImageMappingDAO.Creator.Organization == null ? null : new MobileSync_OrganizationDTO(ItemImageMappingDAO.Creator.Organization);
            this.ProblemStatus = ItemImageMappingDAO.ProblemStatus == null ? null : new MobileSync_ProblemStatusDTO(ItemImageMappingDAO.ProblemStatus);
            this.ProblemType = ItemImageMappingDAO.ProblemType == null ? null : new MobileSync_ProblemTypeDTO(ItemImageMappingDAO.ProblemType);
            this.Store = ItemImageMappingDAO.Store == null ? null : new MobileSync_StoreDTO(ItemImageMappingDAO.Store);
            this.StoreChecking = ItemImageMappingDAO.StoreChecking == null ? null : new MobileSync_StoreCheckingDTO(ItemImageMappingDAO.StoreChecking);
            this.ProblemImageMappings = ItemImageMappingDAO.ProblemImageMappings?.Select(x => new MobileSync_ProblemImageMappingDTO(x)).ToList();
            this.ProblemHistorys = ItemImageMappingDAO.ProblemHistories?.Select(x => new MobileSync_ProblemHistoryDTO(x)).ToList();
        }
    }

    public class MobileSync_ProblemFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter StoreCheckingId { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter ProblemTypeId { get; set; }
        public DateFilter NoteAt { get; set; }
        public DateFilter CompletedAt { get; set; }
        public StringFilter Content { get; set; }
        public IdFilter ProblemStatusId { get; set; }
        public ProblemOrder OrderBy { get; set; }
        public List<MobileSync_ProblemFilterDTO> OrFilters { get; set; }
    }
}
