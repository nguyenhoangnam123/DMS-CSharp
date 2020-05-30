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
using DMS.Services.MKpiPeriod;
using DMS.Services.MItemSpecificKpi;
using DMS.Services.MOrganization;
using DMS.Services.MStatus;

namespace DMS.Rpc.kpi_period
{
    public class KpiPeriodController : RpcController
    {
        private IItemSpecificKpiService ItemSpecificKpiService;
        private IOrganizationService OrganizationService;
        private IStatusService StatusService;
        private IKpiPeriodService KpiPeriodService;
        private ICurrentContext CurrentContext;
        public KpiPeriodController(
            IItemSpecificKpiService ItemSpecificKpiService,
            IOrganizationService OrganizationService,
            IStatusService StatusService,
            IKpiPeriodService KpiPeriodService,
            ICurrentContext CurrentContext
        )
        {
            this.ItemSpecificKpiService = ItemSpecificKpiService;
            this.OrganizationService = OrganizationService;
            this.StatusService = StatusService;
            this.KpiPeriodService = KpiPeriodService;
            this.CurrentContext = CurrentContext;
        }

        [Route(KpiPeriodRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] KpiPeriod_KpiPeriodFilterDTO KpiPeriod_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = ConvertFilterDTOToFilterEntity(KpiPeriod_KpiPeriodFilterDTO);
            KpiPeriodFilter = KpiPeriodService.ToFilter(KpiPeriodFilter);
            int count = await KpiPeriodService.Count(KpiPeriodFilter);
            return count;
        }

        [Route(KpiPeriodRoute.List), HttpPost]
        public async Task<ActionResult<List<KpiPeriod_KpiPeriodDTO>>> List([FromBody] KpiPeriod_KpiPeriodFilterDTO KpiPeriod_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = ConvertFilterDTOToFilterEntity(KpiPeriod_KpiPeriodFilterDTO);
            KpiPeriodFilter = KpiPeriodService.ToFilter(KpiPeriodFilter);
            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            List<KpiPeriod_KpiPeriodDTO> KpiPeriod_KpiPeriodDTOs = KpiPeriods
                .Select(c => new KpiPeriod_KpiPeriodDTO(c)).ToList();
            return KpiPeriod_KpiPeriodDTOs;
        }

        [Route(KpiPeriodRoute.Get), HttpPost]
        public async Task<ActionResult<KpiPeriod_KpiPeriodDTO>> Get([FromBody]KpiPeriod_KpiPeriodDTO KpiPeriod_KpiPeriodDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiPeriod_KpiPeriodDTO.Id))
                return Forbid();

            KpiPeriod KpiPeriod = await KpiPeriodService.Get(KpiPeriod_KpiPeriodDTO.Id);
            return new KpiPeriod_KpiPeriodDTO(KpiPeriod);
        }

        [Route(KpiPeriodRoute.Create), HttpPost]
        public async Task<ActionResult<KpiPeriod_KpiPeriodDTO>> Create([FromBody] KpiPeriod_KpiPeriodDTO KpiPeriod_KpiPeriodDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(KpiPeriod_KpiPeriodDTO.Id))
                return Forbid();

            KpiPeriod KpiPeriod = ConvertDTOToEntity(KpiPeriod_KpiPeriodDTO);
            KpiPeriod = await KpiPeriodService.Create(KpiPeriod);
            KpiPeriod_KpiPeriodDTO = new KpiPeriod_KpiPeriodDTO(KpiPeriod);
            if (KpiPeriod.IsValidated)
                return KpiPeriod_KpiPeriodDTO;
            else
                return BadRequest(KpiPeriod_KpiPeriodDTO);
        }

        [Route(KpiPeriodRoute.Update), HttpPost]
        public async Task<ActionResult<KpiPeriod_KpiPeriodDTO>> Update([FromBody] KpiPeriod_KpiPeriodDTO KpiPeriod_KpiPeriodDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(KpiPeriod_KpiPeriodDTO.Id))
                return Forbid();

            KpiPeriod KpiPeriod = ConvertDTOToEntity(KpiPeriod_KpiPeriodDTO);
            KpiPeriod = await KpiPeriodService.Update(KpiPeriod);
            KpiPeriod_KpiPeriodDTO = new KpiPeriod_KpiPeriodDTO(KpiPeriod);
            if (KpiPeriod.IsValidated)
                return KpiPeriod_KpiPeriodDTO;
            else
                return BadRequest(KpiPeriod_KpiPeriodDTO);
        }

        [Route(KpiPeriodRoute.Delete), HttpPost]
        public async Task<ActionResult<KpiPeriod_KpiPeriodDTO>> Delete([FromBody] KpiPeriod_KpiPeriodDTO KpiPeriod_KpiPeriodDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiPeriod_KpiPeriodDTO.Id))
                return Forbid();

            KpiPeriod KpiPeriod = ConvertDTOToEntity(KpiPeriod_KpiPeriodDTO);
            KpiPeriod = await KpiPeriodService.Delete(KpiPeriod);
            KpiPeriod_KpiPeriodDTO = new KpiPeriod_KpiPeriodDTO(KpiPeriod);
            if (KpiPeriod.IsValidated)
                return KpiPeriod_KpiPeriodDTO;
            else
                return BadRequest(KpiPeriod_KpiPeriodDTO);
        }
        
        [Route(KpiPeriodRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter = KpiPeriodService.ToFilter(KpiPeriodFilter);
            KpiPeriodFilter.Id = new IdFilter { In = Ids };
            KpiPeriodFilter.Selects = KpiPeriodSelect.Id;
            KpiPeriodFilter.Skip = 0;
            KpiPeriodFilter.Take = int.MaxValue;

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            KpiPeriods = await KpiPeriodService.BulkDelete(KpiPeriods);
            return true;
        }
        
        [Route(KpiPeriodRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<KpiPeriod> KpiPeriods = new List<KpiPeriod>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(KpiPeriods);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    
                    KpiPeriod KpiPeriod = new KpiPeriod();
                    KpiPeriod.Code = CodeValue;
                    KpiPeriod.Name = NameValue;
                    
                    KpiPeriods.Add(KpiPeriod);
                }
            }
            KpiPeriods = await KpiPeriodService.Import(KpiPeriods);
            if (KpiPeriods.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < KpiPeriods.Count; i++)
                {
                    KpiPeriod KpiPeriod = KpiPeriods[i];
                    if (!KpiPeriod.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (KpiPeriod.Errors.ContainsKey(nameof(KpiPeriod.Id)))
                            Error += KpiPeriod.Errors[nameof(KpiPeriod.Id)];
                        if (KpiPeriod.Errors.ContainsKey(nameof(KpiPeriod.Code)))
                            Error += KpiPeriod.Errors[nameof(KpiPeriod.Code)];
                        if (KpiPeriod.Errors.ContainsKey(nameof(KpiPeriod.Name)))
                            Error += KpiPeriod.Errors[nameof(KpiPeriod.Name)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(KpiPeriodRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] KpiPeriod_KpiPeriodFilterDTO KpiPeriod_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region KpiPeriod
                var KpiPeriodFilter = ConvertFilterDTOToFilterEntity(KpiPeriod_KpiPeriodFilterDTO);
                KpiPeriodFilter.Skip = 0;
                KpiPeriodFilter.Take = int.MaxValue;
                KpiPeriodFilter = KpiPeriodService.ToFilter(KpiPeriodFilter);
                List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);

                var KpiPeriodHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> KpiPeriodData = new List<object[]>();
                for (int i = 0; i < KpiPeriods.Count; i++)
                {
                    var KpiPeriod = KpiPeriods[i];
                    KpiPeriodData.Add(new Object[]
                    {
                        KpiPeriod.Id,
                        KpiPeriod.Code,
                        KpiPeriod.Name,
                    });
                }
                excel.GenerateWorksheet("KpiPeriod", KpiPeriodHeaders, KpiPeriodData);
                #endregion
                
                #region ItemSpecificKpi
                var ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
                ItemSpecificKpiFilter.Selects = ItemSpecificKpiSelect.ALL;
                ItemSpecificKpiFilter.OrderBy = ItemSpecificKpiOrder.Id;
                ItemSpecificKpiFilter.OrderType = OrderType.ASC;
                ItemSpecificKpiFilter.Skip = 0;
                ItemSpecificKpiFilter.Take = int.MaxValue;
                List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);

                var ItemSpecificKpiHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "OrganizationId",
                        "KpiPeriodId",
                        "StatusId",
                        "EmployeeId",
                        "CreatorId",
                    }
                };
                List<object[]> ItemSpecificKpiData = new List<object[]>();
                for (int i = 0; i < ItemSpecificKpis.Count; i++)
                {
                    var ItemSpecificKpi = ItemSpecificKpis[i];
                    ItemSpecificKpiData.Add(new Object[]
                    {
                        ItemSpecificKpi.Id,
                        ItemSpecificKpi.OrganizationId,
                        ItemSpecificKpi.KpiPeriodId,
                        ItemSpecificKpi.StatusId,
                        ItemSpecificKpi.EmployeeId,
                        ItemSpecificKpi.CreatorId,
                    });
                }
                excel.GenerateWorksheet("ItemSpecificKpi", ItemSpecificKpiHeaders, ItemSpecificKpiData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "KpiPeriod.xlsx");
        }

        [Route(KpiPeriodRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] KpiPeriod_KpiPeriodFilterDTO KpiPeriod_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region KpiPeriod
                var KpiPeriodHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> KpiPeriodData = new List<object[]>();
                excel.GenerateWorksheet("KpiPeriod", KpiPeriodHeaders, KpiPeriodData);
                #endregion
                
                #region ItemSpecificKpi
                var ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
                ItemSpecificKpiFilter.Selects = ItemSpecificKpiSelect.ALL;
                ItemSpecificKpiFilter.OrderBy = ItemSpecificKpiOrder.Id;
                ItemSpecificKpiFilter.OrderType = OrderType.ASC;
                ItemSpecificKpiFilter.Skip = 0;
                ItemSpecificKpiFilter.Take = int.MaxValue;
                List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);

                var ItemSpecificKpiHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "OrganizationId",
                        "KpiPeriodId",
                        "StatusId",
                        "EmployeeId",
                        "CreatorId",
                    }
                };
                List<object[]> ItemSpecificKpiData = new List<object[]>();
                for (int i = 0; i < ItemSpecificKpis.Count; i++)
                {
                    var ItemSpecificKpi = ItemSpecificKpis[i];
                    ItemSpecificKpiData.Add(new Object[]
                    {
                        ItemSpecificKpi.Id,
                        ItemSpecificKpi.OrganizationId,
                        ItemSpecificKpi.KpiPeriodId,
                        ItemSpecificKpi.StatusId,
                        ItemSpecificKpi.EmployeeId,
                        ItemSpecificKpi.CreatorId,
                    });
                }
                excel.GenerateWorksheet("ItemSpecificKpi", ItemSpecificKpiHeaders, ItemSpecificKpiData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "KpiPeriod.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter = KpiPeriodService.ToFilter(KpiPeriodFilter);
            if (Id == 0)
            {

            }
            else
            {
                KpiPeriodFilter.Id = new IdFilter { Equal = Id };
                int count = await KpiPeriodService.Count(KpiPeriodFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private KpiPeriod ConvertDTOToEntity(KpiPeriod_KpiPeriodDTO KpiPeriod_KpiPeriodDTO)
        {
            KpiPeriod KpiPeriod = new KpiPeriod();
            KpiPeriod.Id = KpiPeriod_KpiPeriodDTO.Id;
            KpiPeriod.Code = KpiPeriod_KpiPeriodDTO.Code;
            KpiPeriod.Name = KpiPeriod_KpiPeriodDTO.Name;
            KpiPeriod.ItemSpecificKpis = KpiPeriod_KpiPeriodDTO.ItemSpecificKpis?
                .Select(x => new ItemSpecificKpi
                {
                    Id = x.Id,
                    OrganizationId = x.OrganizationId,
                    StatusId = x.StatusId,
                    EmployeeId = x.EmployeeId,
                    CreatorId = x.CreatorId,
                    Organization = x.Organization == null ? null : new Organization
                    {
                        Id = x.Organization.Id,
                        Code = x.Organization.Code,
                        Name = x.Organization.Name,
                        ParentId = x.Organization.ParentId,
                        Path = x.Organization.Path,
                        Level = x.Organization.Level,
                        StatusId = x.Organization.StatusId,
                        Phone = x.Organization.Phone,
                        Email = x.Organization.Email,
                        Address = x.Organization.Address,
                    },
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).ToList();
            KpiPeriod.BaseLanguage = CurrentContext.Language;
            return KpiPeriod;
        }

        private KpiPeriodFilter ConvertFilterDTOToFilterEntity(KpiPeriod_KpiPeriodFilterDTO KpiPeriod_KpiPeriodFilterDTO)
        {
            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
            KpiPeriodFilter.Skip = KpiPeriod_KpiPeriodFilterDTO.Skip;
            KpiPeriodFilter.Take = KpiPeriod_KpiPeriodFilterDTO.Take;
            KpiPeriodFilter.OrderBy = KpiPeriod_KpiPeriodFilterDTO.OrderBy;
            KpiPeriodFilter.OrderType = KpiPeriod_KpiPeriodFilterDTO.OrderType;

            KpiPeriodFilter.Id = KpiPeriod_KpiPeriodFilterDTO.Id;
            KpiPeriodFilter.Code = KpiPeriod_KpiPeriodFilterDTO.Code;
            KpiPeriodFilter.Name = KpiPeriod_KpiPeriodFilterDTO.Name;
            return KpiPeriodFilter;
        }

        [Route(KpiPeriodRoute.FilterListItemSpecificKpi), HttpPost]
        public async Task<List<KpiPeriod_ItemSpecificKpiDTO>> FilterListItemSpecificKpi([FromBody] KpiPeriod_ItemSpecificKpiFilterDTO KpiPeriod_ItemSpecificKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
            ItemSpecificKpiFilter.Skip = 0;
            ItemSpecificKpiFilter.Take = 20;
            ItemSpecificKpiFilter.OrderBy = ItemSpecificKpiOrder.Id;
            ItemSpecificKpiFilter.OrderType = OrderType.ASC;
            ItemSpecificKpiFilter.Selects = ItemSpecificKpiSelect.ALL;
            ItemSpecificKpiFilter.Id = KpiPeriod_ItemSpecificKpiFilterDTO.Id;
            ItemSpecificKpiFilter.OrganizationId = KpiPeriod_ItemSpecificKpiFilterDTO.OrganizationId;
            ItemSpecificKpiFilter.KpiPeriodId = KpiPeriod_ItemSpecificKpiFilterDTO.KpiPeriodId;
            ItemSpecificKpiFilter.StatusId = KpiPeriod_ItemSpecificKpiFilterDTO.StatusId;
            ItemSpecificKpiFilter.EmployeeId = KpiPeriod_ItemSpecificKpiFilterDTO.EmployeeId;
            ItemSpecificKpiFilter.CreatorId = KpiPeriod_ItemSpecificKpiFilterDTO.CreatorId;

            List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);
            List<KpiPeriod_ItemSpecificKpiDTO> KpiPeriod_ItemSpecificKpiDTOs = ItemSpecificKpis
                .Select(x => new KpiPeriod_ItemSpecificKpiDTO(x)).ToList();
            return KpiPeriod_ItemSpecificKpiDTOs;
        }
        [Route(KpiPeriodRoute.FilterListOrganization), HttpPost]
        public async Task<List<KpiPeriod_OrganizationDTO>> FilterListOrganization([FromBody] KpiPeriod_OrganizationFilterDTO KpiPeriod_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = KpiPeriod_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = KpiPeriod_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = KpiPeriod_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = KpiPeriod_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = KpiPeriod_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = KpiPeriod_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = KpiPeriod_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = KpiPeriod_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = KpiPeriod_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = KpiPeriod_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<KpiPeriod_OrganizationDTO> KpiPeriod_OrganizationDTOs = Organizations
                .Select(x => new KpiPeriod_OrganizationDTO(x)).ToList();
            return KpiPeriod_OrganizationDTOs;
        }
        [Route(KpiPeriodRoute.FilterListStatus), HttpPost]
        public async Task<List<KpiPeriod_StatusDTO>> FilterListStatus([FromBody] KpiPeriod_StatusFilterDTO KpiPeriod_StatusFilterDTO)
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
            List<KpiPeriod_StatusDTO> KpiPeriod_StatusDTOs = Statuses
                .Select(x => new KpiPeriod_StatusDTO(x)).ToList();
            return KpiPeriod_StatusDTOs;
        }

        [Route(KpiPeriodRoute.SingleListItemSpecificKpi), HttpPost]
        public async Task<List<KpiPeriod_ItemSpecificKpiDTO>> SingleListItemSpecificKpi([FromBody] KpiPeriod_ItemSpecificKpiFilterDTO KpiPeriod_ItemSpecificKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
            ItemSpecificKpiFilter.Skip = 0;
            ItemSpecificKpiFilter.Take = 20;
            ItemSpecificKpiFilter.OrderBy = ItemSpecificKpiOrder.Id;
            ItemSpecificKpiFilter.OrderType = OrderType.ASC;
            ItemSpecificKpiFilter.Selects = ItemSpecificKpiSelect.ALL;
            ItemSpecificKpiFilter.Id = KpiPeriod_ItemSpecificKpiFilterDTO.Id;
            ItemSpecificKpiFilter.OrganizationId = KpiPeriod_ItemSpecificKpiFilterDTO.OrganizationId;
            ItemSpecificKpiFilter.KpiPeriodId = KpiPeriod_ItemSpecificKpiFilterDTO.KpiPeriodId;
            ItemSpecificKpiFilter.StatusId = KpiPeriod_ItemSpecificKpiFilterDTO.StatusId;
            ItemSpecificKpiFilter.EmployeeId = KpiPeriod_ItemSpecificKpiFilterDTO.EmployeeId;
            ItemSpecificKpiFilter.CreatorId = KpiPeriod_ItemSpecificKpiFilterDTO.CreatorId;

            List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);
            List<KpiPeriod_ItemSpecificKpiDTO> KpiPeriod_ItemSpecificKpiDTOs = ItemSpecificKpis
                .Select(x => new KpiPeriod_ItemSpecificKpiDTO(x)).ToList();
            return KpiPeriod_ItemSpecificKpiDTOs;
        }
        [Route(KpiPeriodRoute.SingleListOrganization), HttpPost]
        public async Task<List<KpiPeriod_OrganizationDTO>> SingleListOrganization([FromBody] KpiPeriod_OrganizationFilterDTO KpiPeriod_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = KpiPeriod_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = KpiPeriod_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = KpiPeriod_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = KpiPeriod_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = KpiPeriod_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = KpiPeriod_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = KpiPeriod_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = KpiPeriod_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = KpiPeriod_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = KpiPeriod_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<KpiPeriod_OrganizationDTO> KpiPeriod_OrganizationDTOs = Organizations
                .Select(x => new KpiPeriod_OrganizationDTO(x)).ToList();
            return KpiPeriod_OrganizationDTOs;
        }
        [Route(KpiPeriodRoute.SingleListStatus), HttpPost]
        public async Task<List<KpiPeriod_StatusDTO>> SingleListStatus([FromBody] KpiPeriod_StatusFilterDTO KpiPeriod_StatusFilterDTO)
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
            List<KpiPeriod_StatusDTO> KpiPeriod_StatusDTOs = Statuses
                .Select(x => new KpiPeriod_StatusDTO(x)).ToList();
            return KpiPeriod_StatusDTOs;
        }

    }
}

