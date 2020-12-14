using DMS.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAlbum;
using DMS.Services.MAppUser;
using DMS.Services.MERoute;
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
using DMS.Services.MBrand;
using DMS.Services.MSupplier;
using DMS.Services.MProductGrouping;
using System.Text;
using DMS.Services.MNotification;
using DMS.Services.MProblemType;
using DMS.Services.MColor;
using DMS.Models;
using GeoCoordinatePortable;
using DMS.Services.MStoreScoutingType;
using DMS.Services.MStoreStatus;
using DMS.Services.MRewardHistory;
using DMS.Services.MLuckyNumber;
using DMS.Helpers;
using System.Dynamic;
using System.Net.Mime;
using GleamTech.DocumentUltimate;

namespace DMS.Rpc.mobile
{
    public partial class MobileController : SimpleController
    {
        private IAlbumService AlbumService;
        private IBrandService BrandService;
        private IBannerService BannerService;
        private IAppUserService AppUserService;
        private IColorService ColorService;
        private IERouteService ERouteService;
        private IIndirectSalesOrderService IndirectSalesOrderService;
        private IItemService ItemService;
        private ILuckyNumberService LuckyNumberService;
        private IStoreService StoreService;
        private IStoreScoutingService StoreScoutingService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreCheckingService StoreCheckingService;
        private IStoreStatusService StoreStatusService;
        private IStoreTypeService StoreTypeService;
        private ITaxTypeService TaxTypeService;
        private IProductService ProductService;
        private IProblemService ProblemService;
        private IProblemTypeService ProblemTypeService;
        private IStoreScoutingTypeService StoreScoutingTypeService;
        private ISurveyService SurveyService;
        private IProvinceService ProvinceService;
        private IDistrictService DistrictService;
        private IWardService WardService;
        private ISupplierService SupplierService;
        private IProductGroupingService ProductGroupingService;
        private INotificationService NotificationService;
        private IRewardHistoryService RewardHistoryService;
        private ICurrentContext CurrentContext;
        private DataContext DataContext;
        public MobileController(
            IAlbumService AlbumService,
            IBannerService BannerService,
            IBrandService BrandService,
            IAppUserService AppUserService,
            IColorService ColorService,
            IERouteService ERouteService,
            IIndirectSalesOrderService IndirectSalesOrderService,
            IItemService ItemService,
            ILuckyNumberService LuckyNumberService,
            IStoreScoutingService StoreScoutingService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreCheckingService StoreCheckingService,
            IStoreStatusService StoreStatusService,
            IStoreTypeService StoreTypeService,
            IProductService ProductService,
            ITaxTypeService TaxTypeService,
            IProblemService ProblemService,
            IProblemTypeService ProblemTypeService,
            IStoreScoutingTypeService StoreScoutingTypeService,
            ISurveyService SurveyService,
            IProvinceService ProvinceService,
            IDistrictService DistrictService,
            IWardService WardService,
            ISupplierService SupplierService,
            IProductGroupingService ProductGroupingService,
            INotificationService NotificationService,
            IRewardHistoryService RewardHistoryService,
            ICurrentContext CurrentContext,
            DataContext DataContext
        )
        {
            this.AlbumService = AlbumService;
            this.BannerService = BannerService;
            this.BrandService = BrandService;
            this.AppUserService = AppUserService;
            this.ColorService = ColorService;
            this.ERouteService = ERouteService;
            this.IndirectSalesOrderService = IndirectSalesOrderService;
            this.ItemService = ItemService;
            this.LuckyNumberService = LuckyNumberService;
            this.StoreService = StoreService;
            this.StoreScoutingService = StoreScoutingService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreCheckingService = StoreCheckingService;
            this.StoreStatusService = StoreStatusService;
            this.StoreTypeService = StoreTypeService;
            this.TaxTypeService = TaxTypeService;
            this.ProductService = ProductService;
            this.ProblemService = ProblemService;
            this.ProblemTypeService = ProblemTypeService;
            this.StoreScoutingTypeService = StoreScoutingTypeService;
            this.SurveyService = SurveyService;
            this.ProvinceService = ProvinceService;
            this.DistrictService = DistrictService;
            this.WardService = WardService;
            this.SupplierService = SupplierService;
            this.ProductGroupingService = ProductGroupingService;
            this.NotificationService = NotificationService;
            this.RewardHistoryService = RewardHistoryService;
            this.CurrentContext = CurrentContext;
            this.DataContext = DataContext;
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
        public async Task<ActionResult<Mobile_StoreCheckingDTO>> GetStoreChecking([FromBody] Mobile_StoreCheckingDTO Mobile_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreChecking StoreChecking = await StoreCheckingService.Get(Mobile_StoreCheckingDTO.Id);
            return new Mobile_StoreCheckingDTO(StoreChecking);
        }

        [Route(MobileRoute.CheckIn), HttpPost]
        public async Task<ActionResult<Mobile_StoreCheckingDTO>> Checkin([FromBody] Mobile_StoreCheckingDTO Mobile_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreChecking StoreChecking = ConvertDTOToEntity(Mobile_StoreCheckingDTO);
            StoreChecking.DeviceName = HttpContext.Request.Headers["X-Device-Model"];
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

            StoreChecking StoreChecking = ConvertDTOToEntity(Mobile_StoreCheckingDTO);
            StoreChecking = await StoreCheckingService.Update(StoreChecking);
            Mobile_StoreCheckingDTO = new Mobile_StoreCheckingDTO(StoreChecking);
            if (StoreChecking.IsValidated)
                return Mobile_StoreCheckingDTO;
            else
                return BadRequest(Mobile_StoreCheckingDTO);
        }

        [Route(MobileRoute.UpdateStoreCheckingImage), HttpPost]
        public async Task<ActionResult<Mobile_StoreCheckingDTO>> UpdateStoreCheckingImage([FromBody] Mobile_StoreCheckingDTO Mobile_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreChecking StoreChecking = ConvertDTOToEntity(Mobile_StoreCheckingDTO);
            StoreChecking = await StoreCheckingService.UpdateStoreCheckingImage(StoreChecking);
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

            IndirectSalesOrder IndirectSalesOrder = ConvertIndirectSalesOrderDTOToEntity(Mobile_IndirectSalesOrderDTO);
            IndirectSalesOrder.BaseLanguage = CurrentContext.Language;
            IndirectSalesOrder.StoreCheckingId = Mobile_IndirectSalesOrderDTO.StoreCheckingId;
            IndirectSalesOrder = await IndirectSalesOrderService.Create(IndirectSalesOrder);
            Mobile_IndirectSalesOrderDTO = new Mobile_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return Mobile_IndirectSalesOrderDTO;
            else
                return BadRequest(Mobile_IndirectSalesOrderDTO);
        }

        [Route(MobileRoute.UpdateIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<Mobile_IndirectSalesOrderDTO>> UpdateIndirectSalesOrder([FromBody] Mobile_IndirectSalesOrderDTO Mobile_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrder IndirectSalesOrder = ConvertIndirectSalesOrderDTOToEntity(Mobile_IndirectSalesOrderDTO);
            IndirectSalesOrder.BaseLanguage = CurrentContext.Language;
            IndirectSalesOrder.StoreCheckingId = Mobile_IndirectSalesOrderDTO.StoreCheckingId;
            IndirectSalesOrder = await IndirectSalesOrderService.Update(IndirectSalesOrder);
            Mobile_IndirectSalesOrderDTO = new Mobile_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return Mobile_IndirectSalesOrderDTO;
            else
                return BadRequest(Mobile_IndirectSalesOrderDTO);
        }

        [Route(MobileRoute.SendIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<Mobile_IndirectSalesOrderDTO>> SendIndirectSalesOrder([FromBody] Mobile_IndirectSalesOrderDTO Mobile_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrder IndirectSalesOrder = ConvertIndirectSalesOrderDTOToEntity(Mobile_IndirectSalesOrderDTO);
            IndirectSalesOrder.BaseLanguage = CurrentContext.Language;
            IndirectSalesOrder.StoreCheckingId = Mobile_IndirectSalesOrderDTO.StoreCheckingId;
            IndirectSalesOrder = await IndirectSalesOrderService.Send(IndirectSalesOrder);
            Mobile_IndirectSalesOrderDTO = new Mobile_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return Mobile_IndirectSalesOrderDTO;
            else
                return BadRequest(Mobile_IndirectSalesOrderDTO);
        }

        [Route(MobileRoute.PreviewIndirectOrder), HttpPost]
        public async Task<ActionResult> PreviewIndirectOrder([FromBody] Mobile_IndirectSalesOrderDTO Mobile_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrder IndirectSalesOrder = ConvertIndirectSalesOrderDTOToEntity(Mobile_IndirectSalesOrderDTO);
            IndirectSalesOrder.BaseLanguage = CurrentContext.Language;
            IndirectSalesOrder.StoreCheckingId = Mobile_IndirectSalesOrderDTO.StoreCheckingId;
            IndirectSalesOrder = await IndirectSalesOrderService.PreviewIndirectOrder(IndirectSalesOrder);

            Mobile_PrintIndirectOrderDTO Mobile_PrintDTO = new Mobile_PrintIndirectOrderDTO(IndirectSalesOrder);
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-EN");
            var STT = 1;
            if (Mobile_PrintDTO.Contents != null)
            {
                foreach (var IndirectSalesOrderContent in Mobile_PrintDTO.Contents)
                {
                    IndirectSalesOrderContent.STT = STT++;
                    IndirectSalesOrderContent.AmountString = IndirectSalesOrderContent.Amount.ToString("N0", culture);
                    IndirectSalesOrderContent.PrimaryPriceString = IndirectSalesOrderContent.PrimaryPrice.ToString("N0", culture);
                    IndirectSalesOrderContent.QuantityString = IndirectSalesOrderContent.Quantity.ToString("N0", culture);
                    IndirectSalesOrderContent.RequestedQuantityString = IndirectSalesOrderContent.RequestedQuantity.ToString("N0", culture);
                    IndirectSalesOrderContent.SalePriceString = IndirectSalesOrderContent.SalePrice.ToString("N0", culture);
                    IndirectSalesOrderContent.DiscountString = IndirectSalesOrderContent.DiscountPercentage.HasValue ? IndirectSalesOrderContent.DiscountPercentage.Value.ToString("N0", culture) + "%" : "";
                    IndirectSalesOrderContent.TaxPercentageString = IndirectSalesOrderContent.TaxPercentage.HasValue ? IndirectSalesOrderContent.TaxPercentage.Value.ToString("N0", culture) + "%" : "";
                }
            }
            if(Mobile_PrintDTO.Promotions != null)
            {
                foreach (var IndirectSalesOrderPromotion in Mobile_PrintDTO.Promotions)
                {
                    IndirectSalesOrderPromotion.STT = STT++;
                    IndirectSalesOrderPromotion.QuantityString = IndirectSalesOrderPromotion.Quantity.ToString("N0", culture);
                    IndirectSalesOrderPromotion.RequestedQuantityString = IndirectSalesOrderPromotion.RequestedQuantity.ToString("N0", culture);
                }
            }

            Mobile_PrintDTO.SubTotalString = Mobile_PrintDTO.SubTotal.ToString("N0", culture);
            Mobile_PrintDTO.Discount = Mobile_PrintDTO.GeneralDiscountAmount.HasValue ? Mobile_PrintDTO.GeneralDiscountAmount.Value.ToString("N0", culture) : "";
            Mobile_PrintDTO.TotalString = Mobile_PrintDTO.Total.ToString("N0", culture);
            Mobile_PrintDTO.TotalText = Utils.ConvertAmountTostring((long)Mobile_PrintDTO.Total);

            string path = "Templates/Print_Indirect_Mobile.docx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            dynamic Data = new ExpandoObject();
            Data.Order = Mobile_PrintDTO;
            MemoryStream MemoryStream = new MemoryStream();
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "docx"))
            {
                document.Process(Data);
            };
            var documentConverter = new DocumentConverter(output, DocumentFormat.Docx);
            documentConverter.ConvertTo(MemoryStream, DocumentFormat.Pdf);

            ContentDisposition cd = new ContentDisposition
            {
                FileName = $"Don-hang-gian-tiep-preview.pdf",
                Inline = true,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return File(output.ToArray(), "application/pdf;charset=utf-8");
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
            {
                Mobile_ProblemDTO = new Mobile_ProblemDTO(Problem);
                return Mobile_ProblemDTO;
            }
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

            Survey Survey = new Survey();
            Survey.Id = Mobile_SurveyDTO.Id;
            Survey.Title = Mobile_SurveyDTO.Title;
            Survey.Description = Mobile_SurveyDTO.Description;
            Survey.RespondentAddress = Mobile_SurveyDTO.RespondentAddress;
            Survey.RespondentEmail = Mobile_SurveyDTO.RespondentEmail;
            Survey.RespondentName = Mobile_SurveyDTO.RespondentName;
            Survey.RespondentPhone = Mobile_SurveyDTO.RespondentPhone;
            Survey.StoreId = Mobile_SurveyDTO.StoreId;
            Survey.StoreScoutingId = Mobile_SurveyDTO.StoreScoutingId;
            Survey.SurveyRespondentTypeId = Mobile_SurveyDTO.SurveyRespondentTypeId;
            Survey.StartAt = Mobile_SurveyDTO.StartAt;
            Survey.EndAt = Mobile_SurveyDTO.EndAt;
            Survey.StatusId = Mobile_SurveyDTO.StatusId;

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
                    TextResult = x.TextResult,
                }).ToList();
            Survey.BaseLanguage = CurrentContext.Language;
            Survey = await SurveyService.SaveForm(Survey);
            Mobile_SurveyDTO = new Mobile_SurveyDTO(Survey);
            if (Survey.IsValidated)
                return Mobile_SurveyDTO;
            else
                return BadRequest(Mobile_SurveyDTO);
        }


        [Route(MobileRoute.GetStoreScouting), HttpPost]
        public async Task<ActionResult<Mobile_StoreScoutingDTO>> GetStoreScouting([FromBody] Mobile_StoreScoutingDTO Mobile_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScouting StoreScouting = await StoreScoutingService.Get(Mobile_StoreScoutingDTO.Id);
            return new Mobile_StoreScoutingDTO(StoreScouting);
        }

        [Route(MobileRoute.CreateStoreScouting), HttpPost]
        public async Task<ActionResult<Mobile_StoreScoutingDTO>> CreateStoreScouting([FromBody] Mobile_StoreScoutingDTO Mobile_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScouting StoreScouting = ConvertStoreScoutingToEntity(Mobile_StoreScoutingDTO);
            StoreScouting = await StoreScoutingService.Create(StoreScouting);
            Mobile_StoreScoutingDTO = new Mobile_StoreScoutingDTO(StoreScouting);
            if (StoreScouting.IsValidated)
                return Mobile_StoreScoutingDTO;
            else
                return BadRequest(Mobile_StoreScoutingDTO);
        }

        [Route(MobileRoute.UpdateStoreScouting), HttpPost]
        public async Task<ActionResult<Mobile_StoreScoutingDTO>> UpdateStoreScouting([FromBody] Mobile_StoreScoutingDTO Mobile_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScouting StoreScouting = ConvertStoreScoutingToEntity(Mobile_StoreScoutingDTO);
            StoreScouting = await StoreScoutingService.Update(StoreScouting);
            Mobile_StoreScoutingDTO = new Mobile_StoreScoutingDTO(StoreScouting);
            if (StoreScouting.IsValidated)
                return Mobile_StoreScoutingDTO;
            else
                return BadRequest(Mobile_StoreScoutingDTO);
        }

        [Route(MobileRoute.DeleteStoreScouting), HttpPost]
        public async Task<ActionResult<Mobile_StoreScoutingDTO>> DeleteStoreScouting([FromBody] Mobile_StoreScoutingDTO Mobile_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScouting StoreScouting = ConvertStoreScoutingToEntity(Mobile_StoreScoutingDTO);
            StoreScouting = await StoreScoutingService.Delete(StoreScouting);
            Mobile_StoreScoutingDTO = new Mobile_StoreScoutingDTO(StoreScouting);
            if (StoreScouting.IsValidated)
                return Mobile_StoreScoutingDTO;
            else
                return BadRequest(Mobile_StoreScoutingDTO);
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
                ThumbnailUrl = Image.ThumbnailUrl,
            };
            return Ok(Mobile_ImageDTO);
        }

        [Route(MobileRoute.SaveImage64), HttpPost]
        public async Task<ActionResult<Mobile_ImageDTO>> SaveImage64([FromBody] Image64DTO Image64DTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            byte[] array = Convert.FromBase64String(Image64DTO.Content);
            Image Image = new Image
            {
                Name = Image64DTO.FileName,
                Content = array,
            };

            Image = await StoreCheckingService.SaveImage(Image);
            if (Image == null)
                return BadRequest();
            Mobile_ImageDTO Mobile_ImageDTO = new Mobile_ImageDTO
            {
                Id = Image.Id,
                Name = Image.Name,
                Url = Image.Url,
                ThumbnailUrl = Image.ThumbnailUrl,
            };
            return Ok(Mobile_ImageDTO);
        }

        [Route(MobileRoute.UpdateAlbum), HttpPost]
        public async Task<ActionResult<Mobile_AlbumDTO>> UpdateAlbum([FromBody] Mobile_AlbumDTO Mobile_AlbumDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Album Album = new Album
            {
                Id = Mobile_AlbumDTO.Id,
                Name = Mobile_AlbumDTO.Name,
                StatusId = Mobile_AlbumDTO.StatusId,
                AlbumImageMappings = Mobile_AlbumDTO.AlbumImageMappings?.Select(x => new AlbumImageMapping
                {
                    AlbumId = x.AlbumId,
                    ImageId = x.ImageId,
                    StoreId = x.StoreId,
                    OrganizationId = x.OrganizationId,
                    SaleEmployeeId = CurrentContext.UserId,
                    ShootingAt = x.ShootingAt,
                }).ToList()
            };

            var StoreIds = Album.AlbumImageMappings.Select(x => x.StoreId).Distinct().ToList();
            var Stores = await StoreService.List(new StoreFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreSelect.Id | StoreSelect.Longitude | StoreSelect.Latitude,
                Id = new IdFilter { In = StoreIds }
            });
            GeoCoordinate sCoord = new GeoCoordinate((double)CurrentContext.Latitude, (double)CurrentContext.Longitude);
            foreach (AlbumImageMapping AlbumImageMapping in Album.AlbumImageMappings)
            {
                Store Store = Stores.Where(x => x.Id == AlbumImageMapping.StoreId).FirstOrDefault();
                if (Store != null)
                {
                    GeoCoordinate eCoord = new GeoCoordinate((double)Store.Latitude, (double)Store.Longitude);
                    AlbumImageMapping.Distance = (long?)sCoord.GetDistanceTo(eCoord);
                }
            }

            Album = await AlbumService.UpdateMobile(Album);
            Mobile_AlbumDTO = new Mobile_AlbumDTO(Album);
            if (!Album.IsValidated)
                return BadRequest(Mobile_AlbumDTO);
            return Ok(Mobile_AlbumDTO);
        }

        [Route(MobileRoute.CreateStore), HttpPost]
        public async Task<ActionResult<Mobile_StoreDTO>> CreateStore([FromBody] Mobile_StoreDTO Mobile_StoreDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var CurrentUser = await AppUserService.Get(CurrentContext.UserId);

            Store Store = new Store()
            {
                CodeDraft = Mobile_StoreDTO.CodeDraft,
                Name = Mobile_StoreDTO.Name,
                OwnerName = Mobile_StoreDTO.OwnerName,
                OwnerPhone = Mobile_StoreDTO.OwnerPhone,
                StoreTypeId = Mobile_StoreDTO.StoreTypeId,
                ProvinceId = Mobile_StoreDTO.ProvinceId,
                DistrictId = Mobile_StoreDTO.DistrictId,
                WardId = Mobile_StoreDTO.WardId,
                Address = Mobile_StoreDTO.Address,
                Latitude = Mobile_StoreDTO.Latitude,
                Longitude = Mobile_StoreDTO.Longitude,
                OrganizationId = CurrentUser.OrganizationId,
                Organization = CurrentUser.Organization,
                StatusId = StatusEnum.ACTIVE.Id,
                StoreType = Mobile_StoreDTO.StoreType == null ? null : new StoreType
                {
                    Id = Mobile_StoreDTO.StoreType.Id,
                    Code = Mobile_StoreDTO.StoreType.Code,
                    Name = Mobile_StoreDTO.StoreType.Name,
                },
                Province = Mobile_StoreDTO.Province == null ? null : new Province
                {
                    Id = Mobile_StoreDTO.Province.Id,
                    Code = Mobile_StoreDTO.Province.Code,
                    Name = Mobile_StoreDTO.Province.Name,
                },
                District = Mobile_StoreDTO.District == null ? null : new District
                {
                    Id = Mobile_StoreDTO.District.Id,
                    Code = Mobile_StoreDTO.District.Code,
                    Name = Mobile_StoreDTO.District.Name,
                },
                Ward = Mobile_StoreDTO.Ward == null ? null : new Ward
                {
                    Id = Mobile_StoreDTO.Ward.Id,
                    Code = Mobile_StoreDTO.Ward.Code,
                    Name = Mobile_StoreDTO.Ward.Name,
                },
                StoreStatusId = StoreStatusEnum.DRAFT.Id,
                StoreImageMappings = Mobile_StoreDTO.StoreImageMappings?.Select(x => new StoreImageMapping
                {
                    ImageId = x.ImageId,
                    StoreId = x.StoreId,
                }).ToList()
            };
            Store.BaseLanguage = CurrentContext.Language;
            Store = await StoreService.Create(Store);
            Mobile_StoreDTO = new Mobile_StoreDTO(Store);
            if (Store.IsValidated)
                return Mobile_StoreDTO;
            else
                return BadRequest(Mobile_StoreDTO);
        }

        [Route(MobileRoute.UpdateStore), HttpPost]
        public async Task<ActionResult<Mobile_StoreDTO>> Update([FromBody] Mobile_StoreDTO Mobile_StoreDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Store Store = await StoreService.Get(Mobile_StoreDTO.Id);
            if (Store == null)
                return NotFound();
            Store.CodeDraft = Mobile_StoreDTO.CodeDraft;
            Store.OwnerPhone = Mobile_StoreDTO.OwnerPhone;
            Store.ProvinceId = Mobile_StoreDTO.ProvinceId;
            Store.DistrictId = Mobile_StoreDTO.DistrictId;
            Store.WardId = Mobile_StoreDTO.WardId;
            Store.Address = Mobile_StoreDTO.Address;
            Store.Latitude = Mobile_StoreDTO.Latitude;
            Store.Longitude = Mobile_StoreDTO.Longitude;
            Store.BaseLanguage = CurrentContext.Language;
            Store = await StoreService.Update(Store);
            Mobile_StoreDTO = new Mobile_StoreDTO(Store);
            if (Store.IsValidated)
                return Mobile_StoreDTO;
            else
                return BadRequest(Mobile_StoreDTO);
        }

        [Route(MobileRoute.GetNotification), HttpPost]
        public async Task<ActionResult<Mobile_NotificationDTO>> GetNotification([FromBody] Mobile_NotificationDTO Mobile_NotificationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Notification Notification = await NotificationService.Get(Mobile_NotificationDTO.Id);
            return new Mobile_NotificationDTO(Notification);
        }

        [Route(MobileRoute.UpdateGPS), HttpPost]
        public async Task<ActionResult<bool>> UpdateGPS([FromBody] Mobile_AppUserDTO Mobile_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser AppUser = new AppUser
            {
                Id = CurrentContext.UserId,
                Longitude = Mobile_AppUserDTO.Longitude,
                Latitude = Mobile_AppUserDTO.Latitude
            };
            AppUser = await AppUserService.UpdateGPS(AppUser);
            Mobile_AppUserDTO = new Mobile_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return true;
            else
                return false;
        }

        [Route(MobileRoute.PrintIndirectOrder), HttpGet]
        public async Task<ActionResult> PrintIndirectOrder([FromQuery] long Id)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            var IndirectSalesOrder = await IndirectSalesOrderService.Get(Id);
            if (IndirectSalesOrder == null)
                return Content("Đơn hàng không tồn tại");
            Mobile_PrintIndirectOrderDTO Mobile_PrintDTO = new Mobile_PrintIndirectOrderDTO(IndirectSalesOrder);
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-EN");
            var STT = 1;
            if (Mobile_PrintDTO.Contents != null)
            {
                foreach (var IndirectSalesOrderContent in Mobile_PrintDTO.Contents)
                {
                    IndirectSalesOrderContent.STT = STT++;
                    IndirectSalesOrderContent.AmountString = IndirectSalesOrderContent.Amount.ToString("N0", culture);
                    IndirectSalesOrderContent.PrimaryPriceString = IndirectSalesOrderContent.PrimaryPrice.ToString("N0", culture);
                    IndirectSalesOrderContent.QuantityString = IndirectSalesOrderContent.Quantity.ToString("N0", culture);
                    IndirectSalesOrderContent.RequestedQuantityString = IndirectSalesOrderContent.RequestedQuantity.ToString("N0", culture);
                    IndirectSalesOrderContent.SalePriceString = IndirectSalesOrderContent.SalePrice.ToString("N0", culture);
                    IndirectSalesOrderContent.DiscountString = IndirectSalesOrderContent.DiscountPercentage.HasValue ? IndirectSalesOrderContent.DiscountPercentage.Value.ToString("N0", culture) + "%" : "";
                    IndirectSalesOrderContent.TaxPercentageString = IndirectSalesOrderContent.TaxPercentage.HasValue ? IndirectSalesOrderContent.TaxPercentage.Value.ToString("N0", culture) + "%" : "";
                }
            }
            if (Mobile_PrintDTO.Promotions != null)
            {
                foreach (var IndirectSalesOrderPromotion in Mobile_PrintDTO.Promotions)
                {
                    IndirectSalesOrderPromotion.STT = STT++;
                    IndirectSalesOrderPromotion.QuantityString = IndirectSalesOrderPromotion.Quantity.ToString("N0", culture);
                    IndirectSalesOrderPromotion.RequestedQuantityString = IndirectSalesOrderPromotion.RequestedQuantity.ToString("N0", culture);
                }
            }

            Mobile_PrintDTO.SubTotalString = Mobile_PrintDTO.SubTotal.ToString("N0", culture);
            Mobile_PrintDTO.Discount = Mobile_PrintDTO.GeneralDiscountAmount.HasValue ? Mobile_PrintDTO.GeneralDiscountAmount.Value.ToString("N0", culture) : "";
            Mobile_PrintDTO.TotalString = Mobile_PrintDTO.Total.ToString("N0", culture);
            Mobile_PrintDTO.TotalText = Utils.ConvertAmountTostring((long)Mobile_PrintDTO.Total);

            string path = "Templates/Print_Indirect_Mobile.docx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            dynamic Data = new ExpandoObject();
            Data.Order = Mobile_PrintDTO;
            MemoryStream MemoryStream = new MemoryStream();
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "docx"))
            {
                document.Process(Data);
            };
            var documentConverter = new DocumentConverter(output, DocumentFormat.Docx);
            documentConverter.ConvertTo(MemoryStream, DocumentFormat.Pdf);

            ContentDisposition cd = new ContentDisposition
            {
                FileName = $"Don-hang-gian-tiep-{IndirectSalesOrder.Code}.pdf",
                Inline = true,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return File(MemoryStream.ToArray(), "application/pdf;charset=utf-8");
        }

        private StoreChecking ConvertDTOToEntity(Mobile_StoreCheckingDTO Mobile_StoreCheckingDTO)
        {
            StoreChecking StoreChecking = new StoreChecking();
            StoreChecking.Id = Mobile_StoreCheckingDTO.Id;
            StoreChecking.StoreId = Mobile_StoreCheckingDTO.StoreId;
            StoreChecking.SaleEmployeeId = Mobile_StoreCheckingDTO.SaleEmployeeId;
            StoreChecking.Longitude = Mobile_StoreCheckingDTO.Longitude;
            StoreChecking.Latitude = Mobile_StoreCheckingDTO.Latitude;
            StoreChecking.CheckOutLongitude = Mobile_StoreCheckingDTO.CheckOutLongitude;
            StoreChecking.CheckOutLatitude = Mobile_StoreCheckingDTO.CheckOutLatitude;
            StoreChecking.CheckInAt = Mobile_StoreCheckingDTO.CheckInAt;
            StoreChecking.CheckOutAt = Mobile_StoreCheckingDTO.CheckOutAt;
            StoreChecking.CheckInDistance = Mobile_StoreCheckingDTO.CheckInDistance;
            StoreChecking.CheckOutDistance = Mobile_StoreCheckingDTO.CheckOutDistance;
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
                    Distance = x.Distance,
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
                        ThumbnailUrl = x.Image.ThumbnailUrl,
                    },
                    Store = x.Store == null ? null : new Store
                    {
                        Id = x.Store.Id,
                        CodeDraft = x.Store.CodeDraft,
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

        private StoreScouting ConvertStoreScoutingToEntity(Mobile_StoreScoutingDTO Mobile_StoreScoutingDTO)
        {
            StoreScouting StoreScouting = new StoreScouting();
            StoreScouting.Id = Mobile_StoreScoutingDTO.Id;
            StoreScouting.Code = Mobile_StoreScoutingDTO.Code;
            StoreScouting.Name = Mobile_StoreScoutingDTO.Name;
            StoreScouting.OwnerPhone = Mobile_StoreScoutingDTO.OwnerPhone;
            StoreScouting.ProvinceId = Mobile_StoreScoutingDTO.ProvinceId;
            StoreScouting.DistrictId = Mobile_StoreScoutingDTO.DistrictId;
            StoreScouting.WardId = Mobile_StoreScoutingDTO.WardId;
            StoreScouting.Address = Mobile_StoreScoutingDTO.Address;
            StoreScouting.Latitude = Mobile_StoreScoutingDTO.Latitude;
            StoreScouting.Longitude = Mobile_StoreScoutingDTO.Longitude;
            StoreScouting.CreatorId = Mobile_StoreScoutingDTO.CreatorId;
            StoreScouting.StoreScoutingStatusId = Mobile_StoreScoutingDTO.StoreScoutingStatusId;
            StoreScouting.StoreScoutingTypeId = Mobile_StoreScoutingDTO.StoreScoutingTypeId;
            StoreScouting.Creator = Mobile_StoreScoutingDTO.Creator == null ? null : new AppUser
            {
                Id = Mobile_StoreScoutingDTO.Creator.Id,
                Username = Mobile_StoreScoutingDTO.Creator.Username,
                DisplayName = Mobile_StoreScoutingDTO.Creator.DisplayName,
                Address = Mobile_StoreScoutingDTO.Creator.Address,
                Email = Mobile_StoreScoutingDTO.Creator.Email,
                Phone = Mobile_StoreScoutingDTO.Creator.Phone,
                PositionId = Mobile_StoreScoutingDTO.Creator.PositionId,
                Department = Mobile_StoreScoutingDTO.Creator.Department,
                OrganizationId = Mobile_StoreScoutingDTO.Creator.OrganizationId,
                StatusId = Mobile_StoreScoutingDTO.Creator.StatusId,
                Avatar = Mobile_StoreScoutingDTO.Creator.Avatar,
                ProvinceId = Mobile_StoreScoutingDTO.Creator.ProvinceId,
                SexId = Mobile_StoreScoutingDTO.Creator.SexId,
                Birthday = Mobile_StoreScoutingDTO.Creator.Birthday,
            };
            StoreScouting.Organization = Mobile_StoreScoutingDTO.Organization == null ? null : new Organization
            {
                Id = Mobile_StoreScoutingDTO.Organization.Id,
                Code = Mobile_StoreScoutingDTO.Organization.Code,
                Name = Mobile_StoreScoutingDTO.Organization.Name,
                ParentId = Mobile_StoreScoutingDTO.Organization.ParentId,
                Path = Mobile_StoreScoutingDTO.Organization.Path,
                Level = Mobile_StoreScoutingDTO.Organization.Level,
                StatusId = Mobile_StoreScoutingDTO.Organization.StatusId,
                Phone = Mobile_StoreScoutingDTO.Organization.Phone,
                Address = Mobile_StoreScoutingDTO.Organization.Address,
                Email = Mobile_StoreScoutingDTO.Organization.Email,
            };
            StoreScouting.District = Mobile_StoreScoutingDTO.District == null ? null : new District
            {
                Id = Mobile_StoreScoutingDTO.District.Id,
                Code = Mobile_StoreScoutingDTO.District.Code,
                Name = Mobile_StoreScoutingDTO.District.Name,
                Priority = Mobile_StoreScoutingDTO.District.Priority,
                ProvinceId = Mobile_StoreScoutingDTO.District.ProvinceId,
                StatusId = Mobile_StoreScoutingDTO.District.StatusId,
            };
            StoreScouting.Province = Mobile_StoreScoutingDTO.Province == null ? null : new Province
            {
                Id = Mobile_StoreScoutingDTO.Province.Id,
                Code = Mobile_StoreScoutingDTO.Province.Code,
                Name = Mobile_StoreScoutingDTO.Province.Name,
                Priority = Mobile_StoreScoutingDTO.Province.Priority,
                StatusId = Mobile_StoreScoutingDTO.Province.StatusId,
            };
            StoreScouting.StoreScoutingStatus = Mobile_StoreScoutingDTO.StoreScoutingStatus == null ? null : new StoreScoutingStatus
            {
                Id = Mobile_StoreScoutingDTO.StoreScoutingStatus.Id,
                Code = Mobile_StoreScoutingDTO.StoreScoutingStatus.Code,
                Name = Mobile_StoreScoutingDTO.StoreScoutingStatus.Name,
            };
            StoreScouting.StoreScoutingType = Mobile_StoreScoutingDTO.StoreScoutingType == null ? null : new StoreScoutingType
            {
                Id = Mobile_StoreScoutingDTO.StoreScoutingType.Id,
                Code = Mobile_StoreScoutingDTO.StoreScoutingType.Code,
                Name = Mobile_StoreScoutingDTO.StoreScoutingType.Name,
            };
            StoreScouting.Ward = Mobile_StoreScoutingDTO.Ward == null ? null : new Ward
            {
                Id = Mobile_StoreScoutingDTO.Ward.Id,
                Code = Mobile_StoreScoutingDTO.Ward.Code,
                Name = Mobile_StoreScoutingDTO.Ward.Name,
                Priority = Mobile_StoreScoutingDTO.Ward.Priority,
                DistrictId = Mobile_StoreScoutingDTO.Ward.DistrictId,
                StatusId = Mobile_StoreScoutingDTO.Ward.StatusId,
            };
            StoreScouting.StoreScoutingImageMappings = Mobile_StoreScoutingDTO.StoreScoutingImageMappings?.Select(x => new StoreScoutingImageMapping
            {
                StoreScoutingId = x.StoreScoutingId,
                ImageId = x.ImageId,
                Image = x.Image == null ? null : new Image
                {
                    Id = x.Image.Id,
                    Name = x.Image.Name,
                    Url = x.Image.Url,
                    ThumbnailUrl = x.Image.ThumbnailUrl,
                }
            }).ToList();
            StoreScouting.BaseLanguage = CurrentContext.Language;
            return StoreScouting;
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

        private IndirectSalesOrder ConvertIndirectSalesOrderDTOToEntity(Mobile_IndirectSalesOrderDTO Mobile_IndirectSalesOrderDTO)
        {
            IndirectSalesOrder IndirectSalesOrder = new IndirectSalesOrder();
            IndirectSalesOrder.Id = Mobile_IndirectSalesOrderDTO.Id;
            IndirectSalesOrder.Code = Mobile_IndirectSalesOrderDTO.Code;
            IndirectSalesOrder.BuyerStoreId = Mobile_IndirectSalesOrderDTO.BuyerStoreId;
            IndirectSalesOrder.PhoneNumber = Mobile_IndirectSalesOrderDTO.PhoneNumber;
            IndirectSalesOrder.StoreAddress = Mobile_IndirectSalesOrderDTO.StoreAddress;
            IndirectSalesOrder.DeliveryAddress = Mobile_IndirectSalesOrderDTO.DeliveryAddress;
            IndirectSalesOrder.SellerStoreId = Mobile_IndirectSalesOrderDTO.SellerStoreId;
            IndirectSalesOrder.SaleEmployeeId = Mobile_IndirectSalesOrderDTO.SaleEmployeeId;
            IndirectSalesOrder.OrganizationId = Mobile_IndirectSalesOrderDTO.OrganizationId;
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
            IndirectSalesOrder.Organization = Mobile_IndirectSalesOrderDTO.Organization == null ? null : new Organization
            {
                Id = Mobile_IndirectSalesOrderDTO.Organization.Id,
                Code = Mobile_IndirectSalesOrderDTO.Organization.Code,
                Name = Mobile_IndirectSalesOrderDTO.Organization.Name,
                ParentId = Mobile_IndirectSalesOrderDTO.Organization.ParentId,
                Path = Mobile_IndirectSalesOrderDTO.Organization.Path,
                Level = Mobile_IndirectSalesOrderDTO.Organization.Level,
                StatusId = Mobile_IndirectSalesOrderDTO.Organization.StatusId,
                Phone = Mobile_IndirectSalesOrderDTO.Organization.Phone,
                Address = Mobile_IndirectSalesOrderDTO.Organization.Address,
                Email = Mobile_IndirectSalesOrderDTO.Organization.Email,
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
            return IndirectSalesOrder;
        }

    }

    public class Image64DTO : DataDTO
    {
        public string FileName { get; set; }
        public string Content { get; set; }
    }
}

