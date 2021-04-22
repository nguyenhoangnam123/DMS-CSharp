using DMS.Common;
using DMS.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_product_grouping
{
    public partial class KpiProductGroupingController : RpcController
    {
        // list AppUser
        [Route(KpiProductGroupingRoute.ListAppUser), HttpPost]
        public async Task<List<KpiProductGrouping_AppUserDTO>> ListAppUser([FromBody] KpiProductGrouping_AppUserFilterDTO KpiProductGrouping_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = KpiProductGrouping_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiProductGrouping_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiProductGrouping_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = KpiProductGrouping_AppUserFilterDTO.Address;
            AppUserFilter.Email = KpiProductGrouping_AppUserFilterDTO.Email;
            AppUserFilter.Phone = KpiProductGrouping_AppUserFilterDTO.Phone;
            AppUserFilter.SexId = KpiProductGrouping_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = KpiProductGrouping_AppUserFilterDTO.Birthday;
            AppUserFilter.PositionId = KpiProductGrouping_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = KpiProductGrouping_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = KpiProductGrouping_AppUserFilterDTO.OrganizationId;
            AppUserFilter.ProvinceId = KpiProductGrouping_AppUserFilterDTO.ProvinceId;
            AppUserFilter.StatusId = KpiProductGrouping_AppUserFilterDTO.StatusId;
            AppUserFilter.StatusId = new IdFilter { Equal = 1 };
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiProductGrouping_AppUserDTO> KpiProductGrouping_AppUserDTOs = AppUsers
                .Select(x => new KpiProductGrouping_AppUserDTO(x)).ToList();
            return KpiProductGrouping_AppUserDTOs;
        }
        // CountAppUser
        [Route(KpiProductGroupingRoute.CountAppUser), HttpPost]
        public async Task<int> CountAppUser([FromBody] KpiProductGrouping_AppUserFilterDTO KpiProductGrouping_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = KpiProductGrouping_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiProductGrouping_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiProductGrouping_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = KpiProductGrouping_AppUserFilterDTO.Address;
            AppUserFilter.Email = KpiProductGrouping_AppUserFilterDTO.Email;
            AppUserFilter.Phone = KpiProductGrouping_AppUserFilterDTO.Phone;
            AppUserFilter.SexId = KpiProductGrouping_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = KpiProductGrouping_AppUserFilterDTO.Birthday;
            AppUserFilter.PositionId = KpiProductGrouping_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = KpiProductGrouping_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = KpiProductGrouping_AppUserFilterDTO.OrganizationId;
            AppUserFilter.ProvinceId = KpiProductGrouping_AppUserFilterDTO.ProvinceId;
            AppUserFilter.StatusId = KpiProductGrouping_AppUserFilterDTO.StatusId;
            AppUserFilter.StatusId = new IdFilter { Equal = 1 };
            int Total = await AppUserService.Count(AppUserFilter);
            return Total;
        }
        // List Item
        [Route(KpiProductGroupingRoute.ListItem), HttpPost]
        public async Task<List<KpiProductGrouping_ItemDTO>> ListItem([FromBody] KpiProductGrouping_ItemFilterDTO KpiProductGrouping_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            List<Item> Items = await ItemService.List(ItemFilter);
            List<KpiProductGrouping_ItemDTO> KpiProductGrouping_ItemDTOs = Items
                .Select(x => new KpiProductGrouping_ItemDTO(x)).ToList();
            return KpiProductGrouping_ItemDTOs;
        }
        // Count Item
    }
}

