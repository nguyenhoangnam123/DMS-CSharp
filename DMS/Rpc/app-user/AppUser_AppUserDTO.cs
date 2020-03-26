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
        public string DisplayName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long UserStatusId { get; set; }
        public long SexId { get; set; }
        public AppUser_UserStatusDTO UserStatus { get; set; }
        public AppUser_SexDTO Sex { get; set; }
        public List<AppUser_AppUserRoleMappingDTO> AppUserRoleMappings { get; set; }
        public AppUser_AppUserDTO() {}
        public AppUser_AppUserDTO(AppUser AppUser)
        {
            this.Id = AppUser.Id;
            this.Username = AppUser.Username;
            this.Password = AppUser.Password;
            this.DisplayName = AppUser.DisplayName;
            this.Email = AppUser.Email;
            this.Phone = AppUser.Phone;
            this.UserStatusId = AppUser.UserStatusId;
            this.SexId = AppUser.SexId;
            this.UserStatus = AppUser.UserStatus == null ? null : new AppUser_UserStatusDTO(AppUser.UserStatus);
            this.Sex = AppUser.Sex == null ? null : new AppUser_SexDTO(AppUser.Sex);
            this.AppUserRoleMappings = AppUser.AppUserRoleMappings?.Select(x => new AppUser_AppUserRoleMappingDTO(x)).ToList();
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
        public IdFilter UserStatusId { get; set; }
        public IdFilter SexId { get; set; }
        public AppUserOrder OrderBy { get; set; }
    }
}
