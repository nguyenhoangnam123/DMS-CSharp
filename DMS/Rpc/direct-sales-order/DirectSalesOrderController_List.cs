﻿using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MEditedPriceStatus;
using DMS.Services.MDirectSalesOrder;
using DMS.Services.MOrganization;
using DMS.Services.MProduct;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MSupplier;
using DMS.Services.MUnitOfMeasure;
using DMS.Services.MUnitOfMeasureGrouping;
using DMS.Services.MWorkflow;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.direct_sales_order
{
    public partial class DirectSalesOrderController : RpcController
    {

        [Route(DirectSalesOrderRoute.FilterListStore), HttpPost]
        public async Task<List<DirectSalesOrder_StoreDTO>> FilterListStore([FromBody] DirectSalesOrder_StoreFilterDTO DirectSalesOrder_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = DirectSalesOrder_StoreFilterDTO.Id;
            StoreFilter.Code = DirectSalesOrder_StoreFilterDTO.Code;
            StoreFilter.CodeDraft = DirectSalesOrder_StoreFilterDTO.CodeDraft;
            StoreFilter.Name = DirectSalesOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = DirectSalesOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = DirectSalesOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = DirectSalesOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = DirectSalesOrder_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = DirectSalesOrder_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = DirectSalesOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = DirectSalesOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = DirectSalesOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = DirectSalesOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = DirectSalesOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = DirectSalesOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = DirectSalesOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = DirectSalesOrder_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = DirectSalesOrder_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = DirectSalesOrder_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = DirectSalesOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = DirectSalesOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = DirectSalesOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = DirectSalesOrder_StoreFilterDTO.StatusId;

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);


            List<Store> Stores = await StoreService.List(StoreFilter);
            List<DirectSalesOrder_StoreDTO> DirectSalesOrder_StoreDTOs = Stores
                .Select(x => new DirectSalesOrder_StoreDTO(x)).ToList();
            return DirectSalesOrder_StoreDTOs;
        }

        [Route(DirectSalesOrderRoute.FilterListStoreStatus), HttpPost]
        public async Task<List<DirectSalesOrder_StoreStatusDTO>> FilterListStoreStatus([FromBody] DirectSalesOrder_StoreStatusFilterDTO DirectSalesOrder_StoreStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreStatusFilter StoreStatusFilter = new StoreStatusFilter();
            StoreStatusFilter.Skip = 0;
            StoreStatusFilter.Take = 20;
            StoreStatusFilter.OrderBy = StoreStatusOrder.Id;
            StoreStatusFilter.OrderType = OrderType.ASC;
            StoreStatusFilter.Selects = StoreStatusSelect.ALL;
            StoreStatusFilter.Id = DirectSalesOrder_StoreStatusFilterDTO.Id;
            StoreStatusFilter.Code = DirectSalesOrder_StoreStatusFilterDTO.Code;
            StoreStatusFilter.Name = DirectSalesOrder_StoreStatusFilterDTO.Name;

            List<StoreStatus> StoreStatuses = await StoreStatusService.List(StoreStatusFilter);
            List<DirectSalesOrder_StoreStatusDTO> DirectSalesOrder_StoreStatusDTOs = StoreStatuses
                .Select(x => new DirectSalesOrder_StoreStatusDTO(x)).ToList();
            return DirectSalesOrder_StoreStatusDTOs;
        }

        [Route(DirectSalesOrderRoute.FilterListAppUser), HttpPost]
        public async Task<List<DirectSalesOrder_AppUserDTO>> FilterListAppUser([FromBody] DirectSalesOrder_AppUserFilterDTO DirectSalesOrder_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = DirectSalesOrder_AppUserFilterDTO.Id;
            AppUserFilter.Username = DirectSalesOrder_AppUserFilterDTO.Username;
            AppUserFilter.Password = DirectSalesOrder_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = DirectSalesOrder_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = DirectSalesOrder_AppUserFilterDTO.Address;
            AppUserFilter.Email = DirectSalesOrder_AppUserFilterDTO.Email;
            AppUserFilter.Phone = DirectSalesOrder_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = DirectSalesOrder_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = DirectSalesOrder_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = DirectSalesOrder_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = DirectSalesOrder_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = DirectSalesOrder_AppUserFilterDTO.StatusId;
            AppUserFilter.Birthday = DirectSalesOrder_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = DirectSalesOrder_AppUserFilterDTO.ProvinceId;

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<DirectSalesOrder_AppUserDTO> DirectSalesOrder_AppUserDTOs = AppUsers
                .Select(x => new DirectSalesOrder_AppUserDTO(x)).ToList();
            return DirectSalesOrder_AppUserDTOs;
        }

        [Route(DirectSalesOrderRoute.FilterListOrganization), HttpPost]
        public async Task<List<DirectSalesOrder_OrganizationDTO>> FilterListOrganization([FromBody] DirectSalesOrder_OrganizationFilterDTO DirectSalesOrder_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = 99999;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = DirectSalesOrder_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = DirectSalesOrder_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = DirectSalesOrder_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = DirectSalesOrder_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = DirectSalesOrder_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = DirectSalesOrder_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = null;
            OrganizationFilter.Phone = DirectSalesOrder_OrganizationFilterDTO.Phone;
            OrganizationFilter.Address = DirectSalesOrder_OrganizationFilterDTO.Address;
            OrganizationFilter.Email = DirectSalesOrder_OrganizationFilterDTO.Email;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<DirectSalesOrder_OrganizationDTO> DirectSalesOrder_OrganizationDTOs = Organizations
                .Select(x => new DirectSalesOrder_OrganizationDTO(x)).ToList();
            return DirectSalesOrder_OrganizationDTOs;
        }

        [Route(DirectSalesOrderRoute.FilterListUnitOfMeasure), HttpPost]
        public async Task<List<DirectSalesOrder_UnitOfMeasureDTO>> FilterListUnitOfMeasure([FromBody] DirectSalesOrder_UnitOfMeasureFilterDTO DirectSalesOrder_UnitOfMeasureFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = DirectSalesOrder_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = DirectSalesOrder_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = DirectSalesOrder_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = DirectSalesOrder_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = DirectSalesOrder_UnitOfMeasureFilterDTO.StatusId;

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<DirectSalesOrder_UnitOfMeasureDTO> DirectSalesOrder_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new DirectSalesOrder_UnitOfMeasureDTO(x)).ToList();
            return DirectSalesOrder_UnitOfMeasureDTOs;
        }

        [Route(DirectSalesOrderRoute.FilterListItem), HttpPost]
        public async Task<List<DirectSalesOrder_ItemDTO>> FilterListItem([FromBody] DirectSalesOrder_ItemFilterDTO DirectSalesOrder_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = DirectSalesOrder_ItemFilterDTO.Id;
            ItemFilter.ProductId = DirectSalesOrder_ItemFilterDTO.ProductId;
            ItemFilter.Code = DirectSalesOrder_ItemFilterDTO.Code;
            ItemFilter.Name = DirectSalesOrder_ItemFilterDTO.Name;
            ItemFilter.ScanCode = DirectSalesOrder_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = DirectSalesOrder_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = DirectSalesOrder_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = DirectSalesOrder_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<DirectSalesOrder_ItemDTO> DirectSalesOrder_ItemDTOs = Items
                .Select(x => new DirectSalesOrder_ItemDTO(x)).ToList();
            return DirectSalesOrder_ItemDTOs;
        }
        [Route(DirectSalesOrderRoute.FilterListEditedPriceStatus), HttpPost]
        public async Task<List<DirectSalesOrder_EditedPriceStatusDTO>> FilterListEditedPriceStatus([FromBody] DirectSalesOrder_EditedPriceStatusFilterDTO DirectSalesOrder_EditedPriceStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            EditedPriceStatusFilter EditedPriceStatusFilter = new EditedPriceStatusFilter();
            EditedPriceStatusFilter.Skip = 0;
            EditedPriceStatusFilter.Take = 20;
            EditedPriceStatusFilter.OrderBy = EditedPriceStatusOrder.Id;
            EditedPriceStatusFilter.OrderType = OrderType.ASC;
            EditedPriceStatusFilter.Selects = EditedPriceStatusSelect.ALL;
            EditedPriceStatusFilter.Id = DirectSalesOrder_EditedPriceStatusFilterDTO.Id;
            EditedPriceStatusFilter.Code = DirectSalesOrder_EditedPriceStatusFilterDTO.Code;
            EditedPriceStatusFilter.Name = DirectSalesOrder_EditedPriceStatusFilterDTO.Name;

            List<EditedPriceStatus> EditedPriceStatuses = await EditedPriceStatusService.List(EditedPriceStatusFilter);
            List<DirectSalesOrder_EditedPriceStatusDTO> DirectSalesOrder_EditedPriceStatusDTOs = EditedPriceStatuses
                .Select(x => new DirectSalesOrder_EditedPriceStatusDTO(x)).ToList();
            return DirectSalesOrder_EditedPriceStatusDTOs;
        }
        [Route(DirectSalesOrderRoute.FilterListRequestState), HttpPost]
        public async Task<List<DirectSalesOrder_RequestStateDTO>> FilterListRequestState([FromBody] DirectSalesOrder_RequestStateFilterDTO DirectSalesOrder_RequestStateFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RequestStateFilter RequestStateFilter = new RequestStateFilter();
            RequestStateFilter.Skip = 0;
            RequestStateFilter.Take = 20;
            RequestStateFilter.OrderBy = RequestStateOrder.Id;
            RequestStateFilter.OrderType = OrderType.ASC;
            RequestStateFilter.Selects = RequestStateSelect.ALL;
            RequestStateFilter.Id = DirectSalesOrder_RequestStateFilterDTO.Id;
            RequestStateFilter.Code = DirectSalesOrder_RequestStateFilterDTO.Code;
            RequestStateFilter.Name = DirectSalesOrder_RequestStateFilterDTO.Name;

            List<RequestState> RequestStatees = await RequestStateService.List(RequestStateFilter);
            List<DirectSalesOrder_RequestStateDTO> DirectSalesOrder_RequestStateDTOs = RequestStatees
                .Select(x => new DirectSalesOrder_RequestStateDTO(x)).ToList();
            return DirectSalesOrder_RequestStateDTOs;
        }


        [Route(DirectSalesOrderRoute.SingleListOrganization), HttpPost]
        public async Task<List<DirectSalesOrder_OrganizationDTO>> SingleListOrganization([FromBody] DirectSalesOrder_OrganizationFilterDTO DirectSalesOrder_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = 99999;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = DirectSalesOrder_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = DirectSalesOrder_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = DirectSalesOrder_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = DirectSalesOrder_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = DirectSalesOrder_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = DirectSalesOrder_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = DirectSalesOrder_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = DirectSalesOrder_OrganizationFilterDTO.Phone;
            OrganizationFilter.Address = DirectSalesOrder_OrganizationFilterDTO.Address;
            OrganizationFilter.Email = DirectSalesOrder_OrganizationFilterDTO.Email;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<DirectSalesOrder_OrganizationDTO> DirectSalesOrder_OrganizationDTOs = Organizations
                .Select(x => new DirectSalesOrder_OrganizationDTO(x)).ToList();
            return DirectSalesOrder_OrganizationDTOs;
        }

        [Route(DirectSalesOrderRoute.SingleListStore), HttpPost]
        public async Task<List<DirectSalesOrder_StoreDTO>> SingleListStore([FromBody] DirectSalesOrder_StoreFilterDTO DirectSalesOrder_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = DirectSalesOrder_StoreFilterDTO.Id;
            StoreFilter.Code = DirectSalesOrder_StoreFilterDTO.Code;
            StoreFilter.CodeDraft = DirectSalesOrder_StoreFilterDTO.CodeDraft;
            StoreFilter.Name = DirectSalesOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = DirectSalesOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = DirectSalesOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = DirectSalesOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = DirectSalesOrder_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = DirectSalesOrder_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = DirectSalesOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = DirectSalesOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = DirectSalesOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = DirectSalesOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = DirectSalesOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = DirectSalesOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = DirectSalesOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = DirectSalesOrder_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = DirectSalesOrder_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = DirectSalesOrder_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = DirectSalesOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = DirectSalesOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = DirectSalesOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<DirectSalesOrder_StoreDTO> DirectSalesOrder_StoreDTOs = Stores
                .Select(x => new DirectSalesOrder_StoreDTO(x)).ToList();
            return DirectSalesOrder_StoreDTOs;
        }
        [Route(DirectSalesOrderRoute.SingleListAppUser), HttpPost]
        public async Task<List<DirectSalesOrder_AppUserDTO>> SingleListAppUser([FromBody] DirectSalesOrder_AppUserFilterDTO DirectSalesOrder_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = DirectSalesOrder_AppUserFilterDTO.Id;
            AppUserFilter.Username = DirectSalesOrder_AppUserFilterDTO.Username;
            AppUserFilter.Password = DirectSalesOrder_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = DirectSalesOrder_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = DirectSalesOrder_AppUserFilterDTO.Address;
            AppUserFilter.Email = DirectSalesOrder_AppUserFilterDTO.Email;
            AppUserFilter.Phone = DirectSalesOrder_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = DirectSalesOrder_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = DirectSalesOrder_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = DirectSalesOrder_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = DirectSalesOrder_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            AppUserFilter.Birthday = DirectSalesOrder_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = DirectSalesOrder_AppUserFilterDTO.ProvinceId;

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<DirectSalesOrder_AppUserDTO> DirectSalesOrder_AppUserDTOs = AppUsers
                .Select(x => new DirectSalesOrder_AppUserDTO(x)).ToList();
            return DirectSalesOrder_AppUserDTOs;
        }

        [Route(DirectSalesOrderRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<DirectSalesOrder_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] DirectSalesOrder_UnitOfMeasureFilterDTO DirectSalesOrder_UnitOfMeasureFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            //TODO cần optimize lại phần này, sử dụng itemId thay vì productId

            List<Product> Products = await ProductService.List(new ProductFilter
            {
                Id = DirectSalesOrder_UnitOfMeasureFilterDTO.ProductId,
                Selects = ProductSelect.Id,
            });
            long ProductId = Products.Select(p => p.Id).FirstOrDefault();
            Product Product = await ProductService.Get(ProductId);

            List<DirectSalesOrder_UnitOfMeasureDTO> DirectSalesOrder_UnitOfMeasureDTOs = new List<DirectSalesOrder_UnitOfMeasureDTO>();
            if (Product.UnitOfMeasureGrouping != null)
            {
                DirectSalesOrder_UnitOfMeasureDTOs = Product.UnitOfMeasureGrouping.UnitOfMeasureGroupingContents.Select(x => new DirectSalesOrder_UnitOfMeasureDTO(x)).ToList();
            }
            DirectSalesOrder_UnitOfMeasureDTO DirectSalesOrder_UnitOfMeasureDTO = new DirectSalesOrder_UnitOfMeasureDTO
            {
                Id = Product.UnitOfMeasure.Id,
                Code = Product.UnitOfMeasure.Code,
                Name = Product.UnitOfMeasure.Name,
                Description = Product.UnitOfMeasure.Description,
                StatusId = StatusEnum.ACTIVE.Id,
                Factor = 1,
            };
            DirectSalesOrder_UnitOfMeasureDTOs.Add(DirectSalesOrder_UnitOfMeasureDTO);
            DirectSalesOrder_UnitOfMeasureDTOs = DirectSalesOrder_UnitOfMeasureDTOs.Distinct().ToList();
            return DirectSalesOrder_UnitOfMeasureDTOs;
        }

        [Route(DirectSalesOrderRoute.SingleListSupplier), HttpPost]
        public async Task<List<DirectSalesOrder_SupplierDTO>> SingleListSupplier([FromBody] DirectSalesOrder_SupplierFilterDTO Product_SupplierFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Skip = 0;
            SupplierFilter.Take = 20;
            SupplierFilter.OrderBy = SupplierOrder.Id;
            SupplierFilter.OrderType = OrderType.ASC;
            SupplierFilter.Selects = SupplierSelect.ALL;
            SupplierFilter.Id = Product_SupplierFilterDTO.Id;
            SupplierFilter.Code = Product_SupplierFilterDTO.Code;
            SupplierFilter.Name = Product_SupplierFilterDTO.Name;
            SupplierFilter.TaxCode = Product_SupplierFilterDTO.TaxCode;
            SupplierFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            List<DirectSalesOrder_SupplierDTO> DirectSalesOrder_SupplierDTOs = Suppliers
                .Select(x => new DirectSalesOrder_SupplierDTO(x)).ToList();
            return DirectSalesOrder_SupplierDTOs;
        }
        [Route(DirectSalesOrderRoute.SingleListStoreGrouping), HttpPost]
        public async Task<List<DirectSalesOrder_StoreGroupingDTO>> SingleListStoreGrouping([FromBody] DirectSalesOrder_StoreGroupingFilterDTO DirectSalesOrder_StoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Code = DirectSalesOrder_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = DirectSalesOrder_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.Level = DirectSalesOrder_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.Path = DirectSalesOrder_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<DirectSalesOrder_StoreGroupingDTO> DirectSalesOrder_StoreGroupingDTOs = StoreGroupings
                .Select(x => new DirectSalesOrder_StoreGroupingDTO(x)).ToList();
            return DirectSalesOrder_StoreGroupingDTOs;
        }
        [Route(DirectSalesOrderRoute.SingleListStoreType), HttpPost]
        public async Task<List<DirectSalesOrder_StoreTypeDTO>> SingleListStoreType([FromBody] DirectSalesOrder_StoreTypeFilterDTO DirectSalesOrder_StoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = DirectSalesOrder_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = DirectSalesOrder_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = DirectSalesOrder_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<DirectSalesOrder_StoreTypeDTO> DirectSalesOrder_StoreTypeDTOs = StoreTypes
                .Select(x => new DirectSalesOrder_StoreTypeDTO(x)).ToList();
            return DirectSalesOrder_StoreTypeDTOs;
        }
        [Route(DirectSalesOrderRoute.SingleListItem), HttpPost]
        public async Task<List<DirectSalesOrder_ItemDTO>> SingleListItem([FromBody] DirectSalesOrder_ItemFilterDTO DirectSalesOrder_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = DirectSalesOrder_ItemFilterDTO.Id;
            ItemFilter.ProductId = DirectSalesOrder_ItemFilterDTO.ProductId;
            ItemFilter.Code = DirectSalesOrder_ItemFilterDTO.Code;
            ItemFilter.Name = DirectSalesOrder_ItemFilterDTO.Name;
            ItemFilter.ScanCode = DirectSalesOrder_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = DirectSalesOrder_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = DirectSalesOrder_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Item> Items = await ItemService.List(ItemFilter);
            List<DirectSalesOrder_ItemDTO> DirectSalesOrder_ItemDTOs = Items
                .Select(x => new DirectSalesOrder_ItemDTO(x)).ToList();
            return DirectSalesOrder_ItemDTOs;
        }

        [Route(DirectSalesOrderRoute.SingleListProductGrouping), HttpPost]
        public async Task<List<DirectSalesOrder_ProductGroupingDTO>> SingleListProductGrouping([FromBody] DirectSalesOrder_ProductGroupingFilterDTO DirectSalesOrder_ProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code
                | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;

            List<ProductGrouping> DirectSalesOrderGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<DirectSalesOrder_ProductGroupingDTO> DirectSalesOrder_ProductGroupingDTOs = DirectSalesOrderGroupings
                .Select(x => new DirectSalesOrder_ProductGroupingDTO(x)).ToList();
            return DirectSalesOrder_ProductGroupingDTOs;
        }
        [Route(DirectSalesOrderRoute.SingleListProductType), HttpPost]
        public async Task<List<DirectSalesOrder_ProductTypeDTO>> SingleListProductType([FromBody] DirectSalesOrder_ProductTypeFilterDTO DirectSalesOrder_ProductTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Skip = 0;
            ProductTypeFilter.Take = 20;
            ProductTypeFilter.OrderBy = ProductTypeOrder.Id;
            ProductTypeFilter.OrderType = OrderType.ASC;
            ProductTypeFilter.Selects = ProductTypeSelect.ALL;
            ProductTypeFilter.Id = DirectSalesOrder_ProductTypeFilterDTO.Id;
            ProductTypeFilter.Code = DirectSalesOrder_ProductTypeFilterDTO.Code;
            ProductTypeFilter.Name = DirectSalesOrder_ProductTypeFilterDTO.Name;
            ProductTypeFilter.Description = DirectSalesOrder_ProductTypeFilterDTO.Description;
            ProductTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            List<DirectSalesOrder_ProductTypeDTO> DirectSalesOrder_ProductTypeDTOs = ProductTypes
                .Select(x => new DirectSalesOrder_ProductTypeDTO(x)).ToList();
            return DirectSalesOrder_ProductTypeDTOs;
        }

        [Route(DirectSalesOrderRoute.SingleListEditedPriceStatus), HttpPost]
        public async Task<List<DirectSalesOrder_EditedPriceStatusDTO>> SingleListEditedPriceStatus([FromBody] DirectSalesOrder_EditedPriceStatusFilterDTO DirectSalesOrder_EditedPriceStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            EditedPriceStatusFilter EditedPriceStatusFilter = new EditedPriceStatusFilter();
            EditedPriceStatusFilter.Skip = 0;
            EditedPriceStatusFilter.Take = 20;
            EditedPriceStatusFilter.OrderBy = EditedPriceStatusOrder.Id;
            EditedPriceStatusFilter.OrderType = OrderType.ASC;
            EditedPriceStatusFilter.Selects = EditedPriceStatusSelect.ALL;
            EditedPriceStatusFilter.Id = DirectSalesOrder_EditedPriceStatusFilterDTO.Id;
            EditedPriceStatusFilter.Code = DirectSalesOrder_EditedPriceStatusFilterDTO.Code;
            EditedPriceStatusFilter.Name = DirectSalesOrder_EditedPriceStatusFilterDTO.Name;

            List<EditedPriceStatus> EditedPriceStatuses = await EditedPriceStatusService.List(EditedPriceStatusFilter);
            List<DirectSalesOrder_EditedPriceStatusDTO> DirectSalesOrder_EditedPriceStatusDTOs = EditedPriceStatuses
                .Select(x => new DirectSalesOrder_EditedPriceStatusDTO(x)).ToList();
            return DirectSalesOrder_EditedPriceStatusDTOs;
        }
        [Route(DirectSalesOrderRoute.SingleListRequestState), HttpPost]
        public async Task<List<DirectSalesOrder_RequestStateDTO>> SingleListRequestState([FromBody] DirectSalesOrder_RequestStateFilterDTO DirectSalesOrder_RequestStateFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RequestStateFilter RequestStateFilter = new RequestStateFilter();
            RequestStateFilter.Skip = 0;
            RequestStateFilter.Take = 20;
            RequestStateFilter.OrderBy = RequestStateOrder.Id;
            RequestStateFilter.OrderType = OrderType.ASC;
            RequestStateFilter.Selects = RequestStateSelect.ALL;
            RequestStateFilter.Id = DirectSalesOrder_RequestStateFilterDTO.Id;
            RequestStateFilter.Code = DirectSalesOrder_RequestStateFilterDTO.Code;
            RequestStateFilter.Name = DirectSalesOrder_RequestStateFilterDTO.Name;

            List<RequestState> RequestStatees = await RequestStateService.List(RequestStateFilter);
            List<DirectSalesOrder_RequestStateDTO> DirectSalesOrder_RequestStateDTOs = RequestStatees
                .Select(x => new DirectSalesOrder_RequestStateDTO(x)).ToList();
            return DirectSalesOrder_RequestStateDTOs;
        }

        [Route(DirectSalesOrderRoute.SingleListTaxType), HttpPost]
        public async Task<List<DirectSalesOrder_TaxTypeDTO>> SingleListTaxType([FromBody] DirectSalesOrder_TaxTypeFilterDTO DirectSalesOrder_TaxTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TaxTypeFilter TaxTypeFilter = new TaxTypeFilter();
            TaxTypeFilter.Skip = 0;
            TaxTypeFilter.Take = 20;
            TaxTypeFilter.OrderBy = TaxTypeOrder.Id;
            TaxTypeFilter.OrderType = OrderType.ASC;
            TaxTypeFilter.Selects = TaxTypeSelect.ALL;
            TaxTypeFilter.Id = DirectSalesOrder_TaxTypeFilterDTO.Id;
            TaxTypeFilter.Code = DirectSalesOrder_TaxTypeFilterDTO.Code;
            TaxTypeFilter.Name = DirectSalesOrder_TaxTypeFilterDTO.Name;
            TaxTypeFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<TaxType> TaxTypes = await TaxTypeService.List(TaxTypeFilter);
            List<DirectSalesOrder_TaxTypeDTO> DirectSalesOrder_TaxTypeDTOs = TaxTypes
                .Select(x => new DirectSalesOrder_TaxTypeDTO(x)).ToList();
            return DirectSalesOrder_TaxTypeDTOs;
        }

        [Route(DirectSalesOrderRoute.CountBuyerStore), HttpPost]
        public async Task<long> CountBuyerStore([FromBody] DirectSalesOrder_StoreFilterDTO DirectSalesOrder_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Id = DirectSalesOrder_StoreFilterDTO.Id;
            StoreFilter.Code = DirectSalesOrder_StoreFilterDTO.Code;
            StoreFilter.CodeDraft = DirectSalesOrder_StoreFilterDTO.CodeDraft;
            StoreFilter.Name = DirectSalesOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = DirectSalesOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = DirectSalesOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = DirectSalesOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = DirectSalesOrder_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = DirectSalesOrder_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = DirectSalesOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = DirectSalesOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = DirectSalesOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = DirectSalesOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = DirectSalesOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = DirectSalesOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = DirectSalesOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = DirectSalesOrder_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = DirectSalesOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = DirectSalesOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = DirectSalesOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = DirectSalesOrder_StoreFilterDTO.StoreStatusId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);

            if (DirectSalesOrder_StoreFilterDTO.SaleEmployeeId.HasValue && DirectSalesOrder_StoreFilterDTO.SaleEmployeeId.Equal.HasValue)
            {
                int count = await StoreService.CountInScoped(StoreFilter, DirectSalesOrder_StoreFilterDTO.SaleEmployeeId.Equal.Value);
                return count;
            }
            return 0;
        }

        [Route(DirectSalesOrderRoute.ListBuyerStore), HttpPost]
        public async Task<List<DirectSalesOrder_StoreDTO>> ListBuyerStore([FromBody] DirectSalesOrder_StoreFilterDTO DirectSalesOrder_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = DirectSalesOrder_StoreFilterDTO.Skip;
            StoreFilter.Take = DirectSalesOrder_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = DirectSalesOrder_StoreFilterDTO.Id;
            StoreFilter.Code = DirectSalesOrder_StoreFilterDTO.Code;
            StoreFilter.CodeDraft = DirectSalesOrder_StoreFilterDTO.CodeDraft;
            StoreFilter.Name = DirectSalesOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = DirectSalesOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = DirectSalesOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = DirectSalesOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = DirectSalesOrder_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = DirectSalesOrder_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = DirectSalesOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = DirectSalesOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = DirectSalesOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = DirectSalesOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = DirectSalesOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = DirectSalesOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = DirectSalesOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = DirectSalesOrder_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = DirectSalesOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = DirectSalesOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = DirectSalesOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = DirectSalesOrder_StoreFilterDTO.StoreStatusId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);

            if (DirectSalesOrder_StoreFilterDTO.SaleEmployeeId.HasValue && DirectSalesOrder_StoreFilterDTO.SaleEmployeeId.Equal.HasValue)
            {
                List<Store> Stores = await StoreService.ListInScoped(StoreFilter, DirectSalesOrder_StoreFilterDTO.SaleEmployeeId.Equal.Value);
                List<DirectSalesOrder_StoreDTO> DirectSalesOrder_StoreDTOs = Stores
                    .Select(x => new DirectSalesOrder_StoreDTO(x)).ToList();
                return DirectSalesOrder_StoreDTOs;
            }
            return new List<DirectSalesOrder_StoreDTO>();
        }

        [Route(DirectSalesOrderRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] DirectSalesOrder_StoreFilterDTO DirectSalesOrder_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Id = DirectSalesOrder_StoreFilterDTO.Id;
            StoreFilter.Code = DirectSalesOrder_StoreFilterDTO.Code;
            StoreFilter.CodeDraft = DirectSalesOrder_StoreFilterDTO.CodeDraft;
            StoreFilter.Name = DirectSalesOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = DirectSalesOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = DirectSalesOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = DirectSalesOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = DirectSalesOrder_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = DirectSalesOrder_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = DirectSalesOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = DirectSalesOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = DirectSalesOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = DirectSalesOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = DirectSalesOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = DirectSalesOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = DirectSalesOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = DirectSalesOrder_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = DirectSalesOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = DirectSalesOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = DirectSalesOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = new IdFilter { Equal = StoreStatusEnum.OFFICIAL.Id };
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);

            return await StoreService.Count(StoreFilter);
        }

        [Route(DirectSalesOrderRoute.ListStore), HttpPost]
        public async Task<List<DirectSalesOrder_StoreDTO>> ListStore([FromBody] DirectSalesOrder_StoreFilterDTO DirectSalesOrder_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = DirectSalesOrder_StoreFilterDTO.Skip;
            StoreFilter.Take = DirectSalesOrder_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = DirectSalesOrder_StoreFilterDTO.Id;
            StoreFilter.Code = DirectSalesOrder_StoreFilterDTO.Code;
            StoreFilter.CodeDraft = DirectSalesOrder_StoreFilterDTO.CodeDraft;
            StoreFilter.Name = DirectSalesOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = DirectSalesOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = DirectSalesOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = DirectSalesOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = DirectSalesOrder_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = DirectSalesOrder_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = DirectSalesOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = DirectSalesOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = DirectSalesOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = DirectSalesOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = DirectSalesOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = DirectSalesOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = DirectSalesOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = DirectSalesOrder_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = DirectSalesOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = DirectSalesOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = DirectSalesOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = new IdFilter { Equal = StoreStatusEnum.OFFICIAL.Id };
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<DirectSalesOrder_StoreDTO> DirectSalesOrder_StoreDTOs = Stores
                .Select(x => new DirectSalesOrder_StoreDTO(x)).ToList();
            return DirectSalesOrder_StoreDTOs;
        }

        [Route(DirectSalesOrderRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] DirectSalesOrder_ItemFilterDTO DirectSalesOrder_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Id = DirectSalesOrder_ItemFilterDTO.Id;
            ItemFilter.Code = DirectSalesOrder_ItemFilterDTO.Code;
            ItemFilter.Name = DirectSalesOrder_ItemFilterDTO.Name;
            ItemFilter.OtherName = DirectSalesOrder_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = DirectSalesOrder_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = DirectSalesOrder_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = DirectSalesOrder_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = DirectSalesOrder_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = DirectSalesOrder_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = DirectSalesOrder_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = DirectSalesOrder_ItemFilterDTO.SupplierId;
            ItemFilter.Search = DirectSalesOrder_ItemFilterDTO.Search;

            ItemFilter = ItemService.ToFilter(ItemFilter);

            return await ItemService.Count(ItemFilter);
        }

        [Route(DirectSalesOrderRoute.ListItem), HttpPost]
        public async Task<List<DirectSalesOrder_ItemDTO>> ListItem([FromBody] DirectSalesOrder_ItemFilterDTO DirectSalesOrder_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = DirectSalesOrder_ItemFilterDTO.Skip;
            ItemFilter.Take = DirectSalesOrder_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = DirectSalesOrder_ItemFilterDTO.Id;
            ItemFilter.Code = DirectSalesOrder_ItemFilterDTO.Code;
            ItemFilter.Name = DirectSalesOrder_ItemFilterDTO.Name;
            ItemFilter.OtherName = DirectSalesOrder_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = DirectSalesOrder_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = DirectSalesOrder_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = DirectSalesOrder_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = DirectSalesOrder_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = DirectSalesOrder_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = DirectSalesOrder_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = DirectSalesOrder_ItemFilterDTO.SupplierId;
            ItemFilter.Search = DirectSalesOrder_ItemFilterDTO.Search;
            ItemFilter = ItemService.ToFilter(ItemFilter);

            if (DirectSalesOrder_ItemFilterDTO.SalesEmployeeId == null && DirectSalesOrder_ItemFilterDTO.SalesEmployeeId.HasValue == false)
            {
                return new List<DirectSalesOrder_ItemDTO>();
            }
            else if (DirectSalesOrder_ItemFilterDTO.StoreId == null && DirectSalesOrder_ItemFilterDTO.StoreId.HasValue == false)
            {
                return new List<DirectSalesOrder_ItemDTO>();
            }
            else
            {
                List<Item> Items = await DirectSalesOrderService.ListItem(ItemFilter, DirectSalesOrder_ItemFilterDTO.SalesEmployeeId.Equal, DirectSalesOrder_ItemFilterDTO.StoreId.Equal);
                List<DirectSalesOrder_ItemDTO> DirectSalesOrder_ItemDTOs = Items
                    .Select(x => new DirectSalesOrder_ItemDTO(x)).ToList();
                return DirectSalesOrder_ItemDTOs;
            }
        }
    }
}

