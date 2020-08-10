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
using DMS.Services.MPriceList;
using DMS.Services.MOrganization;
using DMS.Services.MPriceListType;
using DMS.Services.MSalesOrderType;
using DMS.Services.MProduct;
using DMS.Services.MStatus;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;

namespace DMS.Rpc.price_list
{
    public partial class PriceListController : RpcController
    {
        private IOrganizationService OrganizationService;
        private IItemService ItemService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreService StoreService;
        private IStoreTypeService StoreTypeService;
        private IPriceListTypeService PriceListTypeService;
        private ISalesOrderTypeService SalesOrderTypeService;
        private IStatusService StatusService;
        private IPriceListService PriceListService;
        private IProductTypeService ProductTypeService;
        private IProductGroupingService ProductGroupingService;
        private ICurrentContext CurrentContext;
        public PriceListController(
            IOrganizationService OrganizationService,
            IItemService ItemService,
            IStoreGroupingService StoreGroupingService,
            IStoreService StoreService,
            IStoreTypeService StoreTypeService,
            IPriceListTypeService PriceListTypeService,
            ISalesOrderTypeService SalesOrderTypeService,
            IStatusService StatusService,
            IPriceListService PriceListService,
            IProductTypeService ProductTypeService,
            IProductGroupingService ProductGroupingService,
            ICurrentContext CurrentContext
        )
        {
            this.OrganizationService = OrganizationService;
            this.ItemService = ItemService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreService = StoreService;
            this.StoreTypeService = StoreTypeService;
            this.PriceListTypeService = PriceListTypeService;
            this.SalesOrderTypeService = SalesOrderTypeService;
            this.StatusService = StatusService;
            this.PriceListService = PriceListService;
            this.ProductTypeService = ProductTypeService;
            this.ProductGroupingService = ProductGroupingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(PriceListRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] PriceList_PriceListFilterDTO PriceList_PriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PriceListFilter PriceListFilter = ConvertFilterDTOToFilterEntity(PriceList_PriceListFilterDTO);
            PriceListFilter = await PriceListService.ToFilter(PriceListFilter);
            int count = await PriceListService.Count(PriceListFilter);
            return count;
        }

        [Route(PriceListRoute.List), HttpPost]
        public async Task<ActionResult<List<PriceList_PriceListDTO>>> List([FromBody] PriceList_PriceListFilterDTO PriceList_PriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PriceListFilter PriceListFilter = ConvertFilterDTOToFilterEntity(PriceList_PriceListFilterDTO);
            PriceListFilter = await PriceListService.ToFilter(PriceListFilter);
            List<PriceList> PriceLists = await PriceListService.List(PriceListFilter);
            List<PriceList_PriceListDTO> PriceList_PriceListDTOs = PriceLists
                .Select(c => new PriceList_PriceListDTO(c)).ToList();
            return PriceList_PriceListDTOs;
        }

        [Route(PriceListRoute.Get), HttpPost]
        public async Task<ActionResult<PriceList_PriceListDTO>> Get([FromBody] PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PriceList_PriceListDTO.Id))
                return Forbid();

            PriceList PriceList = await PriceListService.Get(PriceList_PriceListDTO.Id);
            return new PriceList_PriceListDTO(PriceList);
        }

        [Route(PriceListRoute.Create), HttpPost]
        public async Task<ActionResult<PriceList_PriceListDTO>> Create([FromBody] PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PriceList_PriceListDTO.Id))
                return Forbid();

            PriceList PriceList = ConvertDTOToEntity(PriceList_PriceListDTO);
            PriceList = await PriceListService.Create(PriceList);
            PriceList_PriceListDTO = new PriceList_PriceListDTO(PriceList);
            if (PriceList.IsValidated)
                return PriceList_PriceListDTO;
            else
                return BadRequest(PriceList_PriceListDTO);
        }

        [Route(PriceListRoute.Update), HttpPost]
        public async Task<ActionResult<PriceList_PriceListDTO>> Update([FromBody] PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PriceList_PriceListDTO.Id))
                return Forbid();

            PriceList PriceList = ConvertDTOToEntity(PriceList_PriceListDTO);
            PriceList = await PriceListService.Update(PriceList);
            PriceList_PriceListDTO = new PriceList_PriceListDTO(PriceList);
            if (PriceList.IsValidated)
                return PriceList_PriceListDTO;
            else
                return BadRequest(PriceList_PriceListDTO);
        }

        [Route(PriceListRoute.Delete), HttpPost]
        public async Task<ActionResult<PriceList_PriceListDTO>> Delete([FromBody] PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PriceList_PriceListDTO.Id))
                return Forbid();

            PriceList PriceList = ConvertDTOToEntity(PriceList_PriceListDTO);
            PriceList = await PriceListService.Delete(PriceList);
            PriceList_PriceListDTO = new PriceList_PriceListDTO(PriceList);
            if (PriceList.IsValidated)
                return PriceList_PriceListDTO;
            else
                return BadRequest(PriceList_PriceListDTO);
        }

        [Route(PriceListRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PriceListFilter PriceListFilter = new PriceListFilter();
            PriceListFilter = await PriceListService.ToFilter(PriceListFilter);
            PriceListFilter.Id = new IdFilter { In = Ids };
            PriceListFilter.Selects = PriceListSelect.Id;
            PriceListFilter.Skip = 0;
            PriceListFilter.Take = int.MaxValue;

            List<PriceList> PriceLists = await PriceListService.List(PriceListFilter);
            PriceLists = await PriceListService.BulkDelete(PriceLists);
            if (PriceLists.Any(x => !x.IsValidated))
                return BadRequest(PriceLists.Where(x => !x.IsValidated));
            return true;
        }

        [Route(PriceListRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            PriceListTypeFilter PriceListTypeFilter = new PriceListTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = PriceListTypeSelect.ALL
            };
            List<PriceListType> PriceListTypes = await PriceListTypeService.List(PriceListTypeFilter);
            SalesOrderTypeFilter SalesOrderTypeFilter = new SalesOrderTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SalesOrderTypeSelect.ALL
            };
            List<SalesOrderType> SalesOrderTypes = await SalesOrderTypeService.List(SalesOrderTypeFilter);
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<PriceList> PriceLists = new List<PriceList>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(PriceLists);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int StartDateColumn = 3 + StartColumn;
                int EndDateColumn = 4 + StartColumn;
                int StatusIdColumn = 5 + StartColumn;
                int OrganizationIdColumn = 6 + StartColumn;
                int PriceListTypeIdColumn = 7 + StartColumn;
                int SalesOrderTypeIdColumn = 8 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string StartDateValue = worksheet.Cells[i + StartRow, StartDateColumn].Value?.ToString();
                    string EndDateValue = worksheet.Cells[i + StartRow, EndDateColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    string PriceListTypeIdValue = worksheet.Cells[i + StartRow, PriceListTypeIdColumn].Value?.ToString();
                    string SalesOrderTypeIdValue = worksheet.Cells[i + StartRow, SalesOrderTypeIdColumn].Value?.ToString();

                    PriceList PriceList = new PriceList();
                    PriceList.Code = CodeValue;
                    PriceList.Name = NameValue;
                    PriceList.StartDate = DateTime.TryParse(StartDateValue, out DateTime StartDate) ? StartDate : DateTime.Now;
                    PriceList.EndDate = DateTime.TryParse(EndDateValue, out DateTime EndDate) ? EndDate : DateTime.Now;
                    PriceListType PriceListType = PriceListTypes.Where(x => x.Id.ToString() == PriceListTypeIdValue).FirstOrDefault();
                    PriceList.PriceListTypeId = PriceListType == null ? 0 : PriceListType.Id;
                    PriceList.PriceListType = PriceListType;
                    SalesOrderType SalesOrderType = SalesOrderTypes.Where(x => x.Id.ToString() == SalesOrderTypeIdValue).FirstOrDefault();
                    PriceList.SalesOrderTypeId = SalesOrderType == null ? 0 : SalesOrderType.Id;
                    PriceList.SalesOrderType = SalesOrderType;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    PriceList.StatusId = Status == null ? 0 : Status.Id;
                    PriceList.Status = Status;

                    PriceLists.Add(PriceList);
                }
            }
            PriceLists = await PriceListService.Import(PriceLists);
            if (PriceLists.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < PriceLists.Count; i++)
                {
                    PriceList PriceList = PriceLists[i];
                    if (!PriceList.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (PriceList.Errors.ContainsKey(nameof(PriceList.Id)))
                            Error += PriceList.Errors[nameof(PriceList.Id)];
                        if (PriceList.Errors.ContainsKey(nameof(PriceList.Code)))
                            Error += PriceList.Errors[nameof(PriceList.Code)];
                        if (PriceList.Errors.ContainsKey(nameof(PriceList.Name)))
                            Error += PriceList.Errors[nameof(PriceList.Name)];
                        if (PriceList.Errors.ContainsKey(nameof(PriceList.StartDate)))
                            Error += PriceList.Errors[nameof(PriceList.StartDate)];
                        if (PriceList.Errors.ContainsKey(nameof(PriceList.EndDate)))
                            Error += PriceList.Errors[nameof(PriceList.EndDate)];
                        if (PriceList.Errors.ContainsKey(nameof(PriceList.StatusId)))
                            Error += PriceList.Errors[nameof(PriceList.StatusId)];
                        if (PriceList.Errors.ContainsKey(nameof(PriceList.OrganizationId)))
                            Error += PriceList.Errors[nameof(PriceList.OrganizationId)];
                        if (PriceList.Errors.ContainsKey(nameof(PriceList.PriceListTypeId)))
                            Error += PriceList.Errors[nameof(PriceList.PriceListTypeId)];
                        if (PriceList.Errors.ContainsKey(nameof(PriceList.SalesOrderTypeId)))
                            Error += PriceList.Errors[nameof(PriceList.SalesOrderTypeId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }

        [Route(PriceListRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] PriceList_PriceListFilterDTO PriceList_PriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region PriceList
                var PriceListFilter = ConvertFilterDTOToFilterEntity(PriceList_PriceListFilterDTO);
                PriceListFilter.Skip = 0;
                PriceListFilter.Take = int.MaxValue;
                PriceListFilter = await PriceListService.ToFilter(PriceListFilter);
                List<PriceList> PriceLists = await PriceListService.List(PriceListFilter);

                var PriceListHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                        "StartDate",
                        "EndDate",
                        "StatusId",
                        "OrganizationId",
                        "PriceListTypeId",
                        "SalesOrderTypeId",
                    }
                };
                List<object[]> PriceListData = new List<object[]>();
                for (int i = 0; i < PriceLists.Count; i++)
                {
                    var PriceList = PriceLists[i];
                    PriceListData.Add(new Object[]
                    {
                        PriceList.Id,
                        PriceList.Code,
                        PriceList.Name,
                        PriceList.StartDate,
                        PriceList.EndDate,
                        PriceList.StatusId,
                        PriceList.OrganizationId,
                        PriceList.PriceListTypeId,
                        PriceList.SalesOrderTypeId,
                    });
                }
                excel.GenerateWorksheet("PriceList", PriceListHeaders, PriceListData);
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
                #region PriceListType
                var PriceListTypeFilter = new PriceListTypeFilter();
                PriceListTypeFilter.Selects = PriceListTypeSelect.ALL;
                PriceListTypeFilter.OrderBy = PriceListTypeOrder.Id;
                PriceListTypeFilter.OrderType = OrderType.ASC;
                PriceListTypeFilter.Skip = 0;
                PriceListTypeFilter.Take = int.MaxValue;
                List<PriceListType> PriceListTypes = await PriceListTypeService.List(PriceListTypeFilter);

                var PriceListTypeHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> PriceListTypeData = new List<object[]>();
                for (int i = 0; i < PriceListTypes.Count; i++)
                {
                    var PriceListType = PriceListTypes[i];
                    PriceListTypeData.Add(new Object[]
                    {
                        PriceListType.Id,
                        PriceListType.Code,
                        PriceListType.Name,
                    });
                }
                excel.GenerateWorksheet("PriceListType", PriceListTypeHeaders, PriceListTypeData);
                #endregion
                #region SalesOrderType
                var SalesOrderTypeFilter = new SalesOrderTypeFilter();
                SalesOrderTypeFilter.Selects = SalesOrderTypeSelect.ALL;
                SalesOrderTypeFilter.OrderBy = SalesOrderTypeOrder.Id;
                SalesOrderTypeFilter.OrderType = OrderType.ASC;
                SalesOrderTypeFilter.Skip = 0;
                SalesOrderTypeFilter.Take = int.MaxValue;
                List<SalesOrderType> SalesOrderTypes = await SalesOrderTypeService.List(SalesOrderTypeFilter);

                var SalesOrderTypeHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> SalesOrderTypeData = new List<object[]>();
                for (int i = 0; i < SalesOrderTypes.Count; i++)
                {
                    var SalesOrderType = SalesOrderTypes[i];
                    SalesOrderTypeData.Add(new Object[]
                    {
                        SalesOrderType.Id,
                        SalesOrderType.Code,
                        SalesOrderType.Name,
                    });
                }
                excel.GenerateWorksheet("SalesOrderType", SalesOrderTypeHeaders, SalesOrderTypeData);
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
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "PriceList.xlsx");
        }

        [Route(PriceListRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] PriceList_PriceListFilterDTO PriceList_PriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region PriceList
                var PriceListHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                        "StartDate",
                        "EndDate",
                        "StatusId",
                        "OrganizationId",
                        "PriceListTypeId",
                        "SalesOrderTypeId",
                    }
                };
                List<object[]> PriceListData = new List<object[]>();
                excel.GenerateWorksheet("PriceList", PriceListHeaders, PriceListData);
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
                #region PriceListType
                var PriceListTypeFilter = new PriceListTypeFilter();
                PriceListTypeFilter.Selects = PriceListTypeSelect.ALL;
                PriceListTypeFilter.OrderBy = PriceListTypeOrder.Id;
                PriceListTypeFilter.OrderType = OrderType.ASC;
                PriceListTypeFilter.Skip = 0;
                PriceListTypeFilter.Take = int.MaxValue;
                List<PriceListType> PriceListTypes = await PriceListTypeService.List(PriceListTypeFilter);

                var PriceListTypeHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> PriceListTypeData = new List<object[]>();
                for (int i = 0; i < PriceListTypes.Count; i++)
                {
                    var PriceListType = PriceListTypes[i];
                    PriceListTypeData.Add(new Object[]
                    {
                        PriceListType.Id,
                        PriceListType.Code,
                        PriceListType.Name,
                    });
                }
                excel.GenerateWorksheet("PriceListType", PriceListTypeHeaders, PriceListTypeData);
                #endregion
                #region SalesOrderType
                var SalesOrderTypeFilter = new SalesOrderTypeFilter();
                SalesOrderTypeFilter.Selects = SalesOrderTypeSelect.ALL;
                SalesOrderTypeFilter.OrderBy = SalesOrderTypeOrder.Id;
                SalesOrderTypeFilter.OrderType = OrderType.ASC;
                SalesOrderTypeFilter.Skip = 0;
                SalesOrderTypeFilter.Take = int.MaxValue;
                List<SalesOrderType> SalesOrderTypes = await SalesOrderTypeService.List(SalesOrderTypeFilter);

                var SalesOrderTypeHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> SalesOrderTypeData = new List<object[]>();
                for (int i = 0; i < SalesOrderTypes.Count; i++)
                {
                    var SalesOrderType = SalesOrderTypes[i];
                    SalesOrderTypeData.Add(new Object[]
                    {
                        SalesOrderType.Id,
                        SalesOrderType.Code,
                        SalesOrderType.Name,
                    });
                }
                excel.GenerateWorksheet("SalesOrderType", SalesOrderTypeHeaders, SalesOrderTypeData);
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
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "PriceList.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            PriceListFilter PriceListFilter = new PriceListFilter();
            PriceListFilter = await PriceListService.ToFilter(PriceListFilter);
            if (Id == 0)
            {

            }
            else
            {
                PriceListFilter.Id = new IdFilter { Equal = Id };
                int count = await PriceListService.Count(PriceListFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private PriceList ConvertDTOToEntity(PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            PriceList PriceList = new PriceList();
            PriceList.Id = PriceList_PriceListDTO.Id;
            PriceList.Code = PriceList_PriceListDTO.Code;
            PriceList.Name = PriceList_PriceListDTO.Name;
            PriceList.StartDate = PriceList_PriceListDTO.StartDate;
            PriceList.EndDate = PriceList_PriceListDTO.EndDate;
            PriceList.StatusId = PriceList_PriceListDTO.StatusId;
            PriceList.OrganizationId = PriceList_PriceListDTO.OrganizationId;
            PriceList.PriceListTypeId = PriceList_PriceListDTO.PriceListTypeId;
            PriceList.SalesOrderTypeId = PriceList_PriceListDTO.SalesOrderTypeId;
            PriceList.Organization = PriceList_PriceListDTO.Organization == null ? null : new Organization
            {
                Id = PriceList_PriceListDTO.Organization.Id,
                Code = PriceList_PriceListDTO.Organization.Code,
                Name = PriceList_PriceListDTO.Organization.Name,
                ParentId = PriceList_PriceListDTO.Organization.ParentId,
                Path = PriceList_PriceListDTO.Organization.Path,
                Level = PriceList_PriceListDTO.Organization.Level,
                StatusId = PriceList_PriceListDTO.Organization.StatusId,
                Phone = PriceList_PriceListDTO.Organization.Phone,
                Email = PriceList_PriceListDTO.Organization.Email,
                Address = PriceList_PriceListDTO.Organization.Address,
            };
            PriceList.PriceListType = PriceList_PriceListDTO.PriceListType == null ? null : new PriceListType
            {
                Id = PriceList_PriceListDTO.PriceListType.Id,
                Code = PriceList_PriceListDTO.PriceListType.Code,
                Name = PriceList_PriceListDTO.PriceListType.Name,
            };
            PriceList.SalesOrderType = PriceList_PriceListDTO.SalesOrderType == null ? null : new SalesOrderType
            {
                Id = PriceList_PriceListDTO.SalesOrderType.Id,
                Code = PriceList_PriceListDTO.SalesOrderType.Code,
                Name = PriceList_PriceListDTO.SalesOrderType.Name,
            };
            PriceList.Status = PriceList_PriceListDTO.Status == null ? null : new Status
            {
                Id = PriceList_PriceListDTO.Status.Id,
                Code = PriceList_PriceListDTO.Status.Code,
                Name = PriceList_PriceListDTO.Status.Name,
            };
            PriceList.BaseLanguage = CurrentContext.Language;
            return PriceList;
        }

        private PriceListFilter ConvertFilterDTOToFilterEntity(PriceList_PriceListFilterDTO PriceList_PriceListFilterDTO)
        {
            PriceListFilter PriceListFilter = new PriceListFilter();
            PriceListFilter.Selects = PriceListSelect.ALL;
            PriceListFilter.Skip = PriceList_PriceListFilterDTO.Skip;
            PriceListFilter.Take = PriceList_PriceListFilterDTO.Take;
            PriceListFilter.OrderBy = PriceList_PriceListFilterDTO.OrderBy;
            PriceListFilter.OrderType = PriceList_PriceListFilterDTO.OrderType;

            PriceListFilter.Id = PriceList_PriceListFilterDTO.Id;
            PriceListFilter.Code = PriceList_PriceListFilterDTO.Code;
            PriceListFilter.Name = PriceList_PriceListFilterDTO.Name;
            PriceListFilter.StartDate = PriceList_PriceListFilterDTO.StartDate;
            PriceListFilter.EndDate = PriceList_PriceListFilterDTO.EndDate;
            PriceListFilter.StatusId = PriceList_PriceListFilterDTO.StatusId;
            PriceListFilter.OrganizationId = PriceList_PriceListFilterDTO.OrganizationId;
            PriceListFilter.PriceListTypeId = PriceList_PriceListFilterDTO.PriceListTypeId;
            PriceListFilter.SalesOrderTypeId = PriceList_PriceListFilterDTO.SalesOrderTypeId;
            PriceListFilter.CreatedAt = PriceList_PriceListFilterDTO.CreatedAt;
            PriceListFilter.UpdatedAt = PriceList_PriceListFilterDTO.UpdatedAt;
            return PriceListFilter;
        }
    }
}

