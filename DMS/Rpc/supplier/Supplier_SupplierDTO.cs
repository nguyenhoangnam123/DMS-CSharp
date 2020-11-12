using DMS.Common;
using DMS.Entities;
using System;

namespace DMS.Rpc.supplier
{
    public class Supplier_SupplierDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public long? WardId { get; set; }
        public string OwnerName { get; set; }
        public string Description { get; set; }
        public long? PersonInChargeId { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public DateTime UpdatedTime { get; set; }
        public Supplier_AppUserDTO PersonInCharge { get; set; }
        public Supplier_ProvinceDTO Province { get; set; }
        public Supplier_DistrictDTO District { get; set; }
        public Supplier_WardDTO Ward { get; set; }
        public Supplier_StatusDTO Status { get; set; }
        public Supplier_SupplierDTO() { }
        public Supplier_SupplierDTO(Supplier Supplier)
        {
            this.Id = Supplier.Id;
            this.Code = Supplier.Code;
            this.Name = Supplier.Name;
            this.TaxCode = Supplier.TaxCode;
            this.Phone = Supplier.Phone;
            this.Email = Supplier.Email;
            this.Address = Supplier.Address;
            this.ProvinceId = Supplier.ProvinceId;
            this.DistrictId = Supplier.DistrictId;
            this.WardId = Supplier.WardId;
            this.OwnerName = Supplier.OwnerName;
            this.PersonInChargeId = Supplier.PersonInChargeId;
            this.Description = Supplier.Description;
            this.StatusId = Supplier.StatusId;
            this.Used = Supplier.Used;
            this.UpdatedTime = Supplier.UpdatedTime;
            this.District = Supplier.District == null ? null : new Supplier_DistrictDTO(Supplier.District);
            this.PersonInCharge = Supplier.PersonInCharge == null ? null : new Supplier_AppUserDTO(Supplier.PersonInCharge);
            this.Province = Supplier.Province == null ? null : new Supplier_ProvinceDTO(Supplier.Province);
            this.Status = Supplier.Status == null ? null : new Supplier_StatusDTO(Supplier.Status);
            this.Ward = Supplier.Ward == null ? null : new Supplier_WardDTO(Supplier.Ward);
            this.Errors = Supplier.Errors;
        }
    }

    public class Supplier_SupplierFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter TaxCode { get; set; }
        public StringFilter Phone { get; set; }
        public StringFilter Email { get; set; }
        public StringFilter Address { get; set; }
        public IdFilter ProvinceId { get; set; }
        public IdFilter DistrictId { get; set; }
        public IdFilter WardId { get; set; }
        public StringFilter OwnerName { get; set; }
        public StringFilter Description { get; set; }
        public IdFilter PersonInChargeId { get; set; }
        public IdFilter StatusId { get; set; }
        public DateFilter UpdatedTime { get; set; }
        public SupplierOrder OrderBy { get; set; }
    }
}
