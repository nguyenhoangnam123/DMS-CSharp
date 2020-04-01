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
using DMS.Services.MReseller;
using DMS.Services.MResellerStatus;
using DMS.Services.MResellerType;
using DMS.Services.MAppUser;

namespace DMS.Rpc.reseller
{
    public class ResellerRoute : Root
    {
        public const string Master = Module + "/reseller/reseller-master";
        public const string Detail = Module + "/reseller/reseller-detail";
        private const string Default = Rpc + Module + "/reseller";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";
        public const string SingleListResellerStatus = Default + "/single-list-reseller-status";
        public const string SingleListResellerType = Default + "/single-list-reseller-type";
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ResellerFilter.Id), FieldType.ID },
            { nameof(ResellerFilter.Code), FieldType.STRING },
            { nameof(ResellerFilter.Name), FieldType.STRING },
            { nameof(ResellerFilter.Phone), FieldType.STRING },
            { nameof(ResellerFilter.Email), FieldType.STRING },
            { nameof(ResellerFilter.Address), FieldType.STRING },
            { nameof(ResellerFilter.TaxCode), FieldType.STRING },
            { nameof(ResellerFilter.CompanyName), FieldType.STRING },
            { nameof(ResellerFilter.DeputyName), FieldType.STRING },
            { nameof(ResellerFilter.Description), FieldType.STRING },
            { nameof(ResellerFilter.ResellerTypeId), FieldType.ID },
            { nameof(ResellerFilter.ResellerStatusId), FieldType.ID },
            { nameof(ResellerFilter.StaffId), FieldType.ID },
        };
    }

    public class ResellerController : RpcController
    {
        private IResellerStatusService ResellerStatusService;
        private IResellerTypeService ResellerTypeService;
        private IAppUserService AppUserService;
        private IResellerService ResellerService;
        private ICurrentContext CurrentContext;
        public ResellerController(
            IResellerStatusService ResellerStatusService,
            IResellerTypeService ResellerTypeService,
            IAppUserService AppUserService,
            IResellerService ResellerService,
            ICurrentContext CurrentContext
        )
        {
            this.ResellerStatusService = ResellerStatusService;
            this.ResellerTypeService = ResellerTypeService;
            this.AppUserService = AppUserService;
            this.ResellerService = ResellerService;
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
            return true;
        }
        
        [Route(ResellerRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
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
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Reseller> Resellers = new List<Reseller>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Resellers);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int PhoneColumn = 3 + StartColumn;
                int EmailColumn = 4 + StartColumn;
                int AddressColumn = 5 + StartColumn;
                int TaxCodeColumn = 6 + StartColumn;
                int CompanyNameColumn = 7 + StartColumn;
                int DeputyNameColumn = 8 + StartColumn;
                int DescriptionColumn = 9 + StartColumn;
                int ResellerTypeIdColumn = 10 + StartColumn;
                int ResellerStatusIdColumn = 11 + StartColumn;
                int StaffIdColumn = 12 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string PhoneValue = worksheet.Cells[i + StartRow, PhoneColumn].Value?.ToString();
                    string EmailValue = worksheet.Cells[i + StartRow, EmailColumn].Value?.ToString();
                    string AddressValue = worksheet.Cells[i + StartRow, AddressColumn].Value?.ToString();
                    string TaxCodeValue = worksheet.Cells[i + StartRow, TaxCodeColumn].Value?.ToString();
                    string CompanyNameValue = worksheet.Cells[i + StartRow, CompanyNameColumn].Value?.ToString();
                    string DeputyNameValue = worksheet.Cells[i + StartRow, DeputyNameColumn].Value?.ToString();
                    string DescriptionValue = worksheet.Cells[i + StartRow, DescriptionColumn].Value?.ToString();
                    string ResellerTypeIdValue = worksheet.Cells[i + StartRow, ResellerTypeIdColumn].Value?.ToString();
                    string ResellerStatusIdValue = worksheet.Cells[i + StartRow, ResellerStatusIdColumn].Value?.ToString();
                    string StaffIdValue = worksheet.Cells[i + StartRow, StaffIdColumn].Value?.ToString();
                    
                    Reseller Reseller = new Reseller();
                    Reseller.Code = CodeValue;
                    Reseller.Name = NameValue;
                    Reseller.Phone = PhoneValue;
                    Reseller.Email = EmailValue;
                    Reseller.Address = AddressValue;
                    Reseller.TaxCode = TaxCodeValue;
                    Reseller.CompanyName = CompanyNameValue;
                    Reseller.DeputyName = DeputyNameValue;
                    Reseller.Description = DescriptionValue;
                    ResellerStatus ResellerStatus = ResellerStatuses.Where(x => x.Id.ToString() == ResellerStatusIdValue).FirstOrDefault();
                    Reseller.ResellerStatusId = ResellerStatus == null ? 0 : ResellerStatus.Id;
                    Reseller.ResellerStatus = ResellerStatus;
                    ResellerType ResellerType = ResellerTypes.Where(x => x.Id.ToString() == ResellerTypeIdValue).FirstOrDefault();
                    Reseller.ResellerTypeId = ResellerType == null ? 0 : ResellerType.Id;
                    Reseller.ResellerType = ResellerType;
                    //AppUser AppUser = AppUsers.Where(x => x.Id.ToString() == AppUserIdValue).FirstOrDefault();
                    //Reseller.AppUserId = AppUser == null ? 0 : AppUser.Id;
                    //Reseller.AppUser = AppUser;
                    
                    Resellers.Add(Reseller);
                }
            }
            Resellers = await ResellerService.Import(Resellers);
            if (Resellers.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Resellers.Count; i++)
                {
                    Reseller Reseller = Resellers[i];
                    if (!Reseller.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Reseller.Errors.ContainsKey(nameof(Reseller.Id)))
                            Error += Reseller.Errors[nameof(Reseller.Id)];
                        if (Reseller.Errors.ContainsKey(nameof(Reseller.Code)))
                            Error += Reseller.Errors[nameof(Reseller.Code)];
                        if (Reseller.Errors.ContainsKey(nameof(Reseller.Name)))
                            Error += Reseller.Errors[nameof(Reseller.Name)];
                        if (Reseller.Errors.ContainsKey(nameof(Reseller.Phone)))
                            Error += Reseller.Errors[nameof(Reseller.Phone)];
                        if (Reseller.Errors.ContainsKey(nameof(Reseller.Email)))
                            Error += Reseller.Errors[nameof(Reseller.Email)];
                        if (Reseller.Errors.ContainsKey(nameof(Reseller.Address)))
                            Error += Reseller.Errors[nameof(Reseller.Address)];
                        if (Reseller.Errors.ContainsKey(nameof(Reseller.TaxCode)))
                            Error += Reseller.Errors[nameof(Reseller.TaxCode)];
                        if (Reseller.Errors.ContainsKey(nameof(Reseller.CompanyName)))
                            Error += Reseller.Errors[nameof(Reseller.CompanyName)];
                        if (Reseller.Errors.ContainsKey(nameof(Reseller.DeputyName)))
                            Error += Reseller.Errors[nameof(Reseller.DeputyName)];
                        if (Reseller.Errors.ContainsKey(nameof(Reseller.Description)))
                            Error += Reseller.Errors[nameof(Reseller.Description)];
                        if (Reseller.Errors.ContainsKey(nameof(Reseller.ResellerTypeId)))
                            Error += Reseller.Errors[nameof(Reseller.ResellerTypeId)];
                        if (Reseller.Errors.ContainsKey(nameof(Reseller.ResellerStatusId)))
                            Error += Reseller.Errors[nameof(Reseller.ResellerStatusId)];
                        if (Reseller.Errors.ContainsKey(nameof(Reseller.StaffId)))
                            Error += Reseller.Errors[nameof(Reseller.StaffId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ResellerRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] Reseller_ResellerFilterDTO Reseller_ResellerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var ResellerFilter = ConvertFilterDTOToFilterEntity(Reseller_ResellerFilterDTO);
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
                        "Id",
                        "Code",
                        "Name",
                        "Phone",
                        "Email",
                        "Address",
                        "TaxCode",
                        "CompanyName",
                        "DeputyName",
                        "Description",
                        "ResellerTypeId",
                        "ResellerStatusId",
                        "StaffId",
                    }
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < Resellers.Count; i++)
                {
                    var Reseller = Resellers[i];
                    data.Add(new Object[]
                    {
                        Reseller.Id,
                        Reseller.Code,
                        Reseller.Name,
                        Reseller.Phone,
                        Reseller.Email,
                        Reseller.Address,
                        Reseller.TaxCode,
                        Reseller.CompanyName,
                        Reseller.DeputyName,
                        Reseller.Description,
                        Reseller.ResellerTypeId,
                        Reseller.ResellerStatusId,
                        Reseller.StaffId,
                    });
                    excel.GenerateWorksheet("Reseller", ResellerHeaders, data);
                }
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
                Password = Reseller_ResellerDTO.Staff.Password,
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

    }
}

