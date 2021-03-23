using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MShowingOrder;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MShowingWarehouse;
using DMS.Services.MStatus;
using DMS.Services.MShowingItem;
using DMS.Services.MUnitOfMeasure;
using DMS.Enums;

namespace DMS.Rpc.showing_order
{
    public partial class ShowingOrderController : RpcController
    {
        [Route(ShowingOrderRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] ShowingOrder_StoreFilterDTO ShowingOrder_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Id = ShowingOrder_StoreFilterDTO.Id;
            StoreFilter.Code = ShowingOrder_StoreFilterDTO.Code;
            StoreFilter.CodeDraft = ShowingOrder_StoreFilterDTO.CodeDraft;
            StoreFilter.Name = ShowingOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ShowingOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = ShowingOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ShowingOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ShowingOrder_StoreFilterDTO.StoreGroupingId;
            StoreFilter.Telephone = ShowingOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = ShowingOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = ShowingOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = ShowingOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = ShowingOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = ShowingOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = ShowingOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = ShowingOrder_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = ShowingOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = ShowingOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = ShowingOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = new IdFilter { Equal = StoreStatusEnum.OFFICIAL.Id };
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);

            return await StoreService.Count(StoreFilter);
        }

        [Route(ShowingOrderRoute.ListStore), HttpPost]
        public async Task<List<ShowingOrder_StoreDTO>> ListStore([FromBody] ShowingOrder_StoreFilterDTO ShowingOrder_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = ShowingOrder_StoreFilterDTO.Skip;
            StoreFilter.Take = ShowingOrder_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ShowingOrder_StoreFilterDTO.Id;
            StoreFilter.Code = ShowingOrder_StoreFilterDTO.Code;
            StoreFilter.CodeDraft = ShowingOrder_StoreFilterDTO.CodeDraft;
            StoreFilter.Name = ShowingOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ShowingOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = ShowingOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ShowingOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ShowingOrder_StoreFilterDTO.StoreGroupingId;
            StoreFilter.Telephone = ShowingOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = ShowingOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = ShowingOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = ShowingOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = ShowingOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = ShowingOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = ShowingOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = ShowingOrder_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = ShowingOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = ShowingOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = ShowingOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = new IdFilter { Equal = StoreStatusEnum.OFFICIAL.Id };
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);
            if (StoreFilter.Id.NotEqual.HasValue)
                StoreFilter.Id.In.Remove(StoreFilter.Id.NotEqual.Value);

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ShowingOrder_StoreDTO> ShowingOrder_StoreDTOs = Stores
                .Select(x => new ShowingOrder_StoreDTO(x)).ToList();
            return ShowingOrder_StoreDTOs;
        }
    }
}

