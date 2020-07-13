using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MEditedPriceStatus;
using DMS.Services.MIndirectSalesOrder;
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

namespace DMS.Rpc.indirect_sales_order
{
    public partial class IndirectSalesOrderController : RpcController
    {
        private IEditedPriceStatusService EditedPriceStatusService;
        private IStoreService StoreService;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
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
            IOrganizationService OrganizationService,
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
            this.OrganizationService = OrganizationService;
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
            IndirectSalesOrderFilter = await IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            int count = await IndirectSalesOrderService.Count(IndirectSalesOrderFilter);
            return count;
        }

        [Route(IndirectSalesOrderRoute.List), HttpPost]
        public async Task<ActionResult<List<IndirectSalesOrder_IndirectSalesOrderDTO>>> List([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO);
            IndirectSalesOrderFilter = await IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
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
            IndirectSalesOrderFilter = await IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
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
                        Product = x.Item.Product == null ? null : new Product
                        {
                            Id = x.Item.Product.Id,
                            Code = x.Item.Product.Code,
                            SupplierCode = x.Item.Product.SupplierCode,
                            Name = x.Item.Product.Name,
                            Description = x.Item.Product.Description,
                            ScanCode = x.Item.Product.ScanCode,
                            ProductTypeId = x.Item.Product.ProductTypeId,
                            SupplierId = x.Item.Product.SupplierId,
                            BrandId = x.Item.Product.BrandId,
                            UnitOfMeasureId = x.Item.Product.UnitOfMeasureId,
                            UnitOfMeasureGroupingId = x.Item.Product.UnitOfMeasureGroupingId,
                            RetailPrice = x.Item.Product.RetailPrice,
                            TaxTypeId = x.Item.Product.TaxTypeId,
                            StatusId = x.Item.Product.StatusId,
                            ProductType = x.Item.Product.ProductType == null ? null : new ProductType
                            {
                                Id = x.Item.Product.ProductType.Id,
                                Code = x.Item.Product.ProductType.Code,
                                Name = x.Item.Product.ProductType.Name,
                                Description = x.Item.Product.ProductType.Description,
                                StatusId = x.Item.Product.ProductType.StatusId,
                            },
                            TaxType = x.Item.Product.TaxType == null ? null : new TaxType
                            {
                                Id = x.Item.Product.TaxType.Id,
                                Code = x.Item.Product.TaxType.Code,
                                StatusId = x.Item.Product.TaxType.StatusId,
                                Name = x.Item.Product.TaxType.Name,
                                Percentage = x.Item.Product.TaxType.Percentage,
                            },
                            UnitOfMeasure = x.Item.Product.UnitOfMeasure == null ? null : new UnitOfMeasure
                            {
                                Id = x.Item.Product.UnitOfMeasure.Id,
                                Code = x.Item.Product.UnitOfMeasure.Code,
                                Name = x.Item.Product.UnitOfMeasure.Name,
                                Description = x.Item.Product.UnitOfMeasure.Description,
                                StatusId = x.Item.Product.UnitOfMeasure.StatusId,
                            },
                            UnitOfMeasureGrouping = x.Item.Product.UnitOfMeasureGrouping == null ? null : new UnitOfMeasureGrouping
                            {
                                Id = x.Item.Product.UnitOfMeasureGrouping.Id,
                                Code = x.Item.Product.UnitOfMeasureGrouping.Code,
                                Name = x.Item.Product.UnitOfMeasureGrouping.Name,
                                Description = x.Item.Product.UnitOfMeasureGrouping.Description,
                                UnitOfMeasureId = x.Item.Product.UnitOfMeasureGrouping.UnitOfMeasureId,
                            },
                        }
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
                        SaleStock = x.Item.SaleStock,
                        Product = x.Item.Product == null ? null : new Product
                        {
                            Id = x.Item.Product.Id,
                            Code = x.Item.Product.Code,
                            SupplierCode = x.Item.Product.SupplierCode,
                            Name = x.Item.Product.Name,
                            Description = x.Item.Product.Description,
                            ScanCode = x.Item.Product.ScanCode,
                            ProductTypeId = x.Item.Product.ProductTypeId,
                            SupplierId = x.Item.Product.SupplierId,
                            BrandId = x.Item.Product.BrandId,
                            UnitOfMeasureId = x.Item.Product.UnitOfMeasureId,
                            UnitOfMeasureGroupingId = x.Item.Product.UnitOfMeasureGroupingId,
                            RetailPrice = x.Item.Product.RetailPrice,
                            TaxTypeId = x.Item.Product.TaxTypeId,
                            StatusId = x.Item.Product.StatusId,
                            ProductType = x.Item.Product.ProductType == null ? null : new ProductType
                            {
                                Id = x.Item.Product.ProductType.Id,
                                Code = x.Item.Product.ProductType.Code,
                                Name = x.Item.Product.ProductType.Name,
                                Description = x.Item.Product.ProductType.Description,
                                StatusId = x.Item.Product.ProductType.StatusId,
                            },
                            TaxType = x.Item.Product.TaxType == null ? null : new TaxType
                            {
                                Id = x.Item.Product.TaxType.Id,
                                Code = x.Item.Product.TaxType.Code,
                                StatusId = x.Item.Product.TaxType.StatusId,
                                Name = x.Item.Product.TaxType.Name,
                                Percentage = x.Item.Product.TaxType.Percentage,
                            },
                            UnitOfMeasure = x.Item.Product.UnitOfMeasure == null ? null : new UnitOfMeasure
                            {
                                Id = x.Item.Product.UnitOfMeasure.Id,
                                Code = x.Item.Product.UnitOfMeasure.Code,
                                Name = x.Item.Product.UnitOfMeasure.Name,
                                Description = x.Item.Product.UnitOfMeasure.Description,
                                StatusId = x.Item.Product.UnitOfMeasure.StatusId,
                            },
                            UnitOfMeasureGrouping = x.Item.Product.UnitOfMeasureGrouping == null ? null : new UnitOfMeasureGrouping
                            {
                                Id = x.Item.Product.UnitOfMeasureGrouping.Id,
                                Code = x.Item.Product.UnitOfMeasureGrouping.Code,
                                Name = x.Item.Product.UnitOfMeasureGrouping.Name,
                                Description = x.Item.Product.UnitOfMeasureGrouping.Description,
                                UnitOfMeasureId = x.Item.Product.UnitOfMeasureGrouping.UnitOfMeasureId,
                            },
                        }
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
            IndirectSalesOrderFilter.OrganizationId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrganizationId;
            IndirectSalesOrderFilter.Code = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.BuyerStoreId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.PhoneNumber = IndirectSalesOrder_IndirectSalesOrderFilterDTO.PhoneNumber;
            IndirectSalesOrderFilter.StoreAddress = IndirectSalesOrder_IndirectSalesOrderFilterDTO.StoreAddress;
            IndirectSalesOrderFilter.DeliveryAddress = IndirectSalesOrder_IndirectSalesOrderFilterDTO.DeliveryAddress;
            IndirectSalesOrderFilter.SellerStoreId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.SellerStoreId;
            IndirectSalesOrderFilter.AppUserId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.AppUserId;
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
    }
}

