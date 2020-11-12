using DMS.Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.mobile
{
    public class Mobile_WarehouseDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public long OrganizationId { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public long? WardId { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public List<Mobile_InventoryDTO> Inventories { get; set; }
        public Mobile_WarehouseDTO() { }
        public Mobile_WarehouseDTO(Warehouse Warehouse)
        {
            this.Id = Warehouse.Id;
            this.Code = Warehouse.Code;
            this.Name = Warehouse.Name;
            this.Address = Warehouse.Address;
            this.OrganizationId = Warehouse.OrganizationId;
            this.ProvinceId = Warehouse.ProvinceId;
            this.DistrictId = Warehouse.DistrictId;
            this.WardId = Warehouse.WardId;
            this.StatusId = Warehouse.StatusId;
            this.Used = Warehouse.Used;
            this.Errors = Warehouse.Errors;
        }
    }
}
