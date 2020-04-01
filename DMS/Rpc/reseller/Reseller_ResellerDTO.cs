using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.reseller
{
    public class Reseller_ResellerDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string TaxCode { get; set; }
        public string CompanyName { get; set; }
        public string DeputyName { get; set; }
        public string Description { get; set; }
        public long ResellerTypeId { get; set; }
        public long ResellerStatusId { get; set; }
        public long StaffId { get; set; }
        public Reseller_ResellerStatusDTO ResellerStatus { get; set; }
        public Reseller_ResellerTypeDTO ResellerType { get; set; }
        public Reseller_AppUserDTO Staff { get; set; }
        public Reseller_ResellerDTO() {}
        public Reseller_ResellerDTO(Reseller Reseller)
        {
            this.Id = Reseller.Id;
            this.Code = Reseller.Code;
            this.Name = Reseller.Name;
            this.Phone = Reseller.Phone;
            this.Email = Reseller.Email;
            this.Address = Reseller.Address;
            this.TaxCode = Reseller.TaxCode;
            this.CompanyName = Reseller.CompanyName;
            this.DeputyName = Reseller.DeputyName;
            this.Description = Reseller.Description;
            this.ResellerTypeId = Reseller.ResellerTypeId;
            this.ResellerStatusId = Reseller.ResellerStatusId;
            this.StaffId = Reseller.StaffId;
            this.ResellerStatus = Reseller.ResellerStatus == null ? null : new Reseller_ResellerStatusDTO(Reseller.ResellerStatus);
            this.ResellerType = Reseller.ResellerType == null ? null : new Reseller_ResellerTypeDTO(Reseller.ResellerType);
            this.Staff = Reseller.Staff == null ? null : new Reseller_AppUserDTO(Reseller.Staff);
            this.Errors = Reseller.Errors;
        }
    }

    public class Reseller_ResellerFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Phone { get; set; }
        public StringFilter Email { get; set; }
        public StringFilter Address { get; set; }
        public StringFilter TaxCode { get; set; }
        public StringFilter CompanyName { get; set; }
        public StringFilter DeputyName { get; set; }
        public StringFilter Description { get; set; }
        public IdFilter ResellerTypeId { get; set; }
        public IdFilter ResellerStatusId { get; set; }
        public IdFilter StaffId { get; set; }
        public ResellerOrder OrderBy { get; set; }
    }
}
