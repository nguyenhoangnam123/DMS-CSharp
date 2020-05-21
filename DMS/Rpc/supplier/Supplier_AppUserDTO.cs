using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.supplier
{
    public class Supplier_AppUserDTO : DataDTO
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public long StatusId { get; set; }


        public Supplier_AppUserDTO() { }
        public Supplier_AppUserDTO(AppUser AppUser)
        {

            this.Id = AppUser.Id;

            this.Username = AppUser.Username;

            this.DisplayName = AppUser.DisplayName;

            this.Email = AppUser.Email;

            this.Phone = AppUser.Phone;

            this.StatusId = AppUser.StatusId;

        }
    }

    public class Supplier_AppUserFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Username { get; set; }

        public StringFilter Password { get; set; }

        public StringFilter DisplayName { get; set; }

        public StringFilter Email { get; set; }

        public StringFilter Phone { get; set; }

        public IdFilter UserStatusId { get; set; }

        public AppUserOrder OrderBy { get; set; }
    }
}
