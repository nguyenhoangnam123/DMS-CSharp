using Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.store_grouping
{
    public class StoreGrouping_StoreGroupingDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public string Path { get; set; }
        public long Level { get; set; }
        public bool IsActive { get; set; }
        public List<StoreGrouping_StoreDTO> Stores { get; set; }
        public StoreGrouping_StoreGroupingDTO() { }
        public StoreGrouping_StoreGroupingDTO(StoreGrouping StoreGrouping)
        {
            this.Id = StoreGrouping.Id;
            this.Code = StoreGrouping.Code;
            this.Name = StoreGrouping.Name;
            this.ParentId = StoreGrouping.ParentId;
            this.Path = StoreGrouping.Path;
            this.Level = StoreGrouping.Level;
            this.IsActive = StoreGrouping.IsActive;
            this.Stores = StoreGrouping.Stores?.Select(x => new StoreGrouping_StoreDTO(x)).ToList();
        }
    }

    public class StoreGrouping_StoreGroupingFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter ParentId { get; set; }
        public StringFilter Path { get; set; }
        public LongFilter Level { get; set; }
        public StoreGroupingOrder OrderBy { get; set; }
    }
}
