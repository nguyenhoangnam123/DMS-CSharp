using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAlbum;
using DMS.Services.MAppUser;
using DMS.Services.MERoute;
using DMS.Services.MERouteContent;
using DMS.Services.MIndirectSalesOrder;
using DMS.Services.MProblem;
using DMS.Services.MProduct;
using DMS.Services.MStore;
using DMS.Services.MStoreChecking;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MSurvey;
using DMS.Services.MTaxType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DMS.Services.MBanner;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DMS.Services.MStoreScouting;
using DMS.Services.MProvince;
using DMS.Services.MDistrict;
using DMS.Services.MWard;

namespace DMS.Rpc.mobile
{
    public partial class MobileController : SimpleController
    {
        private IAlbumService AlbumService;
        private IBannerService BannerService;
        private IAppUserService AppUserService;
        private IERouteService ERouteService;
        private IIndirectSalesOrderService IndirectSalesOrderService;
        private IItemService ItemService;
        private IStoreService StoreService;
        private IStoreScoutingService StoreScoutingService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreCheckingService StoreCheckingService;
        private IStoreTypeService StoreTypeService;
        private ITaxTypeService TaxTypeService;
        private IProductService ProductService;
        private IProblemService ProblemService;
        private IProblemTypeService ProblemTypeService;
        private ISurveyService SurveyService;
        private IProvinceService ProvinceService;
        private IDistrictService DistrictService;
        private IWardService WardService;
        private ICurrentContext CurrentContext;
        public MobileController(
            IAlbumService AlbumService,
            IBannerService BannerService,
            IAppUserService AppUserService,
            IERouteService ERouteService,
            IIndirectSalesOrderService IndirectSalesOrderService,
            IItemService ItemService,
            IStoreScoutingService StoreScoutingService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreCheckingService StoreCheckingService,
            IStoreTypeService StoreTypeService,
            IProductService ProductService,
            ITaxTypeService TaxTypeService,
            IProblemService ProblemService,
            IProblemTypeService ProblemTypeService,
            ISurveyService SurveyService,
            IProvinceService ProvinceService,
            IDistrictService DistrictService,
            IWardService WardService,
            ICurrentContext CurrentContext
        )
        {
            this.AlbumService = AlbumService;
            this.BannerService = BannerService;
            this.AppUserService = AppUserService;
            this.ERouteService = ERouteService;
            this.IndirectSalesOrderService = IndirectSalesOrderService;
            this.ItemService = ItemService;
            this.StoreService = StoreService;
            this.StoreScoutingService = StoreScoutingService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreCheckingService = StoreCheckingService;
            this.StoreTypeService = StoreTypeService;
            this.TaxTypeService = TaxTypeService;
            this.ProductService = ProductService;
            this.ProblemService = ProblemService;
            this.ProblemTypeService = ProblemTypeService;
            this.SurveyService = SurveyService;
            this.ProvinceService = ProvinceService;
            this.DistrictService = DistrictService;
            this.WardService = WardService;
            this.CurrentContext = CurrentContext;
        }

        [Route(MobileRoute.CountStoreChecking), HttpPost]
        public async Task<ActionResult<int>> CountStoreChecking([FromBody] Mobile_StoreCheckingFilterDTO Mobile_StoreCheckingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreCheckingFilter StoreCheckingFilter = ConvertFilterDTOToFilterEntity(Mobile_StoreCheckingFilterDTO);
            StoreCheckingFilter = StoreCheckingService.ToFilter(StoreCheckingFilter);
            int count = await StoreCheckingService.Count(StoreCheckingFilter);
            return count;
        }

        [Route(MobileRoute.ListStoreChecking), HttpPost]
        public async Task<ActionResult<List<Mobile_StoreCheckingDTO>>> ListStoreChecking([FromBody] Mobile_StoreCheckingFilterDTO Mobile_StoreCheckingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreCheckingFilter StoreCheckingFilter = ConvertFilterDTOToFilterEntity(Mobile_StoreCheckingFilterDTO);
            StoreCheckingFilter = StoreCheckingService.ToFilter(StoreCheckingFilter);
            List<StoreChecking> StoreCheckings = await StoreCheckingService.List(StoreCheckingFilter);
            List<Mobile_StoreCheckingDTO> Mobile_StoreCheckingDTOs = StoreCheckings
                .Select(c => new Mobile_StoreCheckingDTO(c)).ToList();
            return Mobile_StoreCheckingDTOs;
        }

        [Route(MobileRoute.GetStoreChecking), HttpPost]
        public async Task<ActionResult<Mobile_StoreCheckingDTO>> GetStoreChecking([FromBody]Mobile_StoreCheckingDTO Mobile_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Mobile_StoreCheckingDTO.Id))
                return Forbid();

            StoreChecking StoreChecking = await StoreCheckingService.Get(Mobile_StoreCheckingDTO.Id);
            return new Mobile_StoreCheckingDTO(StoreChecking);
        }

        [Route(MobileRoute.CheckIn), HttpPost]
        public async Task<ActionResult<Mobile_StoreCheckingDTO>> Checkin([FromBody] Mobile_StoreCheckingDTO Mobile_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Mobile_StoreCheckingDTO.Id))
                return Forbid();

            StoreChecking StoreChecking = ConvertDTOToEntity(Mobile_StoreCheckingDTO);
            StoreChecking = await StoreCheckingService.CheckIn(StoreChecking);
            Mobile_StoreCheckingDTO = new Mobile_StoreCheckingDTO(StoreChecking);
            if (StoreChecking.IsValidated)
                return Mobile_StoreCheckingDTO;
            else
                return BadRequest(Mobile_StoreCheckingDTO);
        }

        [Route(MobileRoute.UpdateStoreChecking), HttpPost]
        public async Task<ActionResult<Mobile_StoreCheckingDTO>> Update([FromBody] Mobile_StoreCheckingDTO Mobile_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Mobile_StoreCheckingDTO.Id))
                return Forbid();

            StoreChecking StoreChecking = ConvertDTOToEntity(Mobile_StoreCheckingDTO);
            StoreChecking = await StoreCheckingService.Update(StoreChecking);
            Mobile_StoreCheckingDTO = new Mobile_StoreCheckingDTO(StoreChecking);
            if (StoreChecking.IsValidated)
                return Mobile_StoreCheckingDTO;
            else
                return BadRequest(Mobile_StoreCheckingDTO);
        }

        [Route(MobileRoute.CheckOut), HttpPost]
        public async Task<ActionResult<Mobile_StoreCheckingDTO>> CheckOut([FromBody] Mobile_StoreCheckingDTO Mobile_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Mobile_StoreCheckingDTO.Id))
                return Forbid();

            StoreChecking StoreChecking = ConvertDTOToEntity(Mobile_StoreCheckingDTO);
            StoreChecking = await StoreCheckingService.CheckOut(StoreChecking);
            Mobile_StoreCheckingDTO = new Mobile_StoreCheckingDTO(StoreChecking);
            if (StoreChecking.IsValidated)
                return Mobile_StoreCheckingDTO;
            else
                return BadRequest(Mobile_StoreCheckingDTO);
        }

        [Route(MobileRoute.CreateIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<Mobile_IndirectSalesOrderDTO>> CreateIndirectSalesOrder([FromBody] Mobile_IndirectSalesOrderDTO Mobile_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Mobile_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = new IndirectSalesOrder();
            IndirectSalesOrder.Id = Mobile_IndirectSalesOrderDTO.Id;
            IndirectSalesOrder.Code = Mobile_IndirectSalesOrderDTO.Code;
            IndirectSalesOrder.BuyerStoreId = Mobile_IndirectSalesOrderDTO.BuyerStoreId;
            IndirectSalesOrder.PhoneNumber = Mobile_IndirectSalesOrderDTO.PhoneNumber;
            IndirectSalesOrder.StoreAddress = Mobile_IndirectSalesOrderDTO.StoreAddress;
            IndirectSalesOrder.DeliveryAddress = Mobile_IndirectSalesOrderDTO.DeliveryAddress;
            IndirectSalesOrder.SellerStoreId = Mobile_IndirectSalesOrderDTO.SellerStoreId;
            IndirectSalesOrder.SaleEmployeeId = Mobile_IndirectSalesOrderDTO.SaleEmployeeId;
            IndirectSalesOrder.OrderDate = Mobile_IndirectSalesOrderDTO.OrderDate;
            IndirectSalesOrder.DeliveryDate = Mobile_IndirectSalesOrderDTO.DeliveryDate;
            IndirectSalesOrder.RequestStateId = Mobile_IndirectSalesOrderDTO.RequestStateId;
            IndirectSalesOrder.EditedPriceStatusId = Mobile_IndirectSalesOrderDTO.EditedPriceStatusId;
            IndirectSalesOrder.Note = Mobile_IndirectSalesOrderDTO.Note;
            IndirectSalesOrder.SubTotal = Mobile_IndirectSalesOrderDTO.SubTotal;
            IndirectSalesOrder.GeneralDiscountPercentage = Mobile_IndirectSalesOrderDTO.GeneralDiscountPercentage;
            IndirectSalesOrder.GeneralDiscountAmount = Mobile_IndirectSalesOrderDTO.GeneralDiscountAmount;
            IndirectSalesOrder.TotalTaxAmount = Mobile_IndirectSalesOrderDTO.TotalTaxAmount;
            IndirectSalesOrder.Total = Mobile_IndirectSalesOrderDTO.Total;
            IndirectSalesOrder.BuyerStore = Mobile_IndirectSalesOrderDTO.BuyerStore == null ? null : new Store
            {
                Id = Mobile_IndirectSalesOrderDTO.BuyerStore.Id,
                Code = Mobile_IndirectSalesOrderDTO.BuyerStore.Code,
                Name = Mobile_IndirectSalesOrderDTO.BuyerStore.Name,
                ParentStoreId = Mobile_IndirectSalesOrderDTO.BuyerStore.ParentStoreId,
                OrganizationId = Mobile_IndirectSalesOrderDTO.BuyerStore.OrganizationId,
                StoreTypeId = Mobile_IndirectSalesOrderDTO.BuyerStore.StoreTypeId,
                StoreGroupingId = Mobile_IndirectSalesOrderDTO.BuyerStore.StoreGroupingId,
                ResellerId = Mobile_IndirectSalesOrderDTO.BuyerStore.ResellerId,
                Telephone = Mobile_IndirectSalesOrderDTO.BuyerStore.Telephone,
                ProvinceId = Mobile_IndirectSalesOrderDTO.BuyerStore.ProvinceId,
                DistrictId = Mobile_IndirectSalesOrderDTO.BuyerStore.DistrictId,
                WardId = Mobile_IndirectSalesOrderDTO.BuyerStore.WardId,
                Address = Mobile_IndirectSalesOrderDTO.BuyerStore.Address,
                DeliveryAddress = Mobile_IndirectSalesOrderDTO.BuyerStore.DeliveryAddress,
                Latitude = Mobile_IndirectSalesOrderDTO.BuyerStore.Latitude,
                Longitude = Mobile_IndirectSalesOrderDTO.BuyerStore.Longitude,
                DeliveryLatitude = Mobile_IndirectSalesOrderDTO.BuyerStore.DeliveryLatitude,
                DeliveryLongitude = Mobile_IndirectSalesOrderDTO.BuyerStore.DeliveryLongitude,
                OwnerName = Mobile_IndirectSalesOrderDTO.BuyerStore.OwnerName,
                OwnerPhone = Mobile_IndirectSalesOrderDTO.BuyerStore.OwnerPhone,
                OwnerEmail = Mobile_IndirectSalesOrderDTO.BuyerStore.OwnerEmail,
                TaxCode = Mobile_IndirectSalesOrderDTO.BuyerStore.TaxCode,
                LegalEntity = Mobile_IndirectSalesOrderDTO.BuyerStore.LegalEntity,
                StatusId = Mobile_IndirectSalesOrderDTO.BuyerStore.StatusId,
            };
            IndirectSalesOrder.EditedPriceStatus = Mobile_IndirectSalesOrderDTO.EditedPriceStatus == null ? null : new EditedPriceStatus
            {
                Id = Mobile_IndirectSalesOrderDTO.EditedPriceStatus.Id,
                Code = Mobile_IndirectSalesOrderDTO.EditedPriceStatus.Code,
                Name = Mobile_IndirectSalesOrderDTO.EditedPriceStatus.Name,
            };
            IndirectSalesOrder.SaleEmployee = Mobile_IndirectSalesOrderDTO.SaleEmployee == null ? null : new AppUser
            {
                Id = Mobile_IndirectSalesOrderDTO.SaleEmployee.Id,
                Username = Mobile_IndirectSalesOrderDTO.SaleEmployee.Username,
                DisplayName = Mobile_IndirectSalesOrderDTO.SaleEmployee.DisplayName,
                Address = Mobile_IndirectSalesOrderDTO.SaleEmployee.Address,
                Email = Mobile_IndirectSalesOrderDTO.SaleEmployee.Email,
                Phone = Mobile_IndirectSalesOrderDTO.SaleEmployee.Phone,
                PositionId = Mobile_IndirectSalesOrderDTO.SaleEmployee.PositionId,
                Department = Mobile_IndirectSalesOrderDTO.SaleEmployee.Department,
                OrganizationId = Mobile_IndirectSalesOrderDTO.SaleEmployee.OrganizationId,
                SexId = Mobile_IndirectSalesOrderDTO.SaleEmployee.SexId,
                StatusId = Mobile_IndirectSalesOrderDTO.SaleEmployee.StatusId,
                Avatar = Mobile_IndirectSalesOrderDTO.SaleEmployee.Avatar,
                Birthday = Mobile_IndirectSalesOrderDTO.SaleEmployee.Birthday,
                ProvinceId = Mobile_IndirectSalesOrderDTO.SaleEmployee.ProvinceId,
            };
            IndirectSalesOrder.SellerStore = Mobile_IndirectSalesOrderDTO.SellerStore == null ? null : new Store
            {
                Id = Mobile_IndirectSalesOrderDTO.SellerStore.Id,
                Code = Mobile_IndirectSalesOrderDTO.SellerStore.Code,
                Name = Mobile_IndirectSalesOrderDTO.SellerStore.Name,
                ParentStoreId = Mobile_IndirectSalesOrderDTO.SellerStore.ParentStoreId,
                OrganizationId = Mobile_IndirectSalesOrderDTO.SellerStore.OrganizationId,
                StoreTypeId = Mobile_IndirectSalesOrderDTO.SellerStore.StoreTypeId,
                StoreGroupingId = Mobile_IndirectSalesOrderDTO.SellerStore.StoreGroupingId,
                ResellerId = Mobile_IndirectSalesOrderDTO.SellerStore.ResellerId,
                Telephone = Mobile_IndirectSalesOrderDTO.SellerStore.Telephone,
                ProvinceId = Mobile_IndirectSalesOrderDTO.SellerStore.ProvinceId,
                DistrictId = Mobile_IndirectSalesOrderDTO.SellerStore.DistrictId,
                WardId = Mobile_IndirectSalesOrderDTO.SellerStore.WardId,
                Address = Mobile_IndirectSalesOrderDTO.SellerStore.Address,
                DeliveryAddress = Mobile_IndirectSalesOrderDTO.SellerStore.DeliveryAddress,
                Latitude = Mobile_IndirectSalesOrderDTO.SellerStore.Latitude,
                Longitude = Mobile_IndirectSalesOrderDTO.SellerStore.Longitude,
                DeliveryLatitude = Mobile_IndirectSalesOrderDTO.SellerStore.DeliveryLatitude,
                DeliveryLongitude = Mobile_IndirectSalesOrderDTO.SellerStore.DeliveryLongitude,
                OwnerName = Mobile_IndirectSalesOrderDTO.SellerStore.OwnerName,
                OwnerPhone = Mobile_IndirectSalesOrderDTO.SellerStore.OwnerPhone,
                OwnerEmail = Mobile_IndirectSalesOrderDTO.SellerStore.OwnerEmail,
                TaxCode = Mobile_IndirectSalesOrderDTO.SellerStore.TaxCode,
                LegalEntity = Mobile_IndirectSalesOrderDTO.SellerStore.LegalEntity,
                StatusId = Mobile_IndirectSalesOrderDTO.SellerStore.StatusId,
            };
            IndirectSalesOrder.IndirectSalesOrderContents = Mobile_IndirectSalesOrderDTO.IndirectSalesOrderContents?
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
            IndirectSalesOrder.IndirectSalesOrderPromotions = Mobile_IndirectSalesOrderDTO.IndirectSalesOrderPromotions?
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
            IndirectSalesOrder.StoreCheckingId = Mobile_IndirectSalesOrderDTO.StoreCheckingId;
            IndirectSalesOrder = await IndirectSalesOrderService.Create(IndirectSalesOrder);
            Mobile_IndirectSalesOrderDTO = new Mobile_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return Mobile_IndirectSalesOrderDTO;
            else
                return BadRequest(Mobile_IndirectSalesOrderDTO);
        }

        [Route(MobileRoute.CreateProblem), HttpPost]
        public async Task<ActionResult<Mobile_ProblemDTO>> CreateProblem([FromBody] Mobile_ProblemDTO Mobile_ProblemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Problem Problem = new Problem
            {
                Id = Mobile_ProblemDTO.Id,
                Content = Mobile_ProblemDTO.Content,
                NoteAt = Mobile_ProblemDTO.NoteAt,
                CompletedAt = Mobile_ProblemDTO.CompletedAt,
                ProblemTypeId = Mobile_ProblemDTO.ProblemTypeId,
                ProblemStatusId = Mobile_ProblemDTO.ProblemStatusId,
                StoreCheckingId = Mobile_ProblemDTO.StoreCheckingId,
                CreatorId = Mobile_ProblemDTO.CreatorId,
                StoreId = Mobile_ProblemDTO.StoreId,
                ProblemImageMappings = Mobile_ProblemDTO.ProblemImageMappings?.Select(x => new ProblemImageMapping
                {
                    ImageId = x.ImageId,
                    ProblemId = x.ProblemId
                }).ToList()
            };
            Problem = await ProblemService.Create(Problem);
            if (Problem.IsValidated)
                return Mobile_ProblemDTO;
            else
                return BadRequest(Mobile_ProblemDTO);
        }

        [Route(MobileRoute.GetSurveyForm), HttpPost]
        public async Task<ActionResult<Mobile_SurveyDTO>> GetSurveyForm([FromBody] Mobile_SurveyDTO Mobile_SurveyDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            //if (!await HasPermission(Mobile_SurveyDTO.Id))
            //    return Forbid();

            Survey Survey = await SurveyService.GetForm(Mobile_SurveyDTO.Id);
            Mobile_SurveyDTO = new Mobile_SurveyDTO(Survey);
            if (Survey.IsValidated)
                return Mobile_SurveyDTO;
            else
                return BadRequest(Mobile_SurveyDTO);
        }

        [Route(MobileRoute.SaveSurveyForm), HttpPost]
        public async Task<ActionResult<Mobile_SurveyDTO>> SaveSurveyForm([FromBody] Mobile_SurveyDTO Mobile_SurveyDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Mobile_SurveyDTO.Id))
                return Forbid();

            Survey Survey = new Survey();
            Survey.Id = Mobile_SurveyDTO.Id;
            Survey.Title = Mobile_SurveyDTO.Title;
            Survey.Description = Mobile_SurveyDTO.Description;
            Survey.StartAt = Mobile_SurveyDTO.StartAt;
            Survey.EndAt = Mobile_SurveyDTO.EndAt;
            Survey.StatusId = Mobile_SurveyDTO.StatusId;
            Survey.StoreId = Mobile_SurveyDTO.StoreId;
            Survey.SurveyQuestions = Mobile_SurveyDTO.SurveyQuestions?
                .Select(x => new SurveyQuestion
                {
                    Id = x.Id,
                    Content = x.Content,
                    SurveyQuestionTypeId = x.SurveyQuestionTypeId,
                    IsMandatory = x.IsMandatory,
                    SurveyQuestionType = x.SurveyQuestionType == null ? null : new SurveyQuestionType
                    {
                        Id = x.SurveyQuestionType.Id,
                        Code = x.SurveyQuestionType.Code,
                        Name = x.SurveyQuestionType.Name,
                    },
                    SurveyOptions = x.SurveyOptions?.Select(x => new SurveyOption
                    {
                        Id = x.Id,
                        Content = x.Content,
                        SurveyOptionTypeId = x.SurveyOptionTypeId,
                        SurveyQuestionId = x.SurveyOptionTypeId
                    }).ToList(),
                    TableResult = x.TableResult,
                    ListResult = x.ListResult,
                }).ToList();
            Survey.BaseLanguage = CurrentContext.Language;
            Survey = await SurveyService.SaveForm(Survey);
            Mobile_SurveyDTO = new Mobile_SurveyDTO(Survey);
            if (Survey.IsValidated)
                return Mobile_SurveyDTO;
            else
                return BadRequest(Mobile_SurveyDTO);
        }


        [Route(MobileRoute.SaveImage), HttpPost]
        public async Task<ActionResult<Mobile_ImageDTO>> SaveImage(IFormFile file)
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
            Mobile_ImageDTO Mobile_ImageDTO = new Mobile_ImageDTO
            {
                Id = Image.Id,
                Name = Image.Name,
                Url = Image.Url,
            };
            return Ok(Mobile_ImageDTO);
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

        private StoreChecking ConvertDTOToEntity(Mobile_StoreCheckingDTO Mobile_StoreCheckingDTO)
        {
            StoreChecking StoreChecking = new StoreChecking();
            StoreChecking.Id = Mobile_StoreCheckingDTO.Id;
            StoreChecking.StoreId = Mobile_StoreCheckingDTO.StoreId;
            StoreChecking.SaleEmployeeId = Mobile_StoreCheckingDTO.SaleEmployeeId;
            StoreChecking.Longitude = Mobile_StoreCheckingDTO.Longitude;
            StoreChecking.Latitude = Mobile_StoreCheckingDTO.Latitude;
            StoreChecking.CheckInAt = Mobile_StoreCheckingDTO.CheckInAt;
            StoreChecking.CheckOutAt = Mobile_StoreCheckingDTO.CheckOutAt;
            StoreChecking.CountIndirectSalesOrder = Mobile_StoreCheckingDTO.CountIndirectSalesOrder;
            StoreChecking.ImageCounter = Mobile_StoreCheckingDTO.CountImage;
            StoreChecking.IsOpenedStore = Mobile_StoreCheckingDTO.IsOpenedStore;
            StoreChecking.StoreCheckingImageMappings = Mobile_StoreCheckingDTO.StoreCheckingImageMappings?
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

        private StoreCheckingFilter ConvertFilterDTOToFilterEntity(Mobile_StoreCheckingFilterDTO Mobile_StoreCheckingFilterDTO)
        {
            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter();
            StoreCheckingFilter.Selects = StoreCheckingSelect.ALL;
            StoreCheckingFilter.Skip = Mobile_StoreCheckingFilterDTO.Skip;
            StoreCheckingFilter.Take = Mobile_StoreCheckingFilterDTO.Take;
            StoreCheckingFilter.OrderBy = Mobile_StoreCheckingFilterDTO.OrderBy;
            StoreCheckingFilter.OrderType = Mobile_StoreCheckingFilterDTO.OrderType;

            StoreCheckingFilter.Id = Mobile_StoreCheckingFilterDTO.Id;
            StoreCheckingFilter.StoreId = Mobile_StoreCheckingFilterDTO.StoreId;
            StoreCheckingFilter.SaleEmployeeId = Mobile_StoreCheckingFilterDTO.SaleEmployeeId;
            StoreCheckingFilter.Longitude = Mobile_StoreCheckingFilterDTO.Longitude;
            StoreCheckingFilter.Latitude = Mobile_StoreCheckingFilterDTO.Latitude;
            StoreCheckingFilter.CheckInAt = Mobile_StoreCheckingFilterDTO.CheckInAt;
            StoreCheckingFilter.CheckOutAt = Mobile_StoreCheckingFilterDTO.CheckOutAt;
            StoreCheckingFilter.CountIndirectSalesOrder = Mobile_StoreCheckingFilterDTO.CountIndirectSalesOrder;
            StoreCheckingFilter.CountImage = Mobile_StoreCheckingFilterDTO.CountImage;
            return StoreCheckingFilter;
        }

    }
}

