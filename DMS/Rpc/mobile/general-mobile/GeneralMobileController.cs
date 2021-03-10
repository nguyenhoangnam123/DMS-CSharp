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
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace DMS.Rpc.mobile.general_mobile
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

        [Route(GeneralMobileRoute.CountStoreChecking), HttpPost]
        public async Task<ActionResult<int>> CountStoreChecking([FromBody] GeneralMobile_StoreCheckingFilterDTO GeneralMobile_StoreCheckingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreCheckingFilter StoreCheckingFilter = ConvertFilterDTOToFilterEntity(GeneralMobile_StoreCheckingFilterDTO);
            StoreCheckingFilter = StoreCheckingService.ToFilter(StoreCheckingFilter);
            int count = await StoreCheckingService.Count(StoreCheckingFilter);
            return count;
        }

        [Route(GeneralMobileRoute.ListStoreChecking), HttpPost]
        public async Task<ActionResult<List<GeneralMobile_StoreCheckingDTO>>> ListStoreChecking([FromBody] GeneralMobile_StoreCheckingFilterDTO GeneralMobile_StoreCheckingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreCheckingFilter StoreCheckingFilter = ConvertFilterDTOToFilterEntity(GeneralMobile_StoreCheckingFilterDTO);
            StoreCheckingFilter = StoreCheckingService.ToFilter(StoreCheckingFilter);
            List<StoreChecking> StoreCheckings = await StoreCheckingService.List(StoreCheckingFilter);
            List<GeneralMobile_StoreCheckingDTO> GeneralMobile_StoreCheckingDTOs = StoreCheckings
                .Select(c => new GeneralMobile_StoreCheckingDTO(c)).ToList();
            return GeneralMobile_StoreCheckingDTOs;
        }

        [Route(GeneralMobileRoute.GetStoreChecking), HttpPost]
        public async Task<ActionResult<GeneralMobile_StoreCheckingDTO>> GetStoreChecking([FromBody] GeneralMobile_StoreCheckingDTO GeneralMobile_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreChecking StoreChecking = await StoreCheckingService.Get(GeneralMobile_StoreCheckingDTO.Id);
            return new GeneralMobile_StoreCheckingDTO(StoreChecking);
        }

        [Route(GeneralMobileRoute.CheckIn), HttpPost]
        public async Task<ActionResult<GeneralMobile_StoreCheckingDTO>> Checkin([FromBody] GeneralMobile_StoreCheckingDTO GeneralMobile_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreChecking StoreChecking = ConvertDTOToEntity(GeneralMobile_StoreCheckingDTO);
            StoreChecking.DeviceName = HttpContext.Request.Headers["X-Device-Model"];
            StoreChecking = await StoreCheckingService.CheckIn(StoreChecking);
            StoreChecking.IsOpenedStore = true;
            GeneralMobile_StoreCheckingDTO = new GeneralMobile_StoreCheckingDTO(StoreChecking);
            if (StoreChecking.IsValidated)
                return GeneralMobile_StoreCheckingDTO;
            else
                return BadRequest(GeneralMobile_StoreCheckingDTO);
        }

        [Route(GeneralMobileRoute.UpdateStoreChecking), HttpPost]
        public async Task<ActionResult<GeneralMobile_StoreCheckingDTO>> Update([FromBody] GeneralMobile_StoreCheckingDTO GeneralMobile_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreChecking StoreChecking = ConvertDTOToEntity(GeneralMobile_StoreCheckingDTO);
            StoreChecking = await StoreCheckingService.Update(StoreChecking);
            GeneralMobile_StoreCheckingDTO = new GeneralMobile_StoreCheckingDTO(StoreChecking);
            if (StoreChecking.IsValidated)
                return GeneralMobile_StoreCheckingDTO;
            else
                return BadRequest(GeneralMobile_StoreCheckingDTO);
        }

        [Route(GeneralMobileRoute.UpdateStoreCheckingImage), HttpPost]
        public async Task<ActionResult<GeneralMobile_StoreCheckingDTO>> UpdateStoreCheckingImage([FromBody] GeneralMobile_StoreCheckingDTO GeneralMobile_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreChecking StoreChecking = ConvertDTOToEntity(GeneralMobile_StoreCheckingDTO);
            StoreChecking = await StoreCheckingService.UpdateStoreCheckingImage(StoreChecking);
            GeneralMobile_StoreCheckingDTO = new GeneralMobile_StoreCheckingDTO(StoreChecking);
            if (StoreChecking.IsValidated)
                return GeneralMobile_StoreCheckingDTO;
            else
                return BadRequest(GeneralMobile_StoreCheckingDTO);
        }

        [Route(GeneralMobileRoute.CheckOut), HttpPost]
        public async Task<ActionResult<GeneralMobile_StoreCheckingDTO>> CheckOut([FromBody] GeneralMobile_StoreCheckingDTO GeneralMobile_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreChecking StoreChecking = ConvertDTOToEntity(GeneralMobile_StoreCheckingDTO);
            StoreChecking = await StoreCheckingService.CheckOut(StoreChecking);
            GeneralMobile_StoreCheckingDTO = new GeneralMobile_StoreCheckingDTO(StoreChecking);
            if (StoreChecking.IsValidated)
                return GeneralMobile_StoreCheckingDTO;
            else
                return BadRequest(GeneralMobile_StoreCheckingDTO);
        }


        [Route(GeneralMobileRoute.CreateIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<GeneralMobile_IndirectSalesOrderDTO>> CreateIndirectSalesOrder([FromBody] GeneralMobile_IndirectSalesOrderDTO GeneralMobile_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrder IndirectSalesOrder = ConvertIndirectSalesOrderDTOToEntity(GeneralMobile_IndirectSalesOrderDTO);
            IndirectSalesOrder.BaseLanguage = CurrentContext.Language;
            IndirectSalesOrder.StoreCheckingId = GeneralMobile_IndirectSalesOrderDTO.StoreCheckingId;
            IndirectSalesOrder.SaleEmployeeId = CurrentContext.UserId;
            IndirectSalesOrder = await IndirectSalesOrderService.Create(IndirectSalesOrder);
            GeneralMobile_IndirectSalesOrderDTO = new GeneralMobile_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return GeneralMobile_IndirectSalesOrderDTO;
            else
                return BadRequest(GeneralMobile_IndirectSalesOrderDTO);
        }

        [Route(GeneralMobileRoute.UpdateIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<GeneralMobile_IndirectSalesOrderDTO>> UpdateIndirectSalesOrder([FromBody] GeneralMobile_IndirectSalesOrderDTO GeneralMobile_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrder IndirectSalesOrder = ConvertIndirectSalesOrderDTOToEntity(GeneralMobile_IndirectSalesOrderDTO);
            IndirectSalesOrder.BaseLanguage = CurrentContext.Language;
            IndirectSalesOrder.StoreCheckingId = GeneralMobile_IndirectSalesOrderDTO.StoreCheckingId;
            IndirectSalesOrder.SaleEmployeeId = CurrentContext.UserId;
            IndirectSalesOrder = await IndirectSalesOrderService.Update(IndirectSalesOrder);
            GeneralMobile_IndirectSalesOrderDTO = new GeneralMobile_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return GeneralMobile_IndirectSalesOrderDTO;
            else
                return BadRequest(GeneralMobile_IndirectSalesOrderDTO);
        }

        [Route(GeneralMobileRoute.SendIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<GeneralMobile_IndirectSalesOrderDTO>> SendIndirectSalesOrder([FromBody] GeneralMobile_IndirectSalesOrderDTO GeneralMobile_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrder IndirectSalesOrder = ConvertIndirectSalesOrderDTOToEntity(GeneralMobile_IndirectSalesOrderDTO);
            IndirectSalesOrder.BaseLanguage = CurrentContext.Language;
            IndirectSalesOrder.StoreCheckingId = GeneralMobile_IndirectSalesOrderDTO.StoreCheckingId;
            IndirectSalesOrder = await IndirectSalesOrderService.Send(IndirectSalesOrder);
            GeneralMobile_IndirectSalesOrderDTO = new GeneralMobile_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return GeneralMobile_IndirectSalesOrderDTO;
            else
                return BadRequest(GeneralMobile_IndirectSalesOrderDTO);
        }

        [Route(GeneralMobileRoute.PreviewIndirectOrder), HttpPost]
        public async Task<ActionResult> PreviewIndirectOrder([FromForm] string data)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (string.IsNullOrWhiteSpace(data))
                return BadRequest();

            GeneralMobile_IndirectSalesOrderDTO GeneralMobile_IndirectSalesOrderDTO = JsonConvert.DeserializeObject<GeneralMobile_IndirectSalesOrderDTO>(data);
            IndirectSalesOrder IndirectSalesOrder = ConvertIndirectSalesOrderDTOToEntity(GeneralMobile_IndirectSalesOrderDTO);
            IndirectSalesOrder.BaseLanguage = CurrentContext.Language;
            IndirectSalesOrder.StoreCheckingId = GeneralMobile_IndirectSalesOrderDTO.StoreCheckingId;
            IndirectSalesOrder = await IndirectSalesOrderService.PreviewIndirectOrder(IndirectSalesOrder);

            GeneralMobile_PrintIndirectOrderDTO GeneralMobile_PrintDTO = new GeneralMobile_PrintIndirectOrderDTO(IndirectSalesOrder);
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-EN");
            var STT = 1;
            if (GeneralMobile_PrintDTO.Contents != null)
            {
                foreach (var IndirectSalesOrderContent in GeneralMobile_PrintDTO.Contents)
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
            if (GeneralMobile_PrintDTO.Promotions != null)
            {
                foreach (var IndirectSalesOrderPromotion in GeneralMobile_PrintDTO.Promotions)
                {
                    IndirectSalesOrderPromotion.STT = STT++;
                    IndirectSalesOrderPromotion.QuantityString = IndirectSalesOrderPromotion.Quantity.ToString("N0", culture);
                    IndirectSalesOrderPromotion.RequestedQuantityString = IndirectSalesOrderPromotion.RequestedQuantity.ToString("N0", culture);
                }
            }

            GeneralMobile_PrintDTO.SubTotalString = GeneralMobile_PrintDTO.SubTotal.ToString("N0", culture);
            GeneralMobile_PrintDTO.Discount = GeneralMobile_PrintDTO.GeneralDiscountAmount.HasValue ? GeneralMobile_PrintDTO.GeneralDiscountAmount.Value.ToString("N0", culture) : "";
            GeneralMobile_PrintDTO.TotalString = GeneralMobile_PrintDTO.Total.ToString("N0", culture);
            GeneralMobile_PrintDTO.TotalText = Utils.ConvertAmountTostring((long)GeneralMobile_PrintDTO.Total);

            string path = "Templates/Print_Indirect_Mobile.docx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            dynamic Data = new ExpandoObject();
            Data.Order = GeneralMobile_PrintDTO;
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
            return File(MemoryStream.ToArray(), "application/pdf;charset=utf-8");
        }

        [Route(GeneralMobileRoute.CreateProblem), HttpPost]
        public async Task<ActionResult<GeneralMobile_ProblemDTO>> CreateProblem([FromBody] GeneralMobile_ProblemDTO GeneralMobile_ProblemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Problem Problem = new Problem
            {
                Id = GeneralMobile_ProblemDTO.Id,
                Content = GeneralMobile_ProblemDTO.Content,
                NoteAt = GeneralMobile_ProblemDTO.NoteAt,
                CompletedAt = GeneralMobile_ProblemDTO.CompletedAt,
                ProblemTypeId = GeneralMobile_ProblemDTO.ProblemTypeId,
                ProblemStatusId = GeneralMobile_ProblemDTO.ProblemStatusId,
                StoreCheckingId = GeneralMobile_ProblemDTO.StoreCheckingId,
                CreatorId = GeneralMobile_ProblemDTO.CreatorId,
                StoreId = GeneralMobile_ProblemDTO.StoreId,
                ProblemImageMappings = GeneralMobile_ProblemDTO.ProblemImageMappings?.Select(x => new ProblemImageMapping
                {
                    ImageId = x.ImageId,
                    ProblemId = x.ProblemId
                }).ToList()
            };
            Problem = await ProblemService.Create(Problem);
            if (Problem.IsValidated)
            {
                GeneralMobile_ProblemDTO = new GeneralMobile_ProblemDTO(Problem);
                return GeneralMobile_ProblemDTO;
            }
            else
                return BadRequest(GeneralMobile_ProblemDTO);
        }

        [Route(GeneralMobileRoute.GetSurveyForm), HttpPost]
        public async Task<ActionResult<GeneralMobile_SurveyDTO>> GetSurveyForm([FromBody] GeneralMobile_SurveyDTO GeneralMobile_SurveyDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            //if (!await HasPermission(GeneralMobile_SurveyDTO.Id))
            //    return Forbid();

            Survey Survey = await SurveyService.GetForm(GeneralMobile_SurveyDTO.Id);
            GeneralMobile_SurveyDTO = new GeneralMobile_SurveyDTO(Survey);
            if (Survey.IsValidated)
                return GeneralMobile_SurveyDTO;
            else
                return BadRequest(GeneralMobile_SurveyDTO);
        }

        [Route(GeneralMobileRoute.SaveSurveyForm), HttpPost]
        public async Task<ActionResult<GeneralMobile_SurveyDTO>> SaveSurveyForm([FromBody] GeneralMobile_SurveyDTO GeneralMobile_SurveyDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Survey Survey = new Survey();
            Survey.Id = GeneralMobile_SurveyDTO.Id;
            Survey.Title = GeneralMobile_SurveyDTO.Title;
            Survey.Description = GeneralMobile_SurveyDTO.Description;
            Survey.RespondentAddress = GeneralMobile_SurveyDTO.RespondentAddress;
            Survey.RespondentEmail = GeneralMobile_SurveyDTO.RespondentEmail;
            Survey.RespondentName = GeneralMobile_SurveyDTO.RespondentName;
            Survey.RespondentPhone = GeneralMobile_SurveyDTO.RespondentPhone;
            Survey.StoreId = GeneralMobile_SurveyDTO.StoreId;
            Survey.StoreScoutingId = GeneralMobile_SurveyDTO.StoreScoutingId;
            Survey.SurveyRespondentTypeId = GeneralMobile_SurveyDTO.SurveyRespondentTypeId;
            Survey.StartAt = GeneralMobile_SurveyDTO.StartAt;
            Survey.EndAt = GeneralMobile_SurveyDTO.EndAt;
            Survey.StatusId = GeneralMobile_SurveyDTO.StatusId;

            Survey.SurveyQuestions = GeneralMobile_SurveyDTO.SurveyQuestions?
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
            GeneralMobile_SurveyDTO = new GeneralMobile_SurveyDTO(Survey);
            if (Survey.IsValidated)
                return GeneralMobile_SurveyDTO;
            else
                return BadRequest(GeneralMobile_SurveyDTO);
        }


        [Route(GeneralMobileRoute.GetStoreScouting), HttpPost]
        public async Task<ActionResult<GeneralMobile_StoreScoutingDTO>> GetStoreScouting([FromBody] GeneralMobile_StoreScoutingDTO GeneralMobile_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScouting StoreScouting = await StoreScoutingService.Get(GeneralMobile_StoreScoutingDTO.Id);
            return new GeneralMobile_StoreScoutingDTO(StoreScouting);
        }

        [Route(GeneralMobileRoute.CreateStoreScouting), HttpPost]
        public async Task<ActionResult<GeneralMobile_StoreScoutingDTO>> CreateStoreScouting([FromBody] GeneralMobile_StoreScoutingDTO GeneralMobile_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScouting StoreScouting = ConvertStoreScoutingToEntity(GeneralMobile_StoreScoutingDTO);
            StoreScouting = await StoreScoutingService.Create(StoreScouting);
            GeneralMobile_StoreScoutingDTO = new GeneralMobile_StoreScoutingDTO(StoreScouting);
            if (StoreScouting.IsValidated)
                return GeneralMobile_StoreScoutingDTO;
            else
                return BadRequest(GeneralMobile_StoreScoutingDTO);
        }

        [Route(GeneralMobileRoute.UpdateStoreScouting), HttpPost]
        public async Task<ActionResult<GeneralMobile_StoreScoutingDTO>> UpdateStoreScouting([FromBody] GeneralMobile_StoreScoutingDTO GeneralMobile_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScouting StoreScouting = ConvertStoreScoutingToEntity(GeneralMobile_StoreScoutingDTO);
            StoreScouting = await StoreScoutingService.Update(StoreScouting);
            GeneralMobile_StoreScoutingDTO = new GeneralMobile_StoreScoutingDTO(StoreScouting);
            if (StoreScouting.IsValidated)
                return GeneralMobile_StoreScoutingDTO;
            else
                return BadRequest(GeneralMobile_StoreScoutingDTO);
        }

        [Route(GeneralMobileRoute.DeleteStoreScouting), HttpPost]
        public async Task<ActionResult<GeneralMobile_StoreScoutingDTO>> DeleteStoreScouting([FromBody] GeneralMobile_StoreScoutingDTO GeneralMobile_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScouting StoreScouting = ConvertStoreScoutingToEntity(GeneralMobile_StoreScoutingDTO);
            StoreScouting = await StoreScoutingService.Delete(StoreScouting);
            GeneralMobile_StoreScoutingDTO = new GeneralMobile_StoreScoutingDTO(StoreScouting);
            if (StoreScouting.IsValidated)
                return GeneralMobile_StoreScoutingDTO;
            else
                return BadRequest(GeneralMobile_StoreScoutingDTO);
        }

        [Route(GeneralMobileRoute.SaveImage), HttpPost]
        public async Task<ActionResult<GeneralMobile_ImageDTO>> SaveImage(IFormFile file)
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
            GeneralMobile_ImageDTO GeneralMobile_ImageDTO = new GeneralMobile_ImageDTO
            {
                Id = Image.Id,
                Name = Image.Name,
                Url = Image.Url,
                ThumbnailUrl = Image.ThumbnailUrl,
            };
            return Ok(GeneralMobile_ImageDTO);
        }

        [Route(GeneralMobileRoute.SaveImage64), HttpPost]
        public async Task<ActionResult<GeneralMobile_ImageDTO>> SaveImage64([FromBody] Image64DTO Image64DTO)
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
            GeneralMobile_ImageDTO GeneralMobile_ImageDTO = new GeneralMobile_ImageDTO
            {
                Id = Image.Id,
                Name = Image.Name,
                Url = Image.Url,
                ThumbnailUrl = Image.ThumbnailUrl,
            };
            return Ok(GeneralMobile_ImageDTO);
        }

        [Route(GeneralMobileRoute.UpdateAlbum), HttpPost]
        public async Task<ActionResult<GeneralMobile_AlbumDTO>> UpdateAlbum([FromBody] GeneralMobile_AlbumDTO GeneralMobile_AlbumDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Album Album = new Album
            {
                Id = GeneralMobile_AlbumDTO.Id,
                Name = GeneralMobile_AlbumDTO.Name,
                StatusId = GeneralMobile_AlbumDTO.StatusId,
                AlbumImageMappings = GeneralMobile_AlbumDTO.AlbumImageMappings?.Select(x => new AlbumImageMapping
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
            GeneralMobile_AlbumDTO = new GeneralMobile_AlbumDTO(Album);
            if (!Album.IsValidated)
                return BadRequest(GeneralMobile_AlbumDTO);
            return Ok(GeneralMobile_AlbumDTO);
        }

        [Route(GeneralMobileRoute.CreateStore), HttpPost]
        public async Task<ActionResult<GeneralMobile_StoreDTO>> CreateStore([FromBody] GeneralMobile_StoreDTO GeneralMobile_StoreDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var CurrentUser = await AppUserService.Get(CurrentContext.UserId);

            Store Store = new Store()
            {
                CodeDraft = GeneralMobile_StoreDTO.CodeDraft,
                Name = GeneralMobile_StoreDTO.Name,
                OwnerName = GeneralMobile_StoreDTO.OwnerName,
                OwnerPhone = GeneralMobile_StoreDTO.OwnerPhone,
                StoreTypeId = GeneralMobile_StoreDTO.StoreTypeId,
                ProvinceId = GeneralMobile_StoreDTO.ProvinceId,
                DistrictId = GeneralMobile_StoreDTO.DistrictId,
                WardId = GeneralMobile_StoreDTO.WardId,
                Address = GeneralMobile_StoreDTO.Address,
                DeliveryAddress = GeneralMobile_StoreDTO.Address,
                Latitude = GeneralMobile_StoreDTO.Latitude,
                Longitude = GeneralMobile_StoreDTO.Longitude,
                DeliveryLatitude = GeneralMobile_StoreDTO.Latitude,
                DeliveryLongitude = GeneralMobile_StoreDTO.Longitude,
                OrganizationId = CurrentUser.OrganizationId,
                Organization = CurrentUser.Organization,
                Telephone = GeneralMobile_StoreDTO.OwnerPhone,
                StatusId = StatusEnum.ACTIVE.Id,
                StoreType = GeneralMobile_StoreDTO.StoreType == null ? null : new StoreType
                {
                    Id = GeneralMobile_StoreDTO.StoreType.Id,
                    Code = GeneralMobile_StoreDTO.StoreType.Code,
                    Name = GeneralMobile_StoreDTO.StoreType.Name,
                },
                Province = GeneralMobile_StoreDTO.Province == null ? null : new Province
                {
                    Id = GeneralMobile_StoreDTO.Province.Id,
                    Code = GeneralMobile_StoreDTO.Province.Code,
                    Name = GeneralMobile_StoreDTO.Province.Name,
                },
                District = GeneralMobile_StoreDTO.District == null ? null : new District
                {
                    Id = GeneralMobile_StoreDTO.District.Id,
                    Code = GeneralMobile_StoreDTO.District.Code,
                    Name = GeneralMobile_StoreDTO.District.Name,
                },
                Ward = GeneralMobile_StoreDTO.Ward == null ? null : new Ward
                {
                    Id = GeneralMobile_StoreDTO.Ward.Id,
                    Code = GeneralMobile_StoreDTO.Ward.Code,
                    Name = GeneralMobile_StoreDTO.Ward.Name,
                },
                StoreStatusId = StoreStatusEnum.DRAFT.Id,
                StoreImageMappings = GeneralMobile_StoreDTO.StoreImageMappings?.Select(x => new StoreImageMapping
                {
                    ImageId = x.ImageId,
                    StoreId = x.StoreId,
                }).ToList()
            };
            Store.BaseLanguage = CurrentContext.Language;
            Store.AppUserId = CurrentContext.UserId;
            Store = await StoreService.Create(Store);
            GeneralMobile_StoreDTO = new GeneralMobile_StoreDTO(Store);
            if (Store.IsValidated)
                return GeneralMobile_StoreDTO;
            else
                return BadRequest(GeneralMobile_StoreDTO);
        }

        [Route(GeneralMobileRoute.UpdateStore), HttpPost]
        public async Task<ActionResult<GeneralMobile_StoreDTO>> Update([FromBody] GeneralMobile_StoreDTO GeneralMobile_StoreDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Store Store = await StoreService.Get(GeneralMobile_StoreDTO.Id);
            if (Store == null)
                return NotFound();
            Store.CodeDraft = GeneralMobile_StoreDTO.CodeDraft;
            Store.OwnerPhone = GeneralMobile_StoreDTO.OwnerPhone;
            Store.ProvinceId = GeneralMobile_StoreDTO.ProvinceId;
            Store.DistrictId = GeneralMobile_StoreDTO.DistrictId;
            Store.WardId = GeneralMobile_StoreDTO.WardId;
            Store.Address = GeneralMobile_StoreDTO.Address;
            Store.Latitude = GeneralMobile_StoreDTO.Latitude;
            Store.Longitude = GeneralMobile_StoreDTO.Longitude;
            Store.BaseLanguage = CurrentContext.Language;
            Store = await StoreService.Update(Store);
            GeneralMobile_StoreDTO = new GeneralMobile_StoreDTO(Store);
            if (Store.IsValidated)
                return GeneralMobile_StoreDTO;
            else
                return BadRequest(GeneralMobile_StoreDTO);
        }

        [Route(GeneralMobileRoute.GetNotification), HttpPost]
        public async Task<ActionResult<GeneralMobile_NotificationDTO>> GetNotification([FromBody] GeneralMobile_NotificationDTO GeneralMobile_NotificationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Notification Notification = await NotificationService.Get(GeneralMobile_NotificationDTO.Id);
            return new GeneralMobile_NotificationDTO(Notification);
        }

        [Route(GeneralMobileRoute.UpdateGPS), HttpPost]
        public async Task<ActionResult<bool>> UpdateGPS([FromBody] GeneralMobile_AppUserDTO GeneralMobile_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser AppUser = new AppUser
            {
                Id = CurrentContext.UserId,
                Longitude = GeneralMobile_AppUserDTO.Longitude,
                Latitude = GeneralMobile_AppUserDTO.Latitude
            };
            AppUser = await AppUserService.UpdateGPS(AppUser);
            GeneralMobile_AppUserDTO = new GeneralMobile_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return true;
            else
                return false;
        }

        [Route(GeneralMobileRoute.PrintIndirectOrder), HttpGet]
        public async Task<ActionResult> PrintIndirectOrder([FromQuery] long Id)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            var IndirectSalesOrder = await IndirectSalesOrderService.Get(Id);
            if (IndirectSalesOrder == null)
                return Content("Đơn hàng không tồn tại");
            GeneralMobile_PrintIndirectOrderDTO GeneralMobile_PrintDTO = new GeneralMobile_PrintIndirectOrderDTO(IndirectSalesOrder);
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-EN");
            var STT = 1;
            if (GeneralMobile_PrintDTO.Contents != null)
            {
                foreach (var IndirectSalesOrderContent in GeneralMobile_PrintDTO.Contents)
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
            if (GeneralMobile_PrintDTO.Promotions != null)
            {
                foreach (var IndirectSalesOrderPromotion in GeneralMobile_PrintDTO.Promotions)
                {
                    IndirectSalesOrderPromotion.STT = STT++;
                    IndirectSalesOrderPromotion.QuantityString = IndirectSalesOrderPromotion.Quantity.ToString("N0", culture);
                    IndirectSalesOrderPromotion.RequestedQuantityString = IndirectSalesOrderPromotion.RequestedQuantity.ToString("N0", culture);
                }
            }

            GeneralMobile_PrintDTO.SubTotalString = GeneralMobile_PrintDTO.SubTotal.ToString("N0", culture);
            GeneralMobile_PrintDTO.Discount = GeneralMobile_PrintDTO.GeneralDiscountAmount.HasValue ? GeneralMobile_PrintDTO.GeneralDiscountAmount.Value.ToString("N0", culture) : "";
            GeneralMobile_PrintDTO.TotalString = GeneralMobile_PrintDTO.Total.ToString("N0", culture);
            GeneralMobile_PrintDTO.TotalText = Utils.ConvertAmountTostring((long)GeneralMobile_PrintDTO.Total);

            string path = "Templates/Print_Indirect_Mobile.docx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            dynamic Data = new ExpandoObject();
            Data.Order = GeneralMobile_PrintDTO;
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

        [Route(GeneralMobileRoute.StoreReport), HttpPost]
        public async Task<ActionResult<GeneralMobile_StoreReportDTO>> StoreReport([FromBody] GeneralMobile_StoreReportFilterDTO GeneralMobile_StoreReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (GeneralMobile_StoreReportFilterDTO.StoreId == null || GeneralMobile_StoreReportFilterDTO.StoreId.Equal.HasValue == false)
                return new GeneralMobile_StoreReportDTO();

            DateTime Start = GeneralMobile_StoreReportFilterDTO.Date?.GreaterEqual == null ?
                    StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Date.AddHours(0 - CurrentContext.TimeZone) :
                    GeneralMobile_StoreReportFilterDTO.Date.GreaterEqual.Value;

            DateTime End = GeneralMobile_StoreReportFilterDTO.Date?.LessEqual == null ?
                    StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Date.AddHours(0 - CurrentContext.TimeZone).AddDays(1).AddSeconds(-1) :
                    GeneralMobile_StoreReportFilterDTO.Date.LessEqual.Value;

            GeneralMobile_StoreReportDTO GeneralMobile_StoreReportDTO = new GeneralMobile_StoreReportDTO();
            GeneralMobile_StoreReportDTO.ProblemCounter = await DataContext.Problem
                .Where(x => x.CreatorId == CurrentContext.UserId &&
                Start <= x.NoteAt && x.NoteAt <= End &&
                x.StoreId == GeneralMobile_StoreReportFilterDTO.StoreId.Equal.Value)
                .CountAsync();
            GeneralMobile_StoreReportDTO.ImageCounter = await DataContext.StoreImage
                .Where(x => x.SaleEmployeeId.HasValue && x.SaleEmployeeId == CurrentContext.UserId &&
                x.StoreId == GeneralMobile_StoreReportFilterDTO.StoreId.Equal.Value &&
                Start <= x.ShootingAt && x.ShootingAt <= End)
                .CountAsync();
            GeneralMobile_StoreReportDTO.SurveyResultCounter = await DataContext.SurveyResult
                .Where(x => x.AppUserId == CurrentContext.UserId &&
                x.StoreId == GeneralMobile_StoreReportFilterDTO.StoreId.Equal.Value &&
                Start <= x.Time && x.Time <= End)
                .CountAsync();
            return GeneralMobile_StoreReportDTO;
        }

        private StoreChecking ConvertDTOToEntity(GeneralMobile_StoreCheckingDTO GeneralMobile_StoreCheckingDTO)
        {
            StoreChecking StoreChecking = new StoreChecking();
            StoreChecking.Id = GeneralMobile_StoreCheckingDTO.Id;
            StoreChecking.StoreId = GeneralMobile_StoreCheckingDTO.StoreId;
            StoreChecking.SaleEmployeeId = GeneralMobile_StoreCheckingDTO.SaleEmployeeId;
            StoreChecking.Longitude = GeneralMobile_StoreCheckingDTO.Longitude;
            StoreChecking.Latitude = GeneralMobile_StoreCheckingDTO.Latitude;
            StoreChecking.CheckOutLongitude = GeneralMobile_StoreCheckingDTO.CheckOutLongitude;
            StoreChecking.CheckOutLatitude = GeneralMobile_StoreCheckingDTO.CheckOutLatitude;
            StoreChecking.CheckInAt = GeneralMobile_StoreCheckingDTO.CheckInAt;
            StoreChecking.CheckOutAt = GeneralMobile_StoreCheckingDTO.CheckOutAt;
            StoreChecking.CheckInDistance = GeneralMobile_StoreCheckingDTO.CheckInDistance;
            StoreChecking.CheckOutDistance = GeneralMobile_StoreCheckingDTO.CheckOutDistance;
            StoreChecking.CountIndirectSalesOrder = GeneralMobile_StoreCheckingDTO.CountIndirectSalesOrder;
            StoreChecking.ImageCounter = GeneralMobile_StoreCheckingDTO.CountImage;
            StoreChecking.IsOpenedStore = GeneralMobile_StoreCheckingDTO.IsOpenedStore;
            StoreChecking.StoreCheckingImageMappings = GeneralMobile_StoreCheckingDTO.StoreCheckingImageMappings?
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

        private StoreScouting ConvertStoreScoutingToEntity(GeneralMobile_StoreScoutingDTO GeneralMobile_StoreScoutingDTO)
        {
            StoreScouting StoreScouting = new StoreScouting();
            StoreScouting.Id = GeneralMobile_StoreScoutingDTO.Id;
            StoreScouting.Code = GeneralMobile_StoreScoutingDTO.Code;
            StoreScouting.Name = GeneralMobile_StoreScoutingDTO.Name;
            StoreScouting.OwnerPhone = GeneralMobile_StoreScoutingDTO.OwnerPhone;
            StoreScouting.ProvinceId = GeneralMobile_StoreScoutingDTO.ProvinceId;
            StoreScouting.DistrictId = GeneralMobile_StoreScoutingDTO.DistrictId;
            StoreScouting.WardId = GeneralMobile_StoreScoutingDTO.WardId;
            StoreScouting.Address = GeneralMobile_StoreScoutingDTO.Address;
            StoreScouting.Latitude = GeneralMobile_StoreScoutingDTO.Latitude;
            StoreScouting.Longitude = GeneralMobile_StoreScoutingDTO.Longitude;
            StoreScouting.CreatorId = GeneralMobile_StoreScoutingDTO.CreatorId;
            StoreScouting.StoreScoutingStatusId = GeneralMobile_StoreScoutingDTO.StoreScoutingStatusId;
            StoreScouting.StoreScoutingTypeId = GeneralMobile_StoreScoutingDTO.StoreScoutingTypeId;
            StoreScouting.Creator = GeneralMobile_StoreScoutingDTO.Creator == null ? null : new AppUser
            {
                Id = GeneralMobile_StoreScoutingDTO.Creator.Id,
                Username = GeneralMobile_StoreScoutingDTO.Creator.Username,
                DisplayName = GeneralMobile_StoreScoutingDTO.Creator.DisplayName,
                Address = GeneralMobile_StoreScoutingDTO.Creator.Address,
                Email = GeneralMobile_StoreScoutingDTO.Creator.Email,
                Phone = GeneralMobile_StoreScoutingDTO.Creator.Phone,
                PositionId = GeneralMobile_StoreScoutingDTO.Creator.PositionId,
                Department = GeneralMobile_StoreScoutingDTO.Creator.Department,
                OrganizationId = GeneralMobile_StoreScoutingDTO.Creator.OrganizationId,
                StatusId = GeneralMobile_StoreScoutingDTO.Creator.StatusId,
                Avatar = GeneralMobile_StoreScoutingDTO.Creator.Avatar,
                ProvinceId = GeneralMobile_StoreScoutingDTO.Creator.ProvinceId,
                SexId = GeneralMobile_StoreScoutingDTO.Creator.SexId,
                Birthday = GeneralMobile_StoreScoutingDTO.Creator.Birthday,
            };
            StoreScouting.Organization = GeneralMobile_StoreScoutingDTO.Organization == null ? null : new Organization
            {
                Id = GeneralMobile_StoreScoutingDTO.Organization.Id,
                Code = GeneralMobile_StoreScoutingDTO.Organization.Code,
                Name = GeneralMobile_StoreScoutingDTO.Organization.Name,
                ParentId = GeneralMobile_StoreScoutingDTO.Organization.ParentId,
                Path = GeneralMobile_StoreScoutingDTO.Organization.Path,
                Level = GeneralMobile_StoreScoutingDTO.Organization.Level,
                StatusId = GeneralMobile_StoreScoutingDTO.Organization.StatusId,
                Phone = GeneralMobile_StoreScoutingDTO.Organization.Phone,
                Address = GeneralMobile_StoreScoutingDTO.Organization.Address,
                Email = GeneralMobile_StoreScoutingDTO.Organization.Email,
            };
            StoreScouting.District = GeneralMobile_StoreScoutingDTO.District == null ? null : new District
            {
                Id = GeneralMobile_StoreScoutingDTO.District.Id,
                Code = GeneralMobile_StoreScoutingDTO.District.Code,
                Name = GeneralMobile_StoreScoutingDTO.District.Name,
                Priority = GeneralMobile_StoreScoutingDTO.District.Priority,
                ProvinceId = GeneralMobile_StoreScoutingDTO.District.ProvinceId,
                StatusId = GeneralMobile_StoreScoutingDTO.District.StatusId,
            };
            StoreScouting.Province = GeneralMobile_StoreScoutingDTO.Province == null ? null : new Province
            {
                Id = GeneralMobile_StoreScoutingDTO.Province.Id,
                Code = GeneralMobile_StoreScoutingDTO.Province.Code,
                Name = GeneralMobile_StoreScoutingDTO.Province.Name,
                Priority = GeneralMobile_StoreScoutingDTO.Province.Priority,
                StatusId = GeneralMobile_StoreScoutingDTO.Province.StatusId,
            };
            StoreScouting.StoreScoutingStatus = GeneralMobile_StoreScoutingDTO.StoreScoutingStatus == null ? null : new StoreScoutingStatus
            {
                Id = GeneralMobile_StoreScoutingDTO.StoreScoutingStatus.Id,
                Code = GeneralMobile_StoreScoutingDTO.StoreScoutingStatus.Code,
                Name = GeneralMobile_StoreScoutingDTO.StoreScoutingStatus.Name,
            };
            StoreScouting.StoreScoutingType = GeneralMobile_StoreScoutingDTO.StoreScoutingType == null ? null : new StoreScoutingType
            {
                Id = GeneralMobile_StoreScoutingDTO.StoreScoutingType.Id,
                Code = GeneralMobile_StoreScoutingDTO.StoreScoutingType.Code,
                Name = GeneralMobile_StoreScoutingDTO.StoreScoutingType.Name,
            };
            StoreScouting.Ward = GeneralMobile_StoreScoutingDTO.Ward == null ? null : new Ward
            {
                Id = GeneralMobile_StoreScoutingDTO.Ward.Id,
                Code = GeneralMobile_StoreScoutingDTO.Ward.Code,
                Name = GeneralMobile_StoreScoutingDTO.Ward.Name,
                Priority = GeneralMobile_StoreScoutingDTO.Ward.Priority,
                DistrictId = GeneralMobile_StoreScoutingDTO.Ward.DistrictId,
                StatusId = GeneralMobile_StoreScoutingDTO.Ward.StatusId,
            };
            StoreScouting.StoreScoutingImageMappings = GeneralMobile_StoreScoutingDTO.StoreScoutingImageMappings?.Select(x => new StoreScoutingImageMapping
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
        private StoreCheckingFilter ConvertFilterDTOToFilterEntity(GeneralMobile_StoreCheckingFilterDTO GeneralMobile_StoreCheckingFilterDTO)
        {
            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter();
            StoreCheckingFilter.Selects = StoreCheckingSelect.ALL;
            StoreCheckingFilter.Skip = GeneralMobile_StoreCheckingFilterDTO.Skip;
            StoreCheckingFilter.Take = GeneralMobile_StoreCheckingFilterDTO.Take;
            StoreCheckingFilter.OrderBy = GeneralMobile_StoreCheckingFilterDTO.OrderBy;
            StoreCheckingFilter.OrderType = GeneralMobile_StoreCheckingFilterDTO.OrderType;

            StoreCheckingFilter.Id = GeneralMobile_StoreCheckingFilterDTO.Id;
            StoreCheckingFilter.StoreId = GeneralMobile_StoreCheckingFilterDTO.StoreId;
            StoreCheckingFilter.SaleEmployeeId = GeneralMobile_StoreCheckingFilterDTO.SaleEmployeeId;
            StoreCheckingFilter.Longitude = GeneralMobile_StoreCheckingFilterDTO.Longitude;
            StoreCheckingFilter.Latitude = GeneralMobile_StoreCheckingFilterDTO.Latitude;
            StoreCheckingFilter.CheckInAt = GeneralMobile_StoreCheckingFilterDTO.CheckInAt;
            StoreCheckingFilter.CheckOutAt = GeneralMobile_StoreCheckingFilterDTO.CheckOutAt;
            StoreCheckingFilter.CountIndirectSalesOrder = GeneralMobile_StoreCheckingFilterDTO.CountIndirectSalesOrder;
            StoreCheckingFilter.CountImage = GeneralMobile_StoreCheckingFilterDTO.CountImage;
            return StoreCheckingFilter;
        }

        private IndirectSalesOrder ConvertIndirectSalesOrderDTOToEntity(GeneralMobile_IndirectSalesOrderDTO GeneralMobile_IndirectSalesOrderDTO)
        {
            IndirectSalesOrder IndirectSalesOrder = new IndirectSalesOrder();
            IndirectSalesOrder.Id = GeneralMobile_IndirectSalesOrderDTO.Id;
            IndirectSalesOrder.Code = GeneralMobile_IndirectSalesOrderDTO.Code;
            IndirectSalesOrder.BuyerStoreId = GeneralMobile_IndirectSalesOrderDTO.BuyerStoreId;
            IndirectSalesOrder.PhoneNumber = GeneralMobile_IndirectSalesOrderDTO.PhoneNumber;
            IndirectSalesOrder.StoreAddress = GeneralMobile_IndirectSalesOrderDTO.StoreAddress;
            IndirectSalesOrder.DeliveryAddress = GeneralMobile_IndirectSalesOrderDTO.DeliveryAddress;
            IndirectSalesOrder.SellerStoreId = GeneralMobile_IndirectSalesOrderDTO.SellerStoreId;
            IndirectSalesOrder.SaleEmployeeId = GeneralMobile_IndirectSalesOrderDTO.SaleEmployeeId;
            IndirectSalesOrder.OrganizationId = GeneralMobile_IndirectSalesOrderDTO.OrganizationId;
            IndirectSalesOrder.OrderDate = GeneralMobile_IndirectSalesOrderDTO.OrderDate;
            IndirectSalesOrder.DeliveryDate = GeneralMobile_IndirectSalesOrderDTO.DeliveryDate;
            IndirectSalesOrder.RequestStateId = GeneralMobile_IndirectSalesOrderDTO.RequestStateId;
            IndirectSalesOrder.EditedPriceStatusId = GeneralMobile_IndirectSalesOrderDTO.EditedPriceStatusId;
            IndirectSalesOrder.Note = GeneralMobile_IndirectSalesOrderDTO.Note;
            IndirectSalesOrder.SubTotal = GeneralMobile_IndirectSalesOrderDTO.SubTotal;
            IndirectSalesOrder.GeneralDiscountPercentage = GeneralMobile_IndirectSalesOrderDTO.GeneralDiscountPercentage;
            IndirectSalesOrder.GeneralDiscountAmount = GeneralMobile_IndirectSalesOrderDTO.GeneralDiscountAmount;
            IndirectSalesOrder.Total = GeneralMobile_IndirectSalesOrderDTO.Total;
            IndirectSalesOrder.BuyerStore = GeneralMobile_IndirectSalesOrderDTO.BuyerStore == null ? null : new Store
            {
                Id = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.Id,
                Code = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.Code,
                Name = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.Name,
                ParentStoreId = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.ParentStoreId,
                OrganizationId = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.OrganizationId,
                StoreTypeId = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.StoreTypeId,
                StoreGroupingId = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.StoreGroupingId,
                Telephone = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.Telephone,
                ProvinceId = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.ProvinceId,
                DistrictId = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.DistrictId,
                WardId = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.WardId,
                Address = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.Address,
                DeliveryAddress = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.DeliveryAddress,
                Latitude = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.Latitude,
                Longitude = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.Longitude,
                DeliveryLatitude = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.DeliveryLatitude,
                DeliveryLongitude = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.DeliveryLongitude,
                OwnerName = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.OwnerName,
                OwnerPhone = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.OwnerPhone,
                OwnerEmail = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.OwnerEmail,
                TaxCode = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.TaxCode,
                LegalEntity = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.LegalEntity,
                StatusId = GeneralMobile_IndirectSalesOrderDTO.BuyerStore.StatusId,
            };
            IndirectSalesOrder.EditedPriceStatus = GeneralMobile_IndirectSalesOrderDTO.EditedPriceStatus == null ? null : new EditedPriceStatus
            {
                Id = GeneralMobile_IndirectSalesOrderDTO.EditedPriceStatus.Id,
                Code = GeneralMobile_IndirectSalesOrderDTO.EditedPriceStatus.Code,
                Name = GeneralMobile_IndirectSalesOrderDTO.EditedPriceStatus.Name,
            };
            IndirectSalesOrder.Organization = GeneralMobile_IndirectSalesOrderDTO.Organization == null ? null : new Organization
            {
                Id = GeneralMobile_IndirectSalesOrderDTO.Organization.Id,
                Code = GeneralMobile_IndirectSalesOrderDTO.Organization.Code,
                Name = GeneralMobile_IndirectSalesOrderDTO.Organization.Name,
                ParentId = GeneralMobile_IndirectSalesOrderDTO.Organization.ParentId,
                Path = GeneralMobile_IndirectSalesOrderDTO.Organization.Path,
                Level = GeneralMobile_IndirectSalesOrderDTO.Organization.Level,
                StatusId = GeneralMobile_IndirectSalesOrderDTO.Organization.StatusId,
                Phone = GeneralMobile_IndirectSalesOrderDTO.Organization.Phone,
                Address = GeneralMobile_IndirectSalesOrderDTO.Organization.Address,
                Email = GeneralMobile_IndirectSalesOrderDTO.Organization.Email,
            };
            IndirectSalesOrder.SaleEmployee = GeneralMobile_IndirectSalesOrderDTO.SaleEmployee == null ? null : new AppUser
            {
                Id = GeneralMobile_IndirectSalesOrderDTO.SaleEmployee.Id,
                Username = GeneralMobile_IndirectSalesOrderDTO.SaleEmployee.Username,
                DisplayName = GeneralMobile_IndirectSalesOrderDTO.SaleEmployee.DisplayName,
                Address = GeneralMobile_IndirectSalesOrderDTO.SaleEmployee.Address,
                Email = GeneralMobile_IndirectSalesOrderDTO.SaleEmployee.Email,
                Phone = GeneralMobile_IndirectSalesOrderDTO.SaleEmployee.Phone,
                PositionId = GeneralMobile_IndirectSalesOrderDTO.SaleEmployee.PositionId,
                Department = GeneralMobile_IndirectSalesOrderDTO.SaleEmployee.Department,
                OrganizationId = GeneralMobile_IndirectSalesOrderDTO.SaleEmployee.OrganizationId,
                SexId = GeneralMobile_IndirectSalesOrderDTO.SaleEmployee.SexId,
                StatusId = GeneralMobile_IndirectSalesOrderDTO.SaleEmployee.StatusId,
                Avatar = GeneralMobile_IndirectSalesOrderDTO.SaleEmployee.Avatar,
                Birthday = GeneralMobile_IndirectSalesOrderDTO.SaleEmployee.Birthday,
                ProvinceId = GeneralMobile_IndirectSalesOrderDTO.SaleEmployee.ProvinceId,
            };
            IndirectSalesOrder.SellerStore = GeneralMobile_IndirectSalesOrderDTO.SellerStore == null ? null : new Store
            {
                Id = GeneralMobile_IndirectSalesOrderDTO.SellerStore.Id,
                Code = GeneralMobile_IndirectSalesOrderDTO.SellerStore.Code,
                Name = GeneralMobile_IndirectSalesOrderDTO.SellerStore.Name,
                ParentStoreId = GeneralMobile_IndirectSalesOrderDTO.SellerStore.ParentStoreId,
                OrganizationId = GeneralMobile_IndirectSalesOrderDTO.SellerStore.OrganizationId,
                StoreTypeId = GeneralMobile_IndirectSalesOrderDTO.SellerStore.StoreTypeId,
                StoreGroupingId = GeneralMobile_IndirectSalesOrderDTO.SellerStore.StoreGroupingId,
                Telephone = GeneralMobile_IndirectSalesOrderDTO.SellerStore.Telephone,
                ProvinceId = GeneralMobile_IndirectSalesOrderDTO.SellerStore.ProvinceId,
                DistrictId = GeneralMobile_IndirectSalesOrderDTO.SellerStore.DistrictId,
                WardId = GeneralMobile_IndirectSalesOrderDTO.SellerStore.WardId,
                Address = GeneralMobile_IndirectSalesOrderDTO.SellerStore.Address,
                DeliveryAddress = GeneralMobile_IndirectSalesOrderDTO.SellerStore.DeliveryAddress,
                Latitude = GeneralMobile_IndirectSalesOrderDTO.SellerStore.Latitude,
                Longitude = GeneralMobile_IndirectSalesOrderDTO.SellerStore.Longitude,
                DeliveryLatitude = GeneralMobile_IndirectSalesOrderDTO.SellerStore.DeliveryLatitude,
                DeliveryLongitude = GeneralMobile_IndirectSalesOrderDTO.SellerStore.DeliveryLongitude,
                OwnerName = GeneralMobile_IndirectSalesOrderDTO.SellerStore.OwnerName,
                OwnerPhone = GeneralMobile_IndirectSalesOrderDTO.SellerStore.OwnerPhone,
                OwnerEmail = GeneralMobile_IndirectSalesOrderDTO.SellerStore.OwnerEmail,
                TaxCode = GeneralMobile_IndirectSalesOrderDTO.SellerStore.TaxCode,
                LegalEntity = GeneralMobile_IndirectSalesOrderDTO.SellerStore.LegalEntity,
                StatusId = GeneralMobile_IndirectSalesOrderDTO.SellerStore.StatusId,
            };
            IndirectSalesOrder.IndirectSalesOrderContents = GeneralMobile_IndirectSalesOrderDTO.IndirectSalesOrderContents?
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
            IndirectSalesOrder.IndirectSalesOrderPromotions = GeneralMobile_IndirectSalesOrderDTO.IndirectSalesOrderPromotions?
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

