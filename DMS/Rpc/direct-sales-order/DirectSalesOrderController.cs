using Common;
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
using DMS.Services.MTaxType;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;
using DMS.Helpers;
using Helpers;
using System.IO;
using GleamTech.DocumentUltimate;
using System.Net.Mime;

namespace DMS.Rpc.direct_sales_order
{
    public partial class DirectSalesOrderController : RpcController
    {
        private IEditedPriceStatusService EditedPriceStatusService;
        private IStoreService StoreService;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private IUnitOfMeasureGroupingService UnitOfMeasureGroupingService;
        private IItemService ItemService;
        private IDirectSalesOrderService DirectSalesOrderService;
        private IProductGroupingService ProductGroupingService;
        private IProductTypeService ProductTypeService;
        private IProductService ProductService;
        private IRequestStateService RequestStateService;
        private ISupplierService SupplierService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private ITaxTypeService TaxTypeService;
        private ICurrentContext CurrentContext;
        public DirectSalesOrderController(
            IOrganizationService OrganizationService,
            IEditedPriceStatusService EditedPriceStatusService,
            IStoreService StoreService,
            IAppUserService AppUserService,
            IUnitOfMeasureService UnitOfMeasureService,
            IUnitOfMeasureGroupingService UnitOfMeasureGroupingService,
            IItemService ItemService,
            IDirectSalesOrderService DirectSalesOrderService,
            IProductGroupingService ProductGroupingService,
            IProductTypeService ProductTypeService,
            IProductService ProductService,
            IRequestStateService RequestStateService,
            ISupplierService SupplierService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            ITaxTypeService TaxTypeService,
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
            this.DirectSalesOrderService = DirectSalesOrderService;
            this.ProductGroupingService = ProductGroupingService;
            this.ProductTypeService = ProductTypeService;
            this.ProductService = ProductService;
            this.RequestStateService = RequestStateService;
            this.SupplierService = SupplierService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.TaxTypeService = TaxTypeService;
            this.CurrentContext = CurrentContext;
        }

        [Route(DirectSalesOrderRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] DirectSalesOrder_DirectSalesOrderFilterDTO DirectSalesOrder_DirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderFilter DirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(DirectSalesOrder_DirectSalesOrderFilterDTO);
            DirectSalesOrderFilter = await DirectSalesOrderService.ToFilter(DirectSalesOrderFilter);
            int count = await DirectSalesOrderService.Count(DirectSalesOrderFilter);
            return count;
        }

        [Route(DirectSalesOrderRoute.List), HttpPost]
        public async Task<ActionResult<List<DirectSalesOrder_DirectSalesOrderDTO>>> List([FromBody] DirectSalesOrder_DirectSalesOrderFilterDTO DirectSalesOrder_DirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderFilter DirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(DirectSalesOrder_DirectSalesOrderFilterDTO);
            DirectSalesOrderFilter = await DirectSalesOrderService.ToFilter(DirectSalesOrderFilter);
            List<DirectSalesOrder> DirectSalesOrders = await DirectSalesOrderService.List(DirectSalesOrderFilter);
            List<DirectSalesOrder_DirectSalesOrderDTO> DirectSalesOrder_DirectSalesOrderDTOs = DirectSalesOrders
                .Select(c => new DirectSalesOrder_DirectSalesOrderDTO(c)).ToList();
            return DirectSalesOrder_DirectSalesOrderDTOs;
        }

        [Route(DirectSalesOrderRoute.Get), HttpPost]
        public async Task<ActionResult<DirectSalesOrder_DirectSalesOrderDTO>> Get([FromBody]DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(DirectSalesOrder_DirectSalesOrderDTO.Id))
                return Forbid();

            DirectSalesOrder DirectSalesOrder = await DirectSalesOrderService.Get(DirectSalesOrder_DirectSalesOrderDTO.Id);
            List<TaxType> TaxTypes = await TaxTypeService.List(new TaxTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = TaxTypeSelect.ALL
            });
            DirectSalesOrder_DirectSalesOrderDTO = new DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder);
            foreach (var DirectSalesOrderContent in DirectSalesOrder_DirectSalesOrderDTO.DirectSalesOrderContents)
            {
                TaxType TaxType = TaxTypes.Where(x => x.Percentage == DirectSalesOrderContent.TaxPercentage).FirstOrDefault();
                DirectSalesOrderContent.TaxType = new DirectSalesOrder_TaxTypeDTO(TaxType);
            }
            return DirectSalesOrder_DirectSalesOrderDTO;
        }

        [Route(DirectSalesOrderRoute.Create), HttpPost]
        public async Task<ActionResult<DirectSalesOrder_DirectSalesOrderDTO>> Create([FromBody] DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(DirectSalesOrder_DirectSalesOrderDTO.Id))
                return Forbid();

            DirectSalesOrder DirectSalesOrder = ConvertDTOToEntity(DirectSalesOrder_DirectSalesOrderDTO);
            DirectSalesOrder = await DirectSalesOrderService.Create(DirectSalesOrder);
            DirectSalesOrder_DirectSalesOrderDTO = new DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder);
            if (DirectSalesOrder.IsValidated)
                return DirectSalesOrder_DirectSalesOrderDTO;
            else
                return BadRequest(DirectSalesOrder_DirectSalesOrderDTO);
        }

        [Route(DirectSalesOrderRoute.Update), HttpPost]
        public async Task<ActionResult<DirectSalesOrder_DirectSalesOrderDTO>> Update([FromBody] DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(DirectSalesOrder_DirectSalesOrderDTO.Id))
                return Forbid();

            DirectSalesOrder DirectSalesOrder = ConvertDTOToEntity(DirectSalesOrder_DirectSalesOrderDTO);
            DirectSalesOrder = await DirectSalesOrderService.Update(DirectSalesOrder);
            DirectSalesOrder_DirectSalesOrderDTO = new DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder);
            if (DirectSalesOrder.IsValidated)
                return DirectSalesOrder_DirectSalesOrderDTO;
            else
                return BadRequest(DirectSalesOrder_DirectSalesOrderDTO);
        }

        [Route(DirectSalesOrderRoute.Approve), HttpPost]
        public async Task<ActionResult<DirectSalesOrder_DirectSalesOrderDTO>> Approve([FromBody] DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(DirectSalesOrder_DirectSalesOrderDTO.Id))
                return Forbid();

            DirectSalesOrder DirectSalesOrder = ConvertDTOToEntity(DirectSalesOrder_DirectSalesOrderDTO);
            DirectSalesOrder = await DirectSalesOrderService.Approve(DirectSalesOrder);
            DirectSalesOrder_DirectSalesOrderDTO = new DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder);
            if (DirectSalesOrder.IsValidated)
                return DirectSalesOrder_DirectSalesOrderDTO;
            else
                return BadRequest(DirectSalesOrder_DirectSalesOrderDTO);
        }

        [Route(DirectSalesOrderRoute.Reject), HttpPost]
        public async Task<ActionResult<DirectSalesOrder_DirectSalesOrderDTO>> Reject([FromBody] DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(DirectSalesOrder_DirectSalesOrderDTO.Id))
                return Forbid();

            DirectSalesOrder DirectSalesOrder = ConvertDTOToEntity(DirectSalesOrder_DirectSalesOrderDTO);
            DirectSalesOrder = await DirectSalesOrderService.Reject(DirectSalesOrder);
            DirectSalesOrder_DirectSalesOrderDTO = new DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder);
            if (DirectSalesOrder.IsValidated)
                return DirectSalesOrder_DirectSalesOrderDTO;
            else
                return BadRequest(DirectSalesOrder_DirectSalesOrderDTO);
        }

        [Route(DirectSalesOrderRoute.Delete), HttpPost]
        public async Task<ActionResult<DirectSalesOrder_DirectSalesOrderDTO>> Delete([FromBody] DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(DirectSalesOrder_DirectSalesOrderDTO.Id))
                return Forbid();

            DirectSalesOrder DirectSalesOrder = ConvertDTOToEntity(DirectSalesOrder_DirectSalesOrderDTO);
            DirectSalesOrder = await DirectSalesOrderService.Delete(DirectSalesOrder);
            DirectSalesOrder_DirectSalesOrderDTO = new DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder);
            if (DirectSalesOrder.IsValidated)
                return DirectSalesOrder_DirectSalesOrderDTO;
            else
                return BadRequest(DirectSalesOrder_DirectSalesOrderDTO);
        }

        [Route(DirectSalesOrderRoute.Print), HttpGet]
        public async Task<ActionResult> Print([FromQuery] long Id)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            var DirectSalesOrder = await DirectSalesOrderService.Get(Id);
            if (DirectSalesOrder == null)
                return Content("Đơn hàng không tồn tại");
            DirectSalesOrder_PrintDTO DirectSalesOrder_PrintDTO = new DirectSalesOrder_PrintDTO(DirectSalesOrder);
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-EN");
            var STT = 1;
            foreach (var DirectSalesOrderContent in DirectSalesOrder_PrintDTO.Contents)
            {
                DirectSalesOrderContent.STT = STT++;
                DirectSalesOrderContent.AmountString = DirectSalesOrderContent.Amount.ToString("N0", culture);
                DirectSalesOrderContent.PrimaryPriceString = DirectSalesOrderContent.PrimaryPrice.ToString("N0", culture);
                DirectSalesOrderContent.QuantityString = DirectSalesOrderContent.Quantity.ToString("N0", culture);
                DirectSalesOrderContent.RequestedQuantityString = DirectSalesOrderContent.RequestedQuantity.ToString("N0", culture);
                DirectSalesOrderContent.SalePriceString = DirectSalesOrderContent.SalePrice.ToString("N0", culture);
                DirectSalesOrderContent.DiscountString = DirectSalesOrderContent.DiscountPercentage.HasValue ? DirectSalesOrderContent.DiscountPercentage.Value.ToString("N0", culture) + "%" : "";
                DirectSalesOrderContent.TaxPercentageString = DirectSalesOrderContent.TaxPercentage.HasValue ? DirectSalesOrderContent.TaxPercentage.Value.ToString("N0", culture) + "%" : "";
            }
            foreach (var DirectSalesOrderPromotion in DirectSalesOrder_PrintDTO.Promotions)
            {
                DirectSalesOrderPromotion.STT = STT++;
                DirectSalesOrderPromotion.QuantityString = DirectSalesOrderPromotion.Quantity.ToString("N0", culture);
                DirectSalesOrderPromotion.RequestedQuantityString = DirectSalesOrderPromotion.RequestedQuantity.ToString("N0", culture);
            }

            DirectSalesOrder_PrintDTO.SubTotalString = DirectSalesOrder_PrintDTO.SubTotal.ToString("N0", culture);
            DirectSalesOrder_PrintDTO.Discount = DirectSalesOrder_PrintDTO.GeneralDiscountAmount.HasValue ? DirectSalesOrder_PrintDTO.GeneralDiscountAmount.Value.ToString("N0", culture) : "";
            DirectSalesOrder_PrintDTO.Tax = DirectSalesOrder_PrintDTO.TotalTaxAmount.ToString("N0", culture);
            DirectSalesOrder_PrintDTO.TotalString = DirectSalesOrder_PrintDTO.Total.ToString("N0", culture);
            DirectSalesOrder_PrintDTO.TotalText = Utils.ConvertAmountTostring((long)DirectSalesOrder_PrintDTO.Total);

            string path = "Templates/Print_Direct.docx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            dynamic Data = new ExpandoObject();
            Data.Order = DirectSalesOrder_PrintDTO;
            MemoryStream MemoryStream = new MemoryStream();
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "docx"))
            {
                document.Process(Data);
            };
            ContentDisposition cd = new ContentDisposition
            {
                FileName = $"Don-hang-truc-tiep{DirectSalesOrder.Code}.docx",
                Inline = false,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return File(output.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document;charset=utf-8");
        }

        private async Task<bool> HasPermission(long Id)
        {
            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter();
            DirectSalesOrderFilter = await DirectSalesOrderService.ToFilter(DirectSalesOrderFilter);
            if (Id == 0)
            {

            }
            else
            {
                DirectSalesOrderFilter.Id = new IdFilter { Equal = Id };
                int count = await DirectSalesOrderService.Count(DirectSalesOrderFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private DirectSalesOrder ConvertDTOToEntity(DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            DirectSalesOrder DirectSalesOrder = new DirectSalesOrder();
            DirectSalesOrder.Id = DirectSalesOrder_DirectSalesOrderDTO.Id;
            DirectSalesOrder.Code = DirectSalesOrder_DirectSalesOrderDTO.Code;
            DirectSalesOrder.BuyerStoreId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStoreId;
            DirectSalesOrder.PhoneNumber = DirectSalesOrder_DirectSalesOrderDTO.PhoneNumber;
            DirectSalesOrder.StoreAddress = DirectSalesOrder_DirectSalesOrderDTO.StoreAddress;
            DirectSalesOrder.DeliveryAddress = DirectSalesOrder_DirectSalesOrderDTO.DeliveryAddress;
            DirectSalesOrder.SaleEmployeeId = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployeeId;
            DirectSalesOrder.OrganizationId = DirectSalesOrder_DirectSalesOrderDTO.OrganizationId;
            DirectSalesOrder.OrderDate = DirectSalesOrder_DirectSalesOrderDTO.OrderDate;
            DirectSalesOrder.DeliveryDate = DirectSalesOrder_DirectSalesOrderDTO.DeliveryDate;
            DirectSalesOrder.RequestStateId = DirectSalesOrder_DirectSalesOrderDTO.RequestStateId;
            DirectSalesOrder.EditedPriceStatusId = DirectSalesOrder_DirectSalesOrderDTO.EditedPriceStatusId;
            DirectSalesOrder.Note = DirectSalesOrder_DirectSalesOrderDTO.Note;
            DirectSalesOrder.SubTotal = DirectSalesOrder_DirectSalesOrderDTO.SubTotal;
            DirectSalesOrder.GeneralDiscountPercentage = DirectSalesOrder_DirectSalesOrderDTO.GeneralDiscountPercentage;
            DirectSalesOrder.GeneralDiscountAmount = DirectSalesOrder_DirectSalesOrderDTO.GeneralDiscountAmount;
            DirectSalesOrder.TotalTaxAmount = DirectSalesOrder_DirectSalesOrderDTO.TotalTaxAmount;
            DirectSalesOrder.Total = DirectSalesOrder_DirectSalesOrderDTO.Total;
            DirectSalesOrder.BuyerStore = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore == null ? null : new Store
            {
                Id = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Id,
                Code = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Code,
                Name = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Name,
                ParentStoreId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.ParentStoreId,
                OrganizationId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.OrganizationId,
                StoreTypeId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.StoreTypeId,
                StoreGroupingId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.StoreGroupingId,
                ResellerId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.ResellerId,
                Telephone = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Telephone,
                ProvinceId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.ProvinceId,
                DistrictId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.DistrictId,
                WardId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.WardId,
                Address = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Address,
                DeliveryAddress = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.DeliveryAddress,
                Latitude = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Latitude,
                Longitude = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Longitude,
                DeliveryLatitude = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.DeliveryLatitude,
                DeliveryLongitude = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.DeliveryLongitude,
                OwnerName = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.OwnerName,
                OwnerPhone = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.OwnerPhone,
                OwnerEmail = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.OwnerEmail,
                TaxCode = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.TaxCode,
                LegalEntity = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.LegalEntity,
                StatusId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.StatusId,
            };
            DirectSalesOrder.Organization = DirectSalesOrder_DirectSalesOrderDTO.Organization == null ? null : new Organization
            {
                Id = DirectSalesOrder_DirectSalesOrderDTO.Organization.Id,
                Code = DirectSalesOrder_DirectSalesOrderDTO.Organization.Code,
                Name = DirectSalesOrder_DirectSalesOrderDTO.Organization.Name,
                ParentId = DirectSalesOrder_DirectSalesOrderDTO.Organization.ParentId,
                Path = DirectSalesOrder_DirectSalesOrderDTO.Organization.Path,
                Level = DirectSalesOrder_DirectSalesOrderDTO.Organization.Level,
                StatusId = DirectSalesOrder_DirectSalesOrderDTO.Organization.StatusId,
                Phone = DirectSalesOrder_DirectSalesOrderDTO.Organization.Phone,
                Address = DirectSalesOrder_DirectSalesOrderDTO.Organization.Address,
                Email = DirectSalesOrder_DirectSalesOrderDTO.Organization.Email,
            };
            DirectSalesOrder.EditedPriceStatus = DirectSalesOrder_DirectSalesOrderDTO.EditedPriceStatus == null ? null : new EditedPriceStatus
            {
                Id = DirectSalesOrder_DirectSalesOrderDTO.EditedPriceStatus.Id,
                Code = DirectSalesOrder_DirectSalesOrderDTO.EditedPriceStatus.Code,
                Name = DirectSalesOrder_DirectSalesOrderDTO.EditedPriceStatus.Name,
            };
            DirectSalesOrder.SaleEmployee = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee == null ? null : new AppUser
            {
                Id = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Id,
                Username = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Username,
                DisplayName = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.DisplayName,
                Address = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Address,
                Email = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Email,
                Phone = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Phone,
                PositionId = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.PositionId,
                Department = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Department,
                OrganizationId = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.OrganizationId,
                SexId = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.SexId,
                StatusId = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.StatusId,
                Avatar = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Avatar,
                Birthday = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Birthday,
                ProvinceId = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.ProvinceId,
            };
            DirectSalesOrder.DirectSalesOrderContents = DirectSalesOrder_DirectSalesOrderDTO.DirectSalesOrderContents?
                .Select(x => new DirectSalesOrderContent
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
            DirectSalesOrder.DirectSalesOrderPromotions = DirectSalesOrder_DirectSalesOrderDTO.DirectSalesOrderPromotions?
                .Select(x => new DirectSalesOrderPromotion
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
            DirectSalesOrder.BaseLanguage = CurrentContext.Language;
            return DirectSalesOrder;
        }

        private DirectSalesOrderFilter ConvertFilterDTOToFilterEntity(DirectSalesOrder_DirectSalesOrderFilterDTO DirectSalesOrder_DirectSalesOrderFilterDTO)
        {
            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter();
            DirectSalesOrderFilter.Selects = DirectSalesOrderSelect.ALL;
            DirectSalesOrderFilter.Skip = DirectSalesOrder_DirectSalesOrderFilterDTO.Skip;
            DirectSalesOrderFilter.Take = DirectSalesOrder_DirectSalesOrderFilterDTO.Take;
            DirectSalesOrderFilter.OrderBy = DirectSalesOrder_DirectSalesOrderFilterDTO.OrderBy;
            DirectSalesOrderFilter.OrderType = DirectSalesOrder_DirectSalesOrderFilterDTO.OrderType;

            DirectSalesOrderFilter.Id = DirectSalesOrder_DirectSalesOrderFilterDTO.Id;
            DirectSalesOrderFilter.OrganizationId = DirectSalesOrder_DirectSalesOrderFilterDTO.OrganizationId;
            DirectSalesOrderFilter.Code = DirectSalesOrder_DirectSalesOrderFilterDTO.Code;
            DirectSalesOrderFilter.BuyerStoreId = DirectSalesOrder_DirectSalesOrderFilterDTO.BuyerStoreId;
            DirectSalesOrderFilter.PhoneNumber = DirectSalesOrder_DirectSalesOrderFilterDTO.PhoneNumber;
            DirectSalesOrderFilter.StoreAddress = DirectSalesOrder_DirectSalesOrderFilterDTO.StoreAddress;
            DirectSalesOrderFilter.DeliveryAddress = DirectSalesOrder_DirectSalesOrderFilterDTO.DeliveryAddress;
            DirectSalesOrderFilter.AppUserId = DirectSalesOrder_DirectSalesOrderFilterDTO.AppUserId;
            DirectSalesOrderFilter.OrderDate = DirectSalesOrder_DirectSalesOrderFilterDTO.OrderDate;
            DirectSalesOrderFilter.DeliveryDate = DirectSalesOrder_DirectSalesOrderFilterDTO.DeliveryDate;
            DirectSalesOrderFilter.RequestStateId = DirectSalesOrder_DirectSalesOrderFilterDTO.RequestStateId;
            DirectSalesOrderFilter.EditedPriceStatusId = DirectSalesOrder_DirectSalesOrderFilterDTO.EditedPriceStatusId;
            DirectSalesOrderFilter.Note = DirectSalesOrder_DirectSalesOrderFilterDTO.Note;
            DirectSalesOrderFilter.SubTotal = DirectSalesOrder_DirectSalesOrderFilterDTO.SubTotal;
            DirectSalesOrderFilter.GeneralDiscountPercentage = DirectSalesOrder_DirectSalesOrderFilterDTO.GeneralDiscountPercentage;
            DirectSalesOrderFilter.GeneralDiscountAmount = DirectSalesOrder_DirectSalesOrderFilterDTO.GeneralDiscountAmount;
            DirectSalesOrderFilter.TotalTaxAmount = DirectSalesOrder_DirectSalesOrderFilterDTO.TotalTaxAmount;
            DirectSalesOrderFilter.Total = DirectSalesOrder_DirectSalesOrderFilterDTO.Total;
            return DirectSalesOrderFilter;
        }
    }
}

