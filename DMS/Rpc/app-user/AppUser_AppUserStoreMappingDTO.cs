using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.app_user
{
    public class AppUser_AppUserStoreMappingDTO : DataDTO
    {
        public long AppUserId { get; set; }
        public long StoreId { get; set; }
        public AppUser_StoreDTO Store { get; set; }

        public AppUser_AppUserStoreMappingDTO() { }
        public AppUser_AppUserStoreMappingDTO(AppUserStoreMapping AppUserStoreMapping)
        {
            this.AppUserId = AppUserStoreMapping.AppUserId;
            this.StoreId = AppUserStoreMapping.StoreId;
            this.Store = AppUserStoreMapping.Store == null ? null : new AppUser_StoreDTO(AppUserStoreMapping.Store);
        }
    }

    public class AppUser_AppUserStoreMappingFilterDTO : FilterDTO
    {

        public IdFilter AppUserId { get; set; }

        public IdFilter StoreId { get; set; }

        public AppUserStoreMappingOrder OrderBy { get; set; }
    }
}