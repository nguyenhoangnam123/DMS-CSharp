using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MPriceList;
using DMS.Services.MOrganization;
using DMS.Services.MPriceListType;
using DMS.Services.MSalesOrderType;
using DMS.Services.MStatus;
using DMS.Enums;

namespace DMS.Rpc.price_list
{
    public partial class PriceListController : RpcController
    {
        [Route(PriceListRoute.FilterListOrganization), HttpPost]
        public async Task<List<PriceList_OrganizationDTO>> FilterListOrganization([FromBody] PriceList_OrganizationFilterDTO PriceList_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = PriceList_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = PriceList_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = PriceList_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = PriceList_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = PriceList_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = PriceList_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            OrganizationFilter.Phone = PriceList_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = PriceList_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = PriceList_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<PriceList_OrganizationDTO> PriceList_OrganizationDTOs = Organizations
                .Select(x => new PriceList_OrganizationDTO(x)).ToList();
            return PriceList_OrganizationDTOs;
        }

        [Route(PriceListRoute.FilterListItem), HttpPost]
        public async Task<List<PriceList_ItemDTO>> FilterListItem([FromBody] PriceList_ItemFilterDTO PriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = PriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = PriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = PriceList_ItemFilterDTO.Code;
            ItemFilter.Name = PriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = PriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = PriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = PriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Item> Items = await ItemService.List(ItemFilter);
            List<PriceList_ItemDTO> PriceList_ItemDTOs = Items
                .Select(x => new PriceList_ItemDTO(x)).ToList();
            return PriceList_ItemDTOs;
        }
        [Route(PriceListRoute.FilterListStoreGrouping), HttpPost]
        public async Task<List<PriceList_StoreGroupingDTO>> FilterListStoreGrouping([FromBody] PriceList_StoreGroupingFilterDTO PriceList_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = int.MaxValue;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Id = PriceList_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = PriceList_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = PriceList_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = PriceList_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = PriceList_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = PriceList_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<PriceList_StoreGroupingDTO> PriceList_StoreGroupingDTOs = StoreGroupings
                .Select(x => new PriceList_StoreGroupingDTO(x)).ToList();
            return PriceList_StoreGroupingDTOs;
        }
        [Route(PriceListRoute.FilterListStore), HttpPost]
        public async Task<List<PriceList_StoreDTO>> FilterListStore([FromBody] PriceList_StoreFilterDTO PriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = PriceList_StoreFilterDTO.Id;
            StoreFilter.Code = PriceList_StoreFilterDTO.Code;
            StoreFilter.Name = PriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = PriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = PriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = PriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = PriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = PriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = PriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = PriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = PriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = PriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = PriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = PriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = PriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = PriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = PriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = PriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = PriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = PriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = PriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            StoreFilter.WorkflowDefinitionId = PriceList_StoreFilterDTO.WorkflowDefinitionId;
            StoreFilter.RequestStateId = PriceList_StoreFilterDTO.RequestStateId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<PriceList_StoreDTO> PriceList_StoreDTOs = Stores
                .Select(x => new PriceList_StoreDTO(x)).ToList();
            return PriceList_StoreDTOs;
        }
        [Route(PriceListRoute.FilterListStoreType), HttpPost]
        public async Task<List<PriceList_StoreTypeDTO>> FilterListStoreType([FromBody] PriceList_StoreTypeFilterDTO PriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = PriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = PriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = PriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<PriceList_StoreTypeDTO> PriceList_StoreTypeDTOs = StoreTypes
                .Select(x => new PriceList_StoreTypeDTO(x)).ToList();
            return PriceList_StoreTypeDTOs;
        }

        [Route(PriceListRoute.FilterListPriceListType), HttpPost]
        public async Task<List<PriceList_PriceListTypeDTO>> FilterListPriceListType([FromBody] PriceList_PriceListTypeFilterDTO PriceList_PriceListTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PriceListTypeFilter PriceListTypeFilter = new PriceListTypeFilter();
            PriceListTypeFilter.Skip = 0;
            PriceListTypeFilter.Take = int.MaxValue;
            PriceListTypeFilter.Take = 20;
            PriceListTypeFilter.OrderBy = PriceListTypeOrder.Id;
            PriceListTypeFilter.OrderType = OrderType.ASC;
            PriceListTypeFilter.Selects = PriceListTypeSelect.ALL;

            List<PriceListType> PriceListTypes = await PriceListTypeService.List(PriceListTypeFilter);
            List<PriceList_PriceListTypeDTO> PriceList_PriceListTypeDTOs = PriceListTypes
                .Select(x => new PriceList_PriceListTypeDTO(x)).ToList();
            return PriceList_PriceListTypeDTOs;
        }
        [Route(PriceListRoute.FilterListSalesOrderType), HttpPost]
        public async Task<List<PriceList_SalesOrderTypeDTO>> FilterListSalesOrderType([FromBody] PriceList_SalesOrderTypeFilterDTO PriceList_SalesOrderTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SalesOrderTypeFilter SalesOrderTypeFilter = new SalesOrderTypeFilter();
            SalesOrderTypeFilter.Skip = 0;
            SalesOrderTypeFilter.Take = 20;
            SalesOrderTypeFilter.OrderBy = SalesOrderTypeOrder.Id;
            SalesOrderTypeFilter.OrderType = OrderType.ASC;
            SalesOrderTypeFilter.Selects = SalesOrderTypeSelect.ALL;
            SalesOrderTypeFilter.Id = PriceList_SalesOrderTypeFilterDTO.Id;
            SalesOrderTypeFilter.Code = PriceList_SalesOrderTypeFilterDTO.Code;
            SalesOrderTypeFilter.Name = PriceList_SalesOrderTypeFilterDTO.Name;

            List<SalesOrderType> SalesOrderTypes = await SalesOrderTypeService.List(SalesOrderTypeFilter);
            List<PriceList_SalesOrderTypeDTO> PriceList_SalesOrderTypeDTOs = SalesOrderTypes
                .Select(x => new PriceList_SalesOrderTypeDTO(x)).ToList();
            return PriceList_SalesOrderTypeDTOs;
        }
        [Route(PriceListRoute.FilterListStatus), HttpPost]
        public async Task<List<PriceList_StatusDTO>> FilterListStatus([FromBody] PriceList_StatusFilterDTO PriceList_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<PriceList_StatusDTO> PriceList_StatusDTOs = Statuses
                .Select(x => new PriceList_StatusDTO(x)).ToList();
            return PriceList_StatusDTOs;
        }

        [Route(PriceListRoute.SingleListOrganization), HttpPost]
        public async Task<List<PriceList_OrganizationDTO>> SingleListOrganization([FromBody] PriceList_OrganizationFilterDTO PriceList_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = PriceList_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = PriceList_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = PriceList_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = PriceList_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = PriceList_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = PriceList_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = PriceList_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = PriceList_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = PriceList_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = PriceList_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<PriceList_OrganizationDTO> PriceList_OrganizationDTOs = Organizations
                .Select(x => new PriceList_OrganizationDTO(x)).ToList();
            return PriceList_OrganizationDTOs;
        }

        [Route(PriceListRoute.SingleListItem), HttpPost]
        public async Task<List<PriceList_ItemDTO>> SingleListItem([FromBody] PriceList_ItemFilterDTO PriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = PriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = PriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = PriceList_ItemFilterDTO.Code;
            ItemFilter.Name = PriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = PriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = PriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = PriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = PriceList_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<PriceList_ItemDTO> PriceList_ItemDTOs = Items
                .Select(x => new PriceList_ItemDTO(x)).ToList();
            return PriceList_ItemDTOs;
        }
        [Route(PriceListRoute.SingleListStoreGrouping), HttpPost]
        public async Task<List<PriceList_StoreGroupingDTO>> SingleListStoreGrouping([FromBody] PriceList_StoreGroupingFilterDTO PriceList_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = int.MaxValue;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Id = PriceList_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = PriceList_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = PriceList_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = PriceList_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = PriceList_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = PriceList_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = PriceList_StoreGroupingFilterDTO.StatusId;

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<PriceList_StoreGroupingDTO> PriceList_StoreGroupingDTOs = StoreGroupings
                .Select(x => new PriceList_StoreGroupingDTO(x)).ToList();
            return PriceList_StoreGroupingDTOs;
        }
        [Route(PriceListRoute.SingleListStore), HttpPost]
        public async Task<List<PriceList_StoreDTO>> SingleListStore([FromBody] PriceList_StoreFilterDTO PriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = PriceList_StoreFilterDTO.Id;
            StoreFilter.Code = PriceList_StoreFilterDTO.Code;
            StoreFilter.Name = PriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = PriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = PriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = PriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = PriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = PriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = PriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = PriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = PriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = PriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = PriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = PriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = PriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = PriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = PriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = PriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = PriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = PriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = PriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = PriceList_StoreFilterDTO.StatusId;
            StoreFilter.WorkflowDefinitionId = PriceList_StoreFilterDTO.WorkflowDefinitionId;
            StoreFilter.RequestStateId = PriceList_StoreFilterDTO.RequestStateId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<PriceList_StoreDTO> PriceList_StoreDTOs = Stores
                .Select(x => new PriceList_StoreDTO(x)).ToList();
            return PriceList_StoreDTOs;
        }
        [Route(PriceListRoute.SingleListStoreType), HttpPost]
        public async Task<List<PriceList_StoreTypeDTO>> SingleListStoreType([FromBody] PriceList_StoreTypeFilterDTO PriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = PriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = PriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = PriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = PriceList_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<PriceList_StoreTypeDTO> PriceList_StoreTypeDTOs = StoreTypes
                .Select(x => new PriceList_StoreTypeDTO(x)).ToList();
            return PriceList_StoreTypeDTOs;
        }

        [Route(PriceListRoute.SingleListPriceListType), HttpPost]
        public async Task<List<PriceList_PriceListTypeDTO>> SingleListPriceListType([FromBody] PriceList_PriceListTypeFilterDTO PriceList_PriceListTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PriceListTypeFilter PriceListTypeFilter = new PriceListTypeFilter();
            PriceListTypeFilter.Skip = 0;
            PriceListTypeFilter.Take = int.MaxValue;
            PriceListTypeFilter.Take = 20;
            PriceListTypeFilter.OrderBy = PriceListTypeOrder.Id;
            PriceListTypeFilter.OrderType = OrderType.ASC;
            PriceListTypeFilter.Selects = PriceListTypeSelect.ALL;

            List<PriceListType> PriceListTypes = await PriceListTypeService.List(PriceListTypeFilter);
            List<PriceList_PriceListTypeDTO> PriceList_PriceListTypeDTOs = PriceListTypes
                .Select(x => new PriceList_PriceListTypeDTO(x)).ToList();
            return PriceList_PriceListTypeDTOs;
        }
        [Route(PriceListRoute.SingleListSalesOrderType), HttpPost]
        public async Task<List<PriceList_SalesOrderTypeDTO>> SingleListSalesOrderType([FromBody] PriceList_SalesOrderTypeFilterDTO PriceList_SalesOrderTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SalesOrderTypeFilter SalesOrderTypeFilter = new SalesOrderTypeFilter();
            SalesOrderTypeFilter.Skip = 0;
            SalesOrderTypeFilter.Take = 20;
            SalesOrderTypeFilter.OrderBy = SalesOrderTypeOrder.Id;
            SalesOrderTypeFilter.OrderType = OrderType.ASC;
            SalesOrderTypeFilter.Selects = SalesOrderTypeSelect.ALL;
            SalesOrderTypeFilter.Id = PriceList_SalesOrderTypeFilterDTO.Id;
            SalesOrderTypeFilter.Code = PriceList_SalesOrderTypeFilterDTO.Code;
            SalesOrderTypeFilter.Name = PriceList_SalesOrderTypeFilterDTO.Name;

            List<SalesOrderType> SalesOrderTypes = await SalesOrderTypeService.List(SalesOrderTypeFilter);
            List<PriceList_SalesOrderTypeDTO> PriceList_SalesOrderTypeDTOs = SalesOrderTypes
                .Select(x => new PriceList_SalesOrderTypeDTO(x)).ToList();
            return PriceList_SalesOrderTypeDTOs;
        }
        [Route(PriceListRoute.SingleListStatus), HttpPost]
        public async Task<List<PriceList_StatusDTO>> SingleListStatus([FromBody] PriceList_StatusFilterDTO PriceList_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<PriceList_StatusDTO> PriceList_StatusDTOs = Statuses
                .Select(x => new PriceList_StatusDTO(x)).ToList();
            return PriceList_StatusDTOs;
        }

        [Route(PriceListRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] PriceList_ItemFilterDTO PriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Id = PriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = PriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = PriceList_ItemFilterDTO.Code;
            ItemFilter.Name = PriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = PriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = PriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = PriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await ItemService.Count(ItemFilter);
        }

        [Route(PriceListRoute.ListItem), HttpPost]
        public async Task<List<PriceList_ItemDTO>> ListItem([FromBody] PriceList_ItemFilterDTO PriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = PriceList_ItemFilterDTO.Skip;
            ItemFilter.Take = PriceList_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = PriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = PriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = PriceList_ItemFilterDTO.Code;
            ItemFilter.Name = PriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = PriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = PriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = PriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Item> Items = await ItemService.List(ItemFilter);
            List<PriceList_ItemDTO> PriceList_ItemDTOs = Items
                .Select(x => new PriceList_ItemDTO(x)).ToList();
            return PriceList_ItemDTOs;
        }
        [Route(PriceListRoute.CountStoreGrouping), HttpPost]
        public async Task<long> CountStoreGrouping([FromBody] PriceList_StoreGroupingFilterDTO PriceList_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Id = PriceList_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = PriceList_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = PriceList_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = PriceList_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = PriceList_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = PriceList_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await StoreGroupingService.Count(StoreGroupingFilter);
        }

        [Route(PriceListRoute.ListStoreGrouping), HttpPost]
        public async Task<List<PriceList_StoreGroupingDTO>> ListStoreGrouping([FromBody] PriceList_StoreGroupingFilterDTO PriceList_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = PriceList_StoreGroupingFilterDTO.Skip;
            StoreGroupingFilter.Take = PriceList_StoreGroupingFilterDTO.Take;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Id = PriceList_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = PriceList_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = PriceList_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = PriceList_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = PriceList_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = PriceList_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<PriceList_StoreGroupingDTO> PriceList_StoreGroupingDTOs = StoreGroupings
                .Select(x => new PriceList_StoreGroupingDTO(x)).ToList();
            return PriceList_StoreGroupingDTOs;
        }
        [Route(PriceListRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] PriceList_StoreFilterDTO PriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Id = PriceList_StoreFilterDTO.Id;
            StoreFilter.Code = PriceList_StoreFilterDTO.Code;
            StoreFilter.Name = PriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = PriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = PriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = PriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = PriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = PriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = PriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = PriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = PriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = PriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = PriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = PriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = PriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = PriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = PriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = PriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = PriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = PriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = PriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            StoreFilter.WorkflowDefinitionId = PriceList_StoreFilterDTO.WorkflowDefinitionId;
            StoreFilter.RequestStateId = PriceList_StoreFilterDTO.RequestStateId;

            return await StoreService.Count(StoreFilter);
        }

        [Route(PriceListRoute.ListStore), HttpPost]
        public async Task<List<PriceList_StoreDTO>> ListStore([FromBody] PriceList_StoreFilterDTO PriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = PriceList_StoreFilterDTO.Skip;
            StoreFilter.Take = PriceList_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = PriceList_StoreFilterDTO.Id;
            StoreFilter.Code = PriceList_StoreFilterDTO.Code;
            StoreFilter.Name = PriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = PriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = PriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = PriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = PriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = PriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = PriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = PriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = PriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = PriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = PriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = PriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = PriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = PriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = PriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = PriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = PriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = PriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = PriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            StoreFilter.WorkflowDefinitionId = PriceList_StoreFilterDTO.WorkflowDefinitionId;
            StoreFilter.RequestStateId = PriceList_StoreFilterDTO.RequestStateId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<PriceList_StoreDTO> PriceList_StoreDTOs = Stores
                .Select(x => new PriceList_StoreDTO(x)).ToList();
            return PriceList_StoreDTOs;
        }
        [Route(PriceListRoute.CountStoreType), HttpPost]
        public async Task<long> CountStoreType([FromBody] PriceList_StoreTypeFilterDTO PriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Id = PriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = PriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = PriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await StoreTypeService.Count(StoreTypeFilter);
        }

        [Route(PriceListRoute.ListStoreType), HttpPost]
        public async Task<List<PriceList_StoreTypeDTO>> ListStoreType([FromBody] PriceList_StoreTypeFilterDTO PriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = PriceList_StoreTypeFilterDTO.Skip;
            StoreTypeFilter.Take = PriceList_StoreTypeFilterDTO.Take;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = PriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = PriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = PriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<PriceList_StoreTypeDTO> PriceList_StoreTypeDTOs = StoreTypes
                .Select(x => new PriceList_StoreTypeDTO(x)).ToList();
            return PriceList_StoreTypeDTOs;
        }
    }
}

