using Common;
using DMS.Entities;

namespace DMS.Rpc.app_user
{
    public class AppUser_UserStatusDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public AppUser_UserStatusDTO() { }
        public AppUser_UserStatusDTO(UserStatus UserStatus)
        {

            this.Id = UserStatus.Id;

            this.Code = UserStatus.Code;

            this.Name = UserStatus.Name;

        }
    }

    public class AppUser_UserStatusFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public UserStatusOrder OrderBy { get; set; }
    }
}