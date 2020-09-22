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
using DMS.Services.MPromotion;
using DMS.Services.MOrganization;
using DMS.Services.MPromotionType;
using DMS.Services.MStatus;
using DMS.Services.MPromotionCombo;
using DMS.Services.MPromotionDirectSalesOrder;
using DMS.Services.MPromotionDiscountType;
using DMS.Services.MPromotionPolicy;
using DMS.Services.MPromotionSamePrice;
using DMS.Services.MStoreGrouping;
using DMS.Services.MPromotionStoreGrouping;
using DMS.Services.MStore;
using DMS.Services.MStoreType;
using DMS.Services.MPromotionStoreType;
using DMS.Services.MPromotionStore;

namespace DMS.Rpc.promotion
{
    public partial class PromotionController : RpcController
    {
        private IOrganizationService OrganizationService;
        private IPromotionTypeService PromotionTypeService;
        private IStatusService StatusService;
        private IPromotionComboService PromotionComboService;
        private IPromotionDirectSalesOrderService PromotionDirectSalesOrderService;
        private IPromotionDiscountTypeService PromotionDiscountTypeService;
        private IPromotionPolicyService PromotionPolicyService;
        private IPromotionSamePriceService PromotionSamePriceService;
        private IStoreGroupingService StoreGroupingService;
        private IPromotionStoreGroupingService PromotionStoreGroupingService;
        private IStoreService StoreService;
        private IStoreTypeService StoreTypeService;
        private IPromotionStoreTypeService PromotionStoreTypeService;
        private IPromotionStoreService PromotionStoreService;
        private IPromotionService PromotionService;
        private ICurrentContext CurrentContext;
        public PromotionController(
            IOrganizationService OrganizationService,
            IPromotionTypeService PromotionTypeService,
            IStatusService StatusService,
            IPromotionComboService PromotionComboService,
            IPromotionDirectSalesOrderService PromotionDirectSalesOrderService,
            IPromotionDiscountTypeService PromotionDiscountTypeService,
            IPromotionPolicyService PromotionPolicyService,
            IPromotionSamePriceService PromotionSamePriceService,
            IStoreGroupingService StoreGroupingService,
            IPromotionStoreGroupingService PromotionStoreGroupingService,
            IStoreService StoreService,
            IStoreTypeService StoreTypeService,
            IPromotionStoreTypeService PromotionStoreTypeService,
            IPromotionStoreService PromotionStoreService,
            IPromotionService PromotionService,
            ICurrentContext CurrentContext
        )
        {
            this.OrganizationService = OrganizationService;
            this.PromotionTypeService = PromotionTypeService;
            this.StatusService = StatusService;
            this.PromotionComboService = PromotionComboService;
            this.PromotionDirectSalesOrderService = PromotionDirectSalesOrderService;
            this.PromotionDiscountTypeService = PromotionDiscountTypeService;
            this.PromotionPolicyService = PromotionPolicyService;
            this.PromotionSamePriceService = PromotionSamePriceService;
            this.StoreGroupingService = StoreGroupingService;
            this.PromotionStoreGroupingService = PromotionStoreGroupingService;
            this.StoreService = StoreService;
            this.StoreTypeService = StoreTypeService;
            this.PromotionStoreTypeService = PromotionStoreTypeService;
            this.PromotionStoreService = PromotionStoreService;
            this.PromotionService = PromotionService;
            this.CurrentContext = CurrentContext;
        }

        [Route(PromotionRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Promotion_PromotionFilterDTO Promotion_PromotionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionFilter PromotionFilter = ConvertFilterDTOToFilterEntity(Promotion_PromotionFilterDTO);
            PromotionFilter = await PromotionService.ToFilter(PromotionFilter);
            int count = await PromotionService.Count(PromotionFilter);
            return count;
        }

        [Route(PromotionRoute.List), HttpPost]
        public async Task<ActionResult<List<Promotion_PromotionDTO>>> List([FromBody] Promotion_PromotionFilterDTO Promotion_PromotionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionFilter PromotionFilter = ConvertFilterDTOToFilterEntity(Promotion_PromotionFilterDTO);
            PromotionFilter = await PromotionService.ToFilter(PromotionFilter);
            List<Promotion> Promotions = await PromotionService.List(PromotionFilter);
            List<Promotion_PromotionDTO> Promotion_PromotionDTOs = Promotions
                .Select(c => new Promotion_PromotionDTO(c)).ToList();
            return Promotion_PromotionDTOs;
        }

        [Route(PromotionRoute.Get), HttpPost]
        public async Task<ActionResult<Promotion_PromotionDTO>> Get([FromBody]Promotion_PromotionDTO Promotion_PromotionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Promotion_PromotionDTO.Id))
                return Forbid();

            Promotion Promotion = await PromotionService.Get(Promotion_PromotionDTO.Id);
            return new Promotion_PromotionDTO(Promotion);
        }

        [Route(PromotionRoute.Create), HttpPost]
        public async Task<ActionResult<Promotion_PromotionDTO>> Create([FromBody] Promotion_PromotionDTO Promotion_PromotionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Promotion_PromotionDTO.Id))
                return Forbid();

            Promotion Promotion = ConvertDTOToEntity(Promotion_PromotionDTO);
            Promotion = await PromotionService.Create(Promotion);
            Promotion_PromotionDTO = new Promotion_PromotionDTO(Promotion);
            if (Promotion.IsValidated)
                return Promotion_PromotionDTO;
            else
                return BadRequest(Promotion_PromotionDTO);
        }

        [Route(PromotionRoute.Update), HttpPost]
        public async Task<ActionResult<Promotion_PromotionDTO>> Update([FromBody] Promotion_PromotionDTO Promotion_PromotionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Promotion_PromotionDTO.Id))
                return Forbid();

            Promotion Promotion = ConvertDTOToEntity(Promotion_PromotionDTO);
            Promotion = await PromotionService.Update(Promotion);
            Promotion_PromotionDTO = new Promotion_PromotionDTO(Promotion);
            if (Promotion.IsValidated)
                return Promotion_PromotionDTO;
            else
                return BadRequest(Promotion_PromotionDTO);
        }

        [Route(PromotionRoute.Delete), HttpPost]
        public async Task<ActionResult<Promotion_PromotionDTO>> Delete([FromBody] Promotion_PromotionDTO Promotion_PromotionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Promotion_PromotionDTO.Id))
                return Forbid();

            Promotion Promotion = ConvertDTOToEntity(Promotion_PromotionDTO);
            Promotion = await PromotionService.Delete(Promotion);
            Promotion_PromotionDTO = new Promotion_PromotionDTO(Promotion);
            if (Promotion.IsValidated)
                return Promotion_PromotionDTO;
            else
                return BadRequest(Promotion_PromotionDTO);
        }
        
        [Route(PromotionRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PromotionFilter PromotionFilter = new PromotionFilter();
            PromotionFilter = await PromotionService.ToFilter(PromotionFilter);
            PromotionFilter.Id = new IdFilter { In = Ids };
            PromotionFilter.Selects = PromotionSelect.Id;
            PromotionFilter.Skip = 0;
            PromotionFilter.Take = int.MaxValue;

            List<Promotion> Promotions = await PromotionService.List(PromotionFilter);
            Promotions = await PromotionService.BulkDelete(Promotions);
            if (Promotions.Any(x => !x.IsValidated))
                return BadRequest(Promotions.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(PromotionRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            PromotionTypeFilter PromotionTypeFilter = new PromotionTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = PromotionTypeSelect.ALL
            };
            List<PromotionType> PromotionTypes = await PromotionTypeService.List(PromotionTypeFilter);
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Promotion> Promotions = new List<Promotion>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Promotions);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int StartDateColumn = 3 + StartColumn;
                int EndDateColumn = 4 + StartColumn;
                int OrganizationIdColumn = 5 + StartColumn;
                int PromotionTypeIdColumn = 6 + StartColumn;
                int NoteColumn = 7 + StartColumn;
                int PriorityColumn = 8 + StartColumn;
                int StatusIdColumn = 9 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string StartDateValue = worksheet.Cells[i + StartRow, StartDateColumn].Value?.ToString();
                    string EndDateValue = worksheet.Cells[i + StartRow, EndDateColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    string PromotionTypeIdValue = worksheet.Cells[i + StartRow, PromotionTypeIdColumn].Value?.ToString();
                    string NoteValue = worksheet.Cells[i + StartRow, NoteColumn].Value?.ToString();
                    string PriorityValue = worksheet.Cells[i + StartRow, PriorityColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    
                    Promotion Promotion = new Promotion();
                    Promotion.Code = CodeValue;
                    Promotion.Name = NameValue;
                    Promotion.StartDate = DateTime.TryParse(StartDateValue, out DateTime StartDate) ? StartDate : DateTime.Now;
                    Promotion.EndDate = DateTime.TryParse(EndDateValue, out DateTime EndDate) ? EndDate : DateTime.Now;
                    Promotion.Note = NoteValue;
                    Promotion.Priority = long.TryParse(PriorityValue, out long Priority) ? Priority : 0;
                    PromotionType PromotionType = PromotionTypes.Where(x => x.Id.ToString() == PromotionTypeIdValue).FirstOrDefault();
                    Promotion.PromotionTypeId = PromotionType == null ? 0 : PromotionType.Id;
                    Promotion.PromotionType = PromotionType;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    Promotion.StatusId = Status == null ? 0 : Status.Id;
                    Promotion.Status = Status;
                    
                    Promotions.Add(Promotion);
                }
            }
            Promotions = await PromotionService.Import(Promotions);
            if (Promotions.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Promotions.Count; i++)
                {
                    Promotion Promotion = Promotions[i];
                    if (!Promotion.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Promotion.Errors.ContainsKey(nameof(Promotion.Id)))
                            Error += Promotion.Errors[nameof(Promotion.Id)];
                        if (Promotion.Errors.ContainsKey(nameof(Promotion.Code)))
                            Error += Promotion.Errors[nameof(Promotion.Code)];
                        if (Promotion.Errors.ContainsKey(nameof(Promotion.Name)))
                            Error += Promotion.Errors[nameof(Promotion.Name)];
                        if (Promotion.Errors.ContainsKey(nameof(Promotion.StartDate)))
                            Error += Promotion.Errors[nameof(Promotion.StartDate)];
                        if (Promotion.Errors.ContainsKey(nameof(Promotion.EndDate)))
                            Error += Promotion.Errors[nameof(Promotion.EndDate)];
                        if (Promotion.Errors.ContainsKey(nameof(Promotion.OrganizationId)))
                            Error += Promotion.Errors[nameof(Promotion.OrganizationId)];
                        if (Promotion.Errors.ContainsKey(nameof(Promotion.PromotionTypeId)))
                            Error += Promotion.Errors[nameof(Promotion.PromotionTypeId)];
                        if (Promotion.Errors.ContainsKey(nameof(Promotion.Note)))
                            Error += Promotion.Errors[nameof(Promotion.Note)];
                        if (Promotion.Errors.ContainsKey(nameof(Promotion.Priority)))
                            Error += Promotion.Errors[nameof(Promotion.Priority)];
                        if (Promotion.Errors.ContainsKey(nameof(Promotion.StatusId)))
                            Error += Promotion.Errors[nameof(Promotion.StatusId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(PromotionRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] Promotion_PromotionFilterDTO Promotion_PromotionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Promotion
                var PromotionFilter = ConvertFilterDTOToFilterEntity(Promotion_PromotionFilterDTO);
                PromotionFilter.Skip = 0;
                PromotionFilter.Take = int.MaxValue;
                PromotionFilter = await PromotionService.ToFilter(PromotionFilter);
                List<Promotion> Promotions = await PromotionService.List(PromotionFilter);

                var PromotionHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "StartDate",
                        "EndDate",
                        "OrganizationId",
                        "PromotionTypeId",
                        "Note",
                        "Priority",
                        "StatusId",
                    }
                };
                List<object[]> PromotionData = new List<object[]>();
                for (int i = 0; i < Promotions.Count; i++)
                {
                    var Promotion = Promotions[i];
                    PromotionData.Add(new Object[]
                    {
                        Promotion.Id,
                        Promotion.Code,
                        Promotion.Name,
                        Promotion.StartDate,
                        Promotion.EndDate,
                        Promotion.OrganizationId,
                        Promotion.PromotionTypeId,
                        Promotion.Note,
                        Promotion.Priority,
                        Promotion.StatusId,
                    });
                }
                excel.GenerateWorksheet("Promotion", PromotionHeaders, PromotionData);
                #endregion
                
                #region Organization
                var OrganizationFilter = new OrganizationFilter();
                OrganizationFilter.Selects = OrganizationSelect.ALL;
                OrganizationFilter.OrderBy = OrganizationOrder.Id;
                OrganizationFilter.OrderType = OrderType.ASC;
                OrganizationFilter.Skip = 0;
                OrganizationFilter.Take = int.MaxValue;
                List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);

                var OrganizationHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                        "Phone",
                        "Email",
                        "Address",
                    }
                };
                List<object[]> OrganizationData = new List<object[]>();
                for (int i = 0; i < Organizations.Count; i++)
                {
                    var Organization = Organizations[i];
                    OrganizationData.Add(new Object[]
                    {
                        Organization.Id,
                        Organization.Code,
                        Organization.Name,
                        Organization.ParentId,
                        Organization.Path,
                        Organization.Level,
                        Organization.StatusId,
                        Organization.Phone,
                        Organization.Email,
                        Organization.Address,
                    });
                }
                excel.GenerateWorksheet("Organization", OrganizationHeaders, OrganizationData);
                #endregion
                #region PromotionType
                var PromotionTypeFilter = new PromotionTypeFilter();
                PromotionTypeFilter.Selects = PromotionTypeSelect.ALL;
                PromotionTypeFilter.OrderBy = PromotionTypeOrder.Id;
                PromotionTypeFilter.OrderType = OrderType.ASC;
                PromotionTypeFilter.Skip = 0;
                PromotionTypeFilter.Take = int.MaxValue;
                List<PromotionType> PromotionTypes = await PromotionTypeService.List(PromotionTypeFilter);

                var PromotionTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> PromotionTypeData = new List<object[]>();
                for (int i = 0; i < PromotionTypes.Count; i++)
                {
                    var PromotionType = PromotionTypes[i];
                    PromotionTypeData.Add(new Object[]
                    {
                        PromotionType.Id,
                        PromotionType.Code,
                        PromotionType.Name,
                    });
                }
                excel.GenerateWorksheet("PromotionType", PromotionTypeHeaders, PromotionTypeData);
                #endregion
                #region Status
                var StatusFilter = new StatusFilter();
                StatusFilter.Selects = StatusSelect.ALL;
                StatusFilter.OrderBy = StatusOrder.Id;
                StatusFilter.OrderType = OrderType.ASC;
                StatusFilter.Skip = 0;
                StatusFilter.Take = int.MaxValue;
                List<Status> Statuses = await StatusService.List(StatusFilter);

                var StatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> StatusData = new List<object[]>();
                for (int i = 0; i < Statuses.Count; i++)
                {
                    var Status = Statuses[i];
                    StatusData.Add(new Object[]
                    {
                        Status.Id,
                        Status.Code,
                        Status.Name,
                    });
                }
                excel.GenerateWorksheet("Status", StatusHeaders, StatusData);
                #endregion
                #region PromotionCombo
                var PromotionComboFilter = new PromotionComboFilter();
                PromotionComboFilter.Selects = PromotionComboSelect.ALL;
                PromotionComboFilter.OrderBy = PromotionComboOrder.Id;
                PromotionComboFilter.OrderType = OrderType.ASC;
                PromotionComboFilter.Skip = 0;
                PromotionComboFilter.Take = int.MaxValue;
                List<PromotionCombo> PromotionCombos = await PromotionComboService.List(PromotionComboFilter);

                var PromotionComboHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Note",
                        "Name",
                        "PromotionId",
                    }
                };
                List<object[]> PromotionComboData = new List<object[]>();
                for (int i = 0; i < PromotionCombos.Count; i++)
                {
                    var PromotionCombo = PromotionCombos[i];
                    PromotionComboData.Add(new Object[]
                    {
                        PromotionCombo.Id,
                        PromotionCombo.Note,
                        PromotionCombo.Name,
                        PromotionCombo.PromotionId,
                    });
                }
                excel.GenerateWorksheet("PromotionCombo", PromotionComboHeaders, PromotionComboData);
                #endregion
                #region PromotionDirectSalesOrder
                var PromotionDirectSalesOrderFilter = new PromotionDirectSalesOrderFilter();
                PromotionDirectSalesOrderFilter.Selects = PromotionDirectSalesOrderSelect.ALL;
                PromotionDirectSalesOrderFilter.OrderBy = PromotionDirectSalesOrderOrder.Id;
                PromotionDirectSalesOrderFilter.OrderType = OrderType.ASC;
                PromotionDirectSalesOrderFilter.Skip = 0;
                PromotionDirectSalesOrderFilter.Take = int.MaxValue;
                List<PromotionDirectSalesOrder> PromotionDirectSalesOrders = await PromotionDirectSalesOrderService.List(PromotionDirectSalesOrderFilter);

                var PromotionDirectSalesOrderHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Name",
                        "PromotionId",
                        "Note",
                        "FromValue",
                        "ToValue",
                        "PromotionDiscountTypeId",
                        "DiscountPercentage",
                        "DiscountValue",
                    }
                };
                List<object[]> PromotionDirectSalesOrderData = new List<object[]>();
                for (int i = 0; i < PromotionDirectSalesOrders.Count; i++)
                {
                    var PromotionDirectSalesOrder = PromotionDirectSalesOrders[i];
                    PromotionDirectSalesOrderData.Add(new Object[]
                    {
                        PromotionDirectSalesOrder.Id,
                        PromotionDirectSalesOrder.Name,
                        PromotionDirectSalesOrder.PromotionId,
                        PromotionDirectSalesOrder.Note,
                        PromotionDirectSalesOrder.FromValue,
                        PromotionDirectSalesOrder.ToValue,
                        PromotionDirectSalesOrder.PromotionDiscountTypeId,
                        PromotionDirectSalesOrder.DiscountPercentage,
                        PromotionDirectSalesOrder.DiscountValue,
                    });
                }
                excel.GenerateWorksheet("PromotionDirectSalesOrder", PromotionDirectSalesOrderHeaders, PromotionDirectSalesOrderData);
                #endregion
                #region PromotionDiscountType
                var PromotionDiscountTypeFilter = new PromotionDiscountTypeFilter();
                PromotionDiscountTypeFilter.Selects = PromotionDiscountTypeSelect.ALL;
                PromotionDiscountTypeFilter.OrderBy = PromotionDiscountTypeOrder.Id;
                PromotionDiscountTypeFilter.OrderType = OrderType.ASC;
                PromotionDiscountTypeFilter.Skip = 0;
                PromotionDiscountTypeFilter.Take = int.MaxValue;
                List<PromotionDiscountType> PromotionDiscountTypes = await PromotionDiscountTypeService.List(PromotionDiscountTypeFilter);

                var PromotionDiscountTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> PromotionDiscountTypeData = new List<object[]>();
                for (int i = 0; i < PromotionDiscountTypes.Count; i++)
                {
                    var PromotionDiscountType = PromotionDiscountTypes[i];
                    PromotionDiscountTypeData.Add(new Object[]
                    {
                        PromotionDiscountType.Id,
                        PromotionDiscountType.Code,
                        PromotionDiscountType.Name,
                    });
                }
                excel.GenerateWorksheet("PromotionDiscountType", PromotionDiscountTypeHeaders, PromotionDiscountTypeData);
                #endregion
                #region PromotionPolicy
                var PromotionPolicyFilter = new PromotionPolicyFilter();
                PromotionPolicyFilter.Selects = PromotionPolicySelect.ALL;
                PromotionPolicyFilter.OrderBy = PromotionPolicyOrder.Id;
                PromotionPolicyFilter.OrderType = OrderType.ASC;
                PromotionPolicyFilter.Skip = 0;
                PromotionPolicyFilter.Take = int.MaxValue;
                List<PromotionPolicy> PromotionPolicies = await PromotionPolicyService.List(PromotionPolicyFilter);

                var PromotionPolicyHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> PromotionPolicyData = new List<object[]>();
                for (int i = 0; i < PromotionPolicies.Count; i++)
                {
                    var PromotionPolicy = PromotionPolicies[i];
                    PromotionPolicyData.Add(new Object[]
                    {
                        PromotionPolicy.Id,
                        PromotionPolicy.Code,
                        PromotionPolicy.Name,
                    });
                }
                excel.GenerateWorksheet("PromotionPolicy", PromotionPolicyHeaders, PromotionPolicyData);
                #endregion
                #region PromotionSamePrice
                var PromotionSamePriceFilter = new PromotionSamePriceFilter();
                PromotionSamePriceFilter.Selects = PromotionSamePriceSelect.ALL;
                PromotionSamePriceFilter.OrderBy = PromotionSamePriceOrder.Id;
                PromotionSamePriceFilter.OrderType = OrderType.ASC;
                PromotionSamePriceFilter.Skip = 0;
                PromotionSamePriceFilter.Take = int.MaxValue;
                List<PromotionSamePrice> PromotionSamePrices = await PromotionSamePriceService.List(PromotionSamePriceFilter);

                var PromotionSamePriceHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Note",
                        "Name",
                        "PromotionId",
                        "Price",
                    }
                };
                List<object[]> PromotionSamePriceData = new List<object[]>();
                for (int i = 0; i < PromotionSamePrices.Count; i++)
                {
                    var PromotionSamePrice = PromotionSamePrices[i];
                    PromotionSamePriceData.Add(new Object[]
                    {
                        PromotionSamePrice.Id,
                        PromotionSamePrice.Note,
                        PromotionSamePrice.Name,
                        PromotionSamePrice.PromotionId,
                        PromotionSamePrice.Price,
                    });
                }
                excel.GenerateWorksheet("PromotionSamePrice", PromotionSamePriceHeaders, PromotionSamePriceData);
                #endregion
                #region StoreGrouping
                var StoreGroupingFilter = new StoreGroupingFilter();
                StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
                StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
                StoreGroupingFilter.OrderType = OrderType.ASC;
                StoreGroupingFilter.Skip = 0;
                StoreGroupingFilter.Take = int.MaxValue;
                List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);

                var StoreGroupingHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                    }
                };
                List<object[]> StoreGroupingData = new List<object[]>();
                for (int i = 0; i < StoreGroupings.Count; i++)
                {
                    var StoreGrouping = StoreGroupings[i];
                    StoreGroupingData.Add(new Object[]
                    {
                        StoreGrouping.Id,
                        StoreGrouping.Code,
                        StoreGrouping.Name,
                        StoreGrouping.ParentId,
                        StoreGrouping.Path,
                        StoreGrouping.Level,
                        StoreGrouping.StatusId,
                    });
                }
                excel.GenerateWorksheet("StoreGrouping", StoreGroupingHeaders, StoreGroupingData);
                #endregion
                #region PromotionStoreGrouping
                var PromotionStoreGroupingFilter = new PromotionStoreGroupingFilter();
                PromotionStoreGroupingFilter.Selects = PromotionStoreGroupingSelect.ALL;
                PromotionStoreGroupingFilter.OrderBy = PromotionStoreGroupingOrder.Id;
                PromotionStoreGroupingFilter.OrderType = OrderType.ASC;
                PromotionStoreGroupingFilter.Skip = 0;
                PromotionStoreGroupingFilter.Take = int.MaxValue;
                List<PromotionStoreGrouping> PromotionStoreGroupings = await PromotionStoreGroupingService.List(PromotionStoreGroupingFilter);

                var PromotionStoreGroupingHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "PromotionId",
                        "Note",
                        "FromValue",
                        "ToValue",
                        "PromotionDiscountTypeId",
                        "DiscountPercentage",
                        "DiscountValue",
                    }
                };
                List<object[]> PromotionStoreGroupingData = new List<object[]>();
                for (int i = 0; i < PromotionStoreGroupings.Count; i++)
                {
                    var PromotionStoreGrouping = PromotionStoreGroupings[i];
                    PromotionStoreGroupingData.Add(new Object[]
                    {
                        PromotionStoreGrouping.Id,
                        PromotionStoreGrouping.PromotionId,
                        PromotionStoreGrouping.Note,
                        PromotionStoreGrouping.FromValue,
                        PromotionStoreGrouping.ToValue,
                        PromotionStoreGrouping.PromotionDiscountTypeId,
                        PromotionStoreGrouping.DiscountPercentage,
                        PromotionStoreGrouping.DiscountValue,
                    });
                }
                excel.GenerateWorksheet("PromotionStoreGrouping", PromotionStoreGroupingHeaders, PromotionStoreGroupingData);
                #endregion
                #region Store
                var StoreFilter = new StoreFilter();
                StoreFilter.Selects = StoreSelect.ALL;
                StoreFilter.OrderBy = StoreOrder.Id;
                StoreFilter.OrderType = OrderType.ASC;
                StoreFilter.Skip = 0;
                StoreFilter.Take = int.MaxValue;
                List<Store> Stores = await StoreService.List(StoreFilter);

                var StoreHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "UnsignName",
                        "ParentStoreId",
                        "OrganizationId",
                        "StoreTypeId",
                        "StoreGroupingId",
                        "ResellerId",
                        "Telephone",
                        "ProvinceId",
                        "DistrictId",
                        "WardId",
                        "Address",
                        "UnsignAddress",
                        "DeliveryAddress",
                        "Latitude",
                        "Longitude",
                        "DeliveryLatitude",
                        "DeliveryLongitude",
                        "OwnerName",
                        "OwnerPhone",
                        "OwnerEmail",
                        "TaxCode",
                        "LegalEntity",
                        "StatusId",
                        "Used",
                        "StoreScoutingId",
                    }
                };
                List<object[]> StoreData = new List<object[]>();
                for (int i = 0; i < Stores.Count; i++)
                {
                    var Store = Stores[i];
                    StoreData.Add(new Object[]
                    {
                        Store.Id,
                        Store.Code,
                        Store.Name,
                        Store.UnsignName,
                        Store.ParentStoreId,
                        Store.OrganizationId,
                        Store.StoreTypeId,
                        Store.StoreGroupingId,
                        Store.ResellerId,
                        Store.Telephone,
                        Store.ProvinceId,
                        Store.DistrictId,
                        Store.WardId,
                        Store.Address,
                        Store.UnsignAddress,
                        Store.DeliveryAddress,
                        Store.Latitude,
                        Store.Longitude,
                        Store.DeliveryLatitude,
                        Store.DeliveryLongitude,
                        Store.OwnerName,
                        Store.OwnerPhone,
                        Store.OwnerEmail,
                        Store.TaxCode,
                        Store.LegalEntity,
                        Store.StatusId,
                        Store.Used,
                        Store.StoreScoutingId,
                    });
                }
                excel.GenerateWorksheet("Store", StoreHeaders, StoreData);
                #endregion
                #region StoreType
                var StoreTypeFilter = new StoreTypeFilter();
                StoreTypeFilter.Selects = StoreTypeSelect.ALL;
                StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
                StoreTypeFilter.OrderType = OrderType.ASC;
                StoreTypeFilter.Skip = 0;
                StoreTypeFilter.Take = int.MaxValue;
                List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);

                var StoreTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ColorId",
                        "StatusId",
                        "Used",
                    }
                };
                List<object[]> StoreTypeData = new List<object[]>();
                for (int i = 0; i < StoreTypes.Count; i++)
                {
                    var StoreType = StoreTypes[i];
                    StoreTypeData.Add(new Object[]
                    {
                        StoreType.Id,
                        StoreType.Code,
                        StoreType.Name,
                        StoreType.ColorId,
                        StoreType.StatusId,
                        StoreType.Used,
                    });
                }
                excel.GenerateWorksheet("StoreType", StoreTypeHeaders, StoreTypeData);
                #endregion
                #region PromotionStoreType
                var PromotionStoreTypeFilter = new PromotionStoreTypeFilter();
                PromotionStoreTypeFilter.Selects = PromotionStoreTypeSelect.ALL;
                PromotionStoreTypeFilter.OrderBy = PromotionStoreTypeOrder.Id;
                PromotionStoreTypeFilter.OrderType = OrderType.ASC;
                PromotionStoreTypeFilter.Skip = 0;
                PromotionStoreTypeFilter.Take = int.MaxValue;
                List<PromotionStoreType> PromotionStoreTypes = await PromotionStoreTypeService.List(PromotionStoreTypeFilter);

                var PromotionStoreTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "PromotionId",
                        "Note",
                        "FromValue",
                        "ToValue",
                        "PromotionDiscountTypeId",
                        "DiscountPercentage",
                        "DiscountValue",
                    }
                };
                List<object[]> PromotionStoreTypeData = new List<object[]>();
                for (int i = 0; i < PromotionStoreTypes.Count; i++)
                {
                    var PromotionStoreType = PromotionStoreTypes[i];
                    PromotionStoreTypeData.Add(new Object[]
                    {
                        PromotionStoreType.Id,
                        PromotionStoreType.PromotionId,
                        PromotionStoreType.Note,
                        PromotionStoreType.FromValue,
                        PromotionStoreType.ToValue,
                        PromotionStoreType.PromotionDiscountTypeId,
                        PromotionStoreType.DiscountPercentage,
                        PromotionStoreType.DiscountValue,
                    });
                }
                excel.GenerateWorksheet("PromotionStoreType", PromotionStoreTypeHeaders, PromotionStoreTypeData);
                #endregion
                #region PromotionStore
                var PromotionStoreFilter = new PromotionStoreFilter();
                PromotionStoreFilter.Selects = PromotionStoreSelect.ALL;
                PromotionStoreFilter.OrderBy = PromotionStoreOrder.Id;
                PromotionStoreFilter.OrderType = OrderType.ASC;
                PromotionStoreFilter.Skip = 0;
                PromotionStoreFilter.Take = int.MaxValue;
                List<PromotionStore> PromotionStores = await PromotionStoreService.List(PromotionStoreFilter);

                var PromotionStoreHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "PromotionId",
                        "Note",
                        "FromValue",
                        "ToValue",
                        "PromotionDiscountTypeId",
                        "DiscountPercentage",
                        "DiscountValue",
                    }
                };
                List<object[]> PromotionStoreData = new List<object[]>();
                for (int i = 0; i < PromotionStores.Count; i++)
                {
                    var PromotionStore = PromotionStores[i];
                    PromotionStoreData.Add(new Object[]
                    {
                        PromotionStore.Id,
                        PromotionStore.PromotionId,
                        PromotionStore.Note,
                        PromotionStore.FromValue,
                        PromotionStore.ToValue,
                        PromotionStore.PromotionDiscountTypeId,
                        PromotionStore.DiscountPercentage,
                        PromotionStore.DiscountValue,
                    });
                }
                excel.GenerateWorksheet("PromotionStore", PromotionStoreHeaders, PromotionStoreData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Promotion.xlsx");
        }

        [Route(PromotionRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] Promotion_PromotionFilterDTO Promotion_PromotionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Promotion
                var PromotionHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "StartDate",
                        "EndDate",
                        "OrganizationId",
                        "PromotionTypeId",
                        "Note",
                        "Priority",
                        "StatusId",
                    }
                };
                List<object[]> PromotionData = new List<object[]>();
                excel.GenerateWorksheet("Promotion", PromotionHeaders, PromotionData);
                #endregion
                
                #region Organization
                var OrganizationFilter = new OrganizationFilter();
                OrganizationFilter.Selects = OrganizationSelect.ALL;
                OrganizationFilter.OrderBy = OrganizationOrder.Id;
                OrganizationFilter.OrderType = OrderType.ASC;
                OrganizationFilter.Skip = 0;
                OrganizationFilter.Take = int.MaxValue;
                List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);

                var OrganizationHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                        "Phone",
                        "Email",
                        "Address",
                    }
                };
                List<object[]> OrganizationData = new List<object[]>();
                for (int i = 0; i < Organizations.Count; i++)
                {
                    var Organization = Organizations[i];
                    OrganizationData.Add(new Object[]
                    {
                        Organization.Id,
                        Organization.Code,
                        Organization.Name,
                        Organization.ParentId,
                        Organization.Path,
                        Organization.Level,
                        Organization.StatusId,
                        Organization.Phone,
                        Organization.Email,
                        Organization.Address,
                    });
                }
                excel.GenerateWorksheet("Organization", OrganizationHeaders, OrganizationData);
                #endregion
                #region PromotionType
                var PromotionTypeFilter = new PromotionTypeFilter();
                PromotionTypeFilter.Selects = PromotionTypeSelect.ALL;
                PromotionTypeFilter.OrderBy = PromotionTypeOrder.Id;
                PromotionTypeFilter.OrderType = OrderType.ASC;
                PromotionTypeFilter.Skip = 0;
                PromotionTypeFilter.Take = int.MaxValue;
                List<PromotionType> PromotionTypes = await PromotionTypeService.List(PromotionTypeFilter);

                var PromotionTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> PromotionTypeData = new List<object[]>();
                for (int i = 0; i < PromotionTypes.Count; i++)
                {
                    var PromotionType = PromotionTypes[i];
                    PromotionTypeData.Add(new Object[]
                    {
                        PromotionType.Id,
                        PromotionType.Code,
                        PromotionType.Name,
                    });
                }
                excel.GenerateWorksheet("PromotionType", PromotionTypeHeaders, PromotionTypeData);
                #endregion
                #region Status
                var StatusFilter = new StatusFilter();
                StatusFilter.Selects = StatusSelect.ALL;
                StatusFilter.OrderBy = StatusOrder.Id;
                StatusFilter.OrderType = OrderType.ASC;
                StatusFilter.Skip = 0;
                StatusFilter.Take = int.MaxValue;
                List<Status> Statuses = await StatusService.List(StatusFilter);

                var StatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> StatusData = new List<object[]>();
                for (int i = 0; i < Statuses.Count; i++)
                {
                    var Status = Statuses[i];
                    StatusData.Add(new Object[]
                    {
                        Status.Id,
                        Status.Code,
                        Status.Name,
                    });
                }
                excel.GenerateWorksheet("Status", StatusHeaders, StatusData);
                #endregion
                #region PromotionCombo
                var PromotionComboFilter = new PromotionComboFilter();
                PromotionComboFilter.Selects = PromotionComboSelect.ALL;
                PromotionComboFilter.OrderBy = PromotionComboOrder.Id;
                PromotionComboFilter.OrderType = OrderType.ASC;
                PromotionComboFilter.Skip = 0;
                PromotionComboFilter.Take = int.MaxValue;
                List<PromotionCombo> PromotionCombos = await PromotionComboService.List(PromotionComboFilter);

                var PromotionComboHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Note",
                        "Name",
                        "PromotionId",
                    }
                };
                List<object[]> PromotionComboData = new List<object[]>();
                for (int i = 0; i < PromotionCombos.Count; i++)
                {
                    var PromotionCombo = PromotionCombos[i];
                    PromotionComboData.Add(new Object[]
                    {
                        PromotionCombo.Id,
                        PromotionCombo.Note,
                        PromotionCombo.Name,
                        PromotionCombo.PromotionId,
                    });
                }
                excel.GenerateWorksheet("PromotionCombo", PromotionComboHeaders, PromotionComboData);
                #endregion
                #region PromotionDirectSalesOrder
                var PromotionDirectSalesOrderFilter = new PromotionDirectSalesOrderFilter();
                PromotionDirectSalesOrderFilter.Selects = PromotionDirectSalesOrderSelect.ALL;
                PromotionDirectSalesOrderFilter.OrderBy = PromotionDirectSalesOrderOrder.Id;
                PromotionDirectSalesOrderFilter.OrderType = OrderType.ASC;
                PromotionDirectSalesOrderFilter.Skip = 0;
                PromotionDirectSalesOrderFilter.Take = int.MaxValue;
                List<PromotionDirectSalesOrder> PromotionDirectSalesOrders = await PromotionDirectSalesOrderService.List(PromotionDirectSalesOrderFilter);

                var PromotionDirectSalesOrderHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Name",
                        "PromotionId",
                        "Note",
                        "FromValue",
                        "ToValue",
                        "PromotionDiscountTypeId",
                        "DiscountPercentage",
                        "DiscountValue",
                    }
                };
                List<object[]> PromotionDirectSalesOrderData = new List<object[]>();
                for (int i = 0; i < PromotionDirectSalesOrders.Count; i++)
                {
                    var PromotionDirectSalesOrder = PromotionDirectSalesOrders[i];
                    PromotionDirectSalesOrderData.Add(new Object[]
                    {
                        PromotionDirectSalesOrder.Id,
                        PromotionDirectSalesOrder.Name,
                        PromotionDirectSalesOrder.PromotionId,
                        PromotionDirectSalesOrder.Note,
                        PromotionDirectSalesOrder.FromValue,
                        PromotionDirectSalesOrder.ToValue,
                        PromotionDirectSalesOrder.PromotionDiscountTypeId,
                        PromotionDirectSalesOrder.DiscountPercentage,
                        PromotionDirectSalesOrder.DiscountValue,
                    });
                }
                excel.GenerateWorksheet("PromotionDirectSalesOrder", PromotionDirectSalesOrderHeaders, PromotionDirectSalesOrderData);
                #endregion
                #region PromotionDiscountType
                var PromotionDiscountTypeFilter = new PromotionDiscountTypeFilter();
                PromotionDiscountTypeFilter.Selects = PromotionDiscountTypeSelect.ALL;
                PromotionDiscountTypeFilter.OrderBy = PromotionDiscountTypeOrder.Id;
                PromotionDiscountTypeFilter.OrderType = OrderType.ASC;
                PromotionDiscountTypeFilter.Skip = 0;
                PromotionDiscountTypeFilter.Take = int.MaxValue;
                List<PromotionDiscountType> PromotionDiscountTypes = await PromotionDiscountTypeService.List(PromotionDiscountTypeFilter);

                var PromotionDiscountTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> PromotionDiscountTypeData = new List<object[]>();
                for (int i = 0; i < PromotionDiscountTypes.Count; i++)
                {
                    var PromotionDiscountType = PromotionDiscountTypes[i];
                    PromotionDiscountTypeData.Add(new Object[]
                    {
                        PromotionDiscountType.Id,
                        PromotionDiscountType.Code,
                        PromotionDiscountType.Name,
                    });
                }
                excel.GenerateWorksheet("PromotionDiscountType", PromotionDiscountTypeHeaders, PromotionDiscountTypeData);
                #endregion
                #region PromotionPolicy
                var PromotionPolicyFilter = new PromotionPolicyFilter();
                PromotionPolicyFilter.Selects = PromotionPolicySelect.ALL;
                PromotionPolicyFilter.OrderBy = PromotionPolicyOrder.Id;
                PromotionPolicyFilter.OrderType = OrderType.ASC;
                PromotionPolicyFilter.Skip = 0;
                PromotionPolicyFilter.Take = int.MaxValue;
                List<PromotionPolicy> PromotionPolicies = await PromotionPolicyService.List(PromotionPolicyFilter);

                var PromotionPolicyHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> PromotionPolicyData = new List<object[]>();
                for (int i = 0; i < PromotionPolicies.Count; i++)
                {
                    var PromotionPolicy = PromotionPolicies[i];
                    PromotionPolicyData.Add(new Object[]
                    {
                        PromotionPolicy.Id,
                        PromotionPolicy.Code,
                        PromotionPolicy.Name,
                    });
                }
                excel.GenerateWorksheet("PromotionPolicy", PromotionPolicyHeaders, PromotionPolicyData);
                #endregion
                #region PromotionSamePrice
                var PromotionSamePriceFilter = new PromotionSamePriceFilter();
                PromotionSamePriceFilter.Selects = PromotionSamePriceSelect.ALL;
                PromotionSamePriceFilter.OrderBy = PromotionSamePriceOrder.Id;
                PromotionSamePriceFilter.OrderType = OrderType.ASC;
                PromotionSamePriceFilter.Skip = 0;
                PromotionSamePriceFilter.Take = int.MaxValue;
                List<PromotionSamePrice> PromotionSamePrices = await PromotionSamePriceService.List(PromotionSamePriceFilter);

                var PromotionSamePriceHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Note",
                        "Name",
                        "PromotionId",
                        "Price",
                    }
                };
                List<object[]> PromotionSamePriceData = new List<object[]>();
                for (int i = 0; i < PromotionSamePrices.Count; i++)
                {
                    var PromotionSamePrice = PromotionSamePrices[i];
                    PromotionSamePriceData.Add(new Object[]
                    {
                        PromotionSamePrice.Id,
                        PromotionSamePrice.Note,
                        PromotionSamePrice.Name,
                        PromotionSamePrice.PromotionId,
                        PromotionSamePrice.Price,
                    });
                }
                excel.GenerateWorksheet("PromotionSamePrice", PromotionSamePriceHeaders, PromotionSamePriceData);
                #endregion
                #region StoreGrouping
                var StoreGroupingFilter = new StoreGroupingFilter();
                StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
                StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
                StoreGroupingFilter.OrderType = OrderType.ASC;
                StoreGroupingFilter.Skip = 0;
                StoreGroupingFilter.Take = int.MaxValue;
                List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);

                var StoreGroupingHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                    }
                };
                List<object[]> StoreGroupingData = new List<object[]>();
                for (int i = 0; i < StoreGroupings.Count; i++)
                {
                    var StoreGrouping = StoreGroupings[i];
                    StoreGroupingData.Add(new Object[]
                    {
                        StoreGrouping.Id,
                        StoreGrouping.Code,
                        StoreGrouping.Name,
                        StoreGrouping.ParentId,
                        StoreGrouping.Path,
                        StoreGrouping.Level,
                        StoreGrouping.StatusId,
                    });
                }
                excel.GenerateWorksheet("StoreGrouping", StoreGroupingHeaders, StoreGroupingData);
                #endregion
                #region PromotionStoreGrouping
                var PromotionStoreGroupingFilter = new PromotionStoreGroupingFilter();
                PromotionStoreGroupingFilter.Selects = PromotionStoreGroupingSelect.ALL;
                PromotionStoreGroupingFilter.OrderBy = PromotionStoreGroupingOrder.Id;
                PromotionStoreGroupingFilter.OrderType = OrderType.ASC;
                PromotionStoreGroupingFilter.Skip = 0;
                PromotionStoreGroupingFilter.Take = int.MaxValue;
                List<PromotionStoreGrouping> PromotionStoreGroupings = await PromotionStoreGroupingService.List(PromotionStoreGroupingFilter);

                var PromotionStoreGroupingHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "PromotionId",
                        "Note",
                        "FromValue",
                        "ToValue",
                        "PromotionDiscountTypeId",
                        "DiscountPercentage",
                        "DiscountValue",
                    }
                };
                List<object[]> PromotionStoreGroupingData = new List<object[]>();
                for (int i = 0; i < PromotionStoreGroupings.Count; i++)
                {
                    var PromotionStoreGrouping = PromotionStoreGroupings[i];
                    PromotionStoreGroupingData.Add(new Object[]
                    {
                        PromotionStoreGrouping.Id,
                        PromotionStoreGrouping.PromotionId,
                        PromotionStoreGrouping.Note,
                        PromotionStoreGrouping.FromValue,
                        PromotionStoreGrouping.ToValue,
                        PromotionStoreGrouping.PromotionDiscountTypeId,
                        PromotionStoreGrouping.DiscountPercentage,
                        PromotionStoreGrouping.DiscountValue,
                    });
                }
                excel.GenerateWorksheet("PromotionStoreGrouping", PromotionStoreGroupingHeaders, PromotionStoreGroupingData);
                #endregion
                #region Store
                var StoreFilter = new StoreFilter();
                StoreFilter.Selects = StoreSelect.ALL;
                StoreFilter.OrderBy = StoreOrder.Id;
                StoreFilter.OrderType = OrderType.ASC;
                StoreFilter.Skip = 0;
                StoreFilter.Take = int.MaxValue;
                List<Store> Stores = await StoreService.List(StoreFilter);

                var StoreHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "UnsignName",
                        "ParentStoreId",
                        "OrganizationId",
                        "StoreTypeId",
                        "StoreGroupingId",
                        "ResellerId",
                        "Telephone",
                        "ProvinceId",
                        "DistrictId",
                        "WardId",
                        "Address",
                        "UnsignAddress",
                        "DeliveryAddress",
                        "Latitude",
                        "Longitude",
                        "DeliveryLatitude",
                        "DeliveryLongitude",
                        "OwnerName",
                        "OwnerPhone",
                        "OwnerEmail",
                        "TaxCode",
                        "LegalEntity",
                        "StatusId",
                        "Used",
                        "StoreScoutingId",
                    }
                };
                List<object[]> StoreData = new List<object[]>();
                for (int i = 0; i < Stores.Count; i++)
                {
                    var Store = Stores[i];
                    StoreData.Add(new Object[]
                    {
                        Store.Id,
                        Store.Code,
                        Store.Name,
                        Store.UnsignName,
                        Store.ParentStoreId,
                        Store.OrganizationId,
                        Store.StoreTypeId,
                        Store.StoreGroupingId,
                        Store.ResellerId,
                        Store.Telephone,
                        Store.ProvinceId,
                        Store.DistrictId,
                        Store.WardId,
                        Store.Address,
                        Store.UnsignAddress,
                        Store.DeliveryAddress,
                        Store.Latitude,
                        Store.Longitude,
                        Store.DeliveryLatitude,
                        Store.DeliveryLongitude,
                        Store.OwnerName,
                        Store.OwnerPhone,
                        Store.OwnerEmail,
                        Store.TaxCode,
                        Store.LegalEntity,
                        Store.StatusId,
                        Store.Used,
                        Store.StoreScoutingId,
                    });
                }
                excel.GenerateWorksheet("Store", StoreHeaders, StoreData);
                #endregion
                #region StoreType
                var StoreTypeFilter = new StoreTypeFilter();
                StoreTypeFilter.Selects = StoreTypeSelect.ALL;
                StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
                StoreTypeFilter.OrderType = OrderType.ASC;
                StoreTypeFilter.Skip = 0;
                StoreTypeFilter.Take = int.MaxValue;
                List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);

                var StoreTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ColorId",
                        "StatusId",
                        "Used",
                    }
                };
                List<object[]> StoreTypeData = new List<object[]>();
                for (int i = 0; i < StoreTypes.Count; i++)
                {
                    var StoreType = StoreTypes[i];
                    StoreTypeData.Add(new Object[]
                    {
                        StoreType.Id,
                        StoreType.Code,
                        StoreType.Name,
                        StoreType.ColorId,
                        StoreType.StatusId,
                        StoreType.Used,
                    });
                }
                excel.GenerateWorksheet("StoreType", StoreTypeHeaders, StoreTypeData);
                #endregion
                #region PromotionStoreType
                var PromotionStoreTypeFilter = new PromotionStoreTypeFilter();
                PromotionStoreTypeFilter.Selects = PromotionStoreTypeSelect.ALL;
                PromotionStoreTypeFilter.OrderBy = PromotionStoreTypeOrder.Id;
                PromotionStoreTypeFilter.OrderType = OrderType.ASC;
                PromotionStoreTypeFilter.Skip = 0;
                PromotionStoreTypeFilter.Take = int.MaxValue;
                List<PromotionStoreType> PromotionStoreTypes = await PromotionStoreTypeService.List(PromotionStoreTypeFilter);

                var PromotionStoreTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "PromotionId",
                        "Note",
                        "FromValue",
                        "ToValue",
                        "PromotionDiscountTypeId",
                        "DiscountPercentage",
                        "DiscountValue",
                    }
                };
                List<object[]> PromotionStoreTypeData = new List<object[]>();
                for (int i = 0; i < PromotionStoreTypes.Count; i++)
                {
                    var PromotionStoreType = PromotionStoreTypes[i];
                    PromotionStoreTypeData.Add(new Object[]
                    {
                        PromotionStoreType.Id,
                        PromotionStoreType.PromotionId,
                        PromotionStoreType.Note,
                        PromotionStoreType.FromValue,
                        PromotionStoreType.ToValue,
                        PromotionStoreType.PromotionDiscountTypeId,
                        PromotionStoreType.DiscountPercentage,
                        PromotionStoreType.DiscountValue,
                    });
                }
                excel.GenerateWorksheet("PromotionStoreType", PromotionStoreTypeHeaders, PromotionStoreTypeData);
                #endregion
                #region PromotionStore
                var PromotionStoreFilter = new PromotionStoreFilter();
                PromotionStoreFilter.Selects = PromotionStoreSelect.ALL;
                PromotionStoreFilter.OrderBy = PromotionStoreOrder.Id;
                PromotionStoreFilter.OrderType = OrderType.ASC;
                PromotionStoreFilter.Skip = 0;
                PromotionStoreFilter.Take = int.MaxValue;
                List<PromotionStore> PromotionStores = await PromotionStoreService.List(PromotionStoreFilter);

                var PromotionStoreHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "PromotionId",
                        "Note",
                        "FromValue",
                        "ToValue",
                        "PromotionDiscountTypeId",
                        "DiscountPercentage",
                        "DiscountValue",
                    }
                };
                List<object[]> PromotionStoreData = new List<object[]>();
                for (int i = 0; i < PromotionStores.Count; i++)
                {
                    var PromotionStore = PromotionStores[i];
                    PromotionStoreData.Add(new Object[]
                    {
                        PromotionStore.Id,
                        PromotionStore.PromotionId,
                        PromotionStore.Note,
                        PromotionStore.FromValue,
                        PromotionStore.ToValue,
                        PromotionStore.PromotionDiscountTypeId,
                        PromotionStore.DiscountPercentage,
                        PromotionStore.DiscountValue,
                    });
                }
                excel.GenerateWorksheet("PromotionStore", PromotionStoreHeaders, PromotionStoreData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Promotion.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            PromotionFilter PromotionFilter = new PromotionFilter();
            PromotionFilter = await PromotionService.ToFilter(PromotionFilter);
            if (Id == 0)
            {

            }
            else
            {
                PromotionFilter.Id = new IdFilter { Equal = Id };
                int count = await PromotionService.Count(PromotionFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Promotion ConvertDTOToEntity(Promotion_PromotionDTO Promotion_PromotionDTO)
        {
            Promotion Promotion = new Promotion();
            Promotion.Id = Promotion_PromotionDTO.Id;
            Promotion.Code = Promotion_PromotionDTO.Code;
            Promotion.Name = Promotion_PromotionDTO.Name;
            Promotion.StartDate = Promotion_PromotionDTO.StartDate;
            Promotion.EndDate = Promotion_PromotionDTO.EndDate;
            Promotion.OrganizationId = Promotion_PromotionDTO.OrganizationId;
            Promotion.PromotionTypeId = Promotion_PromotionDTO.PromotionTypeId;
            Promotion.Note = Promotion_PromotionDTO.Note;
            Promotion.Priority = Promotion_PromotionDTO.Priority;
            Promotion.StatusId = Promotion_PromotionDTO.StatusId;
            Promotion.Organization = Promotion_PromotionDTO.Organization == null ? null : new Organization
            {
                Id = Promotion_PromotionDTO.Organization.Id,
                Code = Promotion_PromotionDTO.Organization.Code,
                Name = Promotion_PromotionDTO.Organization.Name,
                ParentId = Promotion_PromotionDTO.Organization.ParentId,
                Path = Promotion_PromotionDTO.Organization.Path,
                Level = Promotion_PromotionDTO.Organization.Level,
                StatusId = Promotion_PromotionDTO.Organization.StatusId,
                Phone = Promotion_PromotionDTO.Organization.Phone,
                Email = Promotion_PromotionDTO.Organization.Email,
                Address = Promotion_PromotionDTO.Organization.Address,
            };
            Promotion.PromotionType = Promotion_PromotionDTO.PromotionType == null ? null : new PromotionType
            {
                Id = Promotion_PromotionDTO.PromotionType.Id,
                Code = Promotion_PromotionDTO.PromotionType.Code,
                Name = Promotion_PromotionDTO.PromotionType.Name,
            };
            Promotion.Status = Promotion_PromotionDTO.Status == null ? null : new Status
            {
                Id = Promotion_PromotionDTO.Status.Id,
                Code = Promotion_PromotionDTO.Status.Code,
                Name = Promotion_PromotionDTO.Status.Name,
            };
            Promotion.PromotionCombos = Promotion_PromotionDTO.PromotionCombos?
                .Select(x => new PromotionCombo
                {
                    Id = x.Id,
                    Note = x.Note,
                    Name = x.Name,
                }).ToList();
            Promotion.PromotionDirectSalesOrders = Promotion_PromotionDTO.PromotionDirectSalesOrders?
                .Select(x => new PromotionDirectSalesOrder
                {
                    Id = x.Id,
                    Name = x.Name,
                    Note = x.Note,
                    FromValue = x.FromValue,
                    ToValue = x.ToValue,
                    PromotionDiscountTypeId = x.PromotionDiscountTypeId,
                    DiscountPercentage = x.DiscountPercentage,
                    DiscountValue = x.DiscountValue,
                    PromotionDiscountType = x.PromotionDiscountType == null ? null : new PromotionDiscountType
                    {
                        Id = x.PromotionDiscountType.Id,
                        Code = x.PromotionDiscountType.Code,
                        Name = x.PromotionDiscountType.Name,
                    },
                }).ToList();
            Promotion.PromotionPromotionPolicyMappings = Promotion_PromotionDTO.PromotionPromotionPolicyMappings?
                .Select(x => new PromotionPromotionPolicyMapping
                {
                    PromotionPolicyId = x.PromotionPolicyId,
                    Note = x.Note,
                    StatusId = x.StatusId,
                    PromotionPolicy = x.PromotionPolicy == null ? null : new PromotionPolicy
                    {
                        Id = x.PromotionPolicy.Id,
                        Code = x.PromotionPolicy.Code,
                        Name = x.PromotionPolicy.Name,
                    },
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).ToList();
            Promotion.PromotionSamePrices = Promotion_PromotionDTO.PromotionSamePrices?
                .Select(x => new PromotionSamePrice
                {
                    Id = x.Id,
                    Note = x.Note,
                    Name = x.Name,
                    Price = x.Price,
                }).ToList();
            Promotion.PromotionStoreGroupingMappings = Promotion_PromotionDTO.PromotionStoreGroupingMappings?
                .Select(x => new PromotionStoreGroupingMapping
                {
                    StoreGroupingId = x.StoreGroupingId,
                    StoreGrouping = x.StoreGrouping == null ? null : new StoreGrouping
                    {
                        Id = x.StoreGrouping.Id,
                        Code = x.StoreGrouping.Code,
                        Name = x.StoreGrouping.Name,
                        ParentId = x.StoreGrouping.ParentId,
                        Path = x.StoreGrouping.Path,
                        Level = x.StoreGrouping.Level,
                        StatusId = x.StoreGrouping.StatusId,
                    },
                }).ToList();
            Promotion.PromotionStoreGroupings = Promotion_PromotionDTO.PromotionStoreGroupings?
                .Select(x => new PromotionStoreGrouping
                {
                    Id = x.Id,
                    Note = x.Note,
                    FromValue = x.FromValue,
                    ToValue = x.ToValue,
                    PromotionDiscountTypeId = x.PromotionDiscountTypeId,
                    DiscountPercentage = x.DiscountPercentage,
                    DiscountValue = x.DiscountValue,
                    PromotionDiscountType = x.PromotionDiscountType == null ? null : new PromotionDiscountType
                    {
                        Id = x.PromotionDiscountType.Id,
                        Code = x.PromotionDiscountType.Code,
                        Name = x.PromotionDiscountType.Name,
                    },
                }).ToList();
            Promotion.PromotionStoreMappings = Promotion_PromotionDTO.PromotionStoreMappings?
                .Select(x => new PromotionStoreMapping
                {
                    StoreId = x.StoreId,
                    Store = x.Store == null ? null : new Store
                    {
                        Id = x.Store.Id,
                        Code = x.Store.Code,
                        Name = x.Store.Name,
                        UnsignName = x.Store.UnsignName,
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
                        UnsignAddress = x.Store.UnsignAddress,
                        DeliveryAddress = x.Store.DeliveryAddress,
                        Latitude = x.Store.Latitude,
                        Longitude = x.Store.Longitude,
                        DeliveryLatitude = x.Store.DeliveryLatitude,
                        DeliveryLongitude = x.Store.DeliveryLongitude,
                        OwnerName = x.Store.OwnerName,
                        OwnerPhone = x.Store.OwnerPhone,
                        OwnerEmail = x.Store.OwnerEmail,
                        TaxCode = x.Store.TaxCode,
                        LegalEntity = x.Store.LegalEntity,
                        StatusId = x.Store.StatusId,
                        Used = x.Store.Used,
                        StoreScoutingId = x.Store.StoreScoutingId,
                    },
                }).ToList();
            Promotion.PromotionStoreTypeMappings = Promotion_PromotionDTO.PromotionStoreTypeMappings?
                .Select(x => new PromotionStoreTypeMapping
                {
                    StoreTypeId = x.StoreTypeId,
                    StoreType = x.StoreType == null ? null : new StoreType
                    {
                        Id = x.StoreType.Id,
                        Code = x.StoreType.Code,
                        Name = x.StoreType.Name,
                        ColorId = x.StoreType.ColorId,
                        StatusId = x.StoreType.StatusId,
                        Used = x.StoreType.Used,
                    },
                }).ToList();
            Promotion.PromotionStoreTypes = Promotion_PromotionDTO.PromotionStoreTypes?
                .Select(x => new PromotionStoreType
                {
                    Id = x.Id,
                    Note = x.Note,
                    FromValue = x.FromValue,
                    ToValue = x.ToValue,
                    PromotionDiscountTypeId = x.PromotionDiscountTypeId,
                    DiscountPercentage = x.DiscountPercentage,
                    DiscountValue = x.DiscountValue,
                    PromotionDiscountType = x.PromotionDiscountType == null ? null : new PromotionDiscountType
                    {
                        Id = x.PromotionDiscountType.Id,
                        Code = x.PromotionDiscountType.Code,
                        Name = x.PromotionDiscountType.Name,
                    },
                }).ToList();
            Promotion.PromotionStores = Promotion_PromotionDTO.PromotionStores?
                .Select(x => new PromotionStore
                {
                    Id = x.Id,
                    Note = x.Note,
                    FromValue = x.FromValue,
                    ToValue = x.ToValue,
                    PromotionDiscountTypeId = x.PromotionDiscountTypeId,
                    DiscountPercentage = x.DiscountPercentage,
                    DiscountValue = x.DiscountValue,
                    PromotionDiscountType = x.PromotionDiscountType == null ? null : new PromotionDiscountType
                    {
                        Id = x.PromotionDiscountType.Id,
                        Code = x.PromotionDiscountType.Code,
                        Name = x.PromotionDiscountType.Name,
                    },
                }).ToList();
            Promotion.BaseLanguage = CurrentContext.Language;
            return Promotion;
        }

        private PromotionFilter ConvertFilterDTOToFilterEntity(Promotion_PromotionFilterDTO Promotion_PromotionFilterDTO)
        {
            PromotionFilter PromotionFilter = new PromotionFilter();
            PromotionFilter.Selects = PromotionSelect.ALL;
            PromotionFilter.Skip = Promotion_PromotionFilterDTO.Skip;
            PromotionFilter.Take = Promotion_PromotionFilterDTO.Take;
            PromotionFilter.OrderBy = Promotion_PromotionFilterDTO.OrderBy;
            PromotionFilter.OrderType = Promotion_PromotionFilterDTO.OrderType;

            PromotionFilter.Id = Promotion_PromotionFilterDTO.Id;
            PromotionFilter.Code = Promotion_PromotionFilterDTO.Code;
            PromotionFilter.Name = Promotion_PromotionFilterDTO.Name;
            PromotionFilter.StartDate = Promotion_PromotionFilterDTO.StartDate;
            PromotionFilter.EndDate = Promotion_PromotionFilterDTO.EndDate;
            PromotionFilter.OrganizationId = Promotion_PromotionFilterDTO.OrganizationId;
            PromotionFilter.PromotionTypeId = Promotion_PromotionFilterDTO.PromotionTypeId;
            PromotionFilter.Note = Promotion_PromotionFilterDTO.Note;
            PromotionFilter.Priority = Promotion_PromotionFilterDTO.Priority;
            PromotionFilter.StatusId = Promotion_PromotionFilterDTO.StatusId;
            PromotionFilter.CreatedAt = Promotion_PromotionFilterDTO.CreatedAt;
            PromotionFilter.UpdatedAt = Promotion_PromotionFilterDTO.UpdatedAt;
            return PromotionFilter;
        }
    }
}

