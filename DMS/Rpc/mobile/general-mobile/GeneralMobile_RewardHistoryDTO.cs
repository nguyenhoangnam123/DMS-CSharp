using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.mobile.general_mobile
{
    public class GeneralMobile_RewardHistoryDTO : DataDTO
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long StoreId { get; set; }
        public long TurnCounter { get; set; }
        public decimal Revenue { get; set; }
        public Guid RowId { get; set; }
        public GeneralMobile_AppUserDTO AppUser { get; set; }
        public GeneralMobile_StoreDTO Store { get; set; }
        public List<GeneralMobile_RewardHistoryContentDTO> RewardHistoryContents { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public GeneralMobile_RewardHistoryDTO() {}
        public GeneralMobile_RewardHistoryDTO(RewardHistory RewardHistory)
        {
            this.Id = RewardHistory.Id;
            this.AppUserId = RewardHistory.AppUserId;
            this.StoreId = RewardHistory.StoreId;
            this.TurnCounter = RewardHistory.TurnCounter;
            this.Revenue = RewardHistory.Revenue;
            this.RowId = RewardHistory.RowId;
            this.AppUser = RewardHistory.AppUser == null ? null : new GeneralMobile_AppUserDTO(RewardHistory.AppUser);
            this.Store = RewardHistory.Store == null ? null : new GeneralMobile_StoreDTO(RewardHistory.Store);
            this.RewardHistoryContents = RewardHistory.RewardHistoryContents?.Select(x => new GeneralMobile_RewardHistoryContentDTO(x)).ToList();
            this.CreatedAt = RewardHistory.CreatedAt;
            this.UpdatedAt = RewardHistory.UpdatedAt;
            this.Errors = RewardHistory.Errors;
        }
    }

    public class GeneralMobile_RewardHistoryFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter StoreId { get; set; }
        public LongFilter TurnCounter { get; set; }
        public DecimalFilter Revenue { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public string Search { get; set; }
        public RewardHistoryOrder OrderBy { get; set; }
    }
}
