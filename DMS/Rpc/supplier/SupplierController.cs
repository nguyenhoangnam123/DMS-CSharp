using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MDistrict;
using DMS.Services.MProvince;
using DMS.Services.MStatus;
using DMS.Services.MSupplier;
using DMS.Services.MWard;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.supplier
{
    public class SupplierRoute : Root
    {
        public const string Master = Module + "/supplier/supplier-master";
        public const string Detail = Module + "/supplier/supplier-detail";
        private const string Default = Rpc + Module + "/supplier";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListPersonInCharge = Default + "/single-list-person-in-charge";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListWard = Default + "/single-list-ward";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(SupplierFilter.Id), FieldType.ID },
            { nameof(SupplierFilter.Code), FieldType.STRING },
            { nameof(SupplierFilter.Name), FieldType.STRING },
            { nameof(SupplierFilter.TaxCode), FieldType.STRING },
            { nameof(SupplierFilter.Phone), FieldType.STRING },
            { nameof(SupplierFilter.Email), FieldType.STRING },
            { nameof(SupplierFilter.Address), FieldType.STRING },
            { nameof(SupplierFilter.ProvinceId), FieldType.ID },
            { nameof(SupplierFilter.DistrictId), FieldType.ID },
            { nameof(SupplierFilter.WardId), FieldType.ID },
            { nameof(SupplierFilter.OwnerName), FieldType.STRING },
            { nameof(SupplierFilter.PersonInChargeId), FieldType.ID },
            { nameof(SupplierFilter.StatusId), FieldType.ID },
            { nameof(SupplierFilter.Description), FieldType.STRING },
            { nameof(SupplierFilter.UpdatedTime), FieldType.DATE },
        };
    }

    public class SupplierController : RpcController
    {
        private IAppUserService AppUserService;
        private IDistrictService DistrictService;
        private IProvinceService ProvinceService;
        private IStatusService StatusService;
        private ISupplierService SupplierService;
        private IWardService WardService;
        private ICurrentContext CurrentContext;
        public SupplierController(
            IAppUserService AppUserService,
            IDistrictService DistrictService,
            IProvinceService ProvinceService,
            IStatusService StatusService,
            ISupplierService SupplierService,
            IWardService WardService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.DistrictService = DistrictService;
            this.ProvinceService = ProvinceService;
            this.StatusService = StatusService;
            this.SupplierService = SupplierService;
            this.WardService = WardService;
            this.CurrentContext = CurrentContext;
        }

        [Route(SupplierRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Supplier_SupplierFilterDTO Supplier_SupplierFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SupplierFilter SupplierFilter = ConvertFilterDTOToFilterEntity(Supplier_SupplierFilterDTO);
            SupplierFilter = SupplierService.ToFilter(SupplierFilter);
            int count = await SupplierService.Count(SupplierFilter);
            return count;
        }

        [Route(SupplierRoute.List), HttpPost]
        public async Task<ActionResult<List<Supplier_SupplierDTO>>> List([FromBody] Supplier_SupplierFilterDTO Supplier_SupplierFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SupplierFilter SupplierFilter = ConvertFilterDTOToFilterEntity(Supplier_SupplierFilterDTO);
            SupplierFilter = SupplierService.ToFilter(SupplierFilter);
            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            List<Supplier_SupplierDTO> Supplier_SupplierDTOs = Suppliers
                .Select(c => new Supplier_SupplierDTO(c)).ToList();
            return Supplier_SupplierDTOs;
        }

        [Route(SupplierRoute.Get), HttpPost]
        public async Task<ActionResult<Supplier_SupplierDTO>> Get([FromBody]Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Supplier_SupplierDTO.Id))
                return Forbid();

            Supplier Supplier = await SupplierService.Get(Supplier_SupplierDTO.Id);
            return new Supplier_SupplierDTO(Supplier);
        }

        [Route(SupplierRoute.Create), HttpPost]
        public async Task<ActionResult<Supplier_SupplierDTO>> Create([FromBody] Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Supplier_SupplierDTO.Id))
                return Forbid();

            Supplier Supplier = ConvertDTOToEntity(Supplier_SupplierDTO);
            Supplier = await SupplierService.Create(Supplier);
            Supplier_SupplierDTO = new Supplier_SupplierDTO(Supplier);
            if (Supplier.IsValidated)
                return Supplier_SupplierDTO;
            else
                return BadRequest(Supplier_SupplierDTO);
        }

        [Route(SupplierRoute.Update), HttpPost]
        public async Task<ActionResult<Supplier_SupplierDTO>> Update([FromBody] Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Supplier_SupplierDTO.Id))
                return Forbid();

            Supplier Supplier = ConvertDTOToEntity(Supplier_SupplierDTO);
            Supplier = await SupplierService.Update(Supplier);
            Supplier_SupplierDTO = new Supplier_SupplierDTO(Supplier);
            if (Supplier.IsValidated)
                return Supplier_SupplierDTO;
            else
                return BadRequest(Supplier_SupplierDTO);
        }

        [Route(SupplierRoute.Delete), HttpPost]
        public async Task<ActionResult<Supplier_SupplierDTO>> Delete([FromBody] Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Supplier_SupplierDTO.Id))
                return Forbid();

            Supplier Supplier = ConvertDTOToEntity(Supplier_SupplierDTO);
            Supplier = await SupplierService.Delete(Supplier);
            Supplier_SupplierDTO = new Supplier_SupplierDTO(Supplier);
            if (Supplier.IsValidated)
                return Supplier_SupplierDTO;
            else
                return BadRequest(Supplier_SupplierDTO);
        }

        [Route(SupplierRoute.Import), HttpPost]
        public async Task<ActionResult<List<Supplier_SupplierDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            List<AppUser> AppUsers = await AppUserService.List(new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.Username
            });

            List<Province> Provinces = await ProvinceService.List(new ProvinceFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProvinceSelect.Id | ProvinceSelect.Code
            });

            List<District> Districts = await DistrictService.List(new DistrictFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = DistrictSelect.Id | DistrictSelect.Code
            });

            List<Ward> Wards = await WardService.List(new WardFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = WardSelect.Id | WardSelect.Code
            });

            List<Status> Statuses = await StatusService.List(new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.Id | StatusSelect.Code
            });
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<Supplier> Suppliers = new List<Supplier>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return null;
                int StartColumn = 1;
                int StartRow = 1;

                int CodeColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int TaxCodeColumn = 2 + StartColumn;
                int PhoneColumn = 3 + StartColumn;
                int EmailColumn = 4 + StartColumn;
                int AddressColumn = 5 + StartColumn;
                int ProvinceCodeColumn = 6 + StartColumn;
                int DistrictCodeColumn = 7 + StartColumn;
                int WardCodeColumn = 8 + StartColumn;
                int DescriptionColumn = 9 + StartColumn;
                int OwnerNameColumn = 10 + StartColumn;
                int PersonInChargeColumn = 11 + StartColumn;
                int StatusCodeColumn = 12 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    // Lấy thông tin từng dòng
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string TaxCodeValue = worksheet.Cells[i + StartRow, TaxCodeColumn].Value?.ToString();
                    string PhoneValue = worksheet.Cells[i + StartRow, PhoneColumn].Value?.ToString();
                    string EmailValue = worksheet.Cells[i + StartRow, EmailColumn].Value?.ToString();
                    string AddressValue = worksheet.Cells[i + StartRow, AddressColumn].Value?.ToString();
                    string ProvinceCodeValue = worksheet.Cells[i + StartRow, ProvinceCodeColumn].Value?.ToString();
                    string DistrictCodeValue = worksheet.Cells[i + StartRow, DistrictCodeColumn].Value?.ToString();
                    string WardCodeValue = worksheet.Cells[i + StartRow, WardCodeColumn].Value?.ToString();
                    string DescriptionValue = worksheet.Cells[i + StartRow, DescriptionColumn].Value?.ToString();
                    string OwnerNameValue = worksheet.Cells[i + StartRow, OwnerNameColumn].Value?.ToString();
                    string PersonInChargeValue = worksheet.Cells[i + StartRow, PersonInChargeColumn].Value?.ToString();
                    string StatusCodeValue = worksheet.Cells[i + StartRow, StatusCodeColumn].Value?.ToString();
                    if (string.IsNullOrEmpty(CodeValue))
                        continue;

                    Supplier Supplier = new Supplier();
                    Supplier.Code = CodeValue;
                    Supplier.Name = NameValue;
                    Supplier.TaxCode = TaxCodeValue;
                    Supplier.Phone = PhoneValue;
                    Supplier.Email = EmailValue;
                    Supplier.Address = AddressValue;
                    Supplier.Description = DescriptionValue;
                    Supplier.OwnerName = OwnerNameValue;

                    Supplier.PersonInCharge = AppUsers.Where(x => x.Username == PersonInChargeValue).FirstOrDefault();
                    Supplier.Province = Provinces.Where(x => x.Code == ProvinceCodeValue).FirstOrDefault();
                    Supplier.District = Districts.Where(x => x.Code == DistrictCodeValue).FirstOrDefault();
                    Supplier.Ward = Wards.Where(x => x.Code == WardCodeValue).FirstOrDefault();
                    Supplier.Status = Statuses.Where(x => x.Code == StatusCodeValue).FirstOrDefault();

                    Suppliers.Add(Supplier);
                }
            }

            Suppliers = await SupplierService.BulkMerge(Suppliers);
            List<Supplier_SupplierDTO> Supplier_SupplierDTOs = Suppliers
                .Select(c => new Supplier_SupplierDTO(c)).ToList();
            return Supplier_SupplierDTOs;
        }

        [Route(SupplierRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Supplier_SupplierFilterDTO Supplier_SupplierFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SupplierFilter SupplierFilter = ConvertFilterDTOToFilterEntity(Supplier_SupplierFilterDTO);
            SupplierFilter.Skip = 0;
            SupplierFilter.Take = int.MaxValue;
            SupplierFilter = SupplierService.ToFilter(SupplierFilter);

            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                var SupplierHeaders = new List<string[]>()
                {
                    new string[]
                    {
                        "Mã NCC",
                        "Tên NCC",
                        "Mã Số Thuế",
                        "Mô Tả",
                        "Địa Chỉ NCC",
                        "Tỉnh/Thành phố",
                        "Quận/Huyện",
                        "Phường/Xã",
                        "Tên Người Liên Hệ",
                        "Điện Thoại",
                        "Email",
                        "Nhân Viên Phụ Trách",
                        "Trạng Thái",
                    }
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < Suppliers.Count; i++)
                {
                    var Supplier = Suppliers[i];
                    data.Add(new Object[]
                    {
                        Supplier.Code,
                        Supplier.Name,
                        Supplier.TaxCode,
                        Supplier.Description,
                        Supplier.Address,
                        Supplier.Province.Name,
                        Supplier.District.Name,
                        Supplier.Ward.Name,
                        Supplier.OwnerName,
                        Supplier.Phone,
                        Supplier.Email,
                        Supplier.PersonInCharge.DisplayName,
                        Supplier.Status.Code
                    });
                }
                excel.GenerateWorksheet("Supplier", SupplierHeaders, data);
                excel.Save();
            }

            return File(memoryStream.ToArray(), "application/octet-stream", "Supplier.xlsx");
        }

        [Route(SupplierRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Id = new IdFilter { In = Ids };
            SupplierFilter.Selects = SupplierSelect.Id;
            SupplierFilter.Skip = 0;
            SupplierFilter.Take = int.MaxValue;

            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            Suppliers = await SupplierService.BulkDelete(Suppliers);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter = SupplierService.ToFilter(SupplierFilter);
            if (Id == 0)
            {

            }
            else
            {
                SupplierFilter.Id = new IdFilter { Equal = Id };
                int count = await SupplierService.Count(SupplierFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Supplier ConvertDTOToEntity(Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            Supplier Supplier = new Supplier();
            Supplier.Id = Supplier_SupplierDTO.Id;
            Supplier.Code = Supplier_SupplierDTO.Code;
            Supplier.Name = Supplier_SupplierDTO.Name;
            Supplier.TaxCode = Supplier_SupplierDTO.TaxCode;
            Supplier.Phone = Supplier_SupplierDTO.Phone;
            Supplier.Email = Supplier_SupplierDTO.Email;
            Supplier.Address = Supplier_SupplierDTO.Address;
            Supplier.ProvinceId = Supplier_SupplierDTO.ProvinceId;
            Supplier.DistrictId = Supplier_SupplierDTO.DistrictId;
            Supplier.WardId = Supplier_SupplierDTO.WardId;
            Supplier.OwnerName = Supplier_SupplierDTO.OwnerName;
            Supplier.PersonInChargeId = Supplier_SupplierDTO.PersonInChargeId;
            Supplier.Description = Supplier_SupplierDTO.Description;
            Supplier.StatusId = Supplier_SupplierDTO.StatusId;
            Supplier.Province = Supplier_SupplierDTO.Province == null ? null : new Province
            {
                Id = Supplier_SupplierDTO.Province.Id,
                Code = Supplier_SupplierDTO.Province.Code,
                Name = Supplier_SupplierDTO.Province.Name,
                Priority = Supplier_SupplierDTO.Province.Priority,
                StatusId = Supplier_SupplierDTO.Province.StatusId
            };
            Supplier.District = Supplier_SupplierDTO.District == null ? null : new District
            {
                Id = Supplier_SupplierDTO.District.Id,
                Code = Supplier_SupplierDTO.District.Code,
                Name = Supplier_SupplierDTO.District.Name,
                Priority = Supplier_SupplierDTO.District.Priority,
                ProvinceId = Supplier_SupplierDTO.District.ProvinceId,
                StatusId = Supplier_SupplierDTO.District.StatusId
            };
            Supplier.Ward = Supplier_SupplierDTO.Ward == null ? null : new Ward
            {
                Id = Supplier_SupplierDTO.Ward.Id,
                Code = Supplier_SupplierDTO.Ward.Code,
                Name = Supplier_SupplierDTO.Ward.Name,
                Priority = Supplier_SupplierDTO.Ward.Priority,
                DistrictId = Supplier_SupplierDTO.Ward.DistrictId,
                StatusId = Supplier_SupplierDTO.Ward.StatusId
            };
            Supplier.PersonInCharge = Supplier_SupplierDTO.PersonInCharge == null ? null : new AppUser
            {
                Id = Supplier_SupplierDTO.PersonInCharge.Id,
                DisplayName = Supplier_SupplierDTO.PersonInCharge.DisplayName,
                Email = Supplier_SupplierDTO.PersonInCharge.Email,
                Phone = Supplier_SupplierDTO.PersonInCharge.Phone,
                Address = Supplier_SupplierDTO.PersonInCharge.Phone,
                StatusId = Supplier_SupplierDTO.PersonInCharge.StatusId
            };
            Supplier.Status = Supplier_SupplierDTO.Status == null ? null : new Status
            {
                Id = Supplier_SupplierDTO.Status.Id,
                Code = Supplier_SupplierDTO.Status.Code,
                Name = Supplier_SupplierDTO.Status.Name,
            };
            Supplier.BaseLanguage = CurrentContext.Language;
            return Supplier;
        }

        private SupplierFilter ConvertFilterDTOToFilterEntity(Supplier_SupplierFilterDTO Supplier_SupplierFilterDTO)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Selects = SupplierSelect.ALL;
            SupplierFilter.Skip = Supplier_SupplierFilterDTO.Skip;
            SupplierFilter.Take = Supplier_SupplierFilterDTO.Take;
            SupplierFilter.OrderBy = Supplier_SupplierFilterDTO.OrderBy;
            SupplierFilter.OrderType = Supplier_SupplierFilterDTO.OrderType;

            SupplierFilter.Id = Supplier_SupplierFilterDTO.Id;
            SupplierFilter.Code = Supplier_SupplierFilterDTO.Code;
            SupplierFilter.Name = Supplier_SupplierFilterDTO.Name;
            SupplierFilter.TaxCode = Supplier_SupplierFilterDTO.TaxCode;
            SupplierFilter.Phone = Supplier_SupplierFilterDTO.Phone;
            SupplierFilter.Email = Supplier_SupplierFilterDTO.Email;
            SupplierFilter.Address = Supplier_SupplierFilterDTO.Address;
            SupplierFilter.ProvinceId = Supplier_SupplierFilterDTO.ProvinceId;
            SupplierFilter.DistrictId = Supplier_SupplierFilterDTO.DistrictId;
            SupplierFilter.WardId = Supplier_SupplierFilterDTO.WardId;
            SupplierFilter.OwnerName = Supplier_SupplierFilterDTO.OwnerName;
            SupplierFilter.PersonInChargeId = Supplier_SupplierFilterDTO.PersonInChargeId;
            SupplierFilter.Description = Supplier_SupplierFilterDTO.Description;
            SupplierFilter.StatusId = Supplier_SupplierFilterDTO.StatusId;
            SupplierFilter.UpdatedTime = Supplier_SupplierFilterDTO.UpdatedTime;
            return SupplierFilter;
        }

        [Route(SupplierRoute.SingleListPersonInCharge), HttpPost]
        public async Task<List<Supplier_AppUserDTO>> SingleListAppUser([FromBody] Supplier_AppUserFilterDTO Supplier_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Supplier_AppUserFilterDTO.Id;
            AppUserFilter.Username = Supplier_AppUserFilterDTO.Username;
            AppUserFilter.Password = Supplier_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = Supplier_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = Supplier_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Supplier_AppUserFilterDTO.Phone;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Supplier_AppUserDTO> Supplier_AppUserDTOs = AppUsers
                .Select(x => new Supplier_AppUserDTO(x)).ToList();
            return Supplier_AppUserDTOs;
        }
        [Route(SupplierRoute.SingleListDistrict), HttpPost]
        public async Task<List<Supplier_DistrictDTO>> SingleListDistrict([FromBody] Supplier_DistrictFilterDTO Supplier_DistrictFilterDTO)
        {
            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Id;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = Supplier_DistrictFilterDTO.Id;
            DistrictFilter.Name = Supplier_DistrictFilterDTO.Name;
            DistrictFilter.Priority = Supplier_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = Supplier_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<Supplier_DistrictDTO> Supplier_DistrictDTOs = Districts
                .Select(x => new Supplier_DistrictDTO(x)).ToList();
            return Supplier_DistrictDTOs;
        }
        [Route(SupplierRoute.SingleListProvince), HttpPost]
        public async Task<List<Supplier_ProvinceDTO>> SingleListProvince([FromBody] Supplier_ProvinceFilterDTO Supplier_ProvinceFilterDTO)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Id;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = Supplier_ProvinceFilterDTO.Id;
            ProvinceFilter.Name = Supplier_ProvinceFilterDTO.Name;
            ProvinceFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<Supplier_ProvinceDTO> Supplier_ProvinceDTOs = Provinces
                .Select(x => new Supplier_ProvinceDTO(x)).ToList();
            return Supplier_ProvinceDTOs;
        }
        [Route(SupplierRoute.SingleListStatus), HttpPost]
        public async Task<List<Supplier_StatusDTO>> SingleListStatus([FromBody] Supplier_StatusFilterDTO Supplier_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Supplier_StatusDTO> Supplier_StatusDTOs = Statuses
                .Select(x => new Supplier_StatusDTO(x)).ToList();
            return Supplier_StatusDTOs;
        }
        [Route(SupplierRoute.SingleListWard), HttpPost]
        public async Task<List<Supplier_WardDTO>> SingleListWard([FromBody] Supplier_WardFilterDTO Supplier_WardFilterDTO)
        {
            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Id;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = Supplier_WardFilterDTO.Id;
            WardFilter.Name = Supplier_WardFilterDTO.Name;
            WardFilter.DistrictId = Supplier_WardFilterDTO.DistrictId;
            WardFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            List<Ward> Wards = await WardService.List(WardFilter);
            List<Supplier_WardDTO> Supplier_WardDTOs = Wards
                .Select(x => new Supplier_WardDTO(x)).ToList();
            return Supplier_WardDTOs;
        }
    }
}

