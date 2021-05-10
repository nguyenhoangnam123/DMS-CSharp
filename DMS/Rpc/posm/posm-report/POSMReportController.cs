using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using System.Dynamic;
using DMS.Entities;
using DMS.Services.MShowingOrder;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MStatus;
using DMS.Services.MShowingItem;
using DMS.Services.MUnitOfMeasure;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Enums;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using Thinktecture.EntityFrameworkCore.TempTables;
using Thinktecture;
using DMS.DWModels;

namespace DMS.Rpc.posm.posm_report
{
    public partial class POSMReportController : RpcController
    {
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IStatusService StatusService;
        private IShowingItemService ShowingItemService;
        private IShowingOrderService ShowingOrderService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private DataContext DataContext;
        private DWContext DWContext;
        private ICurrentContext CurrentContext;
        public POSMReportController(
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            IStatusService StatusService,
            IShowingItemService ShowingItemService,
            IShowingOrderService ShowingOrderService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            DataContext DataContext,
            DWContext DWContext,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.StatusService = StatusService;
            this.ShowingItemService = ShowingItemService;
            this.ShowingOrderService = ShowingOrderService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.DataContext = DataContext;
            this.DWContext = DWContext;
            this.CurrentContext = CurrentContext;
        }

        [Route(POSMReportRoute.Count), HttpPost]
        public async Task<ActionResult<long>> Count([FromBody] POSMReport_POSMReportFilterDTO POSMReport_ShowingOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (POSMReport_ShowingOrderFilterDTO.HasValue == false)
                return 0;

            DateTime Start = POSMReport_ShowingOrderFilterDTO.Date?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    POSMReport_ShowingOrderFilterDTO.Date.GreaterEqual.Value;

            DateTime End = POSMReport_ShowingOrderFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    POSMReport_ShowingOrderFilterDTO.Date.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return 0;

            long? StoreId = POSMReport_ShowingOrderFilterDTO.StoreId?.Equal;
            long? StoreTypeId = POSMReport_ShowingOrderFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = POSMReport_ShowingOrderFilterDTO.StoreGroupingId?.Equal;
            List<long> ShowingItemIds = POSMReport_ShowingOrderFilterDTO.ShowingItemId?.In;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (POSMReport_ShowingOrderFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == POSMReport_ShowingOrderFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                OrganizationId = new IdFilter { In = OrganizationIds },
                Id = new IdFilter { },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            var AppUsers = await AppUserService.List(AppUserFilter);
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            if (StoreId.HasValue)
            {
                var listId = new List<long> { StoreId.Value };
                StoreIds = StoreIds.Intersect(listId).ToList();
            }
            List<long> StoreTypeIds = await FilterStoreType(StoreTypeService, CurrentContext);
            if (StoreTypeId.HasValue)
            {
                var listId = new List<long> { StoreTypeId.Value };
                StoreTypeIds = StoreTypeIds.Intersect(listId).ToList();
            }
            List<long> StoreGroupingIds = await FilterStoreGrouping(StoreGroupingService, CurrentContext);
            if (StoreGroupingId.HasValue)
            {
                var listId = new List<long> { StoreGroupingId.Value };
                StoreGroupingIds = StoreGroupingIds.Intersect(listId).ToList();
            }

            ITempTableQuery<TempTable<long>> tempTableQuery = await DWContext
                       .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            var query = from transaction in DWContext.Fact_POSMTransaction
                        join s in DWContext.Dim_Store on transaction.StoreId equals s.StoreId
                        join tt in tempTableQuery.Query on transaction.StoreId equals tt.Column1
                        join shw in DWContext.Dim_ShowingItem on transaction.ShowingItemId equals shw.ShowingItemId
                        join uom in DWContext.Dim_UnitOfMeasure on transaction.UnitOfMeasureId equals uom.UnitOfMeasureId
                        where Start <= transaction.Date && transaction.Date <= End &&
                            AppUserIds.Contains(transaction.AppUserId) &&
                            (StoreTypeIds.Contains(s.StoreTypeId)) &&
                            (
                                (
                                    StoreGroupingId.HasValue == false &&
                                    (s.StoreGroupingId.HasValue == false || StoreGroupingIds.Contains(s.StoreGroupingId.Value))
                                ) ||
                                (
                                    StoreGroupingId.HasValue &&
                                    StoreGroupingId.Value == s.StoreGroupingId.Value
                                )
                            ) &&
                            OrganizationIds.Contains(transaction.OrganizationId) &&
                            transaction.DeletedAt == null
                        select new
                        {
                            StoreId = transaction.Id,
                            OrganizationId = transaction.OrganizationId
                        };
            return await query.Distinct().CountAsync();
        }

        [Route(POSMReportRoute.List), HttpPost]
        public async Task<ActionResult<List<POSMReport_POSMReportDTO>>> List([FromBody] POSMReport_POSMReportFilterDTO POSMReport_POSMReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (POSMReport_POSMReportFilterDTO.HasValue == false)
                return new List<POSMReport_POSMReportDTO>();

            DateTime Start = POSMReport_POSMReportFilterDTO.Date?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    POSMReport_POSMReportFilterDTO.Date.GreaterEqual.Value;

            DateTime End = POSMReport_POSMReportFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    POSMReport_POSMReportFilterDTO.Date.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            List<POSMReport_POSMReportDTO> POSMReport_POSMReportDTOs = (await ListReportData(POSMReport_POSMReportFilterDTO, Start, End)).Value;

            return POSMReport_POSMReportDTOs;
        }

        //[Route(POSMReportRoute.Export), HttpPost]
        //public async Task<ActionResult> Export([FromBody] POSMReport_POSMReportFilterDTO POSMReport_POSMReportFilterDTO)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);

        //    DateTime Start = POSMReport_POSMReportFilterDTO.Date?.GreaterEqual == null ?
        //            LocalStartDay(CurrentContext) :
        //            POSMReport_POSMReportFilterDTO.Date.GreaterEqual.Value;

        //    DateTime End = POSMReport_POSMReportFilterDTO.Date?.LessEqual == null ?
        //            LocalEndDay(CurrentContext) :
        //            POSMReport_POSMReportFilterDTO.Date.LessEqual.Value;

        //    POSMReport_POSMReportFilterDTO.Skip = 0;
        //    POSMReport_POSMReportFilterDTO.Take = int.MaxValue;
        //    List<POSMReport_POSMReportDTO> POSMReport_POSMReportDTOs = (await ListReportData(POSMReport_POSMReportFilterDTO, Start, End)).Value;

        //    long stt = 1;
        //    foreach (POSMReport_POSMReportDTO POSMReport_POSMReportDTO in POSMReport_POSMReportDTOs)
        //    {
        //        foreach (var Store in POSMReport_POSMReportDTO.Stores)
        //        {
        //            Store.STT = stt;
        //            stt++;
        //        }
        //    }

        //    var Total = POSMReport_POSMReportDTOs.SelectMany(x => x.Stores).Select(x => x.Total).DefaultIfEmpty(0).Sum();
        //    var SumQuantity = POSMReport_POSMReportDTOs.SelectMany(x => x.Stores).SelectMany(x => x.Contents).Select(x => x.Quantity).DefaultIfEmpty(0).Sum();

        //    var OrgRoot = await DataContext.Organization
        //        .Where(x => x.DeletedAt == null &&
        //        x.StatusId == StatusEnum.ACTIVE.Id &&
        //        x.ParentId.HasValue == false)
        //        .FirstOrDefaultAsync();

        //    string path = "Templates/POSM_Report.xlsx";
        //    byte[] arr = System.IO.File.ReadAllBytes(path);
        //    MemoryStream input = new MemoryStream(arr);
        //    MemoryStream output = new MemoryStream();
        //    dynamic Data = new ExpandoObject();
        //    Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
        //    Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
        //    Data.Data = POSMReport_POSMReportDTOs;
        //    Data.RootName = OrgRoot == null ? "" : OrgRoot.Name.ToUpper();
        //    Data.Total = Total;
        //    Data.SumQuantity = SumQuantity;
        //    using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
        //    {
        //        document.Process(Data);
        //    };

        //    return File(output.ToArray(), "application/octet-stream", "POSMReport.xlsx");
        //}

        private async Task<ActionResult<List<POSMReport_POSMReportDTO>>> ListReportData([FromBody] POSMReport_POSMReportFilterDTO POSMReport_ShowingOrderFilterDTO, DateTime Start, DateTime End)
        {
            #region filter
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long? StoreId = POSMReport_ShowingOrderFilterDTO.StoreId?.Equal;
            long? StoreTypeId = POSMReport_ShowingOrderFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = POSMReport_ShowingOrderFilterDTO.StoreGroupingId?.Equal;
            List<long> ShowingItemIds = POSMReport_ShowingOrderFilterDTO.ShowingItemId?.In;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (POSMReport_ShowingOrderFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == POSMReport_ShowingOrderFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                OrganizationId = new IdFilter { In = OrganizationIds },
                Id = new IdFilter { },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            var AppUsers = await AppUserService.List(AppUserFilter);
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            if (StoreId.HasValue)
            {
                var listId = new List<long> { StoreId.Value };
                StoreIds = StoreIds.Intersect(listId).ToList();
            }
            List<long> StoreTypeIds = await FilterStoreType(StoreTypeService, CurrentContext);
            if (StoreTypeId.HasValue)
            {
                var listId = new List<long> { StoreTypeId.Value };
                StoreTypeIds = StoreTypeIds.Intersect(listId).ToList();
            }
            List<long> StoreGroupingIds = await FilterStoreGrouping(StoreGroupingService, CurrentContext);
            if (StoreGroupingId.HasValue)
            {
                var listId = new List<long> { StoreGroupingId.Value };
                StoreGroupingIds = StoreGroupingIds.Intersect(listId).ToList();
            }

            ITempTableQuery<TempTable<long>> tempTableQuery = await DWContext
                       .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            #endregion

            #region lấy data từ filter
            var query = from transaction in DWContext.Fact_POSMTransaction
                        join s in DWContext.Dim_Store on transaction.StoreId equals s.StoreId
                        join tt in tempTableQuery.Query on transaction.StoreId equals tt.Column1
                        join shw in DWContext.Dim_ShowingItem on transaction.ShowingItemId equals shw.ShowingItemId
                        join uom in DWContext.Dim_UnitOfMeasure on transaction.UnitOfMeasureId equals uom.UnitOfMeasureId
                        where Start <= transaction.Date && transaction.Date <= End &&
                            AppUserIds.Contains(transaction.AppUserId) &&
                            (StoreTypeIds.Contains(s.StoreTypeId)) &&
                            (
                                (
                                    StoreGroupingId.HasValue == false &&
                                    (s.StoreGroupingId.HasValue == false || StoreGroupingIds.Contains(s.StoreGroupingId.Value))
                                ) ||
                                (
                                    StoreGroupingId.HasValue &&
                                    StoreGroupingId.Value == s.StoreGroupingId.Value
                                )
                            ) &&
                            OrganizationIds.Contains(transaction.OrganizationId) &&
                            transaction.DeletedAt == null
                        select new
                        {
                            OrganizationId = transaction.OrganizationId,
                            StoreId = transaction.StoreId,
                            ShowingItemId = transaction.ShowingItemId,
                            UnitOfMeasureId = transaction.UnitOfMeasureId,
                            ShowingItemName = shw.Name,
                            ShowingItemCode = shw.Code,
                            UnitOfMeasureName = uom.Name,
                            SalePrice = transaction.SalePrice,
                            Quantity = transaction.Quantity,
                            Amount = transaction.Amount,
                            TransactionTypeId = transaction.TransactionTypeId
                        };

            var Ids = await query
                .Distinct()
                .OrderBy(x => x.OrganizationId)
                .Select(x => new
                {
                    StoreId = x.StoreId,
                    OrganizationId = x.OrganizationId,
                    ShowingItemId = x.ShowingItemId
                })
                .Skip(POSMReport_ShowingOrderFilterDTO.Skip)
                .Take(POSMReport_ShowingOrderFilterDTO.Take)
                .ToListAsync(); // phân trang báo cáo theo Org và Store
            OrganizationIds = Ids.Select(x => x.OrganizationId)
                .Distinct()
                .ToList();
            StoreIds = Ids.Select(x => x.StoreId)
                .Distinct()
                .ToList();
            ShowingItemIds = Ids.Select(x => x.ShowingItemId)
                .Distinct()
                .ToList();
            var Organizations = await DWContext.Dim_Organization
                .Where(x => OrganizationIds.Contains(x.OrganizationId))
                .OrderBy(x => x.OrganizationId)
                .Select(x => new Dim_OrganizationDAO
                {
                    OrganizationId = x.OrganizationId,
                    Name = x.Name
                }).ToListAsync(); // lấy ra toàn bộ Org trong danh sách phân trang
            var Stores = await DWContext.Dim_Store
                .Where(x => StoreIds.Contains(x.StoreId))
                .Select(x => new Dim_StoreDAO
                {
                    StoreId = x.StoreId,
                    Code = x.Code,
                    CodeDraft = x.CodeDraft,
                    Name = x.Name,
                    Address = x.Address,
                }).ToListAsync(); // lấy ra toàn bộ store trong danh sách phân trang
            #endregion

            #region tổng hợp dữ liệu
            List<POSMReport_POSMReportDTO> POSMReport_POSMReportDTOs = new List<POSMReport_POSMReportDTO>();
            foreach (var Organization in Organizations)
            {
                POSMReport_POSMReportDTO POSMReport_POSMReportDTO = new POSMReport_POSMReportDTO()
                {
                    OrganizationId = Organization.OrganizationId,
                    OrganizationName = Organization.Name,
                    Stores = new List<POSMReport_POSMStoreDTO>()
                };
                POSMReport_POSMReportDTO.Stores = Ids
                        .Where(x => x.OrganizationId == Organization.OrganizationId)
                        .Select(x => new POSMReport_POSMStoreDTO
                        {
                            Id = x.StoreId,
                            OrganizationId = Organization.OrganizationId
                        }).ToList();
                POSMReport_POSMReportDTOs.Add(POSMReport_POSMReportDTO);
            } // tạo group cửa hàng bởi Organization

            foreach (POSMReport_POSMReportDTO POSMReport_POSMReportDTO in POSMReport_POSMReportDTOs)
            {
                foreach (POSMReport_POSMStoreDTO POSMReport_POSMStoreDTO in POSMReport_POSMReportDTO.Stores)
                {
                    var Store = Stores.Where(x => x.StoreId == POSMReport_POSMStoreDTO.Id).FirstOrDefault();
                    if (Store != null)
                    {
                        POSMReport_POSMStoreDTO.Code = Store.Code;
                        POSMReport_POSMStoreDTO.CodeDraft = Store.CodeDraft;
                        POSMReport_POSMStoreDTO.Name = Store.Name;
                        POSMReport_POSMStoreDTO.Address = Store.Address;
                    }
                    var LineTransaction = query
                        .Where(x => x.OrganizationId == POSMReport_POSMStoreDTO.OrganizationId && x.StoreId == POSMReport_POSMStoreDTO.Id)
                        .ToList();
                    var OrderTransaction = LineTransaction
                        .Where(x => x.TransactionTypeId == POSMTransactionTypeEnum.ORDER.Id)
                        .ToList(); // lấy ra toàn bộ đơn cấp mới
                    var OrderWithDrawTransaction = LineTransaction
                        .Where(x => x.TransactionTypeId == POSMTransactionTypeEnum.ORDER_WITHDRAW.Id)
                        .ToList(); // lấy ra toàn bộ đơn thu hồi
                    decimal OrderTotal = OrderTransaction.Select(x => x.Amount).DefaultIfEmpty(0).Sum();
                    decimal OrderWithDrawTotal = OrderWithDrawTransaction.Select(x => x.Amount).DefaultIfEmpty(0).Sum();
                    POSMReport_POSMStoreDTO.Total = OrderTotal - OrderWithDrawTotal; // tổng giá trị trưng bày
                    POSMReport_POSMStoreDTO.Contents = new List<POSMReport_POSMReportContentDTO>();
                    foreach (var TransactionItem in LineTransaction)
                    {
                        POSMReport_POSMReportContentDTO ShowingItemContent = POSMReport_POSMStoreDTO.Contents
                            .Where(x => x.ShowingItemId == TransactionItem.ShowingItemId)
                            .Where(x => x.UnitOfMeasureId == TransactionItem.UnitOfMeasureId)
                            .Where(x => x.SalePrice == TransactionItem.SalePrice)
                            .FirstOrDefault(); // tìm dòng giống nhau item, đơn vị và giá
                        if (ShowingItemContent != null)
                        {
                            if (TransactionItem.TransactionTypeId == POSMTransactionTypeEnum.ORDER.Id)
                            {
                                ShowingItemContent.OrderQuantity += TransactionItem.Quantity;
                                ShowingItemContent.Amount += TransactionItem.Amount;
                            } // nếu cấp mới thì thêm vào OrderQuantity và cộng thêm giá trị
                            if (TransactionItem.TransactionTypeId == POSMTransactionTypeEnum.ORDER_WITHDRAW.Id)
                            {
                                ShowingItemContent.OrderWithDrawQuantity += TransactionItem.Quantity;
                                ShowingItemContent.Amount -= TransactionItem.Amount;
                            } // nếu thu hồi thì thêm vào OrderWithDrawQuantity và trừ vào giá trị
                        } // nếu dòng theo ShowingItem, UOM, SalePrice có sẵn
                        else
                        {
                            ShowingItemContent = new POSMReport_POSMReportContentDTO
                            {
                                ShowingItemId = TransactionItem.ShowingItemId,
                                ShowingItemCode = TransactionItem.ShowingItemCode,
                                ShowingItemName = TransactionItem.ShowingItemName,
                                SalePrice = TransactionItem.SalePrice,
                                UnitOfMeasure = TransactionItem.UnitOfMeasureName
                            };
                            if (TransactionItem.TransactionTypeId == POSMTransactionTypeEnum.ORDER.Id)
                            {
                                ShowingItemContent.OrderQuantity = TransactionItem.Quantity;
                                ShowingItemContent.Amount += TransactionItem.Amount;
                            }
                            if (TransactionItem.TransactionTypeId == POSMTransactionTypeEnum.ORDER_WITHDRAW.Id)
                            {
                                ShowingItemContent.OrderWithDrawQuantity = TransactionItem.Quantity;
                                ShowingItemContent.Amount -= TransactionItem.Amount;
                            }
                            POSMReport_POSMStoreDTO.Contents.Add(ShowingItemContent);
                        } // nếu chưa có dòng nào theo ShowingItem, UOM, SalePrice
                    }
                }
            }

            return POSMReport_POSMReportDTOs;
            #endregion
        }
    }
}

