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

namespace DMS.Rpc.store_checking
{
    public partial class StoreCheckingController : SimpleController
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
        public StoreCheckingController(
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

        [Route(StoreCheckingRoute.CheckIn), HttpPost]
        public async Task<ActionResult<StoreChecking_StoreCheckingDTO>> Checkin([FromBody] StoreChecking_StoreCheckingDTO StoreChecking_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreChecking_StoreCheckingDTO.Id))
                return Forbid();

            StoreChecking StoreChecking = ConvertDTOToEntity(StoreChecking_StoreCheckingDTO);
            StoreChecking = await StoreCheckingService.CheckIn(StoreChecking);
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

        [Route(StoreCheckingRoute.CheckOut), HttpPost]
        public async Task<ActionResult<StoreChecking_StoreCheckingDTO>> CheckOut([FromBody] StoreChecking_StoreCheckingDTO StoreChecking_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreChecking_StoreCheckingDTO.Id))
                return Forbid();

            StoreChecking StoreChecking = ConvertDTOToEntity(StoreChecking_StoreCheckingDTO);
            StoreChecking = await StoreCheckingService.CheckOut(StoreChecking);
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
                CompletedAt = StoreChecking_ProblemDTO.CompletedAt,
                ProblemTypeId = StoreChecking_ProblemDTO.ProblemTypeId,
                ProblemStatusId = StoreChecking_ProblemDTO.ProblemStatusId,
                StoreCheckingId = StoreChecking_ProblemDTO.StoreCheckingId,
                CreatorId = StoreChecking_ProblemDTO.CreatorId,
                StoreId = StoreChecking_ProblemDTO.StoreId,
                ProblemImageMappings = StoreChecking_ProblemDTO.ProblemImageMappings?.Select(x => new ProblemImageMapping
                {
                    ImageId = x.ImageId,
                    ProblemId = x.ProblemId
                }).ToList()
            };
            Problem = await ProblemService.Create(Problem);
            if (Problem.IsValidated)
                return StoreChecking_ProblemDTO;
            else
                return BadRequest(StoreChecking_ProblemDTO);
        }

        [Route(StoreCheckingRoute.GetSurveyForm), HttpPost]
        public async Task<ActionResult<StoreChecking_SurveyDTO>> GetSurveyForm([FromBody] StoreChecking_SurveyDTO StoreChecking_SurveyDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            //if (!await HasPermission(StoreChecking_SurveyDTO.Id))
            //    return Forbid();

            Survey Survey = await SurveyService.GetForm(StoreChecking_SurveyDTO.Id);
            StoreChecking_SurveyDTO = new StoreChecking_SurveyDTO(Survey);
            if (Survey.IsValidated)
                return StoreChecking_SurveyDTO;
            else
                return BadRequest(StoreChecking_SurveyDTO);
        }

        [Route(StoreCheckingRoute.SaveSurveyForm), HttpPost]
        public async Task<ActionResult<StoreChecking_SurveyDTO>> SaveSurveyForm([FromBody] StoreChecking_SurveyDTO StoreChecking_SurveyDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreChecking_SurveyDTO.Id))
                return Forbid();

            Survey Survey = new Survey();
            Survey.Id = StoreChecking_SurveyDTO.Id;
            Survey.Title = StoreChecking_SurveyDTO.Title;
            Survey.Description = StoreChecking_SurveyDTO.Description;
            Survey.StartAt = StoreChecking_SurveyDTO.StartAt;
            Survey.EndAt = StoreChecking_SurveyDTO.EndAt;
            Survey.StatusId = StoreChecking_SurveyDTO.StatusId;
            Survey.StoreId = StoreChecking_SurveyDTO.StoreId;
            Survey.SurveyQuestions = StoreChecking_SurveyDTO.SurveyQuestions?
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
            StoreChecking_SurveyDTO = new StoreChecking_SurveyDTO(Survey);
            if (Survey.IsValidated)
                return StoreChecking_SurveyDTO;
            else
                return BadRequest(StoreChecking_SurveyDTO);
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
            StoreChecking.Longitude = StoreChecking_StoreCheckingDTO.Longitude;
            StoreChecking.Latitude = StoreChecking_StoreCheckingDTO.Latitude;
            StoreChecking.CheckInAt = StoreChecking_StoreCheckingDTO.CheckInAt;
            StoreChecking.CheckOutAt = StoreChecking_StoreCheckingDTO.CheckOutAt;
            StoreChecking.CountIndirectSalesOrder = StoreChecking_StoreCheckingDTO.CountIndirectSalesOrder;
            StoreChecking.ImageCounter = StoreChecking_StoreCheckingDTO.CountImage;
            StoreChecking.IsOpenedStore = StoreChecking_StoreCheckingDTO.IsOpenedStore;
            StoreChecking.StoreCheckingImageMappings = StoreChecking_StoreCheckingDTO.StoreCheckingImageMappings?
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
            StoreCheckingFilter.Longitude = StoreChecking_StoreCheckingFilterDTO.Longitude;
            StoreCheckingFilter.Latitude = StoreChecking_StoreCheckingFilterDTO.Latitude;
            StoreCheckingFilter.CheckInAt = StoreChecking_StoreCheckingFilterDTO.CheckInAt;
            StoreCheckingFilter.CheckOutAt = StoreChecking_StoreCheckingFilterDTO.CheckOutAt;
            StoreCheckingFilter.CountIndirectSalesOrder = StoreChecking_StoreCheckingFilterDTO.CountIndirectSalesOrder;
            StoreCheckingFilter.CountImage = StoreChecking_StoreCheckingFilterDTO.CountImage;
            return StoreCheckingFilter;
        }

    }
}

