using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_StoreGroupingDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public long? ParentId { get; set; }

        public string Path { get; set; }

        public long Level { get; set; }

        public long StatusId { get; set; }


        public MobileSync_StoreGroupingDTO() { }
        public MobileSync_StoreGroupingDTO(StoreGroupingDAO StoreGroupingDAO)
        {

            this.Id = StoreGroupingDAO.Id;

            this.Code = StoreGroupingDAO.Code;

            this.Name = StoreGroupingDAO.Name;

            this.ParentId = StoreGroupingDAO.ParentId;

            this.Path = StoreGroupingDAO.Path;

            this.Level = StoreGroupingDAO.Level;
            this.StatusId = StoreGroupingDAO.StatusId;

        }
    }

    public class IndirectSalesOrder_StoreGroupingFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public IdFilter ParentId { get; set; }

        public StringFilter Path { get; set; }

        public LongFilter Level { get; set; }
        public IdFilter StatusId { get; set; }

        public StoreGroupingOrder OrderBy { get; set; }
    }
}
