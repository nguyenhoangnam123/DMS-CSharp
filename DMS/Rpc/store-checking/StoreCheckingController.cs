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
using DMS.Services.MStoreChecking;
using DMS.Services.MAlbum;
using DMS.Services.MAppUser;
using DMS.Services.MImage;
using DMS.Services.MStore;
using DMS.Enums;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MERoute;
using DMS.Services.MTaxType;
using DMS.Services.MProduct;
using DMS.Services.MItem;
using DMS.Services.MIndirectSalesOrder;
using DMS.Services.MProblem;
using DMS.Services.MProblemType;

namespace DMS.Rpc.store_checking
{
    public class StoreCheckingController : RpcController
    {
        private IAlbumService AlbumService;
        private IAppUserService AppUserService;
        private IERouteService ERouteService;
        private IIndirectSalesOrderService IndirectSalesOrderService;
        private IItemService ItemService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreCheckingService StoreCheckingService;
        private IStoreTypeService StoreTypeService;
        private ITaxTypeService TaxTypeService;
        private IProductService ProductService;
        private IProblemService ProblemService;
        private IProblemTypeService ProblemTypeService;
        private ICurrentContext CurrentContext;
        public StoreCheckingController(
            IAlbumService AlbumService,
            IAppUserService AppUserService,
            IERouteService ERouteService,
            IIndirectSalesOrderService IndirectSalesOrderService,
            IItemService ItemService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreCheckingService StoreCheckingService,
            IStoreTypeService StoreTypeService,
            IProductService ProductService,
            ITaxTypeService TaxTypeService,
            IProblemService ProblemService,
            IProblemTypeService ProblemTypeService,
            ICurrentContext CurrentContext
        )
        {
            this.AlbumService = AlbumService;
            this.AppUserService = AppUserService;
            this.ERouteService = ERouteService;
            this.IndirectSalesOrderService = IndirectSalesOrderService;
            this.ItemService = ItemService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreCheckingService = StoreCheckingService;
            this.StoreTypeService = StoreTypeService;
            this.TaxTypeService = TaxTypeService;
            this.ProductService = ProductService;
            this.ProblemService = ProblemService;
            this.ProblemTypeService = ProblemTypeService;
            this.CurrentContext = CurrentContext;
        }

        [Route(StoreCheckingRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] StoreChecking_StoreCheckingFilterDTO StoreChecking_StoreCheckingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreCheckingFilter StoreCheckingFilter = ConvertFilterDTOToFilterEntity(StoreChecking_StoreCheckingFilterDTO);
            StoreCheckingFilter = StoreCheckingService.ToFilter(StoreCheckingFilter);
            int count = await StoreCheckingService.Count(StoreCheckingFilter);
            return count;
        }

        [Route(StoreCheckingRoute.List), HttpPost]
        public async Task<ActionResult<List<StoreChecking_StoreCheckingDTO>>> List([FromBody] StoreChecking_StoreCheckingFilterDTO StoreChecking_StoreCheckingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreCheckingFilter StoreCheckingFilter = ConvertFilterDTOToFilterEntity(StoreChecking_StoreCheckingFilterDTO);
            StoreCheckingFilter = StoreCheckingService.ToFilter(StoreCheckingFilter);
            List<StoreChecking> StoreCheckings = await StoreCheckingService.List(StoreCheckingFilter);
            List<StoreChecking_StoreCheckingDTO> StoreChecking_StoreCheckingDTOs = StoreCheckings
                .Select(c => new StoreChecking_StoreCheckingDTO(c)).ToList();
            return StoreChecking_StoreCheckingDTOs;
        }

        [Route(StoreCheckingRoute.Get), HttpPost]
        public async Task<ActionResult<StoreChecking_StoreCheckingDTO>> Get([FromBody]StoreChecking_StoreCheckingDTO StoreChecking_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreChecking_StoreCheckingDTO.Id))
                return Forbid();

            StoreChecking StoreChecking = await StoreCheckingService.Get(StoreChecking_StoreCheckingDTO.Id);
            return new StoreChecking_StoreCheckingDTO(StoreChecking);
        }

        [Route(StoreCheckingRoute.Create), HttpPost]
        public async Task<ActionResult<StoreChecking_StoreCheckingDTO>> Create([FromBody] StoreChecking_StoreCheckingDTO StoreChecking_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(StoreChecking_StoreCheckingDTO.Id))
                return Forbid();

            StoreChecking StoreChecking = ConvertDTOToEntity(StoreChecking_StoreCheckingDTO);
            StoreChecking = await StoreCheckingService.Create(StoreChecking);
            StoreChecking_StoreCheckingDTO = new StoreChecking_StoreCheckingDTO(StoreChecking);
            if (StoreChecking.IsValidated)
                return StoreChecking_StoreCheckingDTO;
            else
                return BadRequest(StoreChecking_StoreCheckingDTO);
        }

        [Route(StoreCheckingRoute.Update), HttpPost]
        public async Task<ActionResult<StoreChecking_StoreCheckingDTO>> Update([FromBody] StoreChecking_StoreCheckingDTO StoreChecking_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(StoreChecking_StoreCheckingDTO.Id))
                return Forbid();

            StoreChecking StoreChecking = ConvertDTOToEntity(StoreChecking_StoreCheckingDTO);
            StoreChecking = await StoreCheckingService.Update(StoreChecking);
            StoreChecking_StoreCheckingDTO = new StoreChecking_StoreCheckingDTO(StoreChecking);
            if (StoreChecking.IsValidated)
                return StoreChecking_StoreCheckingDTO;
            else
                return BadRequest(StoreChecking_StoreCheckingDTO);
        }

        [Route(StoreCheckingRoute.CreateIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<StoreChecking_IndirectSalesOrderDTO>> CreateIndirectSalesOrder([FromBody] StoreChecking_IndirectSalesOrderDTO StoreChecking_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreChecking_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = new IndirectSalesOrder();
            IndirectSalesOrder.Id = StoreChecking_IndirectSalesOrderDTO.Id;
            IndirectSalesOrder.Code = StoreChecking_IndirectSalesOrderDTO.Code;
            IndirectSalesOrder.BuyerStoreId = StoreChecking_IndirectSalesOrderDTO.BuyerStoreId;
            IndirectSalesOrder.PhoneNumber = StoreChecking_IndirectSalesOrderDTO.PhoneNumber;
            IndirectSalesOrder.StoreAddress = StoreChecking_IndirectSalesOrderDTO.StoreAddress;
            IndirectSalesOrder.DeliveryAddress = StoreChecking_IndirectSalesOrderDTO.DeliveryAddress;
            IndirectSalesOrder.SellerStoreId = StoreChecking_IndirectSalesOrderDTO.SellerStoreId;
            IndirectSalesOrder.SaleEmployeeId = StoreChecking_IndirectSalesOrderDTO.SaleEmployeeId;
            IndirectSalesOrder.OrderDate = StoreChecking_IndirectSalesOrderDTO.OrderDate;
            IndirectSalesOrder.DeliveryDate = StoreChecking_IndirectSalesOrderDTO.DeliveryDate;
            IndirectSalesOrder.RequestStateId = StoreChecking_IndirectSalesOrderDTO.RequestStateId;
            IndirectSalesOrder.EditedPriceStatusId = StoreChecking_IndirectSalesOrderDTO.EditedPriceStatusId;
            IndirectSalesOrder.Note = StoreChecking_IndirectSalesOrderDTO.Note;
            IndirectSalesOrder.SubTotal = StoreChecking_IndirectSalesOrderDTO.SubTotal;
            IndirectSalesOrder.GeneralDiscountPercentage = StoreChecking_IndirectSalesOrderDTO.GeneralDiscountPercentage;
            IndirectSalesOrder.GeneralDiscountAmount = StoreChecking_IndirectSalesOrderDTO.GeneralDiscountAmount;
            IndirectSalesOrder.TotalTaxAmount = StoreChecking_IndirectSalesOrderDTO.TotalTaxAmount;
            IndirectSalesOrder.Total = StoreChecking_IndirectSalesOrderDTO.Total;
            IndirectSalesOrder.BuyerStore = StoreChecking_IndirectSalesOrderDTO.BuyerStore == null ? null : new Store
            {
                Id = StoreChecking_IndirectSalesOrderDTO.BuyerStore.Id,
                Code = StoreChecking_IndirectSalesOrderDTO.BuyerStore.Code,
                Name = StoreChecking_IndirectSalesOrderDTO.BuyerStore.Name,
                ParentStoreId = StoreChecking_IndirectSalesOrderDTO.BuyerStore.ParentStoreId,
                OrganizationId = StoreChecking_IndirectSalesOrderDTO.BuyerStore.OrganizationId,
                StoreTypeId = StoreChecking_IndirectSalesOrderDTO.BuyerStore.StoreTypeId,
                StoreGroupingId = StoreChecking_IndirectSalesOrderDTO.BuyerStore.StoreGroupingId,
                ResellerId = StoreChecking_IndirectSalesOrderDTO.BuyerStore.ResellerId,
                Telephone = StoreChecking_IndirectSalesOrderDTO.BuyerStore.Telephone,
                ProvinceId = StoreChecking_IndirectSalesOrderDTO.BuyerStore.ProvinceId,
                DistrictId = StoreChecking_IndirectSalesOrderDTO.BuyerStore.DistrictId,
                WardId = StoreChecking_IndirectSalesOrderDTO.BuyerStore.WardId,
                Address = StoreChecking_IndirectSalesOrderDTO.BuyerStore.Address,
                DeliveryAddress = StoreChecking_IndirectSalesOrderDTO.BuyerStore.DeliveryAddress,
                Latitude = StoreChecking_IndirectSalesOrderDTO.BuyerStore.Latitude,
                Longitude = StoreChecking_IndirectSalesOrderDTO.BuyerStore.Longitude,
                DeliveryLatitude = StoreChecking_IndirectSalesOrderDTO.BuyerStore.DeliveryLatitude,
                DeliveryLongitude = StoreChecking_IndirectSalesOrderDTO.BuyerStore.DeliveryLongitude,
                OwnerName = StoreChecking_IndirectSalesOrderDTO.BuyerStore.OwnerName,
                OwnerPhone = StoreChecking_IndirectSalesOrderDTO.BuyerStore.OwnerPhone,
                OwnerEmail = StoreChecking_IndirectSalesOrderDTO.BuyerStore.OwnerEmail,
                TaxCode = StoreChecking_IndirectSalesOrderDTO.BuyerStore.TaxCode,
                LegalEntity = StoreChecking_IndirectSalesOrderDTO.BuyerStore.LegalEntity,
                StatusId = StoreChecking_IndirectSalesOrderDTO.BuyerStore.StatusId,
            };
            IndirectSalesOrder.EditedPriceStatus = StoreChecking_IndirectSalesOrderDTO.EditedPriceStatus == null ? null : new EditedPriceStatus
            {
                Id = StoreChecking_IndirectSalesOrderDTO.EditedPriceStatus.Id,
                Code = StoreChecking_IndirectSalesOrderDTO.EditedPriceStatus.Code,
                Name = StoreChecking_IndirectSalesOrderDTO.EditedPriceStatus.Name,
            };
            IndirectSalesOrder.SaleEmployee = StoreChecking_IndirectSalesOrderDTO.SaleEmployee == null ? null : new AppUser
            {
                Id = StoreChecking_IndirectSalesOrderDTO.SaleEmployee.Id,
                Username = StoreChecking_IndirectSalesOrderDTO.SaleEmployee.Username,
                DisplayName = StoreChecking_IndirectSalesOrderDTO.SaleEmployee.DisplayName,
                Address = StoreChecking_IndirectSalesOrderDTO.SaleEmployee.Address,
                Email = StoreChecking_IndirectSalesOrderDTO.SaleEmployee.Email,
                Phone = StoreChecking_IndirectSalesOrderDTO.SaleEmployee.Phone,
                PositionId = StoreChecking_IndirectSalesOrderDTO.SaleEmployee.PositionId,
                Department = StoreChecking_IndirectSalesOrderDTO.SaleEmployee.Department,
                OrganizationId = StoreChecking_IndirectSalesOrderDTO.SaleEmployee.OrganizationId,
                SexId = StoreChecking_IndirectSalesOrderDTO.SaleEmployee.SexId,
                StatusId = StoreChecking_IndirectSalesOrderDTO.SaleEmployee.StatusId,
                Avatar = StoreChecking_IndirectSalesOrderDTO.SaleEmployee.Avatar,
                Birthday = StoreChecking_IndirectSalesOrderDTO.SaleEmployee.Birthday,
                ProvinceId = StoreChecking_IndirectSalesOrderDTO.SaleEmployee.ProvinceId,
            };
            IndirectSalesOrder.SellerStore = StoreChecking_IndirectSalesOrderDTO.SellerStore == null ? null : new Store
            {
                Id = StoreChecking_IndirectSalesOrderDTO.SellerStore.Id,
                Code = StoreChecking_IndirectSalesOrderDTO.SellerStore.Code,
                Name = StoreChecking_IndirectSalesOrderDTO.SellerStore.Name,
                ParentStoreId = StoreChecking_IndirectSalesOrderDTO.SellerStore.ParentStoreId,
                OrganizationId = StoreChecking_IndirectSalesOrderDTO.SellerStore.OrganizationId,
                StoreTypeId = StoreChecking_IndirectSalesOrderDTO.SellerStore.StoreTypeId,
                StoreGroupingId = StoreChecking_IndirectSalesOrderDTO.SellerStore.StoreGroupingId,
                ResellerId = StoreChecking_IndirectSalesOrderDTO.SellerStore.ResellerId,
                Telephone = StoreChecking_IndirectSalesOrderDTO.SellerStore.Telephone,
                ProvinceId = StoreChecking_IndirectSalesOrderDTO.SellerStore.ProvinceId,
                DistrictId = StoreChecking_IndirectSalesOrderDTO.SellerStore.DistrictId,
                WardId = StoreChecking_IndirectSalesOrderDTO.SellerStore.WardId,
                Address = StoreChecking_IndirectSalesOrderDTO.SellerStore.Address,
                DeliveryAddress = StoreChecking_IndirectSalesOrderDTO.SellerStore.DeliveryAddress,
                Latitude = StoreChecking_IndirectSalesOrderDTO.SellerStore.Latitude,
                Longitude = StoreChecking_IndirectSalesOrderDTO.SellerStore.Longitude,
                DeliveryLatitude = StoreChecking_IndirectSalesOrderDTO.SellerStore.DeliveryLatitude,
                DeliveryLongitude = StoreChecking_IndirectSalesOrderDTO.SellerStore.DeliveryLongitude,
                OwnerName = StoreChecking_IndirectSalesOrderDTO.SellerStore.OwnerName,
                OwnerPhone = StoreChecking_IndirectSalesOrderDTO.SellerStore.OwnerPhone,
                OwnerEmail = StoreChecking_IndirectSalesOrderDTO.SellerStore.OwnerEmail,
                TaxCode = StoreChecking_IndirectSalesOrderDTO.SellerStore.TaxCode,
                LegalEntity = StoreChecking_IndirectSalesOrderDTO.SellerStore.LegalEntity,
                StatusId = StoreChecking_IndirectSalesOrderDTO.SellerStore.StatusId,
            };
            IndirectSalesOrder.IndirectSalesOrderContents = StoreChecking_IndirectSalesOrderDTO.IndirectSalesOrderContents?
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
            IndirectSalesOrder.IndirectSalesOrderPromotions = StoreChecking_IndirectSalesOrderDTO.IndirectSalesOrderPromotions?
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
            IndirectSalesOrder.StoreCheckingId = StoreChecking_IndirectSalesOrderDTO.StoreCheckingId;
            IndirectSalesOrder = await IndirectSalesOrderService.Create(IndirectSalesOrder);
            StoreChecking_IndirectSalesOrderDTO = new StoreChecking_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return StoreChecking_IndirectSalesOrderDTO;
            else
                return BadRequest(StoreChecking_IndirectSalesOrderDTO);
        }

        [Route(StoreCheckingRoute.CreateProblem), HttpPost]
        public async Task<ActionResult<StoreChecking_ProblemDTO>> CreateProblem([FromBody] StoreChecking_ProblemDTO StoreChecking_ProblemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Problem Problem = new Problem
            {
                Id = StoreChecking_ProblemDTO.Id,
                Content = StoreChecking_ProblemDTO.Content,
                NoteAt = StoreChecking_ProblemDTO.NoteAt,
                ProblemTypeId = StoreChecking_ProblemDTO.ProblemTypeId,
                StoreCheckingId = StoreChecking_ProblemDTO.StoreCheckingId,
                StoreId = StoreChecking_ProblemDTO.StoreId,
            };
            Problem = await ProblemService.Create(Problem);
            if (Problem.IsValidated)
                return StoreChecking_ProblemDTO;
            else
                return BadRequest(StoreChecking_ProblemDTO);
        }
        [Route(StoreCheckingRoute.SaveImage), HttpPost]
        public async Task<ActionResult<StoreChecking_ImageDTO>> SaveImage(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            Image Image = new Image
            {
                Name = file.FileName,
                Content = memoryStream.ToArray()
            };
            Image = await StoreCheckingService.SaveImage(Image);
            if (Image == null)
                return BadRequest();
            StoreChecking_ImageDTO StoreChecking_ImageDTO = new StoreChecking_ImageDTO
            {
                Id = Image.Id,
                Name = Image.Name,
                Url = Image.Url,
            };
            return Ok(StoreChecking_ImageDTO);
        }
        private async Task<bool> HasPermission(long Id)
        {
            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter();
            StoreCheckingFilter = StoreCheckingService.ToFilter(StoreCheckingFilter);
            if (Id == 0)
            {

            }
            else
            {
                StoreCheckingFilter.Id = new IdFilter { Equal = Id };
                int count = await StoreCheckingService.Count(StoreCheckingFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private StoreChecking ConvertDTOToEntity(StoreChecking_StoreCheckingDTO StoreChecking_StoreCheckingDTO)
        {
            StoreChecking StoreChecking = new StoreChecking();
            StoreChecking.Id = StoreChecking_StoreCheckingDTO.Id;
            StoreChecking.StoreId = StoreChecking_StoreCheckingDTO.StoreId;
            StoreChecking.SaleEmployeeId = StoreChecking_StoreCheckingDTO.SaleEmployeeId;
            StoreChecking.Longtitude = StoreChecking_StoreCheckingDTO.Longtitude;
            StoreChecking.Latitude = StoreChecking_StoreCheckingDTO.Latitude;
            StoreChecking.CheckInAt = StoreChecking_StoreCheckingDTO.CheckInAt;
            StoreChecking.CheckOutAt = StoreChecking_StoreCheckingDTO.CheckOutAt;
            StoreChecking.CountIndirectSalesOrder = StoreChecking_StoreCheckingDTO.CountIndirectSalesOrder;
            StoreChecking.CountImage = StoreChecking_StoreCheckingDTO.CountImage;
            StoreChecking.StoreCheckingImageMappings = StoreChecking_StoreCheckingDTO.ImageStoreCheckingMappings?
                .Select(x => new StoreCheckingImageMapping
                {
                    ImageId = x.ImageId,
                    AlbumId = x.AlbumId,
                    StoreId = x.StoreId,
                    SaleEmployeeId = x.SaleEmployeeId,
                    ShootingAt = x.ShootingAt,
                    Album = x.Album == null ? null : new Album
                    {
                        Id = x.Album.Id,
                        Name = x.Album.Name,
                    },
                    SaleEmployee = x.SaleEmployee == null ? null : new AppUser
                    {
                        Id = x.SaleEmployee.Id,
                        Username = x.SaleEmployee.Username,
                        DisplayName = x.SaleEmployee.DisplayName,
                        Address = x.SaleEmployee.Address,
                        Email = x.SaleEmployee.Email,
                        Phone = x.SaleEmployee.Phone,
                        PositionId = x.SaleEmployee.PositionId,
                        Department = x.SaleEmployee.Department,
                        OrganizationId = x.SaleEmployee.OrganizationId,
                        SexId = x.SaleEmployee.SexId,
                        StatusId = x.SaleEmployee.StatusId,
                        Avatar = x.SaleEmployee.Avatar,
                        Birthday = x.SaleEmployee.Birthday,
                        ProvinceId = x.SaleEmployee.ProvinceId,
                    },
                    Image = x.Image == null ? null : new Image
                    {
                        Id = x.Image.Id,
                        Name = x.Image.Name,
                        Url = x.Image.Url,
                    },
                    Store = x.Store == null ? null : new Store
                    {
                        Id = x.Store.Id,
                        Code = x.Store.Code,
                        Name = x.Store.Name,
                        ParentStoreId = x.Store.ParentStoreId,
                        OrganizationId = x.Store.OrganizationId,
                        StoreTypeId = x.Store.StoreTypeId,
                        StoreGroupingId = x.Store.StoreGroupingId,
                        ResellerId = x.Store.ResellerId,
                        Telephone = x.Store.Telephone,
                        ProvinceId = x.Store.ProvinceId,
                        DistrictId = x.Store.DistrictId,
                        WardId = x.Store.WardId,
                        Address = x.Store.Address,
                        DeliveryAddress = x.Store.DeliveryAddress,
                        Latitude = x.Store.Latitude,
                        Longitude = x.Store.Longitude,
                        DeliveryLatitude = x.Store.DeliveryLatitude,
                        DeliveryLongitude = x.Store.DeliveryLongitude,
                        OwnerName = x.Store.OwnerName,
                        OwnerPhone = x.Store.OwnerPhone,
                        OwnerEmail = x.Store.OwnerEmail,
                        StatusId = x.Store.StatusId,
                    },
                }).ToList();
            StoreChecking.BaseLanguage = CurrentContext.Language;
            return StoreChecking;
        }

        private StoreCheckingFilter ConvertFilterDTOToFilterEntity(StoreChecking_StoreCheckingFilterDTO StoreChecking_StoreCheckingFilterDTO)
        {
            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter();
            StoreCheckingFilter.Selects = StoreCheckingSelect.ALL;
            StoreCheckingFilter.Skip = StoreChecking_StoreCheckingFilterDTO.Skip;
            StoreCheckingFilter.Take = StoreChecking_StoreCheckingFilterDTO.Take;
            StoreCheckingFilter.OrderBy = StoreChecking_StoreCheckingFilterDTO.OrderBy;
            StoreCheckingFilter.OrderType = StoreChecking_StoreCheckingFilterDTO.OrderType;

            StoreCheckingFilter.Id = StoreChecking_StoreCheckingFilterDTO.Id;
            StoreCheckingFilter.StoreId = StoreChecking_StoreCheckingFilterDTO.StoreId;
            StoreCheckingFilter.SaleEmployeeId = StoreChecking_StoreCheckingFilterDTO.SaleEmployeeId;
            StoreCheckingFilter.Longtitude = StoreChecking_StoreCheckingFilterDTO.Longtitude;
            StoreCheckingFilter.Latitude = StoreChecking_StoreCheckingFilterDTO.Latitude;
            StoreCheckingFilter.CheckInAt = StoreChecking_StoreCheckingFilterDTO.CheckInAt;
            StoreCheckingFilter.CheckOutAt = StoreChecking_StoreCheckingFilterDTO.CheckOutAt;
            StoreCheckingFilter.CountIndirectSalesOrder = StoreChecking_StoreCheckingFilterDTO.CountIndirectSalesOrder;
            StoreCheckingFilter.CountImage = StoreChecking_StoreCheckingFilterDTO.CountImage;
            return StoreCheckingFilter;
        }

        [Route(StoreCheckingRoute.FilterListAppUser), HttpPost]
        public async Task<List<StoreChecking_AppUserDTO>> FilterListAppUser([FromBody] StoreChecking_AppUserFilterDTO StoreChecking_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = StoreChecking_AppUserFilterDTO.Id;
            AppUserFilter.Username = StoreChecking_AppUserFilterDTO.Username;
            AppUserFilter.Password = StoreChecking_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = StoreChecking_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = StoreChecking_AppUserFilterDTO.Address;
            AppUserFilter.Email = StoreChecking_AppUserFilterDTO.Email;
            AppUserFilter.Phone = StoreChecking_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = StoreChecking_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = StoreChecking_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = StoreChecking_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = StoreChecking_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = StoreChecking_AppUserFilterDTO.StatusId;
            AppUserFilter.Birthday = StoreChecking_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = StoreChecking_AppUserFilterDTO.ProvinceId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<StoreChecking_AppUserDTO> StoreChecking_AppUserDTOs = AppUsers
                .Select(x => new StoreChecking_AppUserDTO(x)).ToList();
            return StoreChecking_AppUserDTOs;
        }

        [Route(StoreCheckingRoute.FilterListStore), HttpPost]
        public async Task<List<StoreChecking_StoreDTO>> FilterListStore([FromBody] StoreChecking_StoreFilterDTO StoreChecking_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = StoreChecking_StoreFilterDTO.Id;
            StoreFilter.Code = StoreChecking_StoreFilterDTO.Code;
            StoreFilter.Name = StoreChecking_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = StoreChecking_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = StoreChecking_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = StoreChecking_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = StoreChecking_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = StoreChecking_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = StoreChecking_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = StoreChecking_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = StoreChecking_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = StoreChecking_StoreFilterDTO.WardId;
            StoreFilter.Address = StoreChecking_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = StoreChecking_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreChecking_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreChecking_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = StoreChecking_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = StoreChecking_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = StoreChecking_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreChecking_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreChecking_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = StoreChecking_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<StoreChecking_StoreDTO> StoreChecking_StoreDTOs = Stores
                .Select(x => new StoreChecking_StoreDTO(x)).ToList();
            return StoreChecking_StoreDTOs;
        }

        [Route(StoreCheckingRoute.SingleListAlbum), HttpPost]
        public async Task<List<StoreChecking_AlbumDTO>> SingleListAlbum([FromBody] StoreChecking_AlbumFilterDTO StoreChecking_AlbumFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AlbumFilter AlbumFilter = new AlbumFilter();
            AlbumFilter.Skip = 0;
            AlbumFilter.Take = 20;
            AlbumFilter.OrderBy = AlbumOrder.Id;
            AlbumFilter.OrderType = OrderType.ASC;
            AlbumFilter.Selects = AlbumSelect.ALL;
            AlbumFilter.Id = StoreChecking_AlbumFilterDTO.Id;
            AlbumFilter.Name = StoreChecking_AlbumFilterDTO.Name;

            List<Album> Albums = await AlbumService.List(AlbumFilter);
            List<StoreChecking_AlbumDTO> StoreChecking_AlbumDTOs = Albums
                .Select(x => new StoreChecking_AlbumDTO(x)).ToList();
            return StoreChecking_AlbumDTOs;
        }
        [Route(StoreCheckingRoute.SingleListAppUser), HttpPost]
        public async Task<List<StoreChecking_AppUserDTO>> SingleListAppUser([FromBody] StoreChecking_AppUserFilterDTO StoreChecking_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = StoreChecking_AppUserFilterDTO.Id;
            AppUserFilter.Username = StoreChecking_AppUserFilterDTO.Username;
            AppUserFilter.Password = StoreChecking_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = StoreChecking_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = StoreChecking_AppUserFilterDTO.Address;
            AppUserFilter.Email = StoreChecking_AppUserFilterDTO.Email;
            AppUserFilter.Phone = StoreChecking_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = StoreChecking_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = StoreChecking_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = StoreChecking_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = StoreChecking_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = StoreChecking_AppUserFilterDTO.StatusId;
            AppUserFilter.Birthday = StoreChecking_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = StoreChecking_AppUserFilterDTO.ProvinceId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<StoreChecking_AppUserDTO> StoreChecking_AppUserDTOs = AppUsers
                .Select(x => new StoreChecking_AppUserDTO(x)).ToList();
            return StoreChecking_AppUserDTOs;
        }

        [Route(StoreCheckingRoute.SingleListEroute), HttpPost]
        public async Task<List<StoreChecking_ERouteDTO>> SingleListEroute(StoreChecking_ERouteFilterDTO StoreChecking_ERouteFilterDTO)
        {
            ERouteFilter ERouteFilter = new ERouteFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                SaleEmployeeId = new IdFilter { Equal = CurrentContext.UserId },
                Selects = ERouteSelect.Id | ERouteSelect.Name | ERouteSelect.Code
            };

            List<ERoute> ERoutes = await ERouteService.List(ERouteFilter);
            List<StoreChecking_ERouteDTO> StoreChecking_ERouteDTOs = ERoutes
                .Select(x => new StoreChecking_ERouteDTO(x)).ToList();
            return StoreChecking_ERouteDTOs;
        }

        [Route(StoreCheckingRoute.SingleListStore), HttpPost]
        public async Task<List<StoreChecking_StoreDTO>> SingleListStore([FromBody] StoreChecking_StoreFilterDTO StoreChecking_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = StoreChecking_StoreFilterDTO.Id;
            StoreFilter.Code = StoreChecking_StoreFilterDTO.Code;
            StoreFilter.Name = StoreChecking_StoreFilterDTO.Name;
            StoreFilter.DeliveryAddress = StoreChecking_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreChecking_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreChecking_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = StoreChecking_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = StoreChecking_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = StoreChecking_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreChecking_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreChecking_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<StoreChecking_StoreDTO> StoreChecking_StoreDTOs = Stores
                .Select(x => new StoreChecking_StoreDTO(x)).ToList();
            return StoreChecking_StoreDTOs;
        }

        [Route(StoreCheckingRoute.SingleListStoreGrouping), HttpPost]
        public async Task<List<StoreChecking_StoreGroupingDTO>> SingleListStoreGrouping([FromBody] StoreChecking_StoreGroupingFilterDTO StoreChecking_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Code = StoreChecking_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = StoreChecking_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.Level = StoreChecking_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.Path = StoreChecking_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<StoreChecking_StoreGroupingDTO> StoreChecking_StoreGroupingDTOs = StoreGroupings
                .Select(x => new StoreChecking_StoreGroupingDTO(x)).ToList();
            return StoreChecking_StoreGroupingDTOs;
        }
        [Route(StoreCheckingRoute.SingleListStoreType), HttpPost]
        public async Task<List<StoreChecking_StoreTypeDTO>> SingleListStoreType([FromBody] StoreChecking_StoreTypeFilterDTO StoreChecking_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = StoreChecking_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = StoreChecking_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = StoreChecking_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<StoreChecking_StoreTypeDTO> StoreChecking_StoreTypeDTOs = StoreTypes
                .Select(x => new StoreChecking_StoreTypeDTO(x)).ToList();
            return StoreChecking_StoreTypeDTOs;
        }

        [Route(StoreCheckingRoute.SingleListTaxType), HttpPost]
        public async Task<List<StoreChecking_TaxTypeDTO>> SingleListTaxType([FromBody] StoreChecking_TaxTypeFilterDTO StoreChecking_TaxTypeFilterDTO)
        {
            TaxTypeFilter TaxTypeFilter = new TaxTypeFilter();
            TaxTypeFilter.Skip = 0;
            TaxTypeFilter.Take = 20;
            TaxTypeFilter.OrderBy = TaxTypeOrder.Id;
            TaxTypeFilter.OrderType = OrderType.ASC;
            TaxTypeFilter.Selects = TaxTypeSelect.ALL;
            TaxTypeFilter.Id = StoreChecking_TaxTypeFilterDTO.Id;
            TaxTypeFilter.Code = StoreChecking_TaxTypeFilterDTO.Code;
            TaxTypeFilter.Name = StoreChecking_TaxTypeFilterDTO.Name;
            TaxTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<TaxType> TaxTypes = await TaxTypeService.List(TaxTypeFilter);
            List<StoreChecking_TaxTypeDTO> StoreChecking_TaxTypeDTOs = TaxTypes
                .Select(x => new StoreChecking_TaxTypeDTO(x)).ToList();
            return StoreChecking_TaxTypeDTOs;
        }

        [Route(StoreCheckingRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<StoreChecking_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] StoreChecking_UnitOfMeasureFilterDTO StoreChecking_UnitOfMeasureFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var Product = await ProductService.Get(StoreChecking_UnitOfMeasureFilterDTO.ProductId.Equal ?? 0);

            List<StoreChecking_UnitOfMeasureDTO> StoreChecking_UnitOfMeasureDTOs = new List<StoreChecking_UnitOfMeasureDTO>();
            if (Product.UnitOfMeasureGrouping != null && Product.UnitOfMeasureGrouping.StatusId == Enums.StatusEnum.ACTIVE.Id)
            {
                StoreChecking_UnitOfMeasureDTOs = Product.UnitOfMeasureGrouping.UnitOfMeasureGroupingContents.Select(x => new StoreChecking_UnitOfMeasureDTO(x)).ToList();
            }
            StoreChecking_UnitOfMeasureDTOs.Add(new StoreChecking_UnitOfMeasureDTO
            {
                Id = Product.UnitOfMeasure.Id,
                Code = Product.UnitOfMeasure.Code,
                Name = Product.UnitOfMeasure.Name,
                Description = Product.UnitOfMeasure.Description,
                StatusId = Product.UnitOfMeasure.StatusId,
                Factor = 1
            });
            return StoreChecking_UnitOfMeasureDTOs;
        }

        [Route(StoreCheckingRoute.SingleListProblemType), HttpPost]
        public async Task<List<StoreChecking_ProblemTypeDTO>> SingleListProblemType()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProblemTypeFilter ProblemTypeFilter = new ProblemTypeFilter();
            ProblemTypeFilter.Skip = 0;
            ProblemTypeFilter.Take = 20;
            ProblemTypeFilter.OrderBy = ProblemTypeOrder.Id;
            ProblemTypeFilter.OrderType = OrderType.ASC;
            ProblemTypeFilter.Selects = ProblemTypeSelect.ALL;

            List<ProblemType> ProblemTypes = await ProblemTypeService.List(ProblemTypeFilter);
            List<StoreChecking_ProblemTypeDTO> StoreChecking_ProblemTypeDTOs = ProblemTypes

                .Select(x => new StoreChecking_ProblemTypeDTO(x)).ToList();
            return StoreChecking_ProblemTypeDTOs;
        }

        [Route(StoreCheckingRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] StoreChecking_StoreFilterDTO StoreChecking_StoreFilterDTO)
        {
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Id = StoreChecking_StoreFilterDTO.Id;
            StoreFilter.Code = StoreChecking_StoreFilterDTO.Code;
            StoreFilter.Name = StoreChecking_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = StoreChecking_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = new IdFilter { Equal = appUser.OrganizationId };
            StoreFilter.StoreTypeId = StoreChecking_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = StoreChecking_StoreFilterDTO.StoreGroupingId;
            StoreFilter.Telephone = StoreChecking_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = StoreChecking_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = StoreChecking_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = StoreChecking_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = StoreChecking_StoreFilterDTO.WardId;
            StoreFilter.Address = StoreChecking_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = StoreChecking_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreChecking_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreChecking_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = StoreChecking_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = StoreChecking_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = StoreChecking_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreChecking_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreChecking_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            return await StoreService.Count(StoreFilter);
        }

        [Route(StoreCheckingRoute.ListStore), HttpPost]
        public async Task<List<StoreChecking_StoreDTO>> ListStore([FromBody] StoreChecking_StoreFilterDTO StoreChecking_StoreFilterDTO)
        {
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = StoreChecking_StoreFilterDTO.Skip;
            StoreFilter.Take = StoreChecking_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = StoreChecking_StoreFilterDTO.Id;
            StoreFilter.Code = StoreChecking_StoreFilterDTO.Code;
            StoreFilter.Name = StoreChecking_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = StoreChecking_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = new IdFilter { Equal = appUser.OrganizationId };
            StoreFilter.StoreTypeId = StoreChecking_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = StoreChecking_StoreFilterDTO.StoreGroupingId;
            StoreFilter.Telephone = StoreChecking_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = StoreChecking_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = StoreChecking_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = StoreChecking_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = StoreChecking_StoreFilterDTO.WardId;
            StoreFilter.Address = StoreChecking_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = StoreChecking_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreChecking_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreChecking_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = StoreChecking_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = StoreChecking_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = StoreChecking_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreChecking_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreChecking_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<StoreChecking_StoreDTO> StoreChecking_StoreDTOs = Stores
                .Select(x => new StoreChecking_StoreDTO(x)).ToList();
            return StoreChecking_StoreDTOs;
        }

        [Route(StoreCheckingRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] StoreChecking_ItemFilterDTO StoreChecking_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Id = StoreChecking_ItemFilterDTO.Id;
            ItemFilter.Code = StoreChecking_ItemFilterDTO.Code;
            ItemFilter.Name = StoreChecking_ItemFilterDTO.Name;
            ItemFilter.OtherName = StoreChecking_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = StoreChecking_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = StoreChecking_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = StoreChecking_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = StoreChecking_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = StoreChecking_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = StoreChecking_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = StoreChecking_ItemFilterDTO.SupplierId;
            return await ItemService.Count(ItemFilter);
        }

        [Route(StoreCheckingRoute.ListItem), HttpPost]
        public async Task<List<StoreChecking_ItemDTO>> ListItem([FromBody] StoreChecking_ItemFilterDTO StoreChecking_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = StoreChecking_ItemFilterDTO.Skip;
            ItemFilter.Take = StoreChecking_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = StoreChecking_ItemFilterDTO.Id;
            ItemFilter.Code = StoreChecking_ItemFilterDTO.Code;
            ItemFilter.Name = StoreChecking_ItemFilterDTO.Name;
            ItemFilter.OtherName = StoreChecking_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = StoreChecking_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = StoreChecking_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = StoreChecking_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = StoreChecking_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = StoreChecking_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = StoreChecking_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = StoreChecking_ItemFilterDTO.SupplierId;

            if (StoreChecking_ItemFilterDTO.StoreId != null && StoreChecking_ItemFilterDTO.StoreId.Equal.HasValue)
            {
                List<Item> Items = await IndirectSalesOrderService.ListItem(ItemFilter, StoreChecking_ItemFilterDTO.StoreId.Equal.Value);
                List<StoreChecking_ItemDTO> StoreChecking_ItemDTOs = Items
                    .Select(x => new StoreChecking_ItemDTO(x)).ToList();
                return StoreChecking_ItemDTOs;
            }
            else
            {
                List<Item> Items = await ItemService.List(ItemFilter);
                List<StoreChecking_ItemDTO> StoreChecking_ItemDTOs = Items
                    .Select(x => new StoreChecking_ItemDTO(x)).ToList();
                return StoreChecking_ItemDTOs;
            }
        }

        [Route(StoreCheckingRoute.CountProblem), HttpPost]
        public async Task<long> CountProblem([FromBody] StoreChecking_ProblemFilterDTO StoreChecking_ProblemFilterDTO)
        {
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            ProblemFilter ProblemFilter = new ProblemFilter();
            ProblemFilter.Id = StoreChecking_ProblemFilterDTO.Id;
            ProblemFilter.StoreCheckingId = StoreChecking_ProblemFilterDTO.StoreCheckingId;
            ProblemFilter.ProblemTypeId = StoreChecking_ProblemFilterDTO.ProblemTypeId;
            return await ProblemService.Count(ProblemFilter);
        }

        [Route(StoreCheckingRoute.ListProblem), HttpPost]
        public async Task<List<StoreChecking_ProblemDTO>> ListProblem([FromBody] StoreChecking_ProblemFilterDTO StoreChecking_ProblemFilterDTO)
        {
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            ProblemFilter ProblemFilter = new ProblemFilter();
            ProblemFilter.Skip = StoreChecking_ProblemFilterDTO.Skip;
            ProblemFilter.Take = StoreChecking_ProblemFilterDTO.Take;
            ProblemFilter.OrderBy = ProblemOrder.NoteAt;
            ProblemFilter.OrderType = OrderType.ASC;
            ProblemFilter.Selects = ProblemSelect.ALL;
            ProblemFilter.Id = StoreChecking_ProblemFilterDTO.Id;
            ProblemFilter.StoreCheckingId = StoreChecking_ProblemFilterDTO.StoreCheckingId;
            ProblemFilter.ProblemTypeId = StoreChecking_ProblemFilterDTO.ProblemTypeId;

            List<Problem> Problems = await ProblemService.List(ProblemFilter);
            List<StoreChecking_ProblemDTO> StoreChecking_ProblemDTOs = Problems
                .Select(x => new StoreChecking_ProblemDTO(x)).ToList();
            return StoreChecking_ProblemDTOs;
        }
    }
}

