using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MKpiCriteriaItem;
using DMS.Services.MKpiCriteriaTotal;
using DMS.Services.MKpiItem;
using DMS.Services.MKpiItemContent;
using DMS.Services.MKpiPeriod;
using DMS.Services.MKpiYear;
using DMS.Services.MOrganization;
using DMS.Services.MProduct;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;
using DMS.Services.MStatus;
using DMS.Services.MSupplier;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NGS.Templater;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_item
{
    public class KpiItemController : RpcController
    {
        private IAppUserService AppUserService;
        private IKpiPeriodService KpiPeriodService;
        private IKpiYearService KpiYearService;
        private IOrganizationService OrganizationService;
        private IKpiCriteriaItemService KpiCriteriaItemService;
        private IKpiCriteriaTotalService KpiCriteriaTotalService;
        private IStatusService StatusService;
        private IItemService ItemService;
        private IKpiItemContentService KpiItemContentService;
        private IKpiItemService KpiItemService;
        private ISupplierService SupplierService;
        private IProductTypeService ProductTypeService;
        private IProductGroupingService ProductGroupingService;
        private ICurrentContext CurrentContext;
        public KpiItemController(
            IAppUserService AppUserService,
            IKpiPeriodService KpiPeriodService,
            IKpiYearService KpiYearService,
            IOrganizationService OrganizationService,
            IKpiCriteriaItemService KpiCriteriaItemService,
            IKpiCriteriaTotalService KpiCriteriaTotalService,
            IStatusService StatusService,
            IItemService ItemService,
            IKpiItemContentService KpiItemContentService,
            IKpiItemService KpiItemService,
            ISupplierService SupplierService,
            IProductTypeService ProductTypeService,
            IProductGroupingService ProductGroupingService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.KpiPeriodService = KpiPeriodService;
            this.KpiYearService = KpiYearService;
            this.OrganizationService = OrganizationService;
            this.KpiCriteriaItemService = KpiCriteriaItemService;
            this.KpiCriteriaTotalService = KpiCriteriaTotalService;
            this.StatusService = StatusService;
            this.ItemService = ItemService;
            this.KpiItemContentService = KpiItemContentService;
            this.KpiItemService = KpiItemService;
            this.SupplierService = SupplierService;
            this.ProductTypeService = ProductTypeService;
            this.ProductGroupingService = ProductGroupingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(KpiItemRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] KpiItem_KpiItemFilterDTO KpiItem_KpiItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiItemFilter KpiItemFilter = ConvertFilterDTOToFilterEntity(KpiItem_KpiItemFilterDTO);
            KpiItemFilter = await KpiItemService.ToFilter(KpiItemFilter);
            int count = await KpiItemService.Count(KpiItemFilter);
            return count;
        }

        [Route(KpiItemRoute.List), HttpPost]
        public async Task<List<KpiItem_KpiItemDTO>> List([FromBody] KpiItem_KpiItemFilterDTO KpiItem_KpiItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiItemFilter KpiItemFilter = ConvertFilterDTOToFilterEntity(KpiItem_KpiItemFilterDTO);
            KpiItemFilter = await KpiItemService.ToFilter(KpiItemFilter);
            List<KpiItem> KpiItems = await KpiItemService.List(KpiItemFilter);
            List<KpiItem_KpiItemDTO> KpiItem_KpiItemDTOs = KpiItems
                .Select(c => new KpiItem_KpiItemDTO(c)).ToList();
            return KpiItem_KpiItemDTOs;
        }

        [Route(KpiItemRoute.Get), HttpPost]
        public async Task<ActionResult<KpiItem_KpiItemDTO>> Get([FromBody]KpiItem_KpiItemDTO KpiItem_KpiItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiItem_KpiItemDTO.Id))
                return Forbid();

            KpiItem KpiItem = await KpiItemService.Get(KpiItem_KpiItemDTO.Id);
            return new KpiItem_KpiItemDTO(KpiItem);
        }

        [Route(KpiItemRoute.GetDraft), HttpPost]
        public async Task<ActionResult<KpiItem_KpiItemDTO>> GetDraft()
        {
            List<KpiCriteriaTotal> KpiCriteriaTotals = await KpiCriteriaTotalService.List(new KpiCriteriaTotalFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiCriteriaTotalSelect.ALL,
            });
            List<KpiCriteriaItem> KpiCriteriaItems = await KpiCriteriaItemService.List(new KpiCriteriaItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiCriteriaItemSelect.ALL,
            });
            var KpiItem_KpiItemDTO = new KpiItem_KpiItemDTO();
            KpiItem_KpiItemDTO.KpiCriteriaTotals = KpiCriteriaTotals.Select(x => new KpiItem_KpiCriteriaTotalDTO(x)).ToList();
            KpiItem_KpiItemDTO.KpiItemKpiCriteriaTotalMappings = KpiCriteriaTotals.ToDictionary(x => x.Id, y => 0L);
            KpiItem_KpiItemDTO.KpiCriteriaItems = KpiCriteriaItems.Select(x => new KpiItem_KpiCriteriaItemDTO(x)).ToList();
            KpiItem_KpiItemDTO.Status = new KpiItem_StatusDTO
            {
                Code = Enums.StatusEnum.ACTIVE.Code,
                Id = Enums.StatusEnum.ACTIVE.Id,
                Name = Enums.StatusEnum.ACTIVE.Name
            };
            KpiItem_KpiItemDTO.StatusId = Enums.StatusEnum.ACTIVE.Id;
            return KpiItem_KpiItemDTO;
        }

        [Route(KpiItemRoute.Create), HttpPost]
        public async Task<ActionResult<KpiItem_KpiItemDTO>> Create([FromBody] KpiItem_KpiItemDTO KpiItem_KpiItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiItem_KpiItemDTO.Id))
                return Forbid();

            KpiItem KpiItem = ConvertDTOToEntity(KpiItem_KpiItemDTO);
            KpiItem = await KpiItemService.Create(KpiItem);
            KpiItem_KpiItemDTO = new KpiItem_KpiItemDTO(KpiItem);
            if (KpiItem.IsValidated)
                return KpiItem_KpiItemDTO;
            else
                return BadRequest(KpiItem_KpiItemDTO);
        }

        [Route(KpiItemRoute.Update), HttpPost]
        public async Task<ActionResult<KpiItem_KpiItemDTO>> Update([FromBody] KpiItem_KpiItemDTO KpiItem_KpiItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiItem_KpiItemDTO.Id))
                return Forbid();

            KpiItem KpiItem = ConvertDTOToEntity(KpiItem_KpiItemDTO);
            KpiItem = await KpiItemService.Update(KpiItem);
            KpiItem_KpiItemDTO = new KpiItem_KpiItemDTO(KpiItem);
            if (KpiItem.IsValidated)
                return KpiItem_KpiItemDTO;
            else
                return BadRequest(KpiItem_KpiItemDTO);
        }

        [Route(KpiItemRoute.Delete), HttpPost]
        public async Task<ActionResult<KpiItem_KpiItemDTO>> Delete([FromBody] KpiItem_KpiItemDTO KpiItem_KpiItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiItem_KpiItemDTO.Id))
                return Forbid();

            KpiItem KpiItem = ConvertDTOToEntity(KpiItem_KpiItemDTO);
            KpiItem = await KpiItemService.Delete(KpiItem);
            KpiItem_KpiItemDTO = new KpiItem_KpiItemDTO(KpiItem);
            if (KpiItem.IsValidated)
                return KpiItem_KpiItemDTO;
            else
                return BadRequest(KpiItem_KpiItemDTO);
        }

        [Route(KpiItemRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiItemFilter KpiItemFilter = new KpiItemFilter();
            KpiItemFilter = await KpiItemService.ToFilter(KpiItemFilter);
            KpiItemFilter.Id = new IdFilter { In = Ids };
            KpiItemFilter.Selects = KpiItemSelect.Id;
            KpiItemFilter.Skip = 0;
            KpiItemFilter.Take = int.MaxValue;

            List<KpiItem> KpiItems = await KpiItemService.List(KpiItemFilter);
            KpiItems = await KpiItemService.BulkDelete(KpiItems);
            return true;
        }

        [Route(KpiItemRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            FileInfo FileInfo = new FileInfo(file.FileName);
            if (!FileInfo.Extension.Equals(".xlsx"))
                return BadRequest("Định dạng file không hợp lệ");

            var AppUser = await AppUserService.Get(CurrentContext.UserId);

            AppUserFilter EmployeeFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName,
                OrganizationId = new IdFilter { Equal = AppUser.OrganizationId }
            };
            List<AppUser> Employees = await AppUserService.List(EmployeeFilter);

            List<Item> Items = await ItemService.List(new ItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSelect.Id | ItemSelect.Code | ItemSelect.Name,
                OrderBy = ItemOrder.Id,
                OrderType = OrderType.ASC
            });

            List<KpiItem> KpiItems = new List<KpiItem>();
            StringBuilder errorContent = new StringBuilder();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["KPI san pham"];
                if (worksheet == null)
                    return BadRequest("File không đúng biểu mẫu import");

                int StartColumn = 1;
                int StartRow = 5;
                int UsernameColumn = 0 + StartColumn;
                int DisplayNameColumn = 1 + StartColumn;
                int ItemCodeColumn = 2 + StartColumn;
                int QuantityColumn = 3 + StartColumn;
                int RevenueColumn = 4 + StartColumn;
                int SaleOrderColumn = 5 + StartColumn;
                int StoreColumn = 6 + StartColumn;

                string KpiPeriodValue = worksheet.Cells[2, 4].Value?.ToString();
                GenericEnum KpiPeriod;
                if (long.TryParse(KpiPeriodValue, out long KpiPeriodId))
                    KpiPeriod = KpiPeriodEnum.KpiPeriodEnumList.Where(x => x.Id == KpiPeriodId).FirstOrDefault();
                else
                    return BadRequest("Kỳ Kpi không hợp lệ");

                string KpiYearValue = worksheet.Cells[2, 6].Value?.ToString();
                GenericEnum KpiYear;
                if (long.TryParse(KpiYearValue, out long KpiYearId))
                    KpiYear = KpiYearEnum.KpiYearEnumList.Where(x => x.Id == KpiYearId).FirstOrDefault();
                else
                    return BadRequest("Năm Kpi không hợp lệ");

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    string UsernameValue = worksheet.Cells[i + StartRow, UsernameColumn].Value?.ToString();
                    string ItemCodeValue = worksheet.Cells[i + StartRow, ItemCodeColumn].Value?.ToString();
                    if (UsernameValue != null && UsernameValue.ToLower() == "END".ToLower())
                        break;
                    else if (!string.IsNullOrWhiteSpace(UsernameValue) && string.IsNullOrWhiteSpace(ItemCodeValue))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(UsernameValue) && i != worksheet.Dimension.End.Row)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Chưa nhập mã nhân viên");
                        continue;
                    }
                    else if (string.IsNullOrWhiteSpace(UsernameValue) && i == worksheet.Dimension.End.Row)
                        break;

                    Item Item;
                    if (!string.IsNullOrWhiteSpace(ItemCodeValue))
                    {
                        Item = Items.Where(x => x.Code == ItemCodeValue.Trim()).FirstOrDefault();
                        if (Item == null)
                        {
                            errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Sản phẩm không tồn tại");
                            continue;
                        }
                    }

                    string QuantityValue = worksheet.Cells[i + StartRow, QuantityColumn].Value?.ToString();
                    string RevenueValue = worksheet.Cells[i + StartRow, RevenueColumn].Value?.ToString();
                    string SalesOrderValue = worksheet.Cells[i + StartRow, SaleOrderColumn].Value?.ToString();
                    string StoreValue = worksheet.Cells[i + StartRow, StoreColumn].Value?.ToString();

                    AppUser Employee = Employees.Where(x => x.Username == UsernameValue).FirstOrDefault();
                    if (Employee == null)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Nhân viên không tồn tại hoặc không thuộc {AppUser.Organization?.Name}");
                        continue;
                    }
                    KpiItem KpiItem = KpiItems.Where(x => x.EmployeeId == Employee.Id).FirstOrDefault();
                    if(KpiItem == null)
                    {
                        KpiItem = new KpiItem();
                        KpiItem.KpiItemContents = new List<KpiItemContent>();
                        KpiItem.CreatorId = AppUser == null ? 0 : AppUser.Id;
                        KpiItem.Creator = AppUser;
                        KpiItem.OrganizationId = AppUser.OrganizationId.Value;
                        KpiItem.EmployeeId = Employee == null ? 0 : Employee.Id;
                        KpiItem.Employee = Employee;
                        KpiItem.KpiYearId = KpiYear == null ? 0 : KpiYear.Id;
                        KpiItem.KpiPeriodId = KpiPeriod == null ? 0 : KpiPeriod.Id;
                        KpiItem.StatusId = StatusEnum.ACTIVE.Id;
                        KpiItems.Add(KpiItem);
                    }
                    Item = Items.Where(x => x.Code == ItemCodeValue.Trim()).FirstOrDefault();
                    KpiItemContent KpiItemContent = KpiItem.KpiItemContents.Where(x => x.ItemId == Item.Id).FirstOrDefault();
                    if (KpiItemContent == null)
                    {
                        KpiItemContent = new KpiItemContent();
                        KpiItemContent.ItemId = Item.Id;
                        KpiItemContent.KpiItemContentKpiCriteriaItemMappings = new List<KpiItemContentKpiCriteriaItemMapping>();
                        KpiItem.KpiItemContents.Add(KpiItemContent);
                    }

                    #region Sản lượng theo đơn hàng gián tiếp
                    KpiItemContentKpiCriteriaItemMapping
                            IndirectQuantityCriterial = KpiItemContent.KpiItemContentKpiCriteriaItemMappings
                            .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_QUANTITY.Id)
                            .FirstOrDefault();
                    if (IndirectQuantityCriterial == null)
                    {
                        IndirectQuantityCriterial = new KpiItemContentKpiCriteriaItemMapping();
                        IndirectQuantityCriterial.KpiCriteriaItemId = KpiCriteriaItemEnum.INDIRECT_QUANTITY.Id;
                        KpiItemContent.KpiItemContentKpiCriteriaItemMappings.Add(IndirectQuantityCriterial);
                    }
                    if (long.TryParse(QuantityValue, out long Quantity))
                        IndirectQuantityCriterial.Value = Quantity;
                    else
                        IndirectQuantityCriterial.Value = null;
                    #endregion

                    #region Doanh thu theo đơn hàng gián tiếp
                    KpiItemContentKpiCriteriaItemMapping
                            IndirectRevenueCriterial = KpiItemContent.KpiItemContentKpiCriteriaItemMappings
                            .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_REVENUE.Id)
                            .FirstOrDefault();
                    if (IndirectRevenueCriterial == null)
                    {
                        IndirectRevenueCriterial = new KpiItemContentKpiCriteriaItemMapping();
                        IndirectRevenueCriterial.KpiCriteriaItemId = KpiCriteriaItemEnum.INDIRECT_REVENUE.Id;
                        KpiItemContent.KpiItemContentKpiCriteriaItemMappings.Add(IndirectRevenueCriterial);
                    }
                    if (long.TryParse(RevenueValue, out long Revenue))
                        IndirectRevenueCriterial.Value = Revenue;
                    else
                        IndirectRevenueCriterial.Value = null;
                    #endregion

                    #region Số đơn hàng gián tiếp
                    KpiItemContentKpiCriteriaItemMapping
                            IndirectCounterCriterial = KpiItemContent.KpiItemContentKpiCriteriaItemMappings
                            .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_AMOUNT.Id)
                            .FirstOrDefault();
                    if (IndirectCounterCriterial == null)
                    {
                        IndirectCounterCriterial = new KpiItemContentKpiCriteriaItemMapping();
                        IndirectCounterCriterial.KpiCriteriaItemId = KpiCriteriaItemEnum.INDIRECT_AMOUNT.Id;
                        KpiItemContent.KpiItemContentKpiCriteriaItemMappings.Add(IndirectCounterCriterial);
                    }
                    if (long.TryParse(RevenueValue, out long SalesOrder))
                        IndirectCounterCriterial.Value = SalesOrder;
                    else
                        IndirectCounterCriterial.Value = null;
                    #endregion

                    #region Số đại lý theo đơn gián tiếp
                    KpiItemContentKpiCriteriaItemMapping
                            IndirectStoreCriterial = KpiItemContent.KpiItemContentKpiCriteriaItemMappings
                            .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_STORE.Id)
                            .FirstOrDefault();
                    if (IndirectStoreCriterial == null)
                    {
                        IndirectStoreCriterial = new KpiItemContentKpiCriteriaItemMapping();
                        IndirectStoreCriterial.KpiCriteriaItemId = KpiCriteriaItemEnum.INDIRECT_STORE.Id;
                        KpiItemContent.KpiItemContentKpiCriteriaItemMappings.Add(IndirectStoreCriterial);
                    }
                    if (long.TryParse(RevenueValue, out long Store))
                        IndirectStoreCriterial.Value = Store;
                    else
                        IndirectStoreCriterial.Value = null;
                    #endregion

                    #region Sản lượng theo đơn hàng trực tiếp
                    KpiItemContentKpiCriteriaItemMapping
                            DirectQuantityCriterial = KpiItemContent.KpiItemContentKpiCriteriaItemMappings
                            .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_QUANTITY.Id)
                            .FirstOrDefault();
                    if (DirectQuantityCriterial == null)
                    {
                        DirectQuantityCriterial = new KpiItemContentKpiCriteriaItemMapping();
                        DirectQuantityCriterial.KpiCriteriaItemId = KpiCriteriaItemEnum.DIRECT_QUANTITY.Id;
                        KpiItemContent.KpiItemContentKpiCriteriaItemMappings.Add(DirectQuantityCriterial);
                    }
                    if (long.TryParse(QuantityValue, out long DirectQuantity))
                        DirectQuantityCriterial.Value = DirectQuantity;
                    else
                        DirectQuantityCriterial.Value = null;
                    #endregion

                    #region Doanh thu theo đơn hàng trực tiếp
                    KpiItemContentKpiCriteriaItemMapping
                            DirectRevenueCriterial = KpiItemContent.KpiItemContentKpiCriteriaItemMappings
                            .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_REVENUE.Id)
                            .FirstOrDefault();
                    if (DirectRevenueCriterial == null)
                    {
                        DirectRevenueCriterial = new KpiItemContentKpiCriteriaItemMapping();
                        DirectRevenueCriterial.KpiCriteriaItemId = KpiCriteriaItemEnum.DIRECT_REVENUE.Id;
                        KpiItemContent.KpiItemContentKpiCriteriaItemMappings.Add(DirectRevenueCriterial);
                    }
                    if (long.TryParse(RevenueValue, out long DirectRevenue))
                        DirectRevenueCriterial.Value = DirectRevenue;
                    else
                        DirectRevenueCriterial.Value = null;
                    #endregion

                    #region Số đơn hàng trực tiếp
                    KpiItemContentKpiCriteriaItemMapping
                            DirectCounterCriterial = KpiItemContent.KpiItemContentKpiCriteriaItemMappings
                            .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_AMOUNT.Id)
                            .FirstOrDefault();
                    if (DirectCounterCriterial == null)
                    {
                        DirectCounterCriterial = new KpiItemContentKpiCriteriaItemMapping();
                        DirectCounterCriterial.KpiCriteriaItemId = KpiCriteriaItemEnum.DIRECT_AMOUNT.Id;
                        KpiItemContent.KpiItemContentKpiCriteriaItemMappings.Add(DirectCounterCriterial);
                    }
                    if (long.TryParse(RevenueValue, out long DirectCounter))
                        DirectCounterCriterial.Value = DirectCounter;
                    else
                        DirectCounterCriterial.Value = null;
                    #endregion

                    #region Số đại lý theo đơn trực tiếp
                    KpiItemContentKpiCriteriaItemMapping
                            DirectStoreCriterial = KpiItemContent.KpiItemContentKpiCriteriaItemMappings
                            .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_STORE.Id)
                            .FirstOrDefault();
                    if (DirectStoreCriterial == null)
                    {
                        DirectStoreCriterial = new KpiItemContentKpiCriteriaItemMapping();
                        DirectStoreCriterial.KpiCriteriaItemId = KpiCriteriaItemEnum.DIRECT_STORE.Id;
                        KpiItemContent.KpiItemContentKpiCriteriaItemMappings.Add(DirectStoreCriterial);
                    }
                    if (long.TryParse(RevenueValue, out long DirectStore))
                        DirectStoreCriterial.Value = DirectStore;
                    else
                        DirectStoreCriterial.Value = null;
                    #endregion
                }
                if (errorContent.Length > 0)
                    return BadRequest(errorContent.ToString());
            }

            KpiItems = await KpiItemService.Import(KpiItems);
            List<KpiItem_KpiItemDTO> KpiItem_KpiItemDTOs = KpiItems
                .Select(c => new KpiItem_KpiItemDTO(c)).ToList();
            for (int i = 0; i < KpiItems.Count; i++)
            {
                if (!KpiItems[i].IsValidated)
                {
                    errorContent.Append($"Lỗi dòng thứ {i + 2}:");
                    foreach (var Error in KpiItems[i].Errors)
                    {
                        errorContent.Append($" {Error.Value},");
                    }
                    errorContent.AppendLine("");
                }
            }
            if (KpiItems.Any(s => !s.IsValidated))
                return BadRequest(errorContent.ToString());
            return Ok(KpiItem_KpiItemDTOs);
        }

        [Route(KpiItemRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] KpiItem_KpiItemFilterDTO KpiItem_KpiItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (KpiItem_KpiItemFilterDTO.KpiYearId.Equal.HasValue == false)
                return BadRequest("Chưa chọn năm Kpi");

            if (KpiItem_KpiItemFilterDTO.KpiPeriodId.Equal.HasValue == false)
                return BadRequest("Chưa chọn kỳ Kpi");

            long KpiYearId = KpiItem_KpiItemFilterDTO.KpiYearId.Equal.Value;
            var KpiYear = KpiYearEnum.KpiYearEnumList.Where(x => x.Id == KpiYearId).FirstOrDefault();

            long KpiPeriodId = KpiItem_KpiItemFilterDTO.KpiPeriodId.Equal.Value;
            var KpiPeriod = KpiPeriodEnum.KpiPeriodEnumList.Where(x => x.Id == KpiPeriodId).FirstOrDefault();


            KpiItem_KpiItemFilterDTO.Skip = 0;
            KpiItem_KpiItemFilterDTO.Take = int.MaxValue;
            KpiItem_KpiItemFilterDTO.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            List<KpiItem_KpiItemDTO> KpiItem_KpiItemDTOs = await List(KpiItem_KpiItemFilterDTO);
            var KpiItemIds = KpiItem_KpiItemDTOs.Select(x => x.Id).ToList();

            KpiItemContentFilter KpiItemContentFilter = new KpiItemContentFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiItemContentSelect.ALL,
                KpiItemId = new IdFilter { In = KpiItemIds }
            };
            List<KpiItemContent> KpiItemContents = await KpiItemContentService.List(KpiItemContentFilter);

            List<KpiItem_ExportDTO> KpiItem_ExportDTOs = new List<KpiItem_ExportDTO>();
            foreach (var KpiItem in KpiItem_KpiItemDTOs)
            {
                KpiItem_ExportDTO KpiItem_ExportDTO = new KpiItem_ExportDTO();
                KpiItem_ExportDTO.Username = KpiItem.Employee.Username;
                KpiItem_ExportDTO.DisplayName = KpiItem.Employee.DisplayName;

                KpiItem_ExportDTO.Contents = KpiItemContents.Where(x => x.KpiItemId == KpiItem.Id).Select(x => new KpiItem_ExportContentDTO
                {
                    ItemCode = x.Item.Code,
                    ItemName = x.Item.Name,
                    IndirectQuantity = x.KpiItemContentKpiCriteriaItemMappings
                    .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_QUANTITY.Id)
                    .Where(x => x.Value.HasValue)
                    .Select(x => x.Value.Value)
                    .Sum(),
                    IndirectRevenue = x.KpiItemContentKpiCriteriaItemMappings
                    .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_REVENUE.Id)
                    .Where(x => x.Value.HasValue)
                    .Select(x => x.Value.Value)
                    .Sum(),
                    IndirectCounter = x.KpiItemContentKpiCriteriaItemMappings
                    .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_AMOUNT.Id)
                    .Where(x => x.Value.HasValue)
                    .Select(x => x.Value.Value)
                    .Sum(),
                    IndirectStoreCounter = x.KpiItemContentKpiCriteriaItemMappings
                    .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_STORE.Id)
                    .Where(x => x.Value.HasValue)
                    .Select(x => x.Value.Value)
                    .Sum(),
                    DirectQuantity = x.KpiItemContentKpiCriteriaItemMappings
                    .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_QUANTITY.Id)
                    .Where(x => x.Value.HasValue)
                    .Select(x => x.Value.Value)
                    .Sum(),
                    DirectRevenue = x.KpiItemContentKpiCriteriaItemMappings
                    .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_REVENUE.Id)
                    .Where(x => x.Value.HasValue)
                    .Select(x => x.Value.Value)
                    .Sum(),
                    DirectCounter = x.KpiItemContentKpiCriteriaItemMappings
                    .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_AMOUNT.Id)
                    .Where(x => x.Value.HasValue)
                    .Select(x => x.Value.Value)
                    .Sum(),
                    DirectStoreCounter = x.KpiItemContentKpiCriteriaItemMappings
                    .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_STORE.Id)
                    .Where(x => x.Value.HasValue)
                    .Select(x => x.Value.Value)
                    .Sum(),
                }).ToList();
                KpiItem_ExportDTOs.Add(KpiItem_ExportDTO);
            }

            string path = "Templates/Kpi_Item_Export.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.KpiYear = KpiYear.Name;
            Data.KpiPeriod = KpiPeriod.Name;
            Data.KpiItems = KpiItem_ExportDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "KpiItems.xlsx");
        }

        [Route(KpiItemRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            List<Item> Items = await ItemService.List(new ItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSelect.Code | ItemSelect.Name,
                OrderBy = ItemOrder.Id,
                OrderType = OrderType.ASC
            });

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            MemoryStream MemoryStream = new MemoryStream();
            string tempPath = "Templates/Kpi_Item.xlsx";
            using (var xlPackage = new ExcelPackage(new FileInfo(tempPath)))
            {
                #region sheet Item 
                var worksheet = xlPackage.Workbook.Worksheets["San Pham"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow = 2;
                int numberCell = 1;
                for (var i = 0; i < Items.Count; i++)
                {
                    Item Item = Items[i];
                    worksheet.Cells[startRow + i, numberCell].Value = Item.Code;
                    worksheet.Cells[startRow + i, numberCell + 1].Value = Item.Name;
                }
                #endregion
                xlPackage.SaveAs(MemoryStream);
            }
            return File(MemoryStream.ToArray(), "application/octet-stream", "Template_Kpi_Item.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            KpiItemFilter KpiItemFilter = new KpiItemFilter();
            KpiItemFilter = await KpiItemService.ToFilter(KpiItemFilter);
            if (Id == 0)
            {

            }
            else
            {
                KpiItemFilter.Id = new IdFilter { Equal = Id };
                int count = await KpiItemService.Count(KpiItemFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private KpiItem ConvertDTOToEntity(KpiItem_KpiItemDTO KpiItem_KpiItemDTO)
        {
            KpiItem KpiItem = new KpiItem();
            KpiItem.Id = KpiItem_KpiItemDTO.Id;
            KpiItem.OrganizationId = KpiItem_KpiItemDTO.OrganizationId;
            KpiItem.KpiYearId = KpiItem_KpiItemDTO.KpiYearId;
            KpiItem.KpiPeriodId = KpiItem_KpiItemDTO.KpiPeriodId;
            KpiItem.StatusId = KpiItem_KpiItemDTO.StatusId;
            KpiItem.EmployeeId = KpiItem_KpiItemDTO.EmployeeId;
            KpiItem.CreatorId = KpiItem_KpiItemDTO.CreatorId;
            KpiItem.Creator = KpiItem_KpiItemDTO.Creator == null ? null : new AppUser
            {
                Id = KpiItem_KpiItemDTO.Creator.Id,
                Username = KpiItem_KpiItemDTO.Creator.Username,
                DisplayName = KpiItem_KpiItemDTO.Creator.DisplayName,
                Address = KpiItem_KpiItemDTO.Creator.Address,
                Email = KpiItem_KpiItemDTO.Creator.Email,
                Phone = KpiItem_KpiItemDTO.Creator.Phone,
            };
            KpiItem.Employee = KpiItem_KpiItemDTO.Employee == null ? null : new AppUser
            {
                Id = KpiItem_KpiItemDTO.Employee.Id,
                Username = KpiItem_KpiItemDTO.Employee.Username,
                DisplayName = KpiItem_KpiItemDTO.Employee.DisplayName,
                Address = KpiItem_KpiItemDTO.Employee.Address,
                Email = KpiItem_KpiItemDTO.Employee.Email,
                Phone = KpiItem_KpiItemDTO.Employee.Phone,
            };
            KpiItem.KpiYear = KpiItem_KpiItemDTO.KpiYear == null ? null : new KpiYear
            {
                Id = KpiItem_KpiItemDTO.KpiYear.Id,
                Code = KpiItem_KpiItemDTO.KpiYear.Code,
                Name = KpiItem_KpiItemDTO.KpiYear.Name,
            };
            KpiItem.KpiPeriod = KpiItem_KpiItemDTO.KpiPeriod == null ? null : new KpiPeriod
            {
                Id = KpiItem_KpiItemDTO.KpiPeriod.Id,
                Code = KpiItem_KpiItemDTO.KpiPeriod.Code,
                Name = KpiItem_KpiItemDTO.KpiPeriod.Name,
            };
            KpiItem.Organization = KpiItem_KpiItemDTO.Organization == null ? null : new Organization
            {
                Id = KpiItem_KpiItemDTO.Organization.Id,
                Code = KpiItem_KpiItemDTO.Organization.Code,
                Name = KpiItem_KpiItemDTO.Organization.Name,
                ParentId = KpiItem_KpiItemDTO.Organization.ParentId,
            };
            KpiItem.Status = KpiItem_KpiItemDTO.Status == null ? null : new Status
            {
                Id = KpiItem_KpiItemDTO.Status.Id,
                Code = KpiItem_KpiItemDTO.Status.Code,
                Name = KpiItem_KpiItemDTO.Status.Name,
            };
            KpiItem.EmployeeIds = KpiItem_KpiItemDTO.EmployeeIds; // to do
            KpiItem.KpiItemContents = KpiItem_KpiItemDTO.KpiItemContents?
                .Select(x => new KpiItemContent
                {
                    Id = x.Id,
                    ItemId = x.ItemId,
                    Item = x.Item == null ? null : new Item
                    {
                        Id = x.Item.Id,
                        ProductId = x.Item.ProductId,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ScanCode = x.Item.ScanCode,
                        StatusId = x.Item.StatusId,
                    },
                    KpiItemContentKpiCriteriaItemMappings = x.KpiItemContentKpiCriteriaItemMappings.Select(p => new KpiItemContentKpiCriteriaItemMapping
                    {
                        KpiCriteriaItemId = p.Key,
                        Value = p.Value,
                    }).ToList(),
                }).ToList();
            KpiItem.KpiItemKpiCriteriaTotalMappings = KpiItem_KpiItemDTO.KpiItemKpiCriteriaTotalMappings?.Select(p => new KpiItemKpiCriteriaTotalMapping
            {
                KpiCriteriaTotalId = p.Key,
                Value = p.Value
            }).ToList();
            KpiItem.BaseLanguage = CurrentContext.Language;
            return KpiItem;
        }

        private KpiItemFilter ConvertFilterDTOToFilterEntity(KpiItem_KpiItemFilterDTO KpiItem_KpiItemFilterDTO)
        {
            KpiItemFilter KpiItemFilter = new KpiItemFilter();
            KpiItemFilter.Selects = KpiItemSelect.ALL;
            KpiItemFilter.Skip = KpiItem_KpiItemFilterDTO.Skip;
            KpiItemFilter.Take = KpiItem_KpiItemFilterDTO.Take;
            KpiItemFilter.OrderBy = KpiItem_KpiItemFilterDTO.OrderBy;
            KpiItemFilter.OrderType = KpiItem_KpiItemFilterDTO.OrderType;

            KpiItemFilter.Id = KpiItem_KpiItemFilterDTO.Id;
            KpiItemFilter.OrganizationId = KpiItem_KpiItemFilterDTO.OrganizationId;
            KpiItemFilter.KpiYearId = KpiItem_KpiItemFilterDTO.KpiYearId;
            KpiItemFilter.KpiPeriodId = KpiItem_KpiItemFilterDTO.KpiPeriodId;
            KpiItemFilter.StatusId = KpiItem_KpiItemFilterDTO.StatusId;
            KpiItemFilter.AppUserId = KpiItem_KpiItemFilterDTO.AppUserId;
            KpiItemFilter.CreatorId = KpiItem_KpiItemFilterDTO.CreatorId;
            KpiItemFilter.CreatedAt = KpiItem_KpiItemFilterDTO.CreatedAt;
            KpiItemFilter.UpdatedAt = KpiItem_KpiItemFilterDTO.UpdatedAt;
            return KpiItemFilter;
        }

        [Route(KpiItemRoute.FilterListAppUser), HttpPost]
        public async Task<List<KpiItem_AppUserDTO>> FilterListAppUser([FromBody] KpiItem_AppUserFilterDTO KpiItem_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = KpiItem_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiItem_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiItem_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = KpiItem_AppUserFilterDTO.Address;
            AppUserFilter.Email = KpiItem_AppUserFilterDTO.Email;

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiItem_AppUserDTO> KpiItem_AppUserDTOs = AppUsers
                .Select(x => new KpiItem_AppUserDTO(x)).ToList();
            return KpiItem_AppUserDTOs;
        }
        [Route(KpiItemRoute.FilterListCreator), HttpPost]
        public async Task<List<KpiItem_AppUserDTO>> FilterListCreator([FromBody] KpiItem_AppUserFilterDTO KpiItem_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var appUser = await AppUserService.Get(CurrentContext.UserId);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = KpiItem_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiItem_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiItem_AppUserFilterDTO.DisplayName;
            AppUserFilter.OrganizationId = new IdFilter { Equal = appUser.OrganizationId };

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiItem_AppUserDTO> KpiItem_AppUserDTOs = AppUsers
                .Select(x => new KpiItem_AppUserDTO(x)).ToList();
            return KpiItem_AppUserDTOs;
        }
        [Route(KpiItemRoute.FilterListKpiPeriod), HttpPost]
        public async Task<List<KpiItem_KpiPeriodDTO>> FilterListKpiPeriod([FromBody] KpiItem_KpiPeriodFilterDTO KpiItem_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter.Skip = 0;
            KpiPeriodFilter.Take = 20;
            KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
            KpiPeriodFilter.OrderType = OrderType.ASC;
            KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
            KpiPeriodFilter.Id = KpiItem_KpiPeriodFilterDTO.Id;
            KpiPeriodFilter.Code = KpiItem_KpiPeriodFilterDTO.Code;
            KpiPeriodFilter.Name = KpiItem_KpiPeriodFilterDTO.Name;

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            List<KpiItem_KpiPeriodDTO> KpiItem_KpiPeriodDTOs = KpiPeriods
                .Select(x => new KpiItem_KpiPeriodDTO(x)).ToList();
            return KpiItem_KpiPeriodDTOs;
        }

        [Route(KpiItemRoute.FilterListKpiYear), HttpPost]
        public async Task<List<KpiItem_KpiYearDTO>> FilterListKpiYear([FromBody] KpiItem_KpiYearFilterDTO KpiItem_KpiYearFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiYearFilter KpiYearFilter = new KpiYearFilter();
            KpiYearFilter.Skip = 0;
            KpiYearFilter.Take = 20;
            KpiYearFilter.OrderBy = KpiYearOrder.Id;
            KpiYearFilter.OrderType = OrderType.ASC;
            KpiYearFilter.Selects = KpiYearSelect.ALL;
            KpiYearFilter.Id = KpiItem_KpiYearFilterDTO.Id;
            KpiYearFilter.Code = KpiItem_KpiYearFilterDTO.Code;
            KpiYearFilter.Name = KpiItem_KpiYearFilterDTO.Name;

            List<KpiYear> KpiYears = await KpiYearService.List(KpiYearFilter);
            List<KpiItem_KpiYearDTO> KpiItem_KpiYearDTOs = KpiYears
                .Select(x => new KpiItem_KpiYearDTO(x)).ToList();
            return KpiItem_KpiYearDTOs;
        }

        [Route(KpiItemRoute.FilterListOrganization), HttpPost]
        public async Task<List<KpiItem_OrganizationDTO>> FilterListOrganization([FromBody] KpiItem_OrganizationFilterDTO KpiItem_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = KpiItem_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = KpiItem_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = KpiItem_OrganizationFilterDTO.Name;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<KpiItem_OrganizationDTO> KpiItem_OrganizationDTOs = Organizations
                .Select(x => new KpiItem_OrganizationDTO(x)).ToList();
            return KpiItem_OrganizationDTOs;
        }
        [Route(KpiItemRoute.FilterListStatus), HttpPost]
        public async Task<List<KpiItem_StatusDTO>> FilterListStatus()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<KpiItem_StatusDTO> KpiItem_StatusDTOs = Statuses
                .Select(x => new KpiItem_StatusDTO(x)).ToList();
            return KpiItem_StatusDTOs;
        }

        [Route(KpiItemRoute.FilterListKpiCriteriaItem), HttpPost]
        public async Task<List<KpiItem_KpiCriteriaItemDTO>> FilterListKpiCriteriaItem()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiCriteriaItemFilter KpiCriteriaItemFilter = new KpiCriteriaItemFilter();
            KpiCriteriaItemFilter.Skip = 0;
            KpiCriteriaItemFilter.Take = int.MaxValue;
            KpiCriteriaItemFilter.Take = 20;
            KpiCriteriaItemFilter.OrderBy = KpiCriteriaItemOrder.Id;
            KpiCriteriaItemFilter.OrderType = OrderType.ASC;
            KpiCriteriaItemFilter.Selects = KpiCriteriaItemSelect.ALL;

            List<KpiCriteriaItem> KpiCriteriaItemes = await KpiCriteriaItemService.List(KpiCriteriaItemFilter);
            List<KpiItem_KpiCriteriaItemDTO> KpiItem_KpiCriteriaItemDTOs = KpiCriteriaItemes
                .Select(x => new KpiItem_KpiCriteriaItemDTO(x)).ToList();
            return KpiItem_KpiCriteriaItemDTOs;
        }

        [Route(KpiItemRoute.FilterListKpiCriteriaTotal), HttpPost]
        public async Task<List<KpiItem_KpiCriteriaTotalDTO>> FilterListKpiCriteriaTotal()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiCriteriaTotalFilter KpiCriteriaTotalFilter = new KpiCriteriaTotalFilter();
            KpiCriteriaTotalFilter.Skip = 0;
            KpiCriteriaTotalFilter.Take = int.MaxValue;
            KpiCriteriaTotalFilter.Take = 20;
            KpiCriteriaTotalFilter.OrderBy = KpiCriteriaTotalOrder.Id;
            KpiCriteriaTotalFilter.OrderType = OrderType.ASC;
            KpiCriteriaTotalFilter.Selects = KpiCriteriaTotalSelect.ALL;

            List<KpiCriteriaTotal> KpiCriteriaTotales = await KpiCriteriaTotalService.List(KpiCriteriaTotalFilter);
            List<KpiItem_KpiCriteriaTotalDTO> KpiItem_KpiCriteriaTotalDTOs = KpiCriteriaTotales
                .Select(x => new KpiItem_KpiCriteriaTotalDTO(x)).ToList();
            return KpiItem_KpiCriteriaTotalDTOs;
        }

        [Route(KpiItemRoute.FilterListItem), HttpPost]
        public async Task<List<KpiItem_ItemDTO>> FilterListItem([FromBody] KpiItem_ItemFilterDTO KpiItem_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = KpiItem_ItemFilterDTO.Id;
            ItemFilter.ProductId = KpiItem_ItemFilterDTO.ProductId;
            ItemFilter.Code = KpiItem_ItemFilterDTO.Code;
            ItemFilter.Name = KpiItem_ItemFilterDTO.Name;
            ItemFilter.ScanCode = KpiItem_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = KpiItem_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = KpiItem_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = KpiItem_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<KpiItem_ItemDTO> KpiItem_ItemDTOs = Items
                .Select(x => new KpiItem_ItemDTO(x)).ToList();
            return KpiItem_ItemDTOs;
        }

        [Route(KpiItemRoute.FilterListSupplier), HttpPost]
        public async Task<List<KpiItem_SupplierDTO>> FilterListSupplier([FromBody] KpiItem_SupplierFilterDTO KpiItem_SupplierFilterDTO)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Skip = 0;
            SupplierFilter.Take = 20;
            SupplierFilter.OrderBy = SupplierOrder.Id;
            SupplierFilter.OrderType = OrderType.ASC;
            SupplierFilter.Selects = SupplierSelect.ALL;
            SupplierFilter.Id = KpiItem_SupplierFilterDTO.Id;
            SupplierFilter.Code = KpiItem_SupplierFilterDTO.Code;
            SupplierFilter.Name = KpiItem_SupplierFilterDTO.Name;
            SupplierFilter.TaxCode = KpiItem_SupplierFilterDTO.TaxCode;

            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            List<KpiItem_SupplierDTO> KpiItem_SupplierDTOs = Suppliers
                .Select(x => new KpiItem_SupplierDTO(x)).ToList();
            return KpiItem_SupplierDTOs;
        }
        [Route(KpiItemRoute.FilterListProductType), HttpPost]
        public async Task<List<KpiItem_ProductTypeDTO>> FilterListProductType([FromBody] KpiItem_ProductTypeFilterDTO KpiItem_ProductTypeFilterDTO)
        {
            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Skip = 0;
            ProductTypeFilter.Take = 20;
            ProductTypeFilter.OrderBy = ProductTypeOrder.Id;
            ProductTypeFilter.OrderType = OrderType.ASC;
            ProductTypeFilter.Selects = ProductTypeSelect.ALL;
            ProductTypeFilter.Id = KpiItem_ProductTypeFilterDTO.Id;
            ProductTypeFilter.Code = KpiItem_ProductTypeFilterDTO.Code;
            ProductTypeFilter.Name = KpiItem_ProductTypeFilterDTO.Name;
            ProductTypeFilter.Description = KpiItem_ProductTypeFilterDTO.Description;

            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            List<KpiItem_ProductTypeDTO> KpiItem_ProductTypeDTOs = ProductTypes
                .Select(x => new KpiItem_ProductTypeDTO(x)).ToList();
            return KpiItem_ProductTypeDTOs;
        }

        [Route(KpiItemRoute.FilterListProductGrouping), HttpPost]
        public async Task<List<KpiItem_ProductGroupingDTO>> FilterListProductGrouping([FromBody] KpiItem_ProductGroupingFilterDTO KpiItem_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code
                | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<KpiItem_ProductGroupingDTO> KpiItem_ProductGroupingDTOs = ProductGroupings
                .Select(x => new KpiItem_ProductGroupingDTO(x)).ToList();
            return KpiItem_ProductGroupingDTOs;
        }

        [Route(KpiItemRoute.SingleListAppUser), HttpPost]
        public async Task<List<KpiItem_AppUserDTO>> SingleListAppUser([FromBody] KpiItem_AppUserFilterDTO KpiItem_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = KpiItem_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiItem_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiItem_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = KpiItem_AppUserFilterDTO.Address;
            AppUserFilter.Email = KpiItem_AppUserFilterDTO.Email;

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiItem_AppUserDTO> KpiItem_AppUserDTOs = AppUsers
                .Select(x => new KpiItem_AppUserDTO(x)).ToList();
            return KpiItem_AppUserDTOs;
        }
        [Route(KpiItemRoute.SingleListKpiPeriod), HttpPost]
        public async Task<List<KpiItem_KpiPeriodDTO>> SingleListKpiPeriod([FromBody] KpiItem_KpiPeriodFilterDTO KpiItem_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter.Skip = 0;
            KpiPeriodFilter.Take = 20;
            KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
            KpiPeriodFilter.OrderType = OrderType.ASC;
            KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
            KpiPeriodFilter.Id = KpiItem_KpiPeriodFilterDTO.Id;
            KpiPeriodFilter.Code = KpiItem_KpiPeriodFilterDTO.Code;
            KpiPeriodFilter.Name = KpiItem_KpiPeriodFilterDTO.Name;

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            List<KpiItem_KpiPeriodDTO> KpiItem_KpiPeriodDTOs = KpiPeriods
                .Select(x => new KpiItem_KpiPeriodDTO(x)).ToList();
            return KpiItem_KpiPeriodDTOs;
        }
        [Route(KpiItemRoute.SingleListKpiYear), HttpPost]
        public async Task<List<KpiItem_KpiYearDTO>> SingleListKpiYear([FromBody] KpiItem_KpiYearFilterDTO KpiItem_KpiYearFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiYearFilter KpiYearFilter = new KpiYearFilter();
            KpiYearFilter.Skip = 0;
            KpiYearFilter.Take = 20;
            KpiYearFilter.OrderBy = KpiYearOrder.Id;
            KpiYearFilter.OrderType = OrderType.ASC;
            KpiYearFilter.Selects = KpiYearSelect.ALL;
            KpiYearFilter.Id = KpiItem_KpiYearFilterDTO.Id;
            KpiYearFilter.Code = KpiItem_KpiYearFilterDTO.Code;
            KpiYearFilter.Name = KpiItem_KpiYearFilterDTO.Name;

            List<KpiYear> KpiYears = await KpiYearService.List(KpiYearFilter);
            List<KpiItem_KpiYearDTO> KpiItem_KpiYearDTOs = KpiYears
                .Select(x => new KpiItem_KpiYearDTO(x)).ToList();
            return KpiItem_KpiYearDTOs;
        }
        [Route(KpiItemRoute.SingleListOrganization), HttpPost]
        public async Task<List<KpiItem_OrganizationDTO>> SingleListOrganization([FromBody] KpiItem_OrganizationFilterDTO KpiItem_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = KpiItem_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = KpiItem_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = KpiItem_OrganizationFilterDTO.Name;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<KpiItem_OrganizationDTO> KpiItem_OrganizationDTOs = Organizations
                .Select(x => new KpiItem_OrganizationDTO(x)).ToList();
            return KpiItem_OrganizationDTOs;
        }
        [Route(KpiItemRoute.SingleListStatus), HttpPost]
        public async Task<List<KpiItem_StatusDTO>> SingleListStatus()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<KpiItem_StatusDTO> KpiItem_StatusDTOs = Statuses
                .Select(x => new KpiItem_StatusDTO(x)).ToList();
            return KpiItem_StatusDTOs;
        }

        [Route(KpiItemRoute.SingleListKpiCriteriaItem), HttpPost]
        public async Task<List<KpiItem_KpiCriteriaItemDTO>> SingleListKpiCriteriaItem()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiCriteriaItemFilter KpiCriteriaItemFilter = new KpiCriteriaItemFilter();
            KpiCriteriaItemFilter.Skip = 0;
            KpiCriteriaItemFilter.Take = int.MaxValue;
            KpiCriteriaItemFilter.Take = 20;
            KpiCriteriaItemFilter.OrderBy = KpiCriteriaItemOrder.Id;
            KpiCriteriaItemFilter.OrderType = OrderType.ASC;
            KpiCriteriaItemFilter.Selects = KpiCriteriaItemSelect.ALL;

            List<KpiCriteriaItem> KpiCriteriaItemes = await KpiCriteriaItemService.List(KpiCriteriaItemFilter);
            List<KpiItem_KpiCriteriaItemDTO> KpiItem_KpiCriteriaItemDTOs = KpiCriteriaItemes
                .Select(x => new KpiItem_KpiCriteriaItemDTO(x)).ToList();
            return KpiItem_KpiCriteriaItemDTOs;
        }

        [Route(KpiItemRoute.SingleListKpiCriteriaTotal), HttpPost]
        public async Task<List<KpiItem_KpiCriteriaTotalDTO>> SingleListKpiCriteriaTotal()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiCriteriaTotalFilter KpiCriteriaTotalFilter = new KpiCriteriaTotalFilter();
            KpiCriteriaTotalFilter.Skip = 0;
            KpiCriteriaTotalFilter.Take = int.MaxValue;
            KpiCriteriaTotalFilter.Take = 20;
            KpiCriteriaTotalFilter.OrderBy = KpiCriteriaTotalOrder.Id;
            KpiCriteriaTotalFilter.OrderType = OrderType.ASC;
            KpiCriteriaTotalFilter.Selects = KpiCriteriaTotalSelect.ALL;

            List<KpiCriteriaTotal> KpiCriteriaTotales = await KpiCriteriaTotalService.List(KpiCriteriaTotalFilter);
            List<KpiItem_KpiCriteriaTotalDTO> KpiItem_KpiCriteriaTotalDTOs = KpiCriteriaTotales
                .Select(x => new KpiItem_KpiCriteriaTotalDTO(x)).ToList();
            return KpiItem_KpiCriteriaTotalDTOs;
        }

        [Route(KpiItemRoute.SingleListItem), HttpPost]
        public async Task<List<KpiItem_ItemDTO>> SingleListItem([FromBody] KpiItem_ItemFilterDTO KpiItem_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = KpiItem_ItemFilterDTO.Id;
            ItemFilter.ProductId = KpiItem_ItemFilterDTO.ProductId;
            ItemFilter.Code = KpiItem_ItemFilterDTO.Code;
            ItemFilter.Name = KpiItem_ItemFilterDTO.Name;
            ItemFilter.ScanCode = KpiItem_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = KpiItem_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = KpiItem_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = KpiItem_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<KpiItem_ItemDTO> KpiItem_ItemDTOs = Items
                .Select(x => new KpiItem_ItemDTO(x)).ToList();
            return KpiItem_ItemDTOs;
        }

        [Route(KpiItemRoute.CountAppUser), HttpPost]
        public async Task<long> CountAppUser([FromBody] KpiItem_AppUserFilterDTO KpiItem_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Id = KpiItem_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiItem_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiItem_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = KpiItem_AppUserFilterDTO.Email;
            AppUserFilter.Phone = KpiItem_AppUserFilterDTO.Phone;
            AppUserFilter.OrganizationId = KpiItem_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            {
                if (AppUserFilter.Id.In == null) AppUserFilter.Id.In = new List<long>();
                AppUserFilter.Id.In.AddRange(await FilterAppUser(AppUserService, OrganizationService, CurrentContext));
            }

            return await KpiItemService.CountAppUser(AppUserFilter, KpiItem_AppUserFilterDTO.KpiYearId, KpiItem_AppUserFilterDTO.KpiPeriodId);
        }

        [Route(KpiItemRoute.ListAppUser), HttpPost]
        public async Task<List<KpiItem_AppUserDTO>> ListAppUser([FromBody] KpiItem_AppUserFilterDTO KpiItem_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = KpiItem_AppUserFilterDTO.Skip;
            AppUserFilter.Take = KpiItem_AppUserFilterDTO.Take;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = KpiItem_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiItem_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiItem_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = KpiItem_AppUserFilterDTO.Email;
            AppUserFilter.OrganizationId = KpiItem_AppUserFilterDTO.OrganizationId;
            AppUserFilter.Phone = KpiItem_AppUserFilterDTO.Phone;
            AppUserFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            {
                if (AppUserFilter.Id.In == null) AppUserFilter.Id.In = new List<long>();
                AppUserFilter.Id.In.AddRange(await FilterAppUser(AppUserService, OrganizationService, CurrentContext));
            }

            List<AppUser> AppUsers = await KpiItemService.ListAppUser(AppUserFilter, KpiItem_AppUserFilterDTO.KpiYearId, KpiItem_AppUserFilterDTO.KpiPeriodId);
            List<KpiItem_AppUserDTO> KpiItem_AppUserDTOs = AppUsers
                .Select(x => new KpiItem_AppUserDTO(x)).ToList();
            return KpiItem_AppUserDTOs;
        }

        [Route(KpiItemRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] KpiItem_ItemFilterDTO KpiItem_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Code = KpiItem_ItemFilterDTO.Code;
            ItemFilter.Name = KpiItem_ItemFilterDTO.Name;
            ItemFilter.ProductGroupingId = KpiItem_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = KpiItem_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = KpiItem_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = KpiItem_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = KpiItem_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = KpiItem_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = KpiItem_ItemFilterDTO.SupplierId;

            return await ItemService.Count(ItemFilter);
        }
        [Route(KpiItemRoute.ListItem), HttpPost]
        public async Task<List<KpiItem_ItemDTO>> ListItem([FromBody] KpiItem_ItemFilterDTO KpiItem_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = KpiItem_ItemFilterDTO.Skip;
            ItemFilter.Take = KpiItem_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Code = KpiItem_ItemFilterDTO.Code;
            ItemFilter.Name = KpiItem_ItemFilterDTO.Name;
            ItemFilter.ProductGroupingId = KpiItem_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = KpiItem_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = KpiItem_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = KpiItem_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = KpiItem_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = KpiItem_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = KpiItem_ItemFilterDTO.SupplierId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<KpiItem_ItemDTO> KpiItem_ItemDTOs = Items
                .Select(x => new KpiItem_ItemDTO(x)).ToList();
            return KpiItem_ItemDTOs;
        }
    }
}

