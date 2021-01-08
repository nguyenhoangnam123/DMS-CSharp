using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MDistrict;
using DMS.Services.MOrganization;
using DMS.Services.MProvince;
using DMS.Services.MStatus;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreScouting;
using DMS.Services.MStoreStatus;
using DMS.Services.MStoreType;
using DMS.Services.MWard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Rpc.store
{
    public partial class StoreController : RpcController
    {
        private IAppUserService AppUserService;
        private IDistrictService DistrictService;
        private IOrganizationService OrganizationService;
        private IProvinceService ProvinceService;
        private IStoreStatusService StoreStatusService;
        private IStatusService StatusService;
        private IStoreScoutingService StoreScoutingService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private IWardService WardService;
        private IStoreService StoreService;
        private ICurrentContext CurrentContext;
        public StoreController(
            IAppUserService AppUserService,
            IDistrictService DistrictService,
            IOrganizationService OrganizationService,
            IProvinceService ProvinceService,
            IStoreStatusService StoreStatusService,
            IStatusService StatusService,
            IStoreScoutingService StoreScoutingService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            IWardService WardService,
            IStoreService StoreService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.DistrictService = DistrictService;
            this.OrganizationService = OrganizationService;
            this.ProvinceService = ProvinceService;
            this.StoreStatusService = StoreStatusService;
            this.StatusService = StatusService;
            this.StoreScoutingService = StoreScoutingService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.WardService = WardService;
            this.StoreService = StoreService;
            this.CurrentContext = CurrentContext;
        }

        [Route(StoreRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Store_StoreFilterDTO Store_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = ConvertFilterDTOToFilterEntity(Store_StoreFilterDTO);
            StoreFilter = StoreService.ToFilter(StoreFilter);
            int count = await StoreService.Count(StoreFilter);
            return count;
        }

        [Route(StoreRoute.List), HttpPost]
        public async Task<ActionResult<List<Store_StoreDTO>>> List([FromBody] Store_StoreFilterDTO Store_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = ConvertFilterDTOToFilterEntity(Store_StoreFilterDTO);
            StoreFilter = StoreService.ToFilter(StoreFilter);
            List<Store> Stores = await StoreService.List(StoreFilter);
            List<Store_StoreDTO> Store_StoreDTOs = Stores
                .Select(c => new Store_StoreDTO(c)).ToList();
            return Store_StoreDTOs;
        }

        [Route(StoreRoute.Get), HttpPost]
        public async Task<ActionResult<Store_StoreDTO>> Get([FromBody] Store_StoreDTO Store_StoreDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Store_StoreDTO.Id))
                return Forbid();

            Store Store = await StoreService.Get(Store_StoreDTO.Id);
            return new Store_StoreDTO(Store);
        }

        [Route(StoreRoute.GetDraft), HttpPost]
        public async Task<ActionResult<Store_StoreDTO>> GetDraft([FromBody] Store_StoreScoutingDTO Store_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScouting StoreScouting = await StoreScoutingService.Get(Store_StoreScoutingDTO.Id);
            Store_StoreDTO Store_StoreDTO = new Store_StoreDTO
            {
                Address = StoreScouting.Address,
                AppUserId = StoreScouting.CreatorId,
                OrganizationId = StoreScouting.OrganizationId,
                DistrictId = StoreScouting.DistrictId,
                Latitude = StoreScouting.Latitude,
                Longitude = StoreScouting.Longitude,
                Name = StoreScouting.Name,
                OwnerPhone = StoreScouting.OwnerPhone,
                ProvinceId = StoreScouting.ProvinceId,
                StatusId = StatusEnum.INACTIVE.Id,
                StoreStatusId = StoreStatusEnum.OFFICIAL.Id,
                WardId = StoreScouting.WardId,
                AppUser = StoreScouting.Creator == null ? null : new Store_AppUserDTO(StoreScouting.Creator),
                Organization = StoreScouting.Organization == null ? null : new Store_OrganizationDTO(StoreScouting.Organization),
                District = StoreScouting.District == null ? null : new Store_DistrictDTO(StoreScouting.District),
                Province = StoreScouting.Province == null ? null : new Store_ProvinceDTO(StoreScouting.Province),
                Ward = StoreScouting.Ward == null ? null : new Store_WardDTO(StoreScouting.Ward),
                Status = new Store_StatusDTO
                {
                    Id = StatusEnum.INACTIVE.Id,
                    Code = StatusEnum.INACTIVE.Code,
                    Name = StatusEnum.INACTIVE.Name,
                },
                StoreScoutingId = StoreScouting.Id,
                StoreScouting = new Store_StoreScoutingDTO(StoreScouting)
            };
            return Store_StoreDTO;
        }

        [Route(StoreRoute.Create), HttpPost]
        public async Task<ActionResult<Store_StoreDTO>> Create([FromBody] Store_StoreDTO Store_StoreDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Store_StoreDTO.Id))
                return Forbid();

            Store Store = ConvertDTOToEntity(Store_StoreDTO);
            Store = await StoreService.Create(Store);
            Store_StoreDTO = new Store_StoreDTO(Store);
            if (Store.IsValidated)
                return Store_StoreDTO;
            else
                return BadRequest(Store_StoreDTO);
        }

        [Route(StoreRoute.Update), HttpPost]
        public async Task<ActionResult<Store_StoreDTO>> Update([FromBody] Store_StoreDTO Store_StoreDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Store_StoreDTO.Id))
                return Forbid();

            Store Store = ConvertDTOToEntity(Store_StoreDTO);
            Store = await StoreService.Update(Store);
            Store_StoreDTO = new Store_StoreDTO(Store);
            if (Store.IsValidated)
                return Store_StoreDTO;
            else
                return BadRequest(Store_StoreDTO);
        }

        [Route(StoreRoute.Approve), HttpPost]
        public async Task<ActionResult<Store_StoreDTO>> Approve([FromBody] Store_StoreDTO Store_StoreDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Store_StoreDTO.Id))
                return Forbid();

            Store Store = ConvertDTOToEntity(Store_StoreDTO);
            Store.StoreStatusId = StoreStatusEnum.OFFICIAL.Id;
            Store = await StoreService.Update(Store);
            Store_StoreDTO = new Store_StoreDTO(Store);
            if (Store.IsValidated)
                return Store_StoreDTO;
            else
                return BadRequest(Store_StoreDTO);
        }

        [Route(StoreRoute.Delete), HttpPost]
        public async Task<ActionResult<Store_StoreDTO>> Delete([FromBody] Store_StoreDTO Store_StoreDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Store_StoreDTO.Id))
                return Forbid();

            Store Store = ConvertDTOToEntity(Store_StoreDTO);
            Store = await StoreService.Delete(Store);
            Store_StoreDTO = new Store_StoreDTO(Store);
            if (Store.IsValidated)
                return Store_StoreDTO;
            else
                return BadRequest(Store_StoreDTO);
        }

        [Route(StoreRoute.Import), HttpPost]
        public async Task<ActionResult<List<Store_StoreDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            FileInfo FileInfo = new FileInfo(file.FileName);
            if (!FileInfo.Extension.Equals(".xlsx"))
                return BadRequest("Định dạng file không hợp lệ");

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };
            #region MDM
            List<Store> ParentStores = await StoreService.List(new StoreFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreSelect.Code | StoreSelect.Name | StoreSelect.Id
            });
            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Id | OrganizationSelect.Code
            });

            List<StoreType> StoreTypes = await StoreTypeService.List(new StoreTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreTypeSelect.Id | StoreTypeSelect.Code
            });

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(new StoreGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreGroupingSelect.Id | StoreGroupingSelect.Code
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
                Selects = StatusSelect.Id | StatusSelect.Code | StatusSelect.Name
            });

            List<Store> All = await StoreService.List(new StoreFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreSelect.Id | StoreSelect.Code | StoreSelect.Name | StoreSelect.CodeDraft
            });

            Dictionary<string, Store> DictionaryAll = All.ToDictionary(x => x.Code, y => y);
            #endregion
            List<Store_ImportDTO> Store_ImportDTOs = new List<Store_ImportDTO>();

            StringBuilder errorContent = new StringBuilder();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Store"];
                if (worksheet == null)
                    return BadRequest("File không đúng biểu mẫu import");

                #region khai báo các cột
                int StartColumn = 1;
                int StartRow = 1;
                int SttColumnn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int CodeDraftColumn = 2 + StartColumn;
                int NameColumn = 3 + StartColumn;
                int OrganizationCodeColumn = 4 + StartColumn;
                int ParentStoreCodeColumn = 5 + StartColumn;
                int StoreTypeCodeColumn = 6 + StartColumn;
                int StoreGroupingCodeColumn = 7 + StartColumn;
                int LegalEntityColumn = 8 + StartColumn;
                int TaxCodeColumn = 9 + StartColumn;
                int ProvinceCodeColumn = 10 + StartColumn;
                int DistrictCodeColumn = 11 + StartColumn;
                int WardCodeColumn = 12 + StartColumn;
                int AddressColumn = 13 + StartColumn;
                int LongitudeColumn = 14 + StartColumn;
                int LatitudeColumn = 15 + StartColumn;
                int DeliveryAddressColumn = 16 + StartColumn;
                int DeliveryLongitudeColumn = 17 + StartColumn;
                int DeliveryLatitudeColumn = 18 + StartColumn;
                int TelephoneColumn = 19 + StartColumn;
                int OwnerNameColumn = 20 + StartColumn;
                int OwnerPhoneColumn = 21 + StartColumn;
                int OwnerEmailColumn = 22 + StartColumn;
                int StatusColumn = 23 + StartColumn;
                #endregion

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    #region Lấy thông tin từng dòng
                    string stt = worksheet.Cells[i + StartRow, SttColumnn].Value?.ToString();
                    if (stt != null && stt.ToLower() == "END".ToLower())
                        break;
                    bool convert = long.TryParse(stt, out long Stt);
                    if (convert == false)
                        continue;
                    Store_ImportDTO Store_ImportDTO = new Store_ImportDTO();
                    Store_ImportDTOs.Add(Store_ImportDTO);
                    Store_ImportDTO.Stt = Stt;
                    Store_ImportDTO.CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    Store_ImportDTO.CodeDraftValue = worksheet.Cells[i + StartRow, CodeDraftColumn].Value?.ToString();

                    Store_ImportDTO.NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    Store_ImportDTO.OrganizationCodeValue = worksheet.Cells[i + StartRow, OrganizationCodeColumn].Value?.ToString();
                    Store_ImportDTO.ParentStoreCodeValue = worksheet.Cells[i + StartRow, ParentStoreCodeColumn].Value?.ToString();
                    Store_ImportDTO.StoreTypeCodeValue = worksheet.Cells[i + StartRow, StoreTypeCodeColumn].Value?.ToString();
                    Store_ImportDTO.StoreGroupingCodeValue = worksheet.Cells[i + StartRow, StoreGroupingCodeColumn].Value?.ToString();
                    Store_ImportDTO.LegalEntityValue = worksheet.Cells[i + StartRow, LegalEntityColumn].Value?.ToString();
                    Store_ImportDTO.TaxCodeValue = worksheet.Cells[i + StartRow, TaxCodeColumn].Value?.ToString();

                    Store_ImportDTO.ProvinceCodeValue = worksheet.Cells[i + StartRow, ProvinceCodeColumn].Value?.ToString();
                    Store_ImportDTO.DistrictCodeValue = worksheet.Cells[i + StartRow, DistrictCodeColumn].Value?.ToString();
                    Store_ImportDTO.WardCodeValue = worksheet.Cells[i + StartRow, WardCodeColumn].Value?.ToString();

                    Store_ImportDTO.AddressValue = worksheet.Cells[i + StartRow, AddressColumn].Value?.ToString();
                    Store_ImportDTO.LongitudeValue = worksheet.Cells[i + StartRow, LongitudeColumn].Value?.ToString();
                    if (!string.IsNullOrWhiteSpace(Store_ImportDTO.LongitudeValue) && Store_ImportDTO.LongitudeValue.Contains(","))
                        Store_ImportDTO.LongitudeValue = Store_ImportDTO.LongitudeValue.Replace(",", ".");
                    Store_ImportDTO.LatitudeValue = worksheet.Cells[i + StartRow, LatitudeColumn].Value?.ToString();
                    if (!string.IsNullOrWhiteSpace(Store_ImportDTO.LatitudeValue) && Store_ImportDTO.LatitudeValue.Contains(","))
                        Store_ImportDTO.LatitudeValue = Store_ImportDTO.LatitudeValue.Replace(",", ".");

                    Store_ImportDTO.DeliveryAddressValue = worksheet.Cells[i + StartRow, DeliveryAddressColumn].Value?.ToString();
                    Store_ImportDTO.DeliveryLongitudeValue = worksheet.Cells[i + StartRow, DeliveryLongitudeColumn].Value?.ToString();
                    Store_ImportDTO.DeliveryLatitudeValue = worksheet.Cells[i + StartRow, DeliveryLatitudeColumn].Value?.ToString();

                    Store_ImportDTO.TelephoneValue = worksheet.Cells[i + StartRow, TelephoneColumn].Value?.ToString();
                    Store_ImportDTO.OwnerNameValue = worksheet.Cells[i + StartRow, OwnerNameColumn].Value?.ToString();
                    Store_ImportDTO.OwnerPhoneValue = worksheet.Cells[i + StartRow, OwnerPhoneColumn].Value?.ToString();
                    Store_ImportDTO.OwnerEmailValue = worksheet.Cells[i + StartRow, OwnerEmailColumn].Value?.ToString();
                    Store_ImportDTO.StatusNameValue = worksheet.Cells[i + StartRow, StatusColumn].Value?.ToString();
                    #endregion
                }
            }
            Dictionary<long, StringBuilder> Errors = new Dictionary<long, StringBuilder>();
            HashSet<string> StoreCodes = new HashSet<string>(All.Select(x => x.Code).Distinct().ToList());
            foreach (Store_ImportDTO Store_ImportDTO in Store_ImportDTOs)
            {
                Errors.Add(Store_ImportDTO.Stt, new StringBuilder(""));
                Store_ImportDTO.IsNew = false;
            }
            Parallel.ForEach(Store_ImportDTOs, Store_ImportDTO =>
            {
                if (!string.IsNullOrWhiteSpace(Store_ImportDTO.CodeValue))
                {
                    if (!StoreCodes.Contains(Store_ImportDTO.CodeValue))
                    {
                        Errors[Store_ImportDTO.Stt].AppendLine($"Lỗi dòng thứ {Store_ImportDTO.Stt}: Mã đại lý không tồn tại");
                        return;
                    }
                }
                else
                {
                    Store_ImportDTO.IsNew = true;
                }
                Store_ImportDTO.OrganizationId = Organizations.Where(x => x.Code.Equals(Store_ImportDTO.OrganizationCodeValue)).Select(x => x.Id).FirstOrDefault();
                Store_ImportDTO.Longitude = decimal.TryParse(Store_ImportDTO.LongitudeValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal Longitude) ? Longitude : 106;
                Store_ImportDTO.Latitude = decimal.TryParse(Store_ImportDTO.LatitudeValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal Latitude) ? Latitude : 21;
                Store_ImportDTO.DeliveryLongitude = decimal.TryParse(Store_ImportDTO.DeliveryLongitudeValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal DeliveryLongitude) ? DeliveryLongitude : 106;
                Store_ImportDTO.DeliveryLatitude = decimal.TryParse(Store_ImportDTO.DeliveryLatitudeValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal DeliveryLatitude) ? DeliveryLatitude : 21;
                if (!string.IsNullOrWhiteSpace(Store_ImportDTO.StoreTypeCodeValue))
                    Store_ImportDTO.StoreTypeId = StoreTypes.Where(x => x.Code.Equals(Store_ImportDTO.StoreTypeCodeValue)).Select(x => x.Id).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(Store_ImportDTO.StoreGroupingCodeValue))
                    Store_ImportDTO.StoreGroupingId = StoreGroupings.Where(x => x.Code.Equals(Store_ImportDTO.StoreGroupingCodeValue)).Select(x => x.Id).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(Store_ImportDTO.ProvinceCodeValue))
                    Store_ImportDTO.ProvinceId = Provinces.Where(x => x.Code.Equals(Store_ImportDTO.ProvinceCodeValue)).Select(x => (long?)x.Id).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(Store_ImportDTO.DistrictCodeValue))
                    Store_ImportDTO.DistrictId = Districts.Where(x => x.Code.Equals(Store_ImportDTO.DistrictCodeValue)).Select(x => (long?)x.Id).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(Store_ImportDTO.WardCodeValue))
                    Store_ImportDTO.WardId = Wards.Where(x => x.Code.Equals(Store_ImportDTO.WardCodeValue)).Select(x => (long?)x.Id).FirstOrDefault();
                if (string.IsNullOrEmpty(Store_ImportDTO.StatusNameValue))
                {
                    Store_ImportDTO.StatusId = -1;
                }
                else
                {
                    string StatusNameValue = Store_ImportDTO.StatusNameValue;
                    Store_ImportDTO.StatusId = Statuses.Where(x => x.Name.ToLower().Equals(StatusNameValue == null ? string.Empty : StatusNameValue.Trim().ToLower())).Select(x => x.Id).FirstOrDefault();
                }
            });

            Parallel.ForEach(Store_ImportDTOs, Store_ImportDTO =>
            {
                HashSet<string> listCode = Store_ImportDTOs.Select(x => x.CodeValue).ToHashSet();
                if (!string.IsNullOrWhiteSpace(Store_ImportDTO.ParentStoreCodeValue))
                {
                    if (!listCode.Contains(Store_ImportDTO.ParentStoreCodeValue) && !StoreCodes.Contains(Store_ImportDTO.ParentStoreCodeValue))
                        Errors[Store_ImportDTO.Stt].AppendLine($"Lỗi dòng thứ {Store_ImportDTO.Stt}: Đại lý cấp cha không tồn tại");
                }
            });
            string error = string.Join("\n", Errors.Where(x => !string.IsNullOrWhiteSpace(x.Value.ToString())).Select(x => x.Value.ToString()).ToList());
            if (!string.IsNullOrWhiteSpace(error))
                return BadRequest(error);

            Dictionary<long, Store> DictionaryStores = Store_ImportDTOs.ToDictionary(x => x.Stt, y => new Store());
            Parallel.ForEach(Store_ImportDTOs, Store_ImportDTO =>
            {
                Store Store = DictionaryStores[Store_ImportDTO.Stt];
                if (Store_ImportDTO.IsNew == false)
                {
                    Store Old = DictionaryAll[Store_ImportDTO.CodeValue];
                    Store.Id = Old.Id;
                    Store.ParentStoreId = Old.ParentStoreId;
                }
                Store.Code = Store_ImportDTO.CodeValue;
                Store.CodeDraft = Store_ImportDTO.CodeDraftValue;
                Store.Name = Store_ImportDTO.NameValue;
                Store.OrganizationId = Store_ImportDTO.OrganizationId;
                Store.Organization = new Organization { Code = Store_ImportDTO.OrganizationCodeValue };
                Store.ParentStore = new Store { Code = Store_ImportDTO.ParentStoreCodeValue };
                Store.StoreTypeId = Store_ImportDTO.StoreTypeId;
                Store.StoreType = new StoreType { Code = Store_ImportDTO.StoreTypeCodeValue };
                Store.StoreGroupingId = Store_ImportDTO.StoreGroupingId;
                if (Store.StoreGroupingId.HasValue)
                {
                    Store.StoreGrouping = new StoreGrouping { Code = Store_ImportDTO.StoreGroupingCodeValue };
                }
                Store.LegalEntity = Store_ImportDTO.LegalEntityValue;
                Store.TaxCode = Store_ImportDTO.TaxCodeValue;
                Store.ProvinceId = Store_ImportDTO.ProvinceId;
                if (Store.ProvinceId.HasValue)
                {
                    Store.Province = new Province { Code = Store_ImportDTO.ProvinceCodeValue };
                }
                Store.DistrictId = Store_ImportDTO.DistrictId;
                if (Store.DistrictId.HasValue)
                {
                    Store.District = new District { Code = Store_ImportDTO.DistrictCodeValue };
                }
                Store.WardId = Store_ImportDTO.WardId;
                if (Store.WardId.HasValue)
                {
                    Store.Ward = new Ward { Code = Store_ImportDTO.WardCodeValue };
                }
                Store.Address = Store_ImportDTO.AddressValue;
                Store.Longitude = Store_ImportDTO.Longitude;
                Store.Latitude = Store_ImportDTO.Latitude;

                Store.DeliveryAddress = Store_ImportDTO.DeliveryAddressValue;
                Store.DeliveryLongitude = Store_ImportDTO.DeliveryLongitude;
                Store.DeliveryLatitude = Store_ImportDTO.DeliveryLatitude;

                Store.Telephone = Store_ImportDTO.TelephoneValue;
                Store.OwnerName = Store_ImportDTO.OwnerNameValue;
                Store.OwnerPhone = Store_ImportDTO.OwnerPhoneValue;
                Store.OwnerEmail = Store_ImportDTO.OwnerEmailValue;
                Store.StatusId = Store_ImportDTO.StatusId;
                Store.BaseLanguage = CurrentContext.Language;
            });
            List<Store> Stores = DictionaryStores.Select(x => x.Value).ToList();
            errorContent = new StringBuilder(error);
            Stores = await StoreService.Import(Stores);
            if (Stores == null)
                return Ok();
            List<Store_StoreDTO> Store_StoreDTOs = Stores
                .Select(c => new Store_StoreDTO(c)).ToList();
            for (int i = 0; i < Stores.Count; i++)
            {
                if (!Stores[i].IsValidated)
                {
                    errorContent.Append($"Lỗi dòng thứ {i + 2}:");
                    foreach (var Error in Stores[i].Errors)
                    {
                        errorContent.Append($" {Error.Value},");
                    }
                    errorContent.AppendLine("");
                }
            }
            if (Stores.Any(s => !s.IsValidated))
                return BadRequest(errorContent.ToString());
            return Ok(Store_StoreDTOs);
        }

        [Route(StoreRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Store_StoreFilterDTO Store_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Code | OrganizationSelect.Name | OrganizationSelect.Path
            });

            List<StoreType> StoreTypes = await StoreTypeService.List(new StoreTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreTypeSelect.Id | StoreTypeSelect.Code | StoreTypeSelect.Name
            });

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(new StoreGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreGroupingSelect.Id | StoreGroupingSelect.Code | StoreGroupingSelect.Name
            });

            List<Province> Provinces = await ProvinceService.List(new ProvinceFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProvinceSelect.Id | ProvinceSelect.Code | ProvinceSelect.Name
            });

            List<District> Districts = await DistrictService.List(new DistrictFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = DistrictSelect.Id | DistrictSelect.Code | DistrictSelect.Name | DistrictSelect.Province
            });

            List<Ward> Wards = await WardService.List(new WardFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = WardSelect.Id | WardSelect.Code | WardSelect.Name | WardSelect.District
            });

            StoreFilter StoreFilter = ConvertFilterDTOToFilterEntity(Store_StoreFilterDTO);
            StoreFilter = StoreService.ToFilter(StoreFilter);
            StoreFilter.Skip = 0;
            StoreFilter.Take = int.MaxValue;
            StoreFilter = StoreService.ToFilter(StoreFilter);

            List<Store> Stores = await StoreService.List(StoreFilter);
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Store
                var StoreHeaders = new List<string[]>()
                {
                    new string[]
                    {
                        "STT",
                        "Mã đại lý (tự sinh)",
                        "Mã đại lý (tự nhập)",
                        "Tên đại lý",
                        "Đơn vị quản lý",
                        "Đại lý cấp cha",
                        "Cấp đại lý" ,
                        "Nhóm đại lý",
                        "Tư cách pháp nhân",
                        "Mã số thuế",
                        "Tỉnh/Thành phố",
                        "Quận/Huyện",
                        "Phường/Xã",
                        "Địa Chỉ đại lý",
                        "Kinh Độ",
                        "Vĩ Độ",
                        "Địa Chỉ Giao Hàng",
                        "Kinh Độ Giao Hàng",
                        "Vĩ Độ Giao Hàng",
                        "Điện Thoại",
                        "Tên Chủ đại lý",
                        "Điện Thoại Liên Hệ",
                        "Email",
                        "Trạng thái"
                    }
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < Stores.Count; i++)
                {
                    var Store = Stores[i];
                    data.Add(new Object[]
                    {
                        i+1,
                        Store.Code,
                        Store.CodeDraft,
                        Store.Name,
                        Store.Organization?.Name,
                        Store.ParentStore?.Name,
                        Store.StoreType?.Name,
                        Store.StoreGrouping?.Name,
                        Store.LegalEntity,
                        Store.TaxCode,
                        Store.Province?.Name,
                        Store.District?.Name,
                        Store.Ward?.Name,
                        Store.Address,
                        Store.Longitude,
                        Store.Latitude,
                        Store.DeliveryAddress,
                        Store.DeliveryLongitude,
                        Store.DeliveryLatitude,
                        Store.Telephone,
                        Store.OwnerName,
                        Store.OwnerPhone,
                        Store.OwnerEmail,
                        Store.Status?.Name
                    });
                }
                excel.GenerateWorksheet("Store", StoreHeaders, data);
                #endregion

                #region Organization
                var OrganizationHeaders = new List<string[]>()
                {
                    new string[]
                    {
                        "Mã đơn vị",
                        "Tên đơn vị",
                    }
                };
                data = new List<object[]>();
                for (int i = 0; i < Organizations.Count; i++)
                {
                    var Organization = Organizations[i];
                    data.Add(new Object[]
                    {
                        Organization.Code,
                        Organization.Name,
                    });
                }
                excel.GenerateWorksheet("Organization", OrganizationHeaders, data);
                #endregion

                #region StoreType
                var StoreTypeHeaders = new List<string[]>()
                {
                    new string[]
                    {
                        "Mã",
                        "Tên",
                    }
                };
                data = new List<object[]>();
                for (int i = 0; i < StoreTypes.Count; i++)
                {
                    var StoreType = StoreTypes[i];
                    data.Add(new Object[]
                    {
                        StoreType.Code,
                        StoreType.Name,
                    });
                }
                excel.GenerateWorksheet("StoreType", StoreTypeHeaders, data);
                #endregion

                #region StoreGrouping
                var StoreGroupingHeaders = new List<string[]>()
                {
                    new string[]
                    {
                        "Mã",
                        "Tên",
                    }
                };
                data = new List<object[]>();
                for (int i = 0; i < StoreGroupings.Count; i++)
                {
                    var StoreGrouping = StoreGroupings[i];
                    data.Add(new Object[]
                    {
                        StoreGrouping.Code,
                        StoreGrouping.Name,
                    });
                }
                excel.GenerateWorksheet("StoreGroup", StoreGroupingHeaders, data);
                #endregion

                #region Province
                var ProvinceHeaders = new List<string[]>()
                {
                    new string[]
                    {
                        "Mã",
                        "Tên",
                    }
                };
                data = new List<object[]>();
                for (int i = 0; i < Provinces.Count; i++)
                {
                    var Province = Provinces[i];
                    data.Add(new Object[]
                    {
                        Province.Code,
                        Province.Name,
                    });
                }
                excel.GenerateWorksheet("Province", ProvinceHeaders, data);
                #endregion

                #region District
                var DistrictHeaders = new List<string[]>()
                {
                    new string[]
                    {
                        "Mã",
                        "Tên",
                        "Tên tỉnh/thành phố"
                    }
                };
                data = new List<object[]>();
                for (int i = 0; i < Districts.Count; i++)
                {
                    var District = Districts[i];
                    data.Add(new Object[]
                    {
                        District.Code,
                        District.Name,
                        District.Province?.Name,
                    });
                }
                excel.GenerateWorksheet("District", DistrictHeaders, data);
                #endregion

                #region Ward
                var WardHeaders = new List<string[]>()
                {
                    new string[]
                    {
                        "Mã",
                        "Tên",
                        "Tên quận/huyện",
                        "Tên tỉnh/thành phố",
                    }
                };
                data = new List<object[]>();
                for (int i = 0; i < Wards.Count; i++)
                {
                    var Ward = Wards[i];
                    data.Add(new Object[]
                    {
                        Ward.Code,
                        Ward.Name,
                        Ward.District?.Name,
                        Ward.District?.Province?.Name,
                    });
                }
                excel.GenerateWorksheet("Ward", WardHeaders, data);
                #endregion
                excel.Save();
            }

            return File(memoryStream.ToArray(), "application/octet-stream", "Store" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
        }

        [Route(StoreRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate()
        {
            List<Store> ParentStores = await StoreService.List(new StoreFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreSelect.Code | StoreSelect.Name | StoreSelect.Address | StoreSelect.Telephone,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });

            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Code | OrganizationSelect.Name | OrganizationSelect.Path,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });

            List<StoreType> StoreTypes = await StoreTypeService.List(new StoreTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreTypeSelect.Id | StoreTypeSelect.Code | StoreTypeSelect.Name,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(new StoreGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreGroupingSelect.Id | StoreGroupingSelect.Code | StoreGroupingSelect.Name,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });

            List<Province> Provinces = await ProvinceService.List(new ProvinceFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProvinceSelect.Id | ProvinceSelect.Code | ProvinceSelect.Name,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });

            List<District> Districts = await DistrictService.List(new DistrictFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = DistrictSelect.Id | DistrictSelect.Code | DistrictSelect.Name | DistrictSelect.Province,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });

            List<Ward> Wards = await WardService.List(new WardFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = WardSelect.Id | WardSelect.Code | WardSelect.Name | WardSelect.District,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            MemoryStream MemoryStream = new MemoryStream();
            string tempPath = "Templates/Store_Export.xlsx";
            using (var xlPackage = new ExcelPackage(new FileInfo(tempPath)))
            {
                #region sheet Organization 
                var worksheet_Organization = xlPackage.Workbook.Worksheets["Org"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_Organization = 2;
                int numberCell_Organizations = 1;
                for (var i = 0; i < Organizations.Count; i++)
                {
                    Organization Organization = Organizations[i];
                    worksheet_Organization.Cells[startRow_Organization + i, numberCell_Organizations].Value = Organization.Code;
                    worksheet_Organization.Cells[startRow_Organization + i, numberCell_Organizations + 1].Value = Organization.Name;
                }
                #endregion

                #region sheet ParentStore 
                var worksheet_ParentStore = xlPackage.Workbook.Worksheets["ParentStore"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_ParentStore = 2;
                int numberCell_ParentStores = 1;
                for (var i = 0; i < ParentStores.Count; i++)
                {
                    Store Store = ParentStores[i];
                    worksheet_ParentStore.Cells[startRow_ParentStore + i, numberCell_ParentStores].Value = Store.Code;
                    worksheet_ParentStore.Cells[startRow_ParentStore + i, numberCell_ParentStores + 1].Value = Store.Name;
                    worksheet_ParentStore.Cells[startRow_ParentStore + i, numberCell_ParentStores + 2].Value = Store.Address;
                    worksheet_ParentStore.Cells[startRow_ParentStore + i, numberCell_ParentStores + 3].Value = Store.Telephone;
                }
                #endregion

                #region sheet StoreType 
                var worksheet_StoreType = xlPackage.Workbook.Worksheets["StoreType"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_StoreType = 2;
                int numberCell_StoreTypes = 1;
                for (var i = 0; i < StoreTypes.Count; i++)
                {
                    StoreType StoreType = StoreTypes[i];
                    worksheet_StoreType.Cells[startRow_StoreType + i, numberCell_StoreTypes].Value = StoreType.Code;
                    worksheet_StoreType.Cells[startRow_StoreType + i, numberCell_StoreTypes + 1].Value = StoreType.Name;
                }
                #endregion

                #region sheet StoreGrouping 
                var worksheet_StoreGroup = xlPackage.Workbook.Worksheets["StoreGroup"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_StoreGroup = 2;
                int numberCell_StoreGroup = 1;
                for (var i = 0; i < StoreGroupings.Count; i++)
                {
                    StoreGrouping StoreGrouping = StoreGroupings[i];
                    worksheet_StoreGroup.Cells[startRow_StoreGroup + i, numberCell_StoreGroup].Value = StoreGrouping.Code;
                    worksheet_StoreGroup.Cells[startRow_StoreGroup + i, numberCell_StoreGroup + 1].Value = StoreGrouping.Name;
                }
                #endregion

                #region sheet Province 
                var worksheet_Province = xlPackage.Workbook.Worksheets["Province"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_Province = 2;
                int numberCell_Provinces = 1;
                for (var i = 0; i < Provinces.Count; i++)
                {
                    Province Province = Provinces[i];
                    worksheet_Province.Cells[startRow_Province + i, numberCell_Provinces].Value = Province.Code;
                    worksheet_Province.Cells[startRow_Province + i, numberCell_Provinces + 1].Value = Province.Name;
                }
                #endregion

                #region sheet District 
                var worksheet_District = xlPackage.Workbook.Worksheets["District"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_District = 2;
                int numberCell_Districts = 1;
                for (var i = 0; i < Districts.Count; i++)
                {
                    District District = Districts[i];
                    worksheet_District.Cells[startRow_District + i, numberCell_Districts].Value = District.Code;
                    worksheet_District.Cells[startRow_District + i, numberCell_Districts + 1].Value = District.Name;
                    worksheet_District.Cells[startRow_District + i, numberCell_Districts + 2].Value = District.Province?.Name;
                }
                #endregion

                #region sheet Ward 
                var worksheet_Ward = xlPackage.Workbook.Worksheets["Ward"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_Ward = 2;
                int numberCell_Wards = 1;
                for (var i = 0; i < Wards.Count; i++)
                {
                    Ward Ward = Wards[i];
                    worksheet_Ward.Cells[startRow_Ward + i, numberCell_Wards].Value = Ward.Code;
                    worksheet_Ward.Cells[startRow_Ward + i, numberCell_Wards + 1].Value = Ward.Name;
                    worksheet_Ward.Cells[startRow_Ward + i, numberCell_Wards + 2].Value = Ward.District?.Name;
                    worksheet_Ward.Cells[startRow_Ward + i, numberCell_Wards + 3].Value = Ward.District?.Province?.Name;
                }
                #endregion
                xlPackage.SaveAs(MemoryStream);
            }

            return File(MemoryStream.ToArray(), "application/octet-stream", "Template_Store.xlsx");
        }

        [Route(StoreRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Id = new IdFilter { In = Ids };
            StoreFilter.Selects = StoreSelect.Id;
            StoreFilter.Skip = 0;
            StoreFilter.Take = int.MaxValue;

            List<Store> Stores = await StoreService.List(StoreFilter);
            Stores = await StoreService.BulkDelete(Stores);
            if (Stores.Any(x => !x.IsValidated))
                return BadRequest(Stores.Where(x => !x.IsValidated));
            return true;
        }

        [HttpPost]
        [Route(StoreRoute.SaveImage)]
        public async Task<ActionResult<Store_ImageDTO>> SaveImage(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            Image Image = new Image
            {
                Name = file.FileName,
                Content = memoryStream.ToArray(),
            };
            Image = await StoreService.SaveImage(Image);
            if (Image == null)
                return BadRequest();
            Store_ImageDTO Store_ImageDTO = new Store_ImageDTO
            {
                Id = Image.Id,
                Name = Image.Name,
                Url = Image.Url,
                ThumbnailUrl = Image.ThumbnailUrl,
            };
            return Ok(Store_ImageDTO);
        }

        private async Task<bool> HasPermission(long Id)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter = StoreService.ToFilter(StoreFilter);
            if (Id == 0)
            {

            }
            else
            {
                StoreFilter.Id = new IdFilter { Equal = Id };
                int count = await StoreService.Count(StoreFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Store ConvertDTOToEntity(Store_StoreDTO Store_StoreDTO)
        {
            Store Store = new Store();
            Store.Id = Store_StoreDTO.Id;
            Store.Code = Store_StoreDTO.Code;
            Store.CodeDraft = Store_StoreDTO.CodeDraft;
            Store.Name = Store_StoreDTO.Name;
            Store.ParentStoreId = Store_StoreDTO.ParentStoreId;
            Store.OrganizationId = Store_StoreDTO.OrganizationId;
            Store.StoreTypeId = Store_StoreDTO.StoreTypeId;
            Store.StoreGroupingId = Store_StoreDTO.StoreGroupingId;
            Store.Telephone = Store_StoreDTO.Telephone;
            Store.ProvinceId = Store_StoreDTO.ProvinceId;
            Store.DistrictId = Store_StoreDTO.DistrictId;
            Store.WardId = Store_StoreDTO.WardId;
            Store.Address = Store_StoreDTO.Address;
            Store.DeliveryAddress = Store_StoreDTO.DeliveryAddress;
            Store.Latitude = Store_StoreDTO.Latitude;
            Store.Longitude = Store_StoreDTO.Longitude;
            Store.DeliveryLatitude = Store_StoreDTO.DeliveryLatitude;
            Store.DeliveryLongitude = Store_StoreDTO.DeliveryLongitude;
            Store.OwnerName = Store_StoreDTO.OwnerName;
            Store.OwnerPhone = Store_StoreDTO.OwnerPhone;
            Store.OwnerEmail = Store_StoreDTO.OwnerEmail;
            Store.TaxCode = Store_StoreDTO.TaxCode;
            Store.LegalEntity = Store_StoreDTO.LegalEntity;
            Store.StatusId = Store_StoreDTO.StatusId;
            Store.StoreScoutingId = Store_StoreDTO.StoreScoutingId;
            Store.StoreStatusId = Store_StoreDTO.StoreStatusId;
            Store.AppUserId = Store_StoreDTO.AppUserId;
            Store.District = Store_StoreDTO.District == null ? null : new District
            {
                Id = Store_StoreDTO.District.Id,
                Name = Store_StoreDTO.District.Name,
                Priority = Store_StoreDTO.District.Priority,
                ProvinceId = Store_StoreDTO.District.ProvinceId,
                StatusId = Store_StoreDTO.District.StatusId,
            };
            Store.Organization = Store_StoreDTO.Organization == null ? null : new Organization
            {
                Id = Store_StoreDTO.Organization.Id,
                Code = Store_StoreDTO.Organization.Code,
                Name = Store_StoreDTO.Organization.Name,
                ParentId = Store_StoreDTO.Organization.ParentId,
                Path = Store_StoreDTO.Organization.Path,
                Level = Store_StoreDTO.Organization.Level,
                StatusId = Store_StoreDTO.Organization.StatusId,
                Phone = Store_StoreDTO.Organization.Phone,
                Address = Store_StoreDTO.Organization.Address,
                Email = Store_StoreDTO.Organization.Email,
            };
            Store.ParentStore = Store_StoreDTO.ParentStore == null ? null : new Store
            {
                Id = Store_StoreDTO.ParentStore.Id,
                Code = Store_StoreDTO.ParentStore.Code,
                Name = Store_StoreDTO.ParentStore.Name,
                ParentStoreId = Store_StoreDTO.ParentStore.ParentStoreId,
                OrganizationId = Store_StoreDTO.ParentStore.OrganizationId,
                StoreTypeId = Store_StoreDTO.ParentStore.StoreTypeId,
                StoreGroupingId = Store_StoreDTO.ParentStore.StoreGroupingId,
                Telephone = Store_StoreDTO.ParentStore.Telephone,
                ProvinceId = Store_StoreDTO.ParentStore.ProvinceId,
                DistrictId = Store_StoreDTO.ParentStore.DistrictId,
                WardId = Store_StoreDTO.ParentStore.WardId,
                Address = Store_StoreDTO.ParentStore.Address,
                DeliveryAddress = Store_StoreDTO.ParentStore.DeliveryAddress,
                Latitude = Store_StoreDTO.ParentStore.Latitude,
                Longitude = Store_StoreDTO.ParentStore.Longitude,
                DeliveryLatitude = Store_StoreDTO.ParentStore.DeliveryLatitude,
                DeliveryLongitude = Store_StoreDTO.ParentStore.DeliveryLongitude,
                OwnerName = Store_StoreDTO.ParentStore.OwnerName,
                OwnerPhone = Store_StoreDTO.ParentStore.OwnerPhone,
                OwnerEmail = Store_StoreDTO.ParentStore.OwnerEmail,
                StatusId = Store_StoreDTO.ParentStore.StatusId,
            };
            Store.Province = Store_StoreDTO.Province == null ? null : new Province
            {
                Id = Store_StoreDTO.Province.Id,
                Name = Store_StoreDTO.Province.Name,
                Priority = Store_StoreDTO.Province.Priority,
                StatusId = Store_StoreDTO.Province.StatusId,
            };
            Store.Status = Store_StoreDTO.Status == null ? null : new Status
            {
                Id = Store_StoreDTO.Status.Id,
                Code = Store_StoreDTO.Status.Code,
                Name = Store_StoreDTO.Status.Name,
            };
            Store.StoreStatus = Store_StoreDTO.StoreStatus == null ? null : new StoreStatus
            {
                Id = Store_StoreDTO.StoreStatus.Id,
                Code = Store_StoreDTO.StoreStatus.Code,
                Name = Store_StoreDTO.StoreStatus.Name,
            };
            Store.StoreScouting = Store_StoreDTO.StoreScouting == null ? null : new StoreScouting
            {
                Id = Store_StoreDTO.StoreScouting.Id,
                Code = Store_StoreDTO.StoreScouting.Code,
                Name = Store_StoreDTO.StoreScouting.Name,
                Address = Store_StoreDTO.StoreScouting.Address,
                CreatorId = Store_StoreDTO.StoreScouting.CreatorId,
                DistrictId = Store_StoreDTO.StoreScouting.DistrictId,
                Latitude = Store_StoreDTO.StoreScouting.Latitude,
                Longitude = Store_StoreDTO.StoreScouting.Longitude,
                OwnerPhone = Store_StoreDTO.StoreScouting.OwnerPhone,
                ProvinceId = Store_StoreDTO.StoreScouting.ProvinceId,
                StoreScoutingStatusId = Store_StoreDTO.StoreScouting.StoreScoutingStatusId,
                WardId = Store_StoreDTO.StoreScouting.WardId,
            };
            Store.StoreGrouping = Store_StoreDTO.StoreGrouping == null ? null : new StoreGrouping
            {
                Id = Store_StoreDTO.StoreGrouping.Id,
                Code = Store_StoreDTO.StoreGrouping.Code,
                Name = Store_StoreDTO.StoreGrouping.Name,
                ParentId = Store_StoreDTO.StoreGrouping.ParentId,
                Path = Store_StoreDTO.StoreGrouping.Path,
                Level = Store_StoreDTO.StoreGrouping.Level,
            };
            Store.StoreType = Store_StoreDTO.StoreType == null ? null : new StoreType
            {
                Id = Store_StoreDTO.StoreType.Id,
                Code = Store_StoreDTO.StoreType.Code,
                Name = Store_StoreDTO.StoreType.Name,
                StatusId = Store_StoreDTO.StoreType.StatusId,
            };
            Store.Ward = Store_StoreDTO.Ward == null ? null : new Ward
            {
                Id = Store_StoreDTO.Ward.Id,
                Name = Store_StoreDTO.Ward.Name,
                Priority = Store_StoreDTO.Ward.Priority,
                DistrictId = Store_StoreDTO.Ward.DistrictId,
                StatusId = Store_StoreDTO.Ward.StatusId,
            };
            Store.AppUser = Store_StoreDTO.AppUser == null ? null : new AppUser
            {
                Id = Store_StoreDTO.AppUser.Id,
                Username = Store_StoreDTO.AppUser.Username,
                DisplayName = Store_StoreDTO.AppUser.DisplayName,
                Address = Store_StoreDTO.AppUser.Address,
                Email = Store_StoreDTO.AppUser.Email,
                Phone = Store_StoreDTO.AppUser.Phone,
            };
            Store.StoreImageMappings = Store_StoreDTO.StoreImageMappings?
                .Select(x => new StoreImageMapping
                {
                    StoreId = x.StoreId,
                    ImageId = x.ImageId,
                    Image = new Image
                    {
                        Id = x.Image.Id,
                        Name = x.Image.Name,
                        Url = x.Image.Url,
                        ThumbnailUrl = x.Image.ThumbnailUrl,
                    }
                }).ToList();
            Store.BaseLanguage = CurrentContext.Language;
            return Store;
        }

        private StoreFilter ConvertFilterDTOToFilterEntity(Store_StoreFilterDTO Store_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Skip = Store_StoreFilterDTO.Skip;
            StoreFilter.Take = Store_StoreFilterDTO.Take;
            StoreFilter.OrderBy = Store_StoreFilterDTO.OrderBy;
            StoreFilter.OrderType = Store_StoreFilterDTO.OrderType;

            StoreFilter.Id = Store_StoreFilterDTO.Id;
            StoreFilter.Code = Store_StoreFilterDTO.Code;
            StoreFilter.CodeDraft = Store_StoreFilterDTO.CodeDraft;
            StoreFilter.Name = Store_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Store_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = Store_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = Store_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Store_StoreFilterDTO.StoreGroupingId;
            StoreFilter.Telephone = Store_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = Store_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Store_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Store_StoreFilterDTO.WardId;
            StoreFilter.Address = Store_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Store_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Store_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Store_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Store_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Store_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Store_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Store_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Store_StoreFilterDTO.OwnerEmail;
            StoreFilter.AppUserId = Store_StoreFilterDTO.AppUserId;
            StoreFilter.StatusId = Store_StoreFilterDTO.StatusId;
            StoreFilter.StoreStatusId = Store_StoreFilterDTO.StoreStatusId;
            return StoreFilter;
        }
    }
}

