using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.showing_warehouse
{
    public class ShowingWarehouse_ShowingWarehouseDTO : DataDTO
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
        public Guid RowId { get; set; }
        public ShowingWarehouse_DistrictDTO District { get; set; }
        public ShowingWarehouse_OrganizationDTO Organization { get; set; }
        public ShowingWarehouse_ProvinceDTO Province { get; set; }
        public ShowingWarehouse_StatusDTO Status { get; set; }
        public ShowingWarehouse_WardDTO Ward { get; set; }
        public List<ShowingWarehouse_ShowingInventoryDTO> ShowingInventories { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ShowingWarehouse_ShowingWarehouseDTO() {}
        public ShowingWarehouse_ShowingWarehouseDTO(ShowingWarehouse ShowingWarehouse)
        {
            this.Id = ShowingWarehouse.Id;
            this.Code = ShowingWarehouse.Code;
            this.Name = ShowingWarehouse.Name;
            this.Address = ShowingWarehouse.Address;
            this.OrganizationId = ShowingWarehouse.OrganizationId;
            this.ProvinceId = ShowingWarehouse.ProvinceId;
            this.DistrictId = ShowingWarehouse.DistrictId;
            this.WardId = ShowingWarehouse.WardId;
            this.StatusId = ShowingWarehouse.StatusId;
            this.RowId = ShowingWarehouse.RowId;
            this.District = ShowingWarehouse.District == null ? null : new ShowingWarehouse_DistrictDTO(ShowingWarehouse.District);
            this.Organization = ShowingWarehouse.Organization == null ? null : new ShowingWarehouse_OrganizationDTO(ShowingWarehouse.Organization);
            this.Province = ShowingWarehouse.Province == null ? null : new ShowingWarehouse_ProvinceDTO(ShowingWarehouse.Province);
            this.Status = ShowingWarehouse.Status == null ? null : new ShowingWarehouse_StatusDTO(ShowingWarehouse.Status);
            this.Ward = ShowingWarehouse.Ward == null ? null : new ShowingWarehouse_WardDTO(ShowingWarehouse.Ward);
            this.ShowingInventories = ShowingWarehouse.ShowingInventories?.Select(x => new ShowingWarehouse_ShowingInventoryDTO(x)).ToList();
            this.CreatedAt = ShowingWarehouse.CreatedAt;
            this.UpdatedAt = ShowingWarehouse.UpdatedAt;
            this.Errors = ShowingWarehouse.Errors;
        }
    }

    public class ShowingWarehouse_ShowingWarehouseFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Address { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter ProvinceId { get; set; }
        public IdFilter DistrictId { get; set; }
        public IdFilter WardId { get; set; }
        public IdFilter StatusId { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public ShowingWarehouseOrder OrderBy { get; set; }
    }
}
