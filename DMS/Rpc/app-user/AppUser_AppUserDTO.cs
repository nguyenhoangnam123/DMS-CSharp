using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.app_user
{
    public class AppUser_AppUserDTO : DataDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string DisplayName { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public DateTime? Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public long? OrganizationId { get; set; }
        public long? ProvinceId { get; set; }
        public long? SexId { get; set; }
        public long StatusId { get; set; }
        public AppUser_OrganizationDTO Organization { get; set; }
        public AppUser_ProvinceDTO Province { get; set; }
        public AppUser_SexDTO Sex { get; set; }
        public AppUser_StatusDTO Status { get; set; }
        public List<AppUser_AppUserRoleMappingDTO> AppUserRoleMappings { get; set; }
        public AppUser_AppUserDTO() { }
        public AppUser_AppUserDTO(AppUser AppUser)
        {
            this.Id = AppUser.Id;
            this.Username = AppUser.Username;
            this.DisplayName = AppUser.DisplayName;
            this.Address = AppUser.Address;
            this.Avatar = AppUser.Avatar;
            this.Birthday = AppUser.Birthday;
            this.Email = AppUser.Email;
            this.Phone = AppUser.Phone;
            this.Position = AppUser.Position;
            this.Department = AppUser.Department;
            this.OrganizationId = AppUser.OrganizationId;
            this.SexId = AppUser.SexId;
            this.StatusId = AppUser.StatusId;
            this.Organization = AppUser.Organization == null ? null : new AppUser_OrganizationDTO(AppUser.Organization);
            this.Province = AppUser.Province == null ? null : new AppUser_ProvinceDTO(AppUser.Province);
            this.Sex = AppUser.Sex == null ? null : new AppUser_SexDTO(AppUser.Sex);
            this.Status = AppUser.Status == null ? null : new AppUser_StatusDTO(AppUser.Status);
            this.AppUserRoleMappings = AppUser.AppUserRoleMappings?.Select(x => new AppUser_AppUserRoleMappingDTO(x)).ToList();
            this.Errors = AppUser.Errors;
        }
    }

    public class AppUser_AppUserFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Username { get; set; }
        public StringFilter Password { get; set; }
        public StringFilter DisplayName { get; set; }
        public StringFilter Address { get; set; }
        public StringFilter Email { get; set; }
        public StringFilter Phone { get; set; }
        public DateFilter Birthday { get; set; }
        public StringFilter Position { get; set; }
        public StringFilter Department { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter SexId { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter ProvinceId { get; set; }
        public AppUserOrder OrderBy { get; set; }
    }
}
