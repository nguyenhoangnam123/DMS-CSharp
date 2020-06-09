using Common;
using DMS.Entities;

namespace DMS.Rpc.reseller
{
    public class Reseller_AppUserDTO : DataDTO
    {

        public long Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string DisplayName { get; set; }

        public long? SexId { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public long StatusId { get; set; }


        public Reseller_AppUserDTO() { }
        public Reseller_AppUserDTO(AppUser AppUser)
        {

            this.Id = AppUser.Id;

            this.Username = AppUser.Username;

            this.DisplayName = AppUser.DisplayName;

            this.SexId = AppUser.SexId;

            this.Address = AppUser.Address;

            this.Email = AppUser.Email;

            this.Phone = AppUser.Phone;

            this.StatusId = AppUser.StatusId;

            this.Errors = AppUser.Errors;
        }
    }

    public class Reseller_AppUserFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Username { get; set; }

        public StringFilter Password { get; set; }

        public StringFilter DisplayName { get; set; }

        public IdFilter SexId { get; set; }

        public StringFilter Address { get; set; }

        public StringFilter Email { get; set; }

        public StringFilter Phone { get; set; }

        public IdFilter StatusId { get; set; }

        public AppUserOrder OrderBy { get; set; }
    }
}