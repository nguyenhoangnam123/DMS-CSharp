using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Helpers;
using DMS.ABE.Services.MAppUser;
using DMS.ABE.Services.MDirectSalesOrder;
using DMS.ABE.Services.MOrganization;
using DMS.ABE.Services.MStore;
using DMS.ABE.Services.MStoreUser;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Rpc.direct_sales_order
{
    public partial class DirectSalesOrderController : SimpleController
    {
        private IStoreService StoreService;
        private IStoreUserService StoreUserService;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IDirectSalesOrderService DirectSalesOrderService;
        private ICurrentContext CurrentContext;
        public DirectSalesOrderController(
            IStoreService StoreService,
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            IDirectSalesOrderService DirectSalesOrderService,
            IStoreUserService StoreUserService,
            ICurrentContext CurrentContext
        )
        {
            this.StoreService = StoreService;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.DirectSalesOrderService = DirectSalesOrderService;
            this.StoreUserService = StoreUserService;
            this.CurrentContext = CurrentContext;
        }

        [Route(DirectSalesOrderRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] DirectSalesOrder_DirectSalesOrderFilterDTO DirectSalesOrder_DirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderFilter DirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(DirectSalesOrder_DirectSalesOrderFilterDTO);
            int count = await DirectSalesOrderService.Count(DirectSalesOrderFilter);
            return count;
        }

        [Route(DirectSalesOrderRoute.List), HttpPost]
        public async Task<ActionResult<List<DirectSalesOrder_DirectSalesOrderDTO>>> List([FromBody] DirectSalesOrder_DirectSalesOrderFilterDTO DirectSalesOrder_DirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderFilter DirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(DirectSalesOrder_DirectSalesOrderFilterDTO);
            List<DirectSalesOrder> DirectSalesOrders = await DirectSalesOrderService.List(DirectSalesOrderFilter);
            List<DirectSalesOrder_DirectSalesOrderDTO> DirectSalesOrder_DirectSalesOrderDTOs = DirectSalesOrders
                .Select(c => new DirectSalesOrder_DirectSalesOrderDTO(c)).ToList();
            return DirectSalesOrder_DirectSalesOrderDTOs;
        }

        [Route(DirectSalesOrderRoute.Get), HttpPost]
        public async Task<ActionResult<DirectSalesOrder_DirectSalesOrderDTO>> Get([FromBody] DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrder DirectSalesOrder = await DirectSalesOrderService.Get(DirectSalesOrder_DirectSalesOrderDTO.Id);
            return new DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder);
        }

        [Route(DirectSalesOrderRoute.Create), HttpPost]
        public async Task<ActionResult<DirectSalesOrder_DirectSalesOrderDTO>> Create([FromBody] DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrder DirectSalesOrder = ConvertDTOToEntity(DirectSalesOrder_DirectSalesOrderDTO);
            DirectSalesOrder = await DirectSalesOrderService.Create(DirectSalesOrder);
            DirectSalesOrder_DirectSalesOrderDTO = new DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder);
            if (DirectSalesOrder.IsValidated)
                return Ok(DirectSalesOrder_DirectSalesOrderDTO);
            else
                return BadRequest(DirectSalesOrder_DirectSalesOrderDTO);
        }

        [Route(DirectSalesOrderRoute.Approve), HttpPost]
        public async Task<ActionResult<DirectSalesOrder_DirectSalesOrderDTO>> Approve([FromBody] DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrder DirectSalesOrder = ConvertDTOToEntity(DirectSalesOrder_DirectSalesOrderDTO);
            DirectSalesOrder = await DirectSalesOrderService.Approve(DirectSalesOrder);
            DirectSalesOrder_DirectSalesOrderDTO = new DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder);
            if (DirectSalesOrder.IsValidated)
                return Ok(DirectSalesOrder_DirectSalesOrderDTO);
            else
                return BadRequest(DirectSalesOrder_DirectSalesOrderDTO);
        }

        [Route(DirectSalesOrderRoute.Reject), HttpPost]
        public async Task<ActionResult<DirectSalesOrder_DirectSalesOrderDTO>> Reject([FromBody] DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrder DirectSalesOrder = ConvertDTOToEntity(DirectSalesOrder_DirectSalesOrderDTO);
            DirectSalesOrder = await DirectSalesOrderService.Approve(DirectSalesOrder);
            DirectSalesOrder_DirectSalesOrderDTO = new DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder);
            if (DirectSalesOrder.IsValidated)
                return Ok(DirectSalesOrder_DirectSalesOrderDTO);
            else
                return BadRequest(DirectSalesOrder_DirectSalesOrderDTO);
        }

        //[Route(DirectSalesOrderRoute.PrintDirectOrder), HttpGet]
        //public async Task<ActionResult> PrintDirectOrder([FromQuery] long Id)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);
        //    var DirectSalesOrder = await DirectSalesOrderService.Get(Id);
        //    if (DirectSalesOrder == null)
        //        return Content("Đơn hàng không tồn tại");
        //    DirectSalesOrder_PrintDirectOrderDTO DirectSalesOrder_PrintDTO = new DirectSalesOrder_PrintDirectOrderDTO(DirectSalesOrder);
        //    var culture = System.Globalization.CultureInfo.GetCultureInfo("en-EN");
        //    var STT = 1;
        //    if (DirectSalesOrder_PrintDTO.Contents != null)
        //    {
        //        foreach (var DirectSalesOrderContent in DirectSalesOrder_PrintDTO.Contents)
        //        {
        //            DirectSalesOrderContent.STT = STT++;
        //            DirectSalesOrderContent.AmountString = DirectSalesOrderContent.Amount.ToString("N0", culture);
        //            DirectSalesOrderContent.PrimaryPriceString = DirectSalesOrderContent.PrimaryPrice.ToString("N0", culture);
        //            DirectSalesOrderContent.QuantityString = DirectSalesOrderContent.Quantity.ToString("N0", culture);
        //            DirectSalesOrderContent.RequestedQuantityString = DirectSalesOrderContent.RequestedQuantity.ToString("N0", culture);
        //            DirectSalesOrderContent.SalePriceString = DirectSalesOrderContent.SalePrice.ToString("N0", culture);
        //            DirectSalesOrderContent.DiscountString = DirectSalesOrderContent.DiscountPercentage.HasValue ? DirectSalesOrderContent.DiscountPercentage.Value.ToString("N0", culture) + "%" : "";
        //            DirectSalesOrderContent.TaxPercentageString = DirectSalesOrderContent.TaxPercentage.HasValue ? DirectSalesOrderContent.TaxPercentage.Value.ToString("N0", culture) + "%" : "";
        //        }
        //    }
        //    if (DirectSalesOrder_PrintDTO.Promotions != null)
        //    {
        //        foreach (var DirectSalesOrderPromotion in DirectSalesOrder_PrintDTO.Promotions)
        //        {
        //            DirectSalesOrderPromotion.STT = STT++;
        //            DirectSalesOrderPromotion.QuantityString = DirectSalesOrderPromotion.Quantity.ToString("N0", culture);
        //            DirectSalesOrderPromotion.RequestedQuantityString = DirectSalesOrderPromotion.RequestedQuantity.ToString("N0", culture);
        //        }
        //    }

        //    DirectSalesOrder_PrintDTO.SubTotalString = DirectSalesOrder_PrintDTO.SubTotal.ToString("N0", culture);
        //    DirectSalesOrder_PrintDTO.Discount = DirectSalesOrder_PrintDTO.GeneralDiscountAmount.HasValue ? DirectSalesOrder_PrintDTO.GeneralDiscountAmount.Value.ToString("N0", culture) : "";
        //    DirectSalesOrder_PrintDTO.sOrderDate = DirectSalesOrder_PrintDTO.OrderDate.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
        //    DirectSalesOrder_PrintDTO.sDeliveryDate = DirectSalesOrder_PrintDTO.DeliveryDate.HasValue ? DirectSalesOrder_PrintDTO.DeliveryDate.Value.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy") : string.Empty;
        //    DirectSalesOrder_PrintDTO.TotalString = DirectSalesOrder_PrintDTO.Total.ToString("N0", culture);
        //    DirectSalesOrder_PrintDTO.TotalText = Utils.ConvertAmountTostring((long)DirectSalesOrder_PrintDTO.Total);

        //    ExportTemplate ExportTemplate = await ExportTemplateService.Get(ExportTemplateEnum.PRINT_DIRECT_MOBILE.Id);
        //    if (ExportTemplate == null)
        //        return BadRequest("Chưa có mẫu in đơn hàng");

        //    dynamic Data = new ExpandoObject();
        //    Data.Order = DirectSalesOrder_PrintDTO;
        //    MemoryStream MemoryStream = new MemoryStream();
        //    MemoryStream input = new MemoryStream(ExportTemplate.Content);
        //    MemoryStream output = new MemoryStream();
        //    using (var document = StaticParams.DocumentFactory.Open(input, output, "docx"))
        //    {
        //        document.Process(Data);
        //    };
        //    var documentConverter = new DocumentConverter(output, DocumentFormat.Docx);
        //    documentConverter.ConvertTo(MemoryStream, DocumentFormat.Pdf);

        //    ContentDisposition cd = new ContentDisposition
        //    {
        //        FileName = $"Don-hang-gian-tiep-{DirectSalesOrder.Code}.pdf",
        //        Inline = true,
        //    };
        //    Response.Headers.Add("Content-Disposition", cd.ToString());
        //    return File(MemoryStream.ToArray(), "application/pdf;charset=utf-8");
        //}

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
            DirectSalesOrder.CreatedAt = StaticParams.DateTimeNow;
            DirectSalesOrder.UpdatedAt = StaticParams.DateTimeNow;
            DirectSalesOrder.DeliveryDate = DirectSalesOrder_DirectSalesOrderDTO.DeliveryDate;
            DirectSalesOrder.RequestStateId = DirectSalesOrder_DirectSalesOrderDTO.RequestStateId;
            DirectSalesOrder.EditedPriceStatusId = DirectSalesOrder_DirectSalesOrderDTO.EditedPriceStatusId;
            DirectSalesOrder.Note = DirectSalesOrder_DirectSalesOrderDTO.Note;
            DirectSalesOrder.SubTotal = DirectSalesOrder_DirectSalesOrderDTO.SubTotal;
            DirectSalesOrder.GeneralDiscountPercentage = DirectSalesOrder_DirectSalesOrderDTO.GeneralDiscountPercentage;
            DirectSalesOrder.GeneralDiscountAmount = DirectSalesOrder_DirectSalesOrderDTO.GeneralDiscountAmount;
            DirectSalesOrder.TotalTaxAmount = DirectSalesOrder_DirectSalesOrderDTO.TotalTaxAmount;
            DirectSalesOrder.TotalAfterTax = DirectSalesOrder_DirectSalesOrderDTO.TotalAfterTax;
            DirectSalesOrder.PromotionCode = DirectSalesOrder_DirectSalesOrderDTO.PromotionCode;
            DirectSalesOrder.PromotionValue = DirectSalesOrder_DirectSalesOrderDTO.PromotionValue;
            DirectSalesOrder.Total = DirectSalesOrder_DirectSalesOrderDTO.Total;
            DirectSalesOrder.StoreUserCreatorId = DirectSalesOrder_DirectSalesOrderDTO.StoreUserCreatorId; // người tạo là storeUser ứng với đại lý
            DirectSalesOrder.StoreApprovalStateId = DirectSalesOrder_DirectSalesOrderDTO.StoreApprovalStateId; // người tạo là storeUser ứng với đại lý
            DirectSalesOrder.BuyerStore = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore == null ? null : new Store
            {
                Id = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Id,
                Code = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Code,
                Name = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Name,
                ParentStoreId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.ParentStoreId,
                OrganizationId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.OrganizationId,
                StoreTypeId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.StoreTypeId,
                StoreGroupingId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.StoreGroupingId,
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
            DirectSalesOrder.StoreApprovalState = DirectSalesOrder_DirectSalesOrderDTO.StoreApprovalState == null ? null : new StoreApprovalState
            {
                Id = DirectSalesOrder_DirectSalesOrderDTO.StoreApprovalState.Id,
                Code = DirectSalesOrder_DirectSalesOrderDTO.StoreApprovalState.Code,
                Name = DirectSalesOrder_DirectSalesOrderDTO.StoreApprovalState.Name,
            };
            DirectSalesOrder.SaleEmployee = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee == null ? null : new AppUser
            {
                Id = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Id,
                Username = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Username,
                DisplayName = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.DisplayName,
                Address = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Address,
                Email = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Email,
                Phone = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Phone,
                Department = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Department,
                OrganizationId = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.OrganizationId,
                SexId = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.SexId,
                StatusId = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.StatusId,
                Avatar = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Avatar,
                Birthday = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Birthday,
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
            DirectSalesOrderFilter.Code = DirectSalesOrder_DirectSalesOrderFilterDTO.Code;
            DirectSalesOrderFilter.OrganizationId = DirectSalesOrder_DirectSalesOrderFilterDTO.OrganizationId;
            DirectSalesOrderFilter.BuyerStoreId = DirectSalesOrder_DirectSalesOrderFilterDTO.BuyerStoreId;
            DirectSalesOrderFilter.PhoneNumber = DirectSalesOrder_DirectSalesOrderFilterDTO.PhoneNumber;
            DirectSalesOrderFilter.StoreAddress = DirectSalesOrder_DirectSalesOrderFilterDTO.StoreAddress;
            DirectSalesOrderFilter.DeliveryAddress = DirectSalesOrder_DirectSalesOrderFilterDTO.DeliveryAddress;
            DirectSalesOrderFilter.SaleEmployeeId = DirectSalesOrder_DirectSalesOrderFilterDTO.SaleEmployeeId;
            DirectSalesOrderFilter.OrderDate = DirectSalesOrder_DirectSalesOrderFilterDTO.OrderDate;
            DirectSalesOrderFilter.DeliveryDate = DirectSalesOrder_DirectSalesOrderFilterDTO.DeliveryDate;
            DirectSalesOrderFilter.ErpApprovalStateId = DirectSalesOrder_DirectSalesOrderFilterDTO.ErpApprovalStateId;
            DirectSalesOrderFilter.StoreApprovalStateId = DirectSalesOrder_DirectSalesOrderFilterDTO.StoreApprovalStateId;
            DirectSalesOrderFilter.RequestStateId = DirectSalesOrder_DirectSalesOrderFilterDTO.RequestStateId;
            DirectSalesOrderFilter.EditedPriceStatusId = DirectSalesOrder_DirectSalesOrderFilterDTO.EditedPriceStatusId;
            DirectSalesOrderFilter.Note = DirectSalesOrder_DirectSalesOrderFilterDTO.Note;
            DirectSalesOrderFilter.SubTotal = DirectSalesOrder_DirectSalesOrderFilterDTO.SubTotal;
            DirectSalesOrderFilter.GeneralDiscountPercentage = DirectSalesOrder_DirectSalesOrderFilterDTO.GeneralDiscountPercentage;
            DirectSalesOrderFilter.GeneralDiscountAmount = DirectSalesOrder_DirectSalesOrderFilterDTO.GeneralDiscountAmount;
            DirectSalesOrderFilter.TotalTaxAmount = DirectSalesOrder_DirectSalesOrderFilterDTO.TotalTaxAmount;
            DirectSalesOrderFilter.TotalAfterTax = DirectSalesOrder_DirectSalesOrderFilterDTO.TotalAfterTax;
            DirectSalesOrderFilter.PromotionCode = DirectSalesOrder_DirectSalesOrderFilterDTO.PromotionCode;
            DirectSalesOrderFilter.PromotionValue = DirectSalesOrder_DirectSalesOrderFilterDTO.PromotionValue;
            DirectSalesOrderFilter.Total = DirectSalesOrder_DirectSalesOrderFilterDTO.Total;
            DirectSalesOrderFilter.RowId = DirectSalesOrder_DirectSalesOrderFilterDTO.RowId;
            DirectSalesOrderFilter.StoreCheckingId = DirectSalesOrder_DirectSalesOrderFilterDTO.StoreCheckingId;
            DirectSalesOrderFilter.CreatorId = DirectSalesOrder_DirectSalesOrderFilterDTO.CreatorId;
            DirectSalesOrderFilter.DirectSalesOrderSourceTypeId = DirectSalesOrder_DirectSalesOrderFilterDTO.DirectSalesOrderSourceTypeId;
            DirectSalesOrderFilter.CreatedAt = DirectSalesOrder_DirectSalesOrderFilterDTO.CreatedAt;
            DirectSalesOrderFilter.UpdatedAt = DirectSalesOrder_DirectSalesOrderFilterDTO.UpdatedAt;
            return DirectSalesOrderFilter;
        }
    }
}

