using Common;
using DMS.Entities;
using DMS.Models;
using System;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_AppUserDTO : DataDTO
    {

        public long Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string DisplayName { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public long? PositionId { get; set; }

        public string Department { get; set; }

        public long OrganizationId { get; set; }

        public long? SexId { get; set; }

        public long StatusId { get; set; }


        public DateTime? Birthday { get; set; }

        public long? ProvinceId { get; set; }


        public MobileSync_AppUserDTO() { }
        public MobileSync_AppUserDTO(AppUserDAO AppUserDAO)
        {

            this.Id = AppUserDAO.Id;

            this.Username = AppUserDAO.Username;

            this.DisplayName = AppUserDAO.DisplayName;

            this.Address = AppUserDAO.Address;

            this.Email = AppUserDAO.Email;

            this.Phone = AppUserDAO.Phone;

            this.PositionId = AppUserDAO.PositionId;

            this.Department = AppUserDAO.Department;

            this.OrganizationId = AppUserDAO.OrganizationId;

            this.SexId = AppUserDAO.SexId;

            this.StatusId = AppUserDAO.StatusId;

            this.Birthday = AppUserDAO.Birthday;

            this.ProvinceId = AppUserDAO.ProvinceId;
        }
    }
}