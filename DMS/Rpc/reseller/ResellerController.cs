using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MReseller;
using DMS.Services.MResellerStatus;
using DMS.Services.MResellerType;
using DMS.Services.MStatus;
using DMS.Services.MStore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reseller
{


    public class ResellerController : RpcController
    {
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IResellerStatusService ResellerStatusService;
        private IResellerTypeService ResellerTypeService;
        private IResellerService ResellerService;
        private IStatusService StatusService;
        private IStoreService StoreService;
        private ICurrentContext CurrentContext;
        public ResellerController(
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            IResellerStatusService ResellerStatusService,
            IResellerTypeService ResellerTypeService,
            IResellerService ResellerService,
            IStatusService StatusService,
            IStoreService StoreService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.ResellerStatusService = ResellerStatusService;
            this.ResellerTypeService = ResellerTypeService;
            this.ResellerService = ResellerService;
            this.StatusService = StatusService;
            this.StoreService = StoreService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ResellerRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Reseller_ResellerFilterDTO Reseller_ResellerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ResellerFilter ResellerFilter = ConvertFilterDTOToFilterEntity(Reseller_ResellerFilterDTO);
            ResellerFilter = ResellerService.ToFilter(ResellerFilter);
            int count = await ResellerService.Count(ResellerFilter);
            return count;
        }

        [Route(ResellerRoute.List), HttpPost]
        public async Task<ActionResult<List<Reseller_ResellerDTO>>> List([FromBody] Reseller_ResellerFilterDTO Reseller_ResellerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ResellerFilter ResellerFilter = ConvertFilterDTOToFilterEntity(Reseller_ResellerFilterDTO);
            ResellerFilter = ResellerService.ToFilter(ResellerFilter);
            List<Reseller> Resellers = await ResellerService.List(ResellerFilter);
            List<Reseller_ResellerDTO> Reseller_ResellerDTOs = Resellers
                .Select(c => new Reseller_ResellerDTO(c)).ToList();
            return Reseller_ResellerDTOs;
        }

        [Route(ResellerRoute.Get), HttpPost]
        public async Task<ActionResult<Reseller_ResellerDTO>> Get([FromBody]Reseller_ResellerDTO Reseller_ResellerDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Reseller_ResellerDTO.Id))
                return Forbid();

            Reseller Reseller = await ResellerService.Get(Reseller_ResellerDTO.Id);
            return new Reseller_ResellerDTO(Reseller);
        }

        [Route(ResellerRoute.Create), HttpPost]
        public async Task<ActionResult<Reseller_ResellerDTO>> Create([FromBody] Reseller_ResellerDTO Reseller_ResellerDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Reseller_ResellerDTO.Id))
                return Forbid();

            Reseller Reseller = ConvertDTOToEntity(Reseller_ResellerDTO);
            Reseller.ResellerStatusId = ResellerStatusEnum.NEW.Id;
            Reseller.ResellerStatus = new ResellerStatus
            {
                Id = ResellerStatusEnum.NEW.Id,
                Code = ResellerStatusEnum.NEW.Code,
                Name = ResellerStatusEnum.NEW.Name
            };
            Reseller = await ResellerService.Create(Reseller);
            Reseller_ResellerDTO = new Reseller_ResellerDTO(Reseller);
            if (Reseller.IsValidated)
                return Reseller_ResellerDTO;
            else
                return BadRequest(Reseller_ResellerDTO);
        }

        [Route(ResellerRoute.Update), HttpPost]
        public async Task<ActionResult<Reseller_ResellerDTO>> Update([FromBody] Reseller_ResellerDTO Reseller_ResellerDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Reseller_ResellerDTO.Id))
                return Forbid();

            Reseller Reseller = ConvertDTOToEntity(Reseller_ResellerDTO);
            Reseller.ResellerStatusId = ResellerStatusEnum.PENDING.Id;
            Reseller.ResellerStatus = new ResellerStatus
            {
                Id = ResellerStatusEnum.PENDING.Id,
                Code = ResellerStatusEnum.PENDING.Code,
                Name = ResellerStatusEnum.PENDING.Name
            };
            Reseller = await ResellerService.Update(Reseller);
            Reseller_ResellerDTO = new Reseller_ResellerDTO(Reseller);
            if (Reseller.IsValidated)
                return Reseller_ResellerDTO;
            else
                return BadRequest(Reseller_ResellerDTO);
        }

        [Route(ResellerRoute.Approve), HttpPost]
        public async Task<ActionResult<Reseller_ResellerDTO>> Approve([FromBody] Reseller_ResellerDTO Reseller_ResellerDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Reseller_ResellerDTO.Id))
                return Forbid();

            Reseller Reseller = ConvertDTOToEntity(Reseller_ResellerDTO);
            Reseller.ResellerStatusId = ResellerStatusEnum.APPROVED.Id;
            Reseller.ResellerStatus = new ResellerStatus
            {
                Id = ResellerStatusEnum.APPROVED.Id,
                Code = ResellerStatusEnum.APPROVED.Code,
                Name = ResellerStatusEnum.APPROVED.Name
            };
            Reseller = await ResellerService.Update(Reseller);
            Reseller_ResellerDTO = new Reseller_ResellerDTO(Reseller);
            if (Reseller.IsValidated)
                return Reseller_ResellerDTO;
            else
                return BadRequest(Reseller_ResellerDTO);
        }

        [Route(ResellerRoute.Reject), HttpPost]
        public async Task<ActionResult<Reseller_ResellerDTO>> Reject([FromBody] Reseller_ResellerDTO Reseller_ResellerDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Reseller_ResellerDTO.Id))
                return Forbid();

            Reseller Reseller = ConvertDTOToEntity(Reseller_ResellerDTO);
            Reseller.ResellerStatusId = ResellerStatusEnum.REJECTED.Id;
            Reseller.ResellerStatus = new ResellerStatus
            {
                Id = ResellerStatusEnum.REJECTED.Id,
                Code = ResellerStatusEnum.REJECTED.Code,
                Name = ResellerStatusEnum.REJECTED.Name
            };
            Reseller = await ResellerService.Update(Reseller);
            Reseller_ResellerDTO = new Reseller_ResellerDTO(Reseller);
            if (Reseller.IsValidated)
                return Reseller_ResellerDTO;
            else
                return BadRequest(Reseller_ResellerDTO);
        }

        [Route(ResellerRoute.Delete), HttpPost]
        public async Task<ActionResult<Reseller_ResellerDTO>> Delete([FromBody] Reseller_ResellerDTO Reseller_ResellerDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Reseller_ResellerDTO.Id))
                return Forbid();

            Reseller Reseller = ConvertDTOToEntity(Reseller_ResellerDTO);
            Reseller = await ResellerService.Delete(Reseller);
            Reseller_ResellerDTO = new Reseller_ResellerDTO(Reseller);
            if (Reseller.IsValidated)
                return Reseller_ResellerDTO;
            else
                return BadRequest(Reseller_ResellerDTO);
        }

        [Route(ResellerRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ResellerFilter ResellerFilter = new ResellerFilter();
            ResellerFilter = ResellerService.ToFilter(ResellerFilter);
            ResellerFilter.Id = new IdFilter { In = Ids };
            ResellerFilter.Selects = ResellerSelect.Id;
            ResellerFilter.Skip = 0;
            ResellerFilter.Take = int.MaxValue;

            List<Reseller> Resellers = await ResellerService.List(ResellerFilter);
            Resellers = await ResellerService.BulkDelete(Resellers);
            if (Resellers.Any(x => !x.IsValidated))
                return BadRequest(Resellers.Where(x => !x.IsValidated));
            return true;
        }

        [Route(ResellerRoute.Import), HttpPost]
        public async Task<ActionResult<List<Reseller_ResellerDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            OrganizationFilter OrganizationFilter = new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.ALL
            };
            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            ResellerStatusFilter ResellerStatusFilter = new ResellerStatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ResellerStatusSelect.ALL
            };
            List<ResellerStatus> ResellerStatuses = await ResellerStatusService.List(ResellerStatusFilter);
            ResellerTypeFilter ResellerTypeFilter = new ResellerTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ResellerTypeSelect.ALL
            };
            List<ResellerType> ResellerTypes = await ResellerTypeService.List(ResellerTypeFilter);

            List<Reseller> Resellers = new List<Reseller>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Resellers);
                int StartColumn = 1;
                int StartRow = 1;

                int CodeColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int PhoneColumn = 2 + StartColumn;
                int EmailColumn = 3 + StartColumn;
                int AddressColumn = 4 + StartColumn;
                int TaxCodeColumn = 5 + StartColumn;
                int CompanyNameColumn = 6 + StartColumn;
                int DeputyNameColumn = 7 + StartColumn;
                int OrganizationCodeColumn = 8 + StartColumn;
                int ResellerTypeCodeColumn = 9 + StartColumn;
                int ResellerStatusCodeColumn = 10 + StartColumn;
                int StatusCodeColumn = 11 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string PhoneValue = worksheet.Cells[i + StartRow, PhoneColumn].Value?.ToString();
                    string EmailValue = worksheet.Cells[i + StartRow, EmailColumn].Value?.ToString();
                    string AddressValue = worksheet.Cells[i + StartRow, AddressColumn].Value?.ToString();
                    string TaxCodeValue = worksheet.Cells[i + StartRow, TaxCodeColumn].Value?.ToString();
                    string CompanyNameValue = worksheet.Cells[i + StartRow, CompanyNameColumn].Value?.ToString();
                    string DeputyNameValue = worksheet.Cells[i + StartRow, DeputyNameColumn].Value?.ToString();

                    string OrganizationCodeValue = worksheet.Cells[i + StartRow, OrganizationCodeColumn].Value?.ToString();
                    string ResellerTypeCodeValue = worksheet.Cells[i + StartRow, ResellerTypeCodeColumn].Value?.ToString();
                    string ResellerStatusCodeValue = worksheet.Cells[i + StartRow, ResellerStatusCodeColumn].Value?.ToString();
                    string StatusCodeValue = worksheet.Cells[i + StartRow, StatusCodeColumn].Value?.ToString();

                    Reseller Reseller = new Reseller()
                    {
                        Code = CodeValue,
                        Name = NameValue,
                        Phone = PhoneValue,
                        Email = EmailValue,
                        Address = AddressValue,
                        TaxCode = TaxCodeValue,
                        CompanyName = CompanyNameValue,
                        DeputyName = DeputyNameValue,

                        Organization = Organizations.Where(x => x.Code.ToString() == OrganizationCodeValue).FirstOrDefault(),
                        ResellerStatus = ResellerStatuses.Where(x => x.Code.ToString() == ResellerStatusCodeValue).FirstOrDefault(),
                        ResellerType = ResellerTypes.Where(x => x.Code.ToString() == ResellerTypeCodeValue).FirstOrDefault(),
                    };
                    Resellers.Add(Reseller);
                }
            }
            Resellers = await ResellerService.Import(Resellers);

            List<Reseller_ResellerDTO> Reseller_ResellerDTOs = Resellers
                .Select(c => new Reseller_ResellerDTO(c)).ToList();
            return Reseller_ResellerDTOs;
        }


        [Route(ResellerRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] Reseller_ResellerFilterDTO Reseller_ResellerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var ResellerFilter = ConvertFilterDTOToFilterEntity(Reseller_ResellerFilterDTO);
            ResellerFilter = ResellerService.ToFilter(ResellerFilter);
            ResellerFilter.Skip = 0;
            ResellerFilter.Take = int.MaxValue;
            ResellerFilter = ResellerService.ToFilter(ResellerFilter);

            List<Reseller> Resellers = await ResellerService.List(ResellerFilter);
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                var ResellerHeaders = new List<string[]>()
                {
                    new string[] {
                        "Mã khách hàng",
                        "Tên khách hàng",
                        "Số điện thoại",
                        "Email",
                        "Địa chỉ",
                        "Mã số thuế",
                        "Tên công ty",
                        "Tên người đại diện",
                        "Mã tổ chức",
                        "Mã loại khách hàng",
                        "Mã trạng thái khách hàng",
                    }
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < Resellers.Count; i++)
                {
                    var Reseller = Resellers[i];
                    data.Add(new Object[]
                    {
                        Reseller.Code,
                        Reseller.Name,
                        Reseller.Phone,
                        Reseller.Email,
                        Reseller.Address,
                        Reseller.TaxCode,
                        Reseller.CompanyName,
                        Reseller.DeputyName,
                        Reseller.Organization == null ? null : Reseller.Organization.Code,
                        Reseller.ResellerType == null ? null : Reseller.ResellerType.Code,
                        Reseller.ResellerStatus,
                    });
                }
                excel.GenerateWorksheet("Reseller", ResellerHeaders, data);
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Reseller.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ResellerFilter ResellerFilter = new ResellerFilter();
            ResellerFilter = ResellerService.ToFilter(ResellerFilter);
            if (Id == 0)
            {

            }
            else
            {
                ResellerFilter.Id = new IdFilter { Equal = Id };
                int count = await ResellerService.Count(ResellerFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Reseller ConvertDTOToEntity(Reseller_ResellerDTO Reseller_ResellerDTO)
        {
            Reseller Reseller = new Reseller();
            Reseller.Id = Reseller_ResellerDTO.Id;
            Reseller.Code = Reseller_ResellerDTO.Code;
            Reseller.Name = Reseller_ResellerDTO.Name;
            Reseller.Phone = Reseller_ResellerDTO.Phone;
            Reseller.Email = Reseller_ResellerDTO.Email;
            Reseller.Address = Reseller_ResellerDTO.Address;
            Reseller.TaxCode = Reseller_ResellerDTO.TaxCode;
            Reseller.CompanyName = Reseller_ResellerDTO.CompanyName;
            Reseller.DeputyName = Reseller_ResellerDTO.DeputyName;
            Reseller.Description = Reseller_ResellerDTO.Description;
            Reseller.ResellerTypeId = Reseller_ResellerDTO.ResellerTypeId;
            Reseller.ResellerStatusId = Reseller_ResellerDTO.ResellerStatusId;
            Reseller.StaffId = Reseller_ResellerDTO.StaffId;
            Reseller.ResellerStatus = Reseller_ResellerDTO.ResellerStatus == null ? null : new ResellerStatus
            {
                Id = Reseller_ResellerDTO.ResellerStatus.Id,
                Code = Reseller_ResellerDTO.ResellerStatus.Code,
                Name = Reseller_ResellerDTO.ResellerStatus.Name,
            };
            Reseller.ResellerType = Reseller_ResellerDTO.ResellerType == null ? null : new ResellerType
            {
                Id = Reseller_ResellerDTO.ResellerType.Id,
                Code = Reseller_ResellerDTO.ResellerType.Code,
                Name = Reseller_ResellerDTO.ResellerType.Name,
                StatusId = Reseller_ResellerDTO.ResellerType.StatusId,
            };
            Reseller.Staff = Reseller_ResellerDTO.Staff == null ? null : new AppUser
            {
                Id = Reseller_ResellerDTO.Staff.Id,
                Username = Reseller_ResellerDTO.Staff.Username,
                DisplayName = Reseller_ResellerDTO.Staff.DisplayName,
                SexId = Reseller_ResellerDTO.Staff.SexId,
                Address = Reseller_ResellerDTO.Staff.Address,
                Email = Reseller_ResellerDTO.Staff.Email,
                Phone = Reseller_ResellerDTO.Staff.Phone,
                StatusId = Reseller_ResellerDTO.Staff.StatusId,
            };
            Reseller.BaseLanguage = CurrentContext.Language;
            return Reseller;
        }

        private ResellerFilter ConvertFilterDTOToFilterEntity(Reseller_ResellerFilterDTO Reseller_ResellerFilterDTO)
        {
            ResellerFilter ResellerFilter = new ResellerFilter();
            ResellerFilter.Selects = ResellerSelect.ALL;
            ResellerFilter.Skip = Reseller_ResellerFilterDTO.Skip;
            ResellerFilter.Take = Reseller_ResellerFilterDTO.Take;
            ResellerFilter.OrderBy = Reseller_ResellerFilterDTO.OrderBy;
            ResellerFilter.OrderType = Reseller_ResellerFilterDTO.OrderType;

            ResellerFilter.Id = Reseller_ResellerFilterDTO.Id;
            ResellerFilter.Code = Reseller_ResellerFilterDTO.Code;
            ResellerFilter.Name = Reseller_ResellerFilterDTO.Name;
            ResellerFilter.Phone = Reseller_ResellerFilterDTO.Phone;
            ResellerFilter.Email = Reseller_ResellerFilterDTO.Email;
            ResellerFilter.Address = Reseller_ResellerFilterDTO.Address;
            ResellerFilter.TaxCode = Reseller_ResellerFilterDTO.TaxCode;
            ResellerFilter.CompanyName = Reseller_ResellerFilterDTO.CompanyName;
            ResellerFilter.DeputyName = Reseller_ResellerFilterDTO.DeputyName;
            ResellerFilter.Description = Reseller_ResellerFilterDTO.Description;
            ResellerFilter.ResellerTypeId = Reseller_ResellerFilterDTO.ResellerTypeId;
            ResellerFilter.ResellerStatusId = Reseller_ResellerFilterDTO.ResellerStatusId;
            ResellerFilter.StaffId = Reseller_ResellerFilterDTO.StaffId;
            return ResellerFilter;
        }
        [Route(ResellerRoute.SingleListAppUser), HttpPost]
        public async Task<List<Reseller_AppUserDTO>> SingleListAppUser([FromBody] Reseller_AppUserFilterDTO Reseller_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Reseller_AppUserFilterDTO.Id;
            AppUserFilter.Username = Reseller_AppUserFilterDTO.Username;
            AppUserFilter.Password = Reseller_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = Reseller_AppUserFilterDTO.DisplayName;
            AppUserFilter.SexId = Reseller_AppUserFilterDTO.SexId;
            AppUserFilter.Address = Reseller_AppUserFilterDTO.Address;
            AppUserFilter.Email = Reseller_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Reseller_AppUserFilterDTO.Phone;
            AppUserFilter.StatusId = Reseller_AppUserFilterDTO.StatusId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Reseller_AppUserDTO> Reseller_AppUserDTOs = AppUsers
                .Select(x => new Reseller_AppUserDTO(x)).ToList();
            return Reseller_AppUserDTOs;
        }
        [Route(ResellerRoute.SingleListOrganization), HttpPost]
        public async Task<List<Reseller_OrganizationDTO>> SingleListOrganization([FromBody] Reseller_OrganizationFilterDTO Reseller_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = Reseller_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = Reseller_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = Reseller_OrganizationFilterDTO.Name;
            OrganizationFilter.Address = Reseller_OrganizationFilterDTO.Address;
            OrganizationFilter.Email = Reseller_OrganizationFilterDTO.Email;
            OrganizationFilter.ParentId = Reseller_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = Reseller_OrganizationFilterDTO.Path;
            OrganizationFilter.Phone = Reseller_OrganizationFilterDTO.Phone;
            OrganizationFilter.Level = Reseller_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<Reseller_OrganizationDTO> Reseller_OrganizationDTOs = Organizations
                .Select(x => new Reseller_OrganizationDTO(x)).ToList();
            return Reseller_OrganizationDTOs;
        }
        [Route(ResellerRoute.SingleListResellerStatus), HttpPost]
        public async Task<List<Reseller_ResellerStatusDTO>> SingleListResellerStatus([FromBody] Reseller_ResellerStatusFilterDTO Reseller_ResellerStatusFilterDTO)
        {
            ResellerStatusFilter ResellerStatusFilter = new ResellerStatusFilter();
            ResellerStatusFilter.Skip = 0;
            ResellerStatusFilter.Take = 20;
            ResellerStatusFilter.OrderBy = ResellerStatusOrder.Id;
            ResellerStatusFilter.OrderType = OrderType.ASC;
            ResellerStatusFilter.Selects = ResellerStatusSelect.ALL;
            ResellerStatusFilter.Id = Reseller_ResellerStatusFilterDTO.Id;
            ResellerStatusFilter.Code = Reseller_ResellerStatusFilterDTO.Code;
            ResellerStatusFilter.Name = Reseller_ResellerStatusFilterDTO.Name;

            List<ResellerStatus> ResellerStatuses = await ResellerStatusService.List(ResellerStatusFilter);
            List<Reseller_ResellerStatusDTO> Reseller_ResellerStatusDTOs = ResellerStatuses
                .Select(x => new Reseller_ResellerStatusDTO(x)).ToList();
            return Reseller_ResellerStatusDTOs;
        }
        [Route(ResellerRoute.SingleListResellerType), HttpPost]
        public async Task<List<Reseller_ResellerTypeDTO>> SingleListResellerType([FromBody] Reseller_ResellerTypeFilterDTO Reseller_ResellerTypeFilterDTO)
        {
            ResellerTypeFilter ResellerTypeFilter = new ResellerTypeFilter();
            ResellerTypeFilter.Skip = 0;
            ResellerTypeFilter.Take = 20;
            ResellerTypeFilter.OrderBy = ResellerTypeOrder.Id;
            ResellerTypeFilter.OrderType = OrderType.ASC;
            ResellerTypeFilter.Selects = ResellerTypeSelect.ALL;
            ResellerTypeFilter.Id = Reseller_ResellerTypeFilterDTO.Id;
            ResellerTypeFilter.Code = Reseller_ResellerTypeFilterDTO.Code;
            ResellerTypeFilter.Name = Reseller_ResellerTypeFilterDTO.Name;
            ResellerTypeFilter.StatusId = Reseller_ResellerTypeFilterDTO.StatusId;

            List<ResellerType> ResellerTypes = await ResellerTypeService.List(ResellerTypeFilter);
            List<Reseller_ResellerTypeDTO> Reseller_ResellerTypeDTOs = ResellerTypes
                .Select(x => new Reseller_ResellerTypeDTO(x)).ToList();
            return Reseller_ResellerTypeDTOs;
        }

        [Route(ResellerRoute.SingleListStatus), HttpPost]
        public async Task<List<Reseller_StatusDTO>> SingleListStatus([FromBody] Reseller_StatusFilterDTO Reseller_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = Reseller_StatusFilterDTO.Id;
            StatusFilter.Code = Reseller_StatusFilterDTO.Code;
            StatusFilter.Name = Reseller_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Reseller_StatusDTO> Reseller_StatusDTOs = Statuses
                .Select(x => new Reseller_StatusDTO(x)).ToList();
            return Reseller_StatusDTOs;
        }

        [Route(ResellerRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] Reseller_StoreFilterDTO Reseller_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Id = Reseller_StoreFilterDTO.Id;
            StoreFilter.Code = Reseller_StoreFilterDTO.Code;
            StoreFilter.Name = Reseller_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Reseller_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = Reseller_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = Reseller_StoreFilterDTO.StoreTypeId;
            StoreFilter.ResellerId = Reseller_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = Reseller_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = Reseller_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Reseller_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Reseller_StoreFilterDTO.WardId;
            StoreFilter.Address = Reseller_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Reseller_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Reseller_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Reseller_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = Reseller_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Reseller_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Reseller_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            return await StoreService.Count(StoreFilter);
        }

        [Route(ResellerRoute.ListStore), HttpPost]
        public async Task<List<Reseller_StoreDTO>> ListStore([FromBody] Reseller_StoreFilterDTO Reseller_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = Reseller_StoreFilterDTO.Skip;
            StoreFilter.Take = Reseller_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Reseller_StoreFilterDTO.Id;
            StoreFilter.Code = Reseller_StoreFilterDTO.Code;
            StoreFilter.Name = Reseller_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Reseller_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = Reseller_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = Reseller_StoreFilterDTO.StoreTypeId;
            StoreFilter.ResellerId = Reseller_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = Reseller_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = Reseller_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Reseller_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Reseller_StoreFilterDTO.WardId;
            StoreFilter.Address = Reseller_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Reseller_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Reseller_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Reseller_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = Reseller_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Reseller_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Reseller_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<Reseller_StoreDTO> Reseller_StoreDTOs = Stores
                .Select(x => new Reseller_StoreDTO(x)).ToList();
            return Reseller_StoreDTOs;
        }
    }
}

