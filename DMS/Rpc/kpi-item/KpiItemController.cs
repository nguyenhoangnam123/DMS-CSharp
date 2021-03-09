using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MKpiCriteriaItem;
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
using DMS.Helpers;
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
using DMS.Services.MKpiItemType;

namespace DMS.Rpc.kpi_item
{
    public partial class KpiItemController : RpcController
    {
        private IAppUserService AppUserService;
        private IKpiPeriodService KpiPeriodService;
        private IKpiYearService KpiYearService;
        private IOrganizationService OrganizationService;
        private IKpiCriteriaItemService KpiCriteriaItemService;
        private IStatusService StatusService;
        private IItemService ItemService;
        private IKpiItemContentService KpiItemContentService;
        private IKpiItemService KpiItemService;
        private IKpiItemTypeService KpiItemTypeService;
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
            IStatusService StatusService,
            IItemService ItemService,
            IKpiItemContentService KpiItemContentService,
            IKpiItemService KpiItemService,
            IKpiItemTypeService KpiItemTypeService,
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
            this.StatusService = StatusService;
            this.ItemService = ItemService;
            this.KpiItemContentService = KpiItemContentService;
            this.KpiItemService = KpiItemService;
            this.KpiItemTypeService = KpiItemTypeService;
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
            long KpiYearId = StaticParams.DateTimeNow.Year;
            long KpiPeriodId = StaticParams.DateTimeNow.Month + 100;
            List<KpiCriteriaItem> KpiCriteriaItems = await KpiCriteriaItemService.List(new KpiCriteriaItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiCriteriaItemSelect.ALL,
            });
            var KpiItem_KpiItemDTO = new KpiItem_KpiItemDTO();
            (KpiItem_KpiItemDTO.CurrentMonth, KpiItem_KpiItemDTO.CurrentQuarter, KpiItem_KpiItemDTO.CurrentYear) = ConvertDateTime(StaticParams.DateTimeNow);
            KpiItem_KpiItemDTO.KpiYearId = KpiYearId;
            KpiItem_KpiItemDTO.KpiYear = KpiItem_KpiItemDTO.KpiYear = Enums.KpiYearEnum.KpiYearEnumList.Where(x => x.Id == KpiYearId)
                .Select(x => new KpiItem_KpiYearDTO
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name
                })
                .FirstOrDefault();
            KpiItem_KpiItemDTO.KpiPeriodId = KpiPeriodId;
            KpiItem_KpiItemDTO.KpiPeriod = KpiItem_KpiItemDTO.KpiPeriod = Enums.KpiPeriodEnum.KpiPeriodEnumList.Where(x => x.Id == KpiPeriodId)
                .Select(x => new KpiItem_KpiPeriodDTO
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name
                })
                .FirstOrDefault();
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
        public async Task<ActionResult> Import([FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StringBuilder errorContent = new StringBuilder();
            FileInfo FileInfo = new FileInfo(file.FileName);
            if (!FileInfo.Extension.Equals(".xlsx"))
            {
                errorContent.AppendLine("Định dạng file không hợp lệ");
                return BadRequest(errorContent.ToString());
            }

            GenericEnum KpiYear;
            GenericEnum KpiPeriod;
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["KPI san pham"];
                if (worksheet == null)
                {
                    errorContent.AppendLine("File không đúng biểu mẫu import");
                    return BadRequest(errorContent.ToString());
                }

                string KpiPeriodValue = worksheet.Cells[2, 4].Value?.ToString();
                
                if (!string.IsNullOrWhiteSpace(KpiPeriodValue))
                    KpiPeriod = KpiPeriodEnum.KpiPeriodEnumList.Where(x => x.Name == KpiPeriodValue).FirstOrDefault();
                else
                {
                    errorContent.AppendLine("Chưa chọn kỳ Kpi hoặc kỳ Kpi không hợp lệ");
                    return BadRequest(errorContent.ToString());
                }

                string KpiYearValue = worksheet.Cells[2, 6].Value?.ToString();
                
                if (!string.IsNullOrWhiteSpace(KpiYearValue))
                    KpiYear = KpiYearEnum.KpiYearEnumList.Where(x => x.Name == KpiYearValue).FirstOrDefault();
                else
                {
                    errorContent.AppendLine("Chưa chọn măm Kpi hoặc năm Kpi không hợp lệ");
                    return BadRequest(errorContent.ToString());
                }
            }

            HashSet<long> KpiPeriodIds = new HashSet<long>();
            long CurrentMonth = 100 + StaticParams.DateTimeNow.Month;
            long CurrentQuater = 0;
            if (Enums.KpiPeriodEnum.PERIOD_MONTH01.Id <= CurrentMonth && CurrentMonth <= Enums.KpiPeriodEnum.PERIOD_MONTH03.Id)
                CurrentQuater = Enums.KpiPeriodEnum.PERIOD_QUATER01.Id;
            if (Enums.KpiPeriodEnum.PERIOD_MONTH04.Id <= CurrentMonth && CurrentMonth <= Enums.KpiPeriodEnum.PERIOD_MONTH06.Id)
                CurrentQuater = Enums.KpiPeriodEnum.PERIOD_QUATER02.Id;
            if (Enums.KpiPeriodEnum.PERIOD_MONTH07.Id <= CurrentMonth && CurrentMonth <= Enums.KpiPeriodEnum.PERIOD_MONTH09.Id)
                CurrentQuater = Enums.KpiPeriodEnum.PERIOD_QUATER03.Id;
            if (Enums.KpiPeriodEnum.PERIOD_MONTH10.Id <= CurrentMonth && CurrentMonth <= Enums.KpiPeriodEnum.PERIOD_MONTH12.Id)
                CurrentQuater = Enums.KpiPeriodEnum.PERIOD_QUATER04.Id;
            if (KpiYear.Id >= StaticParams.DateTimeNow.Year)
            {
                KpiPeriodIds.Add(KpiYear.Id);
            }
            foreach (var kpiPeriod in KpiPeriodEnum.KpiPeriodEnumList)
            {
                if (CurrentMonth <= kpiPeriod.Id && kpiPeriod.Id <= Enums.KpiPeriodEnum.PERIOD_MONTH12.Id)
                    KpiPeriodIds.Add(kpiPeriod.Id);
                if (CurrentQuater <= kpiPeriod.Id && kpiPeriod.Id <= Enums.KpiPeriodEnum.PERIOD_QUATER04.Id)
                    KpiPeriodIds.Add(kpiPeriod.Id);
            }

            if (!KpiPeriodIds.Contains(KpiPeriod.Id))
            {
                errorContent.AppendLine("Không thể nhập Kpi cho các kỳ trong quá khứ");
                return BadRequest(errorContent.ToString());
            }

            var AppUser = await AppUserService.Get(CurrentContext.UserId);
            AppUserFilter EmployeeFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName | AppUserSelect.Organization,
                OrganizationId = new IdFilter { Equal = AppUser.OrganizationId }
            };
            List<AppUser> Employees = await AppUserService.List(EmployeeFilter);
            var AppUserIds = Employees.Select(x => x.Id).ToList();
            List<KpiItem> KpiItems = await KpiItemService.List(new KpiItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                AppUserId = new IdFilter { In = AppUserIds },
                KpiYearId = new IdFilter { Equal = KpiYear.Id },
                KpiPeriodId = new IdFilter { Equal = KpiPeriod.Id },
                Selects = KpiItemSelect.ALL
            });
            var KpiItemIds = KpiItems.Select(x => x.Id).ToList();
            List<KpiItemContent> KpiItemContents = await KpiItemContentService.List(new KpiItemContentFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiItemContentSelect.ALL,
                KpiItemId = new IdFilter { In = KpiItemIds },
            });
            Parallel.ForEach(KpiItems, KpiItem =>
            {
                KpiItem.KpiItemContents = KpiItemContents.Where(x => x.KpiItemId == KpiItem.Id).ToList();
            });

            List<Item> Items = await ItemService.List(new ItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSelect.Id | ItemSelect.Code | ItemSelect.Name,
                OrderBy = ItemOrder.Id,
                OrderType = OrderType.ASC
            });

            List<KpiItem_ImportDTO> KpiItem_ImportDTOs = new List<KpiItem_ImportDTO>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["KPI san pham"];

                int StartColumn = 1;
                int StartRow = 5;
                int UsernameColumn = 0 + StartColumn;
                int KpiItemTypeColumn = 2 + StartColumn;
                int ItemCodeColumn = 3 + StartColumn;

                int RevenueColumn = 4 + StartColumn;
                int StoreColumn = 5 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    string UsernameValue = worksheet.Cells[i, UsernameColumn].Value?.ToString();
                    string KpiItemTypeValue = worksheet.Cells[i, KpiItemTypeColumn].Value?.ToString();
                    string ItemCodeValue = worksheet.Cells[i, ItemCodeColumn].Value?.ToString();
                    if (UsernameValue != null && UsernameValue.ToLower() == "END".ToLower())
                        break;
                    else if (!string.IsNullOrWhiteSpace(UsernameValue) && string.IsNullOrWhiteSpace(ItemCodeValue))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(UsernameValue) && i != worksheet.Dimension.End.Row)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa nhập mã nhân viên");
                        continue;
                    }
                    else if (string.IsNullOrWhiteSpace(UsernameValue) && i == worksheet.Dimension.End.Row)
                        break;

                    KpiItem_ImportDTO KpiItem_ImportDTO = new KpiItem_ImportDTO();
                    AppUser Employee = Employees.Where(x => x.Username.ToLower() == UsernameValue.ToLower()).FirstOrDefault();
                    if (Employee == null)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Nhân viên không tồn tại");
                        continue;
                    }
                    else
                    {
                        KpiItem_ImportDTO.EmployeeId = Employee.Id;
                    }
                    GenericEnum KpiItemType;
                    if (!string.IsNullOrWhiteSpace(KpiItemTypeValue))
                    {
                        KpiItemType = KpiItemTypeEnum.KpiItemTypeEnumList.Where(x => x.Name == KpiItemTypeValue.Trim()).FirstOrDefault();
                        if(KpiItemType == null)
                        {
                            errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Loại KPI sản phẩm không tồn tại");
                            continue;
                        }
                        else
                        {
                            KpiItem_ImportDTO.KpiItemTypeId = KpiItemType.Id;
                        }
                    }
                    else
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa chọn loại KPI sản phẩm");
                        continue;
                    }
                    Item Item;
                    if (!string.IsNullOrWhiteSpace(ItemCodeValue))
                    {
                        Item = Items.Where(x => x.Code == ItemCodeValue.Trim()).FirstOrDefault();
                        if (Item == null)
                        {
                            errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Sản phẩm không tồn tại");
                            continue;
                        }
                        else
                        {
                            KpiItem_ImportDTO.ItemId = Item.Id;
                        }
                    }

                    KpiItem_ImportDTO.Stt = i;
                    KpiItem_ImportDTO.UsernameValue = UsernameValue;
                    KpiItem_ImportDTO.ItemCodeValue = ItemCodeValue;
                    KpiItem_ImportDTO.IndirectRevenue = worksheet.Cells[i, RevenueColumn].Value?.ToString();
                    KpiItem_ImportDTO.IndirectStoreCounter = worksheet.Cells[i, StoreColumn].Value?.ToString();
                    KpiItem_ImportDTO.KpiPeriodId = KpiPeriod.Id;
                    KpiItem_ImportDTO.KpiYearId = KpiYear.Id;
                    KpiItem_ImportDTOs.Add(KpiItem_ImportDTO);
                }
            }

            Dictionary<long, StringBuilder> Errors = new Dictionary<long, StringBuilder>();
            HashSet<KpiItem_RowDTO> KpiItem_RowDTOs = new HashSet<KpiItem_RowDTO>(KpiItems.Select(x => new KpiItem_RowDTO 
            { 
                AppUserId = x.EmployeeId, 
                KpiPeriodId = x.KpiPeriodId, 
                KpiYearId = x.KpiYearId,
                KpiItemTypeId = x.KpiItemTypeId
            }).ToList());
            foreach (KpiItem_ImportDTO KpiItem_ImportDTO in KpiItem_ImportDTOs)
            {
                Errors.Add(KpiItem_ImportDTO.Stt, new StringBuilder(""));
                KpiItem_ImportDTO.IsNew = false;
                if (!KpiItem_RowDTOs.Contains(new KpiItem_RowDTO 
                                                { 
                                                    AppUserId = KpiItem_ImportDTO.EmployeeId, 
                                                    KpiPeriodId = KpiItem_ImportDTO.KpiPeriodId, 
                                                    KpiYearId = KpiItem_ImportDTO.KpiYearId ,
                                                    KpiItemTypeId = KpiItem_ImportDTO.KpiItemTypeId
                                                }
                ))
                {
                    KpiItem_RowDTOs.Add(new KpiItem_RowDTO 
                    { 
                        AppUserId = KpiItem_ImportDTO.EmployeeId, 
                        KpiPeriodId = KpiItem_ImportDTO.KpiPeriodId, 
                        KpiYearId = KpiItem_ImportDTO.KpiYearId,
                        KpiItemTypeId = KpiItem_ImportDTO.KpiItemTypeId
                    });
                    KpiItem_ImportDTO.IsNew = true;

                    var Employee = Employees.Where(x => x.Username == KpiItem_ImportDTO.UsernameValue).FirstOrDefault();
                    KpiItem_ImportDTO.OrganizationId = Employee.OrganizationId;
                    KpiItem_ImportDTO.EmployeeId = Employee.Id;
                }
            }

            

            foreach (var KpiItem_ImportDTO in KpiItem_ImportDTOs)
            {
                if (KpiItem_ImportDTO.HasValue == false)
                {
                    Errors[KpiItem_ImportDTO.Stt].Append($"Lỗi dòng thứ {KpiItem_ImportDTO.Stt}: Chưa nhập chỉ tiêu");
                    continue;
                }
                KpiItem KpiItem;
                if (KpiItem_ImportDTO.IsNew)
                {
                    KpiItem = new KpiItem();
                    KpiItems.Add(KpiItem);
                    KpiItem.EmployeeId = KpiItem_ImportDTO.EmployeeId;
                    KpiItem.OrganizationId = KpiItem_ImportDTO.OrganizationId;
                    KpiItem.KpiPeriodId = KpiItem_ImportDTO.KpiPeriodId;
                    KpiItem.KpiYearId = KpiItem_ImportDTO.KpiYearId;
                    KpiItem.KpiItemTypeId = KpiItem_ImportDTO.KpiItemTypeId;
                    KpiItem.RowId = Guid.NewGuid();
                    KpiItem.KpiItemContents = new List<KpiItemContent>();
                    KpiItem.KpiItemContents.Add(new KpiItemContent
                    {
                        ItemId = KpiItem_ImportDTO.ItemId,
                        RowId = Guid.NewGuid(),
                        KpiItemContentKpiCriteriaItemMappings = KpiCriteriaItemEnum.KpiCriteriaItemEnumList.Select(x => new KpiItemContentKpiCriteriaItemMapping
                        {
                            KpiCriteriaItemId = x.Id,
                        }).ToList()
                    });
                }
                else
                {
                    KpiItem = KpiItems.Where(x => x.EmployeeId == KpiItem_ImportDTO.EmployeeId && x.KpiPeriodId == KpiItem_ImportDTO.KpiPeriodId && x.KpiYearId == KpiItem_ImportDTO.KpiYearId).FirstOrDefault();
                    var content = KpiItem.KpiItemContents.Where(x => x.ItemId == KpiItem_ImportDTO.ItemId).FirstOrDefault();
                    if(content == null)
                    {
                        KpiItem.KpiItemContents.Add(new KpiItemContent
                        {
                            ItemId = KpiItem_ImportDTO.ItemId,
                            RowId = Guid.NewGuid(),
                            KpiItemContentKpiCriteriaItemMappings = KpiCriteriaItemEnum.KpiCriteriaItemEnumList.Select(x => new KpiItemContentKpiCriteriaItemMapping
                            {
                                KpiCriteriaItemId = x.Id,
                            }).ToList()
                        });
                    }
                }

                KpiItemContent KpiItemContent = KpiItem.KpiItemContents.Where(x => x.ItemId == KpiItem_ImportDTO.ItemId).FirstOrDefault();
                if (KpiItemContent != null)
                {
                    foreach (var KpiItemContentKpiCriteriaItemMapping in KpiItemContent.KpiItemContentKpiCriteriaItemMappings)
                    {
                        //if (long.TryParse(KpiItem_ImportDTO.IndirectQuantity, out long IndirectQuantity) && KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_QUANTITY.Id)
                        //{
                        //    KpiItemContentKpiCriteriaItemMapping.Value = IndirectQuantity;
                        //}
                        /*else*/ if (long.TryParse(KpiItem_ImportDTO.IndirectRevenue, out long IndirectRevenue) && KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_REVENUE.Id)
                        {
                            KpiItemContentKpiCriteriaItemMapping.Value = IndirectRevenue;
                        }
                        //else if (long.TryParse(KpiItem_ImportDTO.IndirectCounter, out long IndirectCounter) && KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_AMOUNT.Id)
                        //{
                        //    KpiItemContentKpiCriteriaItemMapping.Value = IndirectCounter;
                        //}
                        else if (long.TryParse(KpiItem_ImportDTO.IndirectStoreCounter, out long IndirectStoreCounter) && KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_STORE.Id)
                        {
                            KpiItemContentKpiCriteriaItemMapping.Value = IndirectStoreCounter;
                        }
                        //else if (long.TryParse(KpiItem_ImportDTO.DirectQuantity, out long DirectQuantity) && KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_QUANTITY.Id)
                        //{
                        //    KpiItemContentKpiCriteriaItemMapping.Value = DirectQuantity;
                        //}
                        //else if (long.TryParse(KpiItem_ImportDTO.DirectRevenue, out long DirectRevenue) && KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_REVENUE.Id)
                        //{
                        //    KpiItemContentKpiCriteriaItemMapping.Value = DirectRevenue;
                        //}
                        //else if (long.TryParse(KpiItem_ImportDTO.DirectCounter, out long DirectCounter) && KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_AMOUNT.Id)
                        //{
                        //    KpiItemContentKpiCriteriaItemMapping.Value = DirectCounter;
                        //}
                        //else if (long.TryParse(KpiItem_ImportDTO.DirectStoreCounter, out long DirectStoreCounter) && KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_STORE.Id)
                        //{
                        //    KpiItemContentKpiCriteriaItemMapping.Value = DirectStoreCounter;
                        //}
                    }
                }

                KpiItem.CreatorId = AppUser.Id;
                KpiItem.StatusId = StatusEnum.ACTIVE.Id;
            }
            if (errorContent.Length > 0)
                return BadRequest(errorContent.ToString());
            string error = string.Join("\n", Errors.Where(x => !string.IsNullOrWhiteSpace(x.Value.ToString())).Select(x => x.Value.ToString()).ToList());
            if (!string.IsNullOrWhiteSpace(error))
                return BadRequest(error);

            KpiItems = await KpiItemService.Import(KpiItems);
            List<KpiItem_KpiItemDTO> KpiItem_KpiItemDTOs = KpiItems
                .Select(c => new KpiItem_KpiItemDTO(c)).ToList();
            return Ok(KpiItem_KpiItemDTOs);
        }

        [Route(KpiItemRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] KpiItem_KpiItemFilterDTO KpiItem_KpiItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (KpiItem_KpiItemFilterDTO.KpiYearId.Equal.HasValue == false)
                return BadRequest(new { message = "Chưa chọn năm Kpi" });

            if (KpiItem_KpiItemFilterDTO.KpiPeriodId.Equal.HasValue == false)
                return BadRequest(new { message = "Chưa chọn kỳ Kpi" });

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
                KpiItem_ExportDTO.KpiItemTypeName = KpiItem.KpiItemType.Name;

                KpiItem_ExportDTO.Contents = KpiItemContents.Where(x => x.KpiItemId == KpiItem.Id).Select(x => new KpiItem_ExportContentDTO
                {
                    ItemCode = x.Item.Code,
                    ItemName = x.Item.Name,
                    //IndirectQuantity = x.KpiItemContentKpiCriteriaItemMappings
                    //.Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_QUANTITY.Id)
                    //.Where(x => x.Value.HasValue)
                    //.Select(x => x.Value.Value)
                    //.Sum(),
                    IndirectRevenue = x.KpiItemContentKpiCriteriaItemMappings
                    .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_REVENUE.Id)
                    .Where(x => x.Value.HasValue)
                    .Select(x => x.Value.Value)
                    .Sum(),
                    //IndirectCounter = x.KpiItemContentKpiCriteriaItemMappings
                    //.Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_AMOUNT.Id)
                    //.Where(x => x.Value.HasValue)
                    //.Select(x => x.Value.Value)
                    //.Sum(),
                    IndirectStoreCounter = x.KpiItemContentKpiCriteriaItemMappings
                    .Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_STORE.Id)
                    .Where(x => x.Value.HasValue)
                    .Select(x => x.Value.Value)
                    .Sum(),
                    //DirectQuantity = x.KpiItemContentKpiCriteriaItemMappings
                    //.Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_QUANTITY.Id)
                    //.Where(x => x.Value.HasValue)
                    //.Select(x => x.Value.Value)
                    //.Sum(),
                    //DirectRevenue = x.KpiItemContentKpiCriteriaItemMappings
                    //.Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_REVENUE.Id)
                    //.Where(x => x.Value.HasValue)
                    //.Select(x => x.Value.Value)
                    //.Sum(),
                    //DirectCounter = x.KpiItemContentKpiCriteriaItemMappings
                    //.Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_AMOUNT.Id)
                    //.Where(x => x.Value.HasValue)
                    //.Select(x => x.Value.Value)
                    //.Sum(),
                    //DirectStoreCounter = x.KpiItemContentKpiCriteriaItemMappings
                    //.Where(x => x.KpiCriteriaItemId == KpiCriteriaItemEnum.DIRECT_STORE.Id)
                    //.Where(x => x.Value.HasValue)
                    //.Select(x => x.Value.Value)
                    //.Sum(),
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
        public async Task<ActionResult> ExportTemplate()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var appUser = await AppUserService.Get(CurrentContext.UserId);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = int.MaxValue;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.OrganizationId = new IdFilter { Equal = appUser.OrganizationId };
            AppUserFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            {
                if (AppUserFilter.Id.In == null) AppUserFilter.Id.In = new List<long>();
                AppUserFilter.Id.In.AddRange(await FilterAppUser(AppUserService, OrganizationService, CurrentContext));
            }
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

            List<Item> Items = await ItemService.List(new ItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSelect.Code | ItemSelect.Name | ItemSelect.Product,
                OrderBy = ItemOrder.Id,
                OrderType = OrderType.ASC
            });

            List<Item> NewItems = Items.Where(x => x.Product.IsNew).ToList();

            List<KpiItem_ExportDTO> KpiItem_ExportDTOs = new List<KpiItem_ExportDTO>();
            foreach (var AppUser in AppUsers)
            {
                KpiItem_ExportDTO KpiItem_ExportDTO = new KpiItem_ExportDTO();
                KpiItem_ExportDTO.Username = AppUser.Username;
                KpiItem_ExportDTO.DisplayName = AppUser.DisplayName;
                KpiItem_ExportDTOs.Add(KpiItem_ExportDTO);
            }

            string path = "Templates/Kpi_Item.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.KpiItems = KpiItem_ExportDTOs;
            Data.Items = Items;
            Data.NewItems = NewItems;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
        
            return File(output.ToArray(), "application/octet-stream", "Template_Kpi_Item.xlsx");
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
        private Tuple<GenericEnum, GenericEnum, GenericEnum> ConvertDateTime(DateTime date)
        {
            GenericEnum monthName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
            GenericEnum quarterName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
            GenericEnum yearName = Enums.KpiYearEnum.KpiYearEnumList.Where(x => x.Id == StaticParams.DateTimeNow.Year).FirstOrDefault();
            switch (date.Month)
            {
                case 1:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER01;
                    break;
                case 2:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH02;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER01;
                    break;
                case 3:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH03;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER01;
                    break;
                case 4:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH04;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 5:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH05;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 6:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH06;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 7:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH07;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 8:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH08;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 9:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH09;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 10:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH10;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER04;
                    break;
                case 11:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH11;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER04;
                    break;
                case 12:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH12;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER04;
                    break;
            }
            return Tuple.Create(monthName, quarterName, yearName);
        }
        private KpiItem ConvertDTOToEntity(KpiItem_KpiItemDTO KpiItem_KpiItemDTO)
        {
            KpiItem KpiItem = new KpiItem();
            KpiItem.Id = KpiItem_KpiItemDTO.Id;
            KpiItem.OrganizationId = KpiItem_KpiItemDTO.OrganizationId;
            KpiItem.KpiYearId = KpiItem_KpiItemDTO.KpiYearId;
            KpiItem.KpiPeriodId = KpiItem_KpiItemDTO.KpiPeriodId;
            KpiItem.KpiItemTypeId = KpiItem_KpiItemDTO.KpiItemTypeId;
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
            KpiItem.KpiItemType = KpiItem_KpiItemDTO.KpiItemType == null ? null : new KpiItemType
            {
                Id = KpiItem_KpiItemDTO.KpiItemType.Id,
                Code = KpiItem_KpiItemDTO.KpiItemType.Code,
                Name = KpiItem_KpiItemDTO.KpiItemType.Name,
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
            KpiItemFilter.KpiItemTypeId = KpiItem_KpiItemFilterDTO.KpiItemTypeId;
            KpiItemFilter.StatusId = KpiItem_KpiItemFilterDTO.StatusId;
            KpiItemFilter.AppUserId = KpiItem_KpiItemFilterDTO.AppUserId;
            KpiItemFilter.CreatorId = KpiItem_KpiItemFilterDTO.CreatorId;
            KpiItemFilter.CreatedAt = KpiItem_KpiItemFilterDTO.CreatedAt;
            KpiItemFilter.UpdatedAt = KpiItem_KpiItemFilterDTO.UpdatedAt;
            return KpiItemFilter;
        }
    }
}

