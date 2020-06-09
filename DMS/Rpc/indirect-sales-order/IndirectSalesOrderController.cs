using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MEditedPriceStatus;
using DMS.Services.MIndirectSalesOrder;
using DMS.Services.MItem;
using DMS.Services.MProduct;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;
using DMS.Services.MRequestState;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MSupplier;
using DMS.Services.MUnitOfMeasure;
using DMS.Services.MUnitOfMeasureGrouping;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.indirect_sales_order
{
    public class IndirectSalesOrderController : RpcController
    {
        private IEditedPriceStatusService EditedPriceStatusService;
        private IStoreService StoreService;
        private IAppUserService AppUserService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private IUnitOfMeasureGroupingService UnitOfMeasureGroupingService;
        private IItemService ItemService;
        private IIndirectSalesOrderService IndirectSalesOrderService;
        private IProductGroupingService ProductGroupingService;
        private IProductTypeService ProductTypeService;
        private IProductService ProductService;
        private IRequestStateService RequestStateService;
        private ISupplierService SupplierService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private ICurrentContext CurrentContext;
        public IndirectSalesOrderController(
            IEditedPriceStatusService EditedPriceStatusService,
            IStoreService StoreService,
            IAppUserService AppUserService,
            IUnitOfMeasureService UnitOfMeasureService,
            IUnitOfMeasureGroupingService UnitOfMeasureGroupingService,
            IItemService ItemService,
            IIndirectSalesOrderService IndirectSalesOrderService,
            IProductGroupingService ProductGroupingService,
            IProductTypeService ProductTypeService,
            IProductService ProductService,
            IRequestStateService RequestStateService,
            ISupplierService SupplierService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            ICurrentContext CurrentContext
        )
        {
            this.EditedPriceStatusService = EditedPriceStatusService;
            this.StoreService = StoreService;
            this.AppUserService = AppUserService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.UnitOfMeasureGroupingService = UnitOfMeasureGroupingService;
            this.ItemService = ItemService;
            this.IndirectSalesOrderService = IndirectSalesOrderService;
            this.ProductGroupingService = ProductGroupingService;
            this.ProductTypeService = ProductTypeService;
            this.ProductService = ProductService;
            this.RequestStateService = RequestStateService;
            this.SupplierService = SupplierService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.CurrentContext = CurrentContext;
        }

        [Route(IndirectSalesOrderRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO);
            IndirectSalesOrderFilter = IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            int count = await IndirectSalesOrderService.Count(IndirectSalesOrderFilter);
            return count;
        }

        [Route(IndirectSalesOrderRoute.List), HttpPost]
        public async Task<ActionResult<List<IndirectSalesOrder_IndirectSalesOrderDTO>>> List([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO);
            IndirectSalesOrderFilter = IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);
            List<IndirectSalesOrder_IndirectSalesOrderDTO> IndirectSalesOrder_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(c => new IndirectSalesOrder_IndirectSalesOrderDTO(c)).ToList();
            return IndirectSalesOrder_IndirectSalesOrderDTOs;
        }

        [Route(IndirectSalesOrderRoute.Get), HttpPost]
        public async Task<ActionResult<IndirectSalesOrder_IndirectSalesOrderDTO>> Get([FromBody]IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrder_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = await IndirectSalesOrderService.Get(IndirectSalesOrder_IndirectSalesOrderDTO.Id);
            return new IndirectSalesOrder_IndirectSalesOrderDTO(IndirectSalesOrder);
        }

        [Route(IndirectSalesOrderRoute.Create), HttpPost]
        public async Task<ActionResult<IndirectSalesOrder_IndirectSalesOrderDTO>> Create([FromBody] IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrder_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = ConvertDTOToEntity(IndirectSalesOrder_IndirectSalesOrderDTO);
            IndirectSalesOrder = await IndirectSalesOrderService.Create(IndirectSalesOrder);
            IndirectSalesOrder_IndirectSalesOrderDTO = new IndirectSalesOrder_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return IndirectSalesOrder_IndirectSalesOrderDTO;
            else
                return BadRequest(IndirectSalesOrder_IndirectSalesOrderDTO);
        }

        [Route(IndirectSalesOrderRoute.Update), HttpPost]
        public async Task<ActionResult<IndirectSalesOrder_IndirectSalesOrderDTO>> Update([FromBody] IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrder_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = ConvertDTOToEntity(IndirectSalesOrder_IndirectSalesOrderDTO);
            IndirectSalesOrder = await IndirectSalesOrderService.Update(IndirectSalesOrder);
            IndirectSalesOrder_IndirectSalesOrderDTO = new IndirectSalesOrder_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return IndirectSalesOrder_IndirectSalesOrderDTO;
            else
                return BadRequest(IndirectSalesOrder_IndirectSalesOrderDTO);
        }

        [Route(IndirectSalesOrderRoute.Approve), HttpPost]
        public async Task<ActionResult<IndirectSalesOrder_IndirectSalesOrderDTO>> Approve([FromBody] IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrder_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = ConvertDTOToEntity(IndirectSalesOrder_IndirectSalesOrderDTO);
            IndirectSalesOrder = await IndirectSalesOrderService.Approve(IndirectSalesOrder);
            IndirectSalesOrder_IndirectSalesOrderDTO = new IndirectSalesOrder_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return IndirectSalesOrder_IndirectSalesOrderDTO;
            else
                return BadRequest(IndirectSalesOrder_IndirectSalesOrderDTO);
        }

        [Route(IndirectSalesOrderRoute.Reject), HttpPost]
        public async Task<ActionResult<IndirectSalesOrder_IndirectSalesOrderDTO>> Reject([FromBody] IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrder_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = ConvertDTOToEntity(IndirectSalesOrder_IndirectSalesOrderDTO);
            IndirectSalesOrder = await IndirectSalesOrderService.Reject(IndirectSalesOrder);
            IndirectSalesOrder_IndirectSalesOrderDTO = new IndirectSalesOrder_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return IndirectSalesOrder_IndirectSalesOrderDTO;
            else
                return BadRequest(IndirectSalesOrder_IndirectSalesOrderDTO);
        }

        [Route(IndirectSalesOrderRoute.Delete), HttpPost]
        public async Task<ActionResult<IndirectSalesOrder_IndirectSalesOrderDTO>> Delete([FromBody] IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrder_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = ConvertDTOToEntity(IndirectSalesOrder_IndirectSalesOrderDTO);
            IndirectSalesOrder = await IndirectSalesOrderService.Delete(IndirectSalesOrder);
            IndirectSalesOrder_IndirectSalesOrderDTO = new IndirectSalesOrder_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return IndirectSalesOrder_IndirectSalesOrderDTO;
            else
                return BadRequest(IndirectSalesOrder_IndirectSalesOrderDTO);
        }

        private async Task<bool> HasPermission(long Id)
        {
            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter = IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            if (Id == 0)
            {

            }
            else
            {
                IndirectSalesOrderFilter.Id = new IdFilter { Equal = Id };
                int count = await IndirectSalesOrderService.Count(IndirectSalesOrderFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private IndirectSalesOrder ConvertDTOToEntity(IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            IndirectSalesOrder IndirectSalesOrder = new IndirectSalesOrder();
            IndirectSalesOrder.Id = IndirectSalesOrder_IndirectSalesOrderDTO.Id;
            IndirectSalesOrder.Code = IndirectSalesOrder_IndirectSalesOrderDTO.Code;
            IndirectSalesOrder.BuyerStoreId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStoreId;
            IndirectSalesOrder.PhoneNumber = IndirectSalesOrder_IndirectSalesOrderDTO.PhoneNumber;
            IndirectSalesOrder.StoreAddress = IndirectSalesOrder_IndirectSalesOrderDTO.StoreAddress;
            IndirectSalesOrder.DeliveryAddress = IndirectSalesOrder_IndirectSalesOrderDTO.DeliveryAddress;
            IndirectSalesOrder.SellerStoreId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStoreId;
            IndirectSalesOrder.SaleEmployeeId = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployeeId;
            IndirectSalesOrder.OrderDate = IndirectSalesOrder_IndirectSalesOrderDTO.OrderDate;
            IndirectSalesOrder.DeliveryDate = IndirectSalesOrder_IndirectSalesOrderDTO.DeliveryDate;
            IndirectSalesOrder.RequestStateId = IndirectSalesOrder_IndirectSalesOrderDTO.RequestStateId;
            IndirectSalesOrder.EditedPriceStatusId = IndirectSalesOrder_IndirectSalesOrderDTO.EditedPriceStatusId;
            IndirectSalesOrder.Note = IndirectSalesOrder_IndirectSalesOrderDTO.Note;
            IndirectSalesOrder.SubTotal = IndirectSalesOrder_IndirectSalesOrderDTO.SubTotal;
            IndirectSalesOrder.GeneralDiscountPercentage = IndirectSalesOrder_IndirectSalesOrderDTO.GeneralDiscountPercentage;
            IndirectSalesOrder.GeneralDiscountAmount = IndirectSalesOrder_IndirectSalesOrderDTO.GeneralDiscountAmount;
            IndirectSalesOrder.TotalTaxAmount = IndirectSalesOrder_IndirectSalesOrderDTO.TotalTaxAmount;
            IndirectSalesOrder.Total = IndirectSalesOrder_IndirectSalesOrderDTO.Total;
            IndirectSalesOrder.BuyerStore = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore == null ? null : new Store
            {
                Id = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.Id,
                Code = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.Code,
                Name = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.Name,
                ParentStoreId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.ParentStoreId,
                OrganizationId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.OrganizationId,
                StoreTypeId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.StoreTypeId,
                StoreGroupingId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.StoreGroupingId,
                ResellerId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.ResellerId,
                Telephone = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.Telephone,
                ProvinceId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.ProvinceId,
                DistrictId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.DistrictId,
                WardId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.WardId,
                Address = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.Address,
                DeliveryAddress = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.DeliveryAddress,
                Latitude = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.Latitude,
                Longitude = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.Longitude,
                DeliveryLatitude = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.DeliveryLatitude,
                DeliveryLongitude = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.DeliveryLongitude,
                OwnerName = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.OwnerName,
                OwnerPhone = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.OwnerPhone,
                OwnerEmail = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.OwnerEmail,
                TaxCode = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.TaxCode,
                LegalEntity = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.LegalEntity,
                StatusId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.StatusId,
            };
            IndirectSalesOrder.EditedPriceStatus = IndirectSalesOrder_IndirectSalesOrderDTO.EditedPriceStatus == null ? null : new EditedPriceStatus
            {
                Id = IndirectSalesOrder_IndirectSalesOrderDTO.EditedPriceStatus.Id,
                Code = IndirectSalesOrder_IndirectSalesOrderDTO.EditedPriceStatus.Code,
                Name = IndirectSalesOrder_IndirectSalesOrderDTO.EditedPriceStatus.Name,
            };
            IndirectSalesOrder.SaleEmployee = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee == null ? null : new AppUser
            {
                Id = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Id,
                Username = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Username,
                DisplayName = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.DisplayName,
                Address = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Address,
                Email = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Email,
                Phone = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Phone,
                PositionId = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.PositionId,
                Department = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Department,
                OrganizationId = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.OrganizationId,
                SexId = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.SexId,
                StatusId = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.StatusId,
                Avatar = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Avatar,
                Birthday = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Birthday,
                ProvinceId = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.ProvinceId,
            };
            IndirectSalesOrder.SellerStore = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore == null ? null : new Store
            {
                Id = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.Id,
                Code = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.Code,
                Name = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.Name,
                ParentStoreId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.ParentStoreId,
                OrganizationId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.OrganizationId,
                StoreTypeId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.StoreTypeId,
                StoreGroupingId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.StoreGroupingId,
                ResellerId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.ResellerId,
                Telephone = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.Telephone,
                ProvinceId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.ProvinceId,
                DistrictId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.DistrictId,
                WardId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.WardId,
                Address = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.Address,
                DeliveryAddress = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.DeliveryAddress,
                Latitude = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.Latitude,
                Longitude = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.Longitude,
                DeliveryLatitude = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.DeliveryLatitude,
                DeliveryLongitude = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.DeliveryLongitude,
                OwnerName = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.OwnerName,
                OwnerPhone = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.OwnerPhone,
                OwnerEmail = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.OwnerEmail,
                TaxCode = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.TaxCode,
                LegalEntity = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.LegalEntity,
                StatusId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.StatusId,
            };
            IndirectSalesOrder.IndirectSalesOrderContents = IndirectSalesOrder_IndirectSalesOrderDTO.IndirectSalesOrderContents?
                .Select(x => new IndirectSalesOrderContent
                {
                    Id = x.Id,
                    ItemId = x.ItemId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Quantity = x.Quantity,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    RequestedQuantity = x.RequestedQuantity,
                    PrimaryPrice = x.PrimaryPrice,
                    SalePrice = x.SalePrice,
                    DiscountPercentage = x.DiscountPercentage,
                    DiscountAmount = x.DiscountAmount,
                    GeneralDiscountPercentage = x.GeneralDiscountPercentage,
                    GeneralDiscountAmount = x.GeneralDiscountAmount,
                    Amount = x.Amount,
                    TaxPercentage = x.TaxPercentage,
                    TaxAmount = x.TaxAmount,
                    Factor = x.Factor,
                    Item = x.Item == null ? null : new Item
                    {
                        Id = x.Item.Id,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ProductId = x.Item.ProductId,
                        RetailPrice = x.Item.RetailPrice,
                        SalePrice = x.Item.SalePrice,
                        SaleStock = x.Item.SaleStock,
                        ScanCode = x.Item.ScanCode,
                        StatusId = x.Item.StatusId,
                    },
                    PrimaryUnitOfMeasure = x.PrimaryUnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.PrimaryUnitOfMeasure.Id,
                        Code = x.PrimaryUnitOfMeasure.Code,
                        Name = x.PrimaryUnitOfMeasure.Name,
                        Description = x.PrimaryUnitOfMeasure.Description,
                        StatusId = x.PrimaryUnitOfMeasure.StatusId,
                    },
                    UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                    },
                }).ToList();
            IndirectSalesOrder.IndirectSalesOrderPromotions = IndirectSalesOrder_IndirectSalesOrderDTO.IndirectSalesOrderPromotions?
                .Select(x => new IndirectSalesOrderPromotion
                {
                    Id = x.Id,
                    ItemId = x.ItemId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Quantity = x.Quantity,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    RequestedQuantity = x.RequestedQuantity,
                    Note = x.Note,
                    Factor = x.Factor,
                    Item = x.Item == null ? null : new Item
                    {
                        Id = x.Item.Id,
                        ProductId = x.Item.ProductId,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ScanCode = x.Item.ScanCode,
                        SalePrice = x.Item.SalePrice,
                        RetailPrice = x.Item.RetailPrice,
                        StatusId = x.Item.StatusId,
                    },
                    PrimaryUnitOfMeasure = x.PrimaryUnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.PrimaryUnitOfMeasure.Id,
                        Code = x.PrimaryUnitOfMeasure.Code,
                        Name = x.PrimaryUnitOfMeasure.Name,
                        Description = x.PrimaryUnitOfMeasure.Description,
                        StatusId = x.PrimaryUnitOfMeasure.StatusId,
                    },
                    UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                    },
                }).ToList();
            IndirectSalesOrder.BaseLanguage = CurrentContext.Language;
            return IndirectSalesOrder;
        }

        private IndirectSalesOrderFilter ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
            IndirectSalesOrderFilter.Skip = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Skip;
            IndirectSalesOrderFilter.Take = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Take;
            IndirectSalesOrderFilter.OrderBy = IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrderBy;
            IndirectSalesOrderFilter.OrderType = IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrderType;

            IndirectSalesOrderFilter.Id = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Id;
            IndirectSalesOrderFilter.Code = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.BuyerStoreId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.PhoneNumber = IndirectSalesOrder_IndirectSalesOrderFilterDTO.PhoneNumber;
            IndirectSalesOrderFilter.StoreAddress = IndirectSalesOrder_IndirectSalesOrderFilterDTO.StoreAddress;
            IndirectSalesOrderFilter.DeliveryAddress = IndirectSalesOrder_IndirectSalesOrderFilterDTO.DeliveryAddress;
            IndirectSalesOrderFilter.SellerStoreId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.SellerStoreId;
            IndirectSalesOrderFilter.SaleEmployeeId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.SaleEmployeeId;
            IndirectSalesOrderFilter.OrderDate = IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrderDate;
            IndirectSalesOrderFilter.DeliveryDate = IndirectSalesOrder_IndirectSalesOrderFilterDTO.DeliveryDate;
            IndirectSalesOrderFilter.RequestStateId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.RequestStateId;
            IndirectSalesOrderFilter.EditedPriceStatusId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.EditedPriceStatusId;
            IndirectSalesOrderFilter.Note = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Note;
            IndirectSalesOrderFilter.SubTotal = IndirectSalesOrder_IndirectSalesOrderFilterDTO.SubTotal;
            IndirectSalesOrderFilter.GeneralDiscountPercentage = IndirectSalesOrder_IndirectSalesOrderFilterDTO.GeneralDiscountPercentage;
            IndirectSalesOrderFilter.GeneralDiscountAmount = IndirectSalesOrder_IndirectSalesOrderFilterDTO.GeneralDiscountAmount;
            IndirectSalesOrderFilter.TotalTaxAmount = IndirectSalesOrder_IndirectSalesOrderFilterDTO.TotalTaxAmount;
            IndirectSalesOrderFilter.Total = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Total;
            return IndirectSalesOrderFilter;
        }

        [Route(IndirectSalesOrderRoute.FilterListStore), HttpPost]
        public async Task<List<IndirectSalesOrder_StoreDTO>> FilterListStore([FromBody] IndirectSalesOrder_StoreFilterDTO IndirectSalesOrder_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = IndirectSalesOrder_StoreFilterDTO.Id;
            StoreFilter.Code = IndirectSalesOrder_StoreFilterDTO.Code;
            StoreFilter.Name = IndirectSalesOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = IndirectSalesOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = IndirectSalesOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = IndirectSalesOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = IndirectSalesOrder_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = IndirectSalesOrder_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = IndirectSalesOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = IndirectSalesOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = IndirectSalesOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = IndirectSalesOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = IndirectSalesOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = IndirectSalesOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = IndirectSalesOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = IndirectSalesOrder_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = IndirectSalesOrder_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = IndirectSalesOrder_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = IndirectSalesOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = IndirectSalesOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = IndirectSalesOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = IndirectSalesOrder_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<IndirectSalesOrder_StoreDTO> IndirectSalesOrder_StoreDTOs = Stores
                .Select(x => new IndirectSalesOrder_StoreDTO(x)).ToList();
            return IndirectSalesOrder_StoreDTOs;
        }

        [Route(IndirectSalesOrderRoute.FilterListAppUser), HttpPost]
        public async Task<List<IndirectSalesOrder_AppUserDTO>> FilterListAppUser([FromBody] IndirectSalesOrder_AppUserFilterDTO IndirectSalesOrder_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = IndirectSalesOrder_AppUserFilterDTO.Id;
            AppUserFilter.Username = IndirectSalesOrder_AppUserFilterDTO.Username;
            AppUserFilter.Password = IndirectSalesOrder_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = IndirectSalesOrder_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = IndirectSalesOrder_AppUserFilterDTO.Address;
            AppUserFilter.Email = IndirectSalesOrder_AppUserFilterDTO.Email;
            AppUserFilter.Phone = IndirectSalesOrder_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = IndirectSalesOrder_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = IndirectSalesOrder_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = IndirectSalesOrder_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = IndirectSalesOrder_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = IndirectSalesOrder_AppUserFilterDTO.StatusId;
            AppUserFilter.Birthday = IndirectSalesOrder_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = IndirectSalesOrder_AppUserFilterDTO.ProvinceId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<IndirectSalesOrder_AppUserDTO> IndirectSalesOrder_AppUserDTOs = AppUsers
                .Select(x => new IndirectSalesOrder_AppUserDTO(x)).ToList();
            return IndirectSalesOrder_AppUserDTOs;
        }

        [Route(IndirectSalesOrderRoute.FilterListUnitOfMeasure), HttpPost]
        public async Task<List<IndirectSalesOrder_UnitOfMeasureDTO>> FilterListUnitOfMeasure([FromBody] IndirectSalesOrder_UnitOfMeasureFilterDTO IndirectSalesOrder_UnitOfMeasureFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = IndirectSalesOrder_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = IndirectSalesOrder_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = IndirectSalesOrder_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = IndirectSalesOrder_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = IndirectSalesOrder_UnitOfMeasureFilterDTO.StatusId;

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<IndirectSalesOrder_UnitOfMeasureDTO> IndirectSalesOrder_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new IndirectSalesOrder_UnitOfMeasureDTO(x)).ToList();
            return IndirectSalesOrder_UnitOfMeasureDTOs;
        }

        [Route(IndirectSalesOrderRoute.FilterListItem), HttpPost]
        public async Task<List<IndirectSalesOrder_ItemDTO>> FilterListItem([FromBody] IndirectSalesOrder_ItemFilterDTO IndirectSalesOrder_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = IndirectSalesOrder_ItemFilterDTO.Id;
            ItemFilter.ProductId = IndirectSalesOrder_ItemFilterDTO.ProductId;
            ItemFilter.Code = IndirectSalesOrder_ItemFilterDTO.Code;
            ItemFilter.Name = IndirectSalesOrder_ItemFilterDTO.Name;
            ItemFilter.ScanCode = IndirectSalesOrder_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = IndirectSalesOrder_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = IndirectSalesOrder_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = IndirectSalesOrder_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<IndirectSalesOrder_ItemDTO> IndirectSalesOrder_ItemDTOs = Items
                .Select(x => new IndirectSalesOrder_ItemDTO(x)).ToList();
            return IndirectSalesOrder_ItemDTOs;
        }
        [Route(IndirectSalesOrderRoute.FilterListEditedPriceStatus), HttpPost]
        public async Task<List<IndirectSalesOrder_EditedPriceStatusDTO>> FilterListEditedPriceStatus([FromBody] IndirectSalesOrder_EditedPriceStatusFilterDTO IndirectSalesOrder_EditedPriceStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            EditedPriceStatusFilter EditedPriceStatusFilter = new EditedPriceStatusFilter();
            EditedPriceStatusFilter.Skip = 0;
            EditedPriceStatusFilter.Take = 20;
            EditedPriceStatusFilter.OrderBy = EditedPriceStatusOrder.Id;
            EditedPriceStatusFilter.OrderType = OrderType.ASC;
            EditedPriceStatusFilter.Selects = EditedPriceStatusSelect.ALL;
            EditedPriceStatusFilter.Id = IndirectSalesOrder_EditedPriceStatusFilterDTO.Id;
            EditedPriceStatusFilter.Code = IndirectSalesOrder_EditedPriceStatusFilterDTO.Code;
            EditedPriceStatusFilter.Name = IndirectSalesOrder_EditedPriceStatusFilterDTO.Name;

            List<EditedPriceStatus> EditedPriceStatuses = await EditedPriceStatusService.List(EditedPriceStatusFilter);
            List<IndirectSalesOrder_EditedPriceStatusDTO> IndirectSalesOrder_EditedPriceStatusDTOs = EditedPriceStatuses
                .Select(x => new IndirectSalesOrder_EditedPriceStatusDTO(x)).ToList();
            return IndirectSalesOrder_EditedPriceStatusDTOs;
        }

        [Route(IndirectSalesOrderRoute.SingleListStore), HttpPost]
        public async Task<List<IndirectSalesOrder_StoreDTO>> SingleListStore([FromBody] IndirectSalesOrder_StoreFilterDTO IndirectSalesOrder_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = IndirectSalesOrder_StoreFilterDTO.Id;
            StoreFilter.Code = IndirectSalesOrder_StoreFilterDTO.Code;
            StoreFilter.Name = IndirectSalesOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = IndirectSalesOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = IndirectSalesOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = IndirectSalesOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = IndirectSalesOrder_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = IndirectSalesOrder_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = IndirectSalesOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = IndirectSalesOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = IndirectSalesOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = IndirectSalesOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = IndirectSalesOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = IndirectSalesOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = IndirectSalesOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = IndirectSalesOrder_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = IndirectSalesOrder_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = IndirectSalesOrder_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = IndirectSalesOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = IndirectSalesOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = IndirectSalesOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<IndirectSalesOrder_StoreDTO> IndirectSalesOrder_StoreDTOs = Stores
                .Select(x => new IndirectSalesOrder_StoreDTO(x)).ToList();
            return IndirectSalesOrder_StoreDTOs;
        }
        [Route(IndirectSalesOrderRoute.SingleListAppUser), HttpPost]
        public async Task<List<IndirectSalesOrder_AppUserDTO>> SingleListAppUser([FromBody] IndirectSalesOrder_AppUserFilterDTO IndirectSalesOrder_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = IndirectSalesOrder_AppUserFilterDTO.Id;
            AppUserFilter.Username = IndirectSalesOrder_AppUserFilterDTO.Username;
            AppUserFilter.Password = IndirectSalesOrder_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = IndirectSalesOrder_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = IndirectSalesOrder_AppUserFilterDTO.Address;
            AppUserFilter.Email = IndirectSalesOrder_AppUserFilterDTO.Email;
            AppUserFilter.Phone = IndirectSalesOrder_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = IndirectSalesOrder_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = IndirectSalesOrder_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = IndirectSalesOrder_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = IndirectSalesOrder_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            AppUserFilter.Birthday = IndirectSalesOrder_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = IndirectSalesOrder_AppUserFilterDTO.ProvinceId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<IndirectSalesOrder_AppUserDTO> IndirectSalesOrder_AppUserDTOs = AppUsers
                .Select(x => new IndirectSalesOrder_AppUserDTO(x)).ToList();
            return IndirectSalesOrder_AppUserDTOs;
        }

        [Route(IndirectSalesOrderRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<IndirectSalesOrder_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] IndirectSalesOrder_UnitOfMeasureFilterDTO IndirectSalesOrder_UnitOfMeasureFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var Product = (await ProductService.List(new ProductFilter
            {
                Id = IndirectSalesOrder_UnitOfMeasureFilterDTO.ProductId,
                Selects = ProductSelect.UnitOfMeasure | ProductSelect.UnitOfMeasureGrouping
            })).FirstOrDefault();

            List<IndirectSalesOrder_UnitOfMeasureDTO> IndirectSalesOrder_UnitOfMeasureDTOs = new List<IndirectSalesOrder_UnitOfMeasureDTO>();
            if (Product.UnitOfMeasureGrouping != null && Product.UnitOfMeasureGrouping.StatusId == Enums.StatusEnum.ACTIVE.Id)
            {
                IndirectSalesOrder_UnitOfMeasureDTOs = Product.UnitOfMeasureGrouping.UnitOfMeasureGroupingContents.Select(x => new IndirectSalesOrder_UnitOfMeasureDTO(x)).ToList();
            }
            if (Product.UnitOfMeasure.StatusId == Enums.StatusEnum.ACTIVE.Id)
            {
                IndirectSalesOrder_UnitOfMeasureDTOs.Add(new IndirectSalesOrder_UnitOfMeasureDTO
                {
                    Id = Product.UnitOfMeasure.Id,
                    Code = Product.UnitOfMeasure.Code,
                    Name = Product.UnitOfMeasure.Name,
                    Description = Product.UnitOfMeasure.Description,
                    StatusId = Product.UnitOfMeasure.StatusId,
                    Factor = 1
                });
            }

            return IndirectSalesOrder_UnitOfMeasureDTOs;
        }

        [Route(IndirectSalesOrderRoute.SingleListSupplier), HttpPost]
        public async Task<List<IndirectSalesOrder_SupplierDTO>> SingleListSupplier([FromBody] IndirectSalesOrder_SupplierFilterDTO Product_SupplierFilterDTO)
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
            List<IndirectSalesOrder_SupplierDTO> IndirectSalesOrder_SupplierDTOs = Suppliers
                .Select(x => new IndirectSalesOrder_SupplierDTO(x)).ToList();
            return IndirectSalesOrder_SupplierDTOs;
        }
        [Route(IndirectSalesOrderRoute.SingleListStoreGrouping), HttpPost]
        public async Task<List<IndirectSalesOrder_StoreGroupingDTO>> SingleListStoreGrouping([FromBody] IndirectSalesOrder_StoreGroupingFilterDTO IndirectSalesOrder_StoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Code = IndirectSalesOrder_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = IndirectSalesOrder_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.Level = IndirectSalesOrder_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.Path = IndirectSalesOrder_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<IndirectSalesOrder_StoreGroupingDTO> IndirectSalesOrder_StoreGroupingDTOs = StoreGroupings
                .Select(x => new IndirectSalesOrder_StoreGroupingDTO(x)).ToList();
            return IndirectSalesOrder_StoreGroupingDTOs;
        }
        [Route(IndirectSalesOrderRoute.SingleListStoreType), HttpPost]
        public async Task<List<IndirectSalesOrder_StoreTypeDTO>> SingleListStoreType([FromBody] IndirectSalesOrder_StoreTypeFilterDTO IndirectSalesOrder_StoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = IndirectSalesOrder_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = IndirectSalesOrder_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = IndirectSalesOrder_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<IndirectSalesOrder_StoreTypeDTO> IndirectSalesOrder_StoreTypeDTOs = StoreTypes
                .Select(x => new IndirectSalesOrder_StoreTypeDTO(x)).ToList();
            return IndirectSalesOrder_StoreTypeDTOs;
        }
        [Route(IndirectSalesOrderRoute.SingleListItem), HttpPost]
        public async Task<List<IndirectSalesOrder_ItemDTO>> SingleListItem([FromBody] IndirectSalesOrder_ItemFilterDTO IndirectSalesOrder_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = IndirectSalesOrder_ItemFilterDTO.Id;
            ItemFilter.ProductId = IndirectSalesOrder_ItemFilterDTO.ProductId;
            ItemFilter.Code = IndirectSalesOrder_ItemFilterDTO.Code;
            ItemFilter.Name = IndirectSalesOrder_ItemFilterDTO.Name;
            ItemFilter.ScanCode = IndirectSalesOrder_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = IndirectSalesOrder_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = IndirectSalesOrder_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Item> Items = await ItemService.List(ItemFilter);
            List<IndirectSalesOrder_ItemDTO> IndirectSalesOrder_ItemDTOs = Items
                .Select(x => new IndirectSalesOrder_ItemDTO(x)).ToList();
            return IndirectSalesOrder_ItemDTOs;
        }

        [Route(IndirectSalesOrderRoute.SingleListProductGrouping), HttpPost]
        public async Task<List<IndirectSalesOrder_ProductGroupingDTO>> SingleListProductGrouping([FromBody] IndirectSalesOrder_ProductGroupingFilterDTO IndirectSalesOrder_ProductGroupingFilterDTO)
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

            List<ProductGrouping> IndirectSalesOrderGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<IndirectSalesOrder_ProductGroupingDTO> IndirectSalesOrder_ProductGroupingDTOs = IndirectSalesOrderGroupings
                .Select(x => new IndirectSalesOrder_ProductGroupingDTO(x)).ToList();
            return IndirectSalesOrder_ProductGroupingDTOs;
        }
        [Route(IndirectSalesOrderRoute.SingleListProductType), HttpPost]
        public async Task<List<IndirectSalesOrder_ProductTypeDTO>> SingleListProductType([FromBody] IndirectSalesOrder_ProductTypeFilterDTO IndirectSalesOrder_ProductTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Skip = 0;
            ProductTypeFilter.Take = 20;
            ProductTypeFilter.OrderBy = ProductTypeOrder.Id;
            ProductTypeFilter.OrderType = OrderType.ASC;
            ProductTypeFilter.Selects = ProductTypeSelect.ALL;
            ProductTypeFilter.Id = IndirectSalesOrder_ProductTypeFilterDTO.Id;
            ProductTypeFilter.Code = IndirectSalesOrder_ProductTypeFilterDTO.Code;
            ProductTypeFilter.Name = IndirectSalesOrder_ProductTypeFilterDTO.Name;
            ProductTypeFilter.Description = IndirectSalesOrder_ProductTypeFilterDTO.Description;
            ProductTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            List<IndirectSalesOrder_ProductTypeDTO> IndirectSalesOrder_ProductTypeDTOs = ProductTypes
                .Select(x => new IndirectSalesOrder_ProductTypeDTO(x)).ToList();
            return IndirectSalesOrder_ProductTypeDTOs;
        }

        [Route(IndirectSalesOrderRoute.SingleListEditedPriceStatus), HttpPost]
        public async Task<List<IndirectSalesOrder_EditedPriceStatusDTO>> SingleListEditedPriceStatus([FromBody] IndirectSalesOrder_EditedPriceStatusFilterDTO IndirectSalesOrder_EditedPriceStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            EditedPriceStatusFilter EditedPriceStatusFilter = new EditedPriceStatusFilter();
            EditedPriceStatusFilter.Skip = 0;
            EditedPriceStatusFilter.Take = 20;
            EditedPriceStatusFilter.OrderBy = EditedPriceStatusOrder.Id;
            EditedPriceStatusFilter.OrderType = OrderType.ASC;
            EditedPriceStatusFilter.Selects = EditedPriceStatusSelect.ALL;
            EditedPriceStatusFilter.Id = IndirectSalesOrder_EditedPriceStatusFilterDTO.Id;
            EditedPriceStatusFilter.Code = IndirectSalesOrder_EditedPriceStatusFilterDTO.Code;
            EditedPriceStatusFilter.Name = IndirectSalesOrder_EditedPriceStatusFilterDTO.Name;

            List<EditedPriceStatus> EditedPriceStatuses = await EditedPriceStatusService.List(EditedPriceStatusFilter);
            List<IndirectSalesOrder_EditedPriceStatusDTO> IndirectSalesOrder_EditedPriceStatusDTOs = EditedPriceStatuses
                .Select(x => new IndirectSalesOrder_EditedPriceStatusDTO(x)).ToList();
            return IndirectSalesOrder_EditedPriceStatusDTOs;
        }
        [Route(IndirectSalesOrderRoute.SingleListRequestState), HttpPost]
        public async Task<List<IndirectSalesOrder_RequestStateDTO>> SingleListRequestState([FromBody] IndirectSalesOrder_RequestStateFilterDTO IndirectSalesOrder_RequestStateFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RequestStateFilter RequestStateFilter = new RequestStateFilter();
            RequestStateFilter.Skip = 0;
            RequestStateFilter.Take = 20;
            RequestStateFilter.OrderBy = RequestStateOrder.Id;
            RequestStateFilter.OrderType = OrderType.ASC;
            RequestStateFilter.Selects = RequestStateSelect.ALL;
            RequestStateFilter.Id = IndirectSalesOrder_RequestStateFilterDTO.Id;
            RequestStateFilter.Code = IndirectSalesOrder_RequestStateFilterDTO.Code;
            RequestStateFilter.Name = IndirectSalesOrder_RequestStateFilterDTO.Name;

            List<RequestState> RequestStatees = await RequestStateService.List(RequestStateFilter);
            List<IndirectSalesOrder_RequestStateDTO> IndirectSalesOrder_RequestStateDTOs = RequestStatees
                .Select(x => new IndirectSalesOrder_RequestStateDTO(x)).ToList();
            return IndirectSalesOrder_RequestStateDTOs;
        }
        [Route(IndirectSalesOrderRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] IndirectSalesOrder_StoreFilterDTO IndirectSalesOrder_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Id = IndirectSalesOrder_StoreFilterDTO.Id;
            StoreFilter.Code = IndirectSalesOrder_StoreFilterDTO.Code;
            StoreFilter.Name = IndirectSalesOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = IndirectSalesOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = IndirectSalesOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = IndirectSalesOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.ResellerId = IndirectSalesOrder_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = IndirectSalesOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = IndirectSalesOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = IndirectSalesOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = IndirectSalesOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = IndirectSalesOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = IndirectSalesOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = IndirectSalesOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = IndirectSalesOrder_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = IndirectSalesOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = IndirectSalesOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = IndirectSalesOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            return await StoreService.Count(StoreFilter);
        }

        [Route(IndirectSalesOrderRoute.ListStore), HttpPost]
        public async Task<List<IndirectSalesOrder_StoreDTO>> ListStore([FromBody] IndirectSalesOrder_StoreFilterDTO IndirectSalesOrder_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = IndirectSalesOrder_StoreFilterDTO.Skip;
            StoreFilter.Take = IndirectSalesOrder_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = IndirectSalesOrder_StoreFilterDTO.Id;
            StoreFilter.Code = IndirectSalesOrder_StoreFilterDTO.Code;
            StoreFilter.Name = IndirectSalesOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = IndirectSalesOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = IndirectSalesOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = IndirectSalesOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.ResellerId = IndirectSalesOrder_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = IndirectSalesOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = IndirectSalesOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = IndirectSalesOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = IndirectSalesOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = IndirectSalesOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = IndirectSalesOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = IndirectSalesOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = IndirectSalesOrder_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = IndirectSalesOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = IndirectSalesOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = IndirectSalesOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<IndirectSalesOrder_StoreDTO> IndirectSalesOrder_StoreDTOs = Stores
                .Select(x => new IndirectSalesOrder_StoreDTO(x)).ToList();
            return IndirectSalesOrder_StoreDTOs;
        }

        [Route(IndirectSalesOrderRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] IndirectSalesOrder_ItemFilterDTO IndirectSalesOrder_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Id = IndirectSalesOrder_ItemFilterDTO.Id;
            ItemFilter.Code = IndirectSalesOrder_ItemFilterDTO.Code;
            ItemFilter.Name = IndirectSalesOrder_ItemFilterDTO.Name;
            ItemFilter.OtherName = IndirectSalesOrder_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = IndirectSalesOrder_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = IndirectSalesOrder_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = IndirectSalesOrder_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = IndirectSalesOrder_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = IndirectSalesOrder_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = IndirectSalesOrder_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = IndirectSalesOrder_ItemFilterDTO.SupplierId;
            return await ItemService.Count(ItemFilter);
        }

        [Route(IndirectSalesOrderRoute.ListItem), HttpPost]
        public async Task<List<IndirectSalesOrder_ItemDTO>> ListItem([FromBody] IndirectSalesOrder_ItemFilterDTO IndirectSalesOrder_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = IndirectSalesOrder_ItemFilterDTO.Skip;
            ItemFilter.Take = IndirectSalesOrder_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = IndirectSalesOrder_ItemFilterDTO.Id;
            ItemFilter.Code = IndirectSalesOrder_ItemFilterDTO.Code;
            ItemFilter.Name = IndirectSalesOrder_ItemFilterDTO.Name;
            ItemFilter.OtherName = IndirectSalesOrder_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = IndirectSalesOrder_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = IndirectSalesOrder_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = IndirectSalesOrder_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = IndirectSalesOrder_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = IndirectSalesOrder_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = IndirectSalesOrder_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = IndirectSalesOrder_ItemFilterDTO.SupplierId;

            if (IndirectSalesOrder_ItemFilterDTO.StoreId != null && IndirectSalesOrder_ItemFilterDTO.StoreId.Equal.HasValue)
            {
                List<Item> Items = await IndirectSalesOrderService.ListItem(ItemFilter, IndirectSalesOrder_ItemFilterDTO.StoreId.Equal.Value);
                List<IndirectSalesOrder_ItemDTO> IndirectSalesOrder_ItemDTOs = Items
                    .Select(x => new IndirectSalesOrder_ItemDTO(x)).ToList();
                return IndirectSalesOrder_ItemDTOs;
            }
            else
            {
                List<Item> Items = await ItemService.List(ItemFilter);
                List<IndirectSalesOrder_ItemDTO> IndirectSalesOrder_ItemDTOs = Items
                    .Select(x => new IndirectSalesOrder_ItemDTO(x)).ToList();
                return IndirectSalesOrder_ItemDTOs;
            }
        }
    }
}

