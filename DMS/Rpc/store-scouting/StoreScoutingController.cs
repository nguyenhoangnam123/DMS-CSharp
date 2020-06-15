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
using DMS.Services.MStoreScouting;
using DMS.Services.MAppUser;
using DMS.Services.MDistrict;
using DMS.Services.MOrganization;
using DMS.Services.MProvince;
using DMS.Services.MStore;
using DMS.Services.MStoreScoutingStatus;
using DMS.Services.MWard;

namespace DMS.Rpc.store_scouting
{
    public class StoreScoutingController : RpcController
    {
        private IAppUserService AppUserService;
        private IDistrictService DistrictService;
        private IOrganizationService OrganizationService;
        private IProvinceService ProvinceService;
        private IStoreService StoreService;
        private IStoreScoutingStatusService StoreScoutingStatusService;
        private IWardService WardService;
        private IStoreScoutingService StoreScoutingService;
        private ICurrentContext CurrentContext;
        public StoreScoutingController(
            IAppUserService AppUserService,
            IDistrictService DistrictService,
            IOrganizationService OrganizationService,
            IProvinceService ProvinceService,
            IStoreService StoreService,
            IStoreScoutingStatusService StoreScoutingStatusService,
            IWardService WardService,
            IStoreScoutingService StoreScoutingService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.DistrictService = DistrictService;
            this.OrganizationService = OrganizationService;
            this.ProvinceService = ProvinceService;
            this.StoreService = StoreService;
            this.StoreScoutingStatusService = StoreScoutingStatusService;
            this.WardService = WardService;
            this.StoreScoutingService = StoreScoutingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(StoreScoutingRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] StoreScouting_StoreScoutingFilterDTO StoreScouting_StoreScoutingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScoutingFilter StoreScoutingFilter = ConvertFilterDTOToFilterEntity(StoreScouting_StoreScoutingFilterDTO);
            StoreScoutingFilter = StoreScoutingService.ToFilter(StoreScoutingFilter);
            int count = await StoreScoutingService.Count(StoreScoutingFilter);
            return count;
        }

        [Route(StoreScoutingRoute.List), HttpPost]
        public async Task<ActionResult<List<StoreScouting_StoreScoutingDTO>>> List([FromBody] StoreScouting_StoreScoutingFilterDTO StoreScouting_StoreScoutingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScoutingFilter StoreScoutingFilter = ConvertFilterDTOToFilterEntity(StoreScouting_StoreScoutingFilterDTO);
            StoreScoutingFilter = StoreScoutingService.ToFilter(StoreScoutingFilter);
            List<StoreScouting> StoreScoutings = await StoreScoutingService.List(StoreScoutingFilter);
            List<StoreScouting_StoreScoutingDTO> StoreScouting_StoreScoutingDTOs = StoreScoutings
                .Select(c => new StoreScouting_StoreScoutingDTO(c)).ToList();
            return StoreScouting_StoreScoutingDTOs;
        }

        [Route(StoreScoutingRoute.Get), HttpPost]
        public async Task<ActionResult<StoreScouting_StoreScoutingDTO>> Get([FromBody]StoreScouting_StoreScoutingDTO StoreScouting_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreScouting_StoreScoutingDTO.Id))
                return Forbid();

            StoreScouting StoreScouting = await StoreScoutingService.Get(StoreScouting_StoreScoutingDTO.Id);
            return new StoreScouting_StoreScoutingDTO(StoreScouting);
        }

        [Route(StoreScoutingRoute.Create), HttpPost]
        public async Task<ActionResult<StoreScouting_StoreScoutingDTO>> Create([FromBody] StoreScouting_StoreScoutingDTO StoreScouting_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(StoreScouting_StoreScoutingDTO.Id))
                return Forbid();

            StoreScouting StoreScouting = ConvertDTOToEntity(StoreScouting_StoreScoutingDTO);
            StoreScouting = await StoreScoutingService.Create(StoreScouting);
            StoreScouting_StoreScoutingDTO = new StoreScouting_StoreScoutingDTO(StoreScouting);
            if (StoreScouting.IsValidated)
                return StoreScouting_StoreScoutingDTO;
            else
                return BadRequest(StoreScouting_StoreScoutingDTO);
        }

        [Route(StoreScoutingRoute.Update), HttpPost]
        public async Task<ActionResult<StoreScouting_StoreScoutingDTO>> Update([FromBody] StoreScouting_StoreScoutingDTO StoreScouting_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(StoreScouting_StoreScoutingDTO.Id))
                return Forbid();

            StoreScouting StoreScouting = ConvertDTOToEntity(StoreScouting_StoreScoutingDTO);
            StoreScouting = await StoreScoutingService.Update(StoreScouting);
            StoreScouting_StoreScoutingDTO = new StoreScouting_StoreScoutingDTO(StoreScouting);
            if (StoreScouting.IsValidated)
                return StoreScouting_StoreScoutingDTO;
            else
                return BadRequest(StoreScouting_StoreScoutingDTO);
        }

        [Route(StoreScoutingRoute.Delete), HttpPost]
        public async Task<ActionResult<StoreScouting_StoreScoutingDTO>> Delete([FromBody] StoreScouting_StoreScoutingDTO StoreScouting_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreScouting_StoreScoutingDTO.Id))
                return Forbid();

            StoreScouting StoreScouting = ConvertDTOToEntity(StoreScouting_StoreScoutingDTO);
            StoreScouting = await StoreScoutingService.Delete(StoreScouting);
            StoreScouting_StoreScoutingDTO = new StoreScouting_StoreScoutingDTO(StoreScouting);
            if (StoreScouting.IsValidated)
                return StoreScouting_StoreScoutingDTO;
            else
                return BadRequest(StoreScouting_StoreScoutingDTO);
        }
        
        [Route(StoreScoutingRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScoutingFilter StoreScoutingFilter = new StoreScoutingFilter();
            StoreScoutingFilter = StoreScoutingService.ToFilter(StoreScoutingFilter);
            StoreScoutingFilter.Id = new IdFilter { In = Ids };
            StoreScoutingFilter.Selects = StoreScoutingSelect.Id;
            StoreScoutingFilter.Skip = 0;
            StoreScoutingFilter.Take = int.MaxValue;

            List<StoreScouting> StoreScoutings = await StoreScoutingService.List(StoreScoutingFilter);
            StoreScoutings = await StoreScoutingService.BulkDelete(StoreScoutings);
            return true;
        }
        
        [Route(StoreScoutingRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUserFilter CreatorFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Creators = await AppUserService.List(CreatorFilter);
            DistrictFilter DistrictFilter = new DistrictFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = DistrictSelect.ALL
            };
            List<District> Districts = await DistrictService.List(DistrictFilter);
            ProvinceFilter ProvinceFilter = new ProvinceFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProvinceSelect.ALL
            };
            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            StoreFilter StoreFilter = new StoreFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreSelect.ALL
            };
            List<Store> Stores = await StoreService.List(StoreFilter);
            StoreScoutingStatusFilter StoreScoutingStatusFilter = new StoreScoutingStatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreScoutingStatusSelect.ALL
            };
            List<StoreScoutingStatus> StoreScoutingStatuses = await StoreScoutingStatusService.List(StoreScoutingStatusFilter);
            WardFilter WardFilter = new WardFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = WardSelect.ALL
            };
            List<Ward> Wards = await WardService.List(WardFilter);
            List<StoreScouting> StoreScoutings = new List<StoreScouting>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(StoreScoutings);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int OwnerPhoneColumn = 3 + StartColumn;
                int ProvinceIdColumn = 4 + StartColumn;
                int DistrictIdColumn = 5 + StartColumn;
                int WardIdColumn = 6 + StartColumn;
                int OrganizationIdColumn = 7 + StartColumn;
                int AddressColumn = 8 + StartColumn;
                int LatitudeColumn = 9 + StartColumn;
                int LongitudeColumn = 10 + StartColumn;
                int StoreIdColumn = 11 + StartColumn;
                int CreatorIdColumn = 12 + StartColumn;
                int StoreScoutingStatusIdColumn = 13 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string OwnerPhoneValue = worksheet.Cells[i + StartRow, OwnerPhoneColumn].Value?.ToString();
                    string ProvinceIdValue = worksheet.Cells[i + StartRow, ProvinceIdColumn].Value?.ToString();
                    string DistrictIdValue = worksheet.Cells[i + StartRow, DistrictIdColumn].Value?.ToString();
                    string WardIdValue = worksheet.Cells[i + StartRow, WardIdColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    string AddressValue = worksheet.Cells[i + StartRow, AddressColumn].Value?.ToString();
                    string LatitudeValue = worksheet.Cells[i + StartRow, LatitudeColumn].Value?.ToString();
                    string LongitudeValue = worksheet.Cells[i + StartRow, LongitudeColumn].Value?.ToString();
                    string StoreIdValue = worksheet.Cells[i + StartRow, StoreIdColumn].Value?.ToString();
                    string CreatorIdValue = worksheet.Cells[i + StartRow, CreatorIdColumn].Value?.ToString();
                    string StoreScoutingStatusIdValue = worksheet.Cells[i + StartRow, StoreScoutingStatusIdColumn].Value?.ToString();
                    
                    StoreScouting StoreScouting = new StoreScouting();
                    StoreScouting.Code = CodeValue;
                    StoreScouting.Name = NameValue;
                    StoreScouting.OwnerPhone = OwnerPhoneValue;
                    StoreScouting.Address = AddressValue;
                    StoreScouting.Latitude = decimal.TryParse(LatitudeValue, out decimal Latitude) ? Latitude : 0;
                    StoreScouting.Longitude = decimal.TryParse(LongitudeValue, out decimal Longitude) ? Longitude : 0;
                    AppUser Creator = Creators.Where(x => x.Id.ToString() == CreatorIdValue).FirstOrDefault();
                    StoreScouting.CreatorId = Creator == null ? 0 : Creator.Id;
                    StoreScouting.Creator = Creator;
                    District District = Districts.Where(x => x.Id.ToString() == DistrictIdValue).FirstOrDefault();
                    StoreScouting.DistrictId = District == null ? 0 : District.Id;
                    StoreScouting.District = District;
                    Province Province = Provinces.Where(x => x.Id.ToString() == ProvinceIdValue).FirstOrDefault();
                    StoreScouting.ProvinceId = Province == null ? 0 : Province.Id;
                    StoreScouting.Province = Province;
                    Store Store = Stores.Where(x => x.Id.ToString() == StoreIdValue).FirstOrDefault();
                    StoreScouting.StoreId = Store == null ? 0 : Store.Id;
                    StoreScouting.Store = Store;
                    StoreScoutingStatus StoreScoutingStatus = StoreScoutingStatuses.Where(x => x.Id.ToString() == StoreScoutingStatusIdValue).FirstOrDefault();
                    StoreScouting.StoreScoutingStatusId = StoreScoutingStatus == null ? 0 : StoreScoutingStatus.Id;
                    StoreScouting.StoreScoutingStatus = StoreScoutingStatus;
                    Ward Ward = Wards.Where(x => x.Id.ToString() == WardIdValue).FirstOrDefault();
                    StoreScouting.WardId = Ward == null ? 0 : Ward.Id;
                    StoreScouting.Ward = Ward;
                    
                    StoreScoutings.Add(StoreScouting);
                }
            }
            StoreScoutings = await StoreScoutingService.Import(StoreScoutings);
            if (StoreScoutings.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < StoreScoutings.Count; i++)
                {
                    StoreScouting StoreScouting = StoreScoutings[i];
                    if (!StoreScouting.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (StoreScouting.Errors.ContainsKey(nameof(StoreScouting.Id)))
                            Error += StoreScouting.Errors[nameof(StoreScouting.Id)];
                        if (StoreScouting.Errors.ContainsKey(nameof(StoreScouting.Code)))
                            Error += StoreScouting.Errors[nameof(StoreScouting.Code)];
                        if (StoreScouting.Errors.ContainsKey(nameof(StoreScouting.Name)))
                            Error += StoreScouting.Errors[nameof(StoreScouting.Name)];
                        if (StoreScouting.Errors.ContainsKey(nameof(StoreScouting.OwnerPhone)))
                            Error += StoreScouting.Errors[nameof(StoreScouting.OwnerPhone)];
                        if (StoreScouting.Errors.ContainsKey(nameof(StoreScouting.ProvinceId)))
                            Error += StoreScouting.Errors[nameof(StoreScouting.ProvinceId)];
                        if (StoreScouting.Errors.ContainsKey(nameof(StoreScouting.DistrictId)))
                            Error += StoreScouting.Errors[nameof(StoreScouting.DistrictId)];
                        if (StoreScouting.Errors.ContainsKey(nameof(StoreScouting.WardId)))
                            Error += StoreScouting.Errors[nameof(StoreScouting.WardId)];
                        if (StoreScouting.Errors.ContainsKey(nameof(StoreScouting.OrganizationId)))
                            Error += StoreScouting.Errors[nameof(StoreScouting.OrganizationId)];
                        if (StoreScouting.Errors.ContainsKey(nameof(StoreScouting.Address)))
                            Error += StoreScouting.Errors[nameof(StoreScouting.Address)];
                        if (StoreScouting.Errors.ContainsKey(nameof(StoreScouting.Latitude)))
                            Error += StoreScouting.Errors[nameof(StoreScouting.Latitude)];
                        if (StoreScouting.Errors.ContainsKey(nameof(StoreScouting.Longitude)))
                            Error += StoreScouting.Errors[nameof(StoreScouting.Longitude)];
                        if (StoreScouting.Errors.ContainsKey(nameof(StoreScouting.StoreId)))
                            Error += StoreScouting.Errors[nameof(StoreScouting.StoreId)];
                        if (StoreScouting.Errors.ContainsKey(nameof(StoreScouting.CreatorId)))
                            Error += StoreScouting.Errors[nameof(StoreScouting.CreatorId)];
                        if (StoreScouting.Errors.ContainsKey(nameof(StoreScouting.StoreScoutingStatusId)))
                            Error += StoreScouting.Errors[nameof(StoreScouting.StoreScoutingStatusId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(StoreScoutingRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] StoreScouting_StoreScoutingFilterDTO StoreScouting_StoreScoutingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region StoreScouting
                var StoreScoutingFilter = ConvertFilterDTOToFilterEntity(StoreScouting_StoreScoutingFilterDTO);
                StoreScoutingFilter.Skip = 0;
                StoreScoutingFilter.Take = int.MaxValue;
                StoreScoutingFilter = StoreScoutingService.ToFilter(StoreScoutingFilter);
                List<StoreScouting> StoreScoutings = await StoreScoutingService.List(StoreScoutingFilter);

                var StoreScoutingHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "OwnerPhone",
                        "ProvinceId",
                        "DistrictId",
                        "WardId",
                        "OrganizationId",
                        "Address",
                        "Latitude",
                        "Longitude",
                        "StoreId",
                        "CreatorId",
                        "StoreScoutingStatusId",
                    }
                };
                List<object[]> StoreScoutingData = new List<object[]>();
                for (int i = 0; i < StoreScoutings.Count; i++)
                {
                    var StoreScouting = StoreScoutings[i];
                    StoreScoutingData.Add(new Object[]
                    {
                        StoreScouting.Id,
                        StoreScouting.Code,
                        StoreScouting.Name,
                        StoreScouting.OwnerPhone,
                        StoreScouting.ProvinceId,
                        StoreScouting.DistrictId,
                        StoreScouting.WardId,
                        StoreScouting.OrganizationId,
                        StoreScouting.Address,
                        StoreScouting.Latitude,
                        StoreScouting.Longitude,
                        StoreScouting.StoreId,
                        StoreScouting.CreatorId,
                        StoreScouting.StoreScoutingStatusId,
                    });
                }
                excel.GenerateWorksheet("StoreScouting", StoreScoutingHeaders, StoreScoutingData);
                #endregion
                
                #region AppUser
                var AppUserFilter = new AppUserFilter();
                AppUserFilter.Selects = AppUserSelect.ALL;
                AppUserFilter.OrderBy = AppUserOrder.Id;
                AppUserFilter.OrderType = OrderType.ASC;
                AppUserFilter.Skip = 0;
                AppUserFilter.Take = int.MaxValue;
                List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

                var AppUserHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Username",
                        "DisplayName",
                        "Address",
                        "Email",
                        "Phone",
                        "PositionId",
                        "Department",
                        "OrganizationId",
                        "StatusId",
                        "Avatar",
                        "ProvinceId",
                        "SexId",
                        "Birthday",
                    }
                };
                List<object[]> AppUserData = new List<object[]>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    var AppUser = AppUsers[i];
                    AppUserData.Add(new Object[]
                    {
                        AppUser.Id,
                        AppUser.Username,
                        AppUser.DisplayName,
                        AppUser.Address,
                        AppUser.Email,
                        AppUser.Phone,
                        AppUser.PositionId,
                        AppUser.Department,
                        AppUser.OrganizationId,
                        AppUser.StatusId,
                        AppUser.Avatar,
                        AppUser.ProvinceId,
                        AppUser.SexId,
                        AppUser.Birthday,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                #region District
                var DistrictFilter = new DistrictFilter();
                DistrictFilter.Selects = DistrictSelect.ALL;
                DistrictFilter.OrderBy = DistrictOrder.Id;
                DistrictFilter.OrderType = OrderType.ASC;
                DistrictFilter.Skip = 0;
                DistrictFilter.Take = int.MaxValue;
                List<District> Districts = await DistrictService.List(DistrictFilter);

                var DistrictHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Priority",
                        "ProvinceId",
                        "StatusId",
                    }
                };
                List<object[]> DistrictData = new List<object[]>();
                for (int i = 0; i < Districts.Count; i++)
                {
                    var District = Districts[i];
                    DistrictData.Add(new Object[]
                    {
                        District.Id,
                        District.Code,
                        District.Name,
                        District.Priority,
                        District.ProvinceId,
                        District.StatusId,
                    });
                }
                excel.GenerateWorksheet("District", DistrictHeaders, DistrictData);
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
                #region Province
                var ProvinceFilter = new ProvinceFilter();
                ProvinceFilter.Selects = ProvinceSelect.ALL;
                ProvinceFilter.OrderBy = ProvinceOrder.Id;
                ProvinceFilter.OrderType = OrderType.ASC;
                ProvinceFilter.Skip = 0;
                ProvinceFilter.Take = int.MaxValue;
                List<Province> Provinces = await ProvinceService.List(ProvinceFilter);

                var ProvinceHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Priority",
                        "StatusId",
                    }
                };
                List<object[]> ProvinceData = new List<object[]>();
                for (int i = 0; i < Provinces.Count; i++)
                {
                    var Province = Provinces[i];
                    ProvinceData.Add(new Object[]
                    {
                        Province.Id,
                        Province.Code,
                        Province.Name,
                        Province.Priority,
                        Province.StatusId,
                    });
                }
                excel.GenerateWorksheet("Province", ProvinceHeaders, ProvinceData);
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
                    });
                }
                excel.GenerateWorksheet("Store", StoreHeaders, StoreData);
                #endregion
                #region StoreScoutingStatus
                var StoreScoutingStatusFilter = new StoreScoutingStatusFilter();
                StoreScoutingStatusFilter.Selects = StoreScoutingStatusSelect.ALL;
                StoreScoutingStatusFilter.OrderBy = StoreScoutingStatusOrder.Id;
                StoreScoutingStatusFilter.OrderType = OrderType.ASC;
                StoreScoutingStatusFilter.Skip = 0;
                StoreScoutingStatusFilter.Take = int.MaxValue;
                List<StoreScoutingStatus> StoreScoutingStatuses = await StoreScoutingStatusService.List(StoreScoutingStatusFilter);

                var StoreScoutingStatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> StoreScoutingStatusData = new List<object[]>();
                for (int i = 0; i < StoreScoutingStatuses.Count; i++)
                {
                    var StoreScoutingStatus = StoreScoutingStatuses[i];
                    StoreScoutingStatusData.Add(new Object[]
                    {
                        StoreScoutingStatus.Id,
                        StoreScoutingStatus.Code,
                        StoreScoutingStatus.Name,
                    });
                }
                excel.GenerateWorksheet("StoreScoutingStatus", StoreScoutingStatusHeaders, StoreScoutingStatusData);
                #endregion
                #region Ward
                var WardFilter = new WardFilter();
                WardFilter.Selects = WardSelect.ALL;
                WardFilter.OrderBy = WardOrder.Id;
                WardFilter.OrderType = OrderType.ASC;
                WardFilter.Skip = 0;
                WardFilter.Take = int.MaxValue;
                List<Ward> Wards = await WardService.List(WardFilter);

                var WardHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Priority",
                        "DistrictId",
                        "StatusId",
                    }
                };
                List<object[]> WardData = new List<object[]>();
                for (int i = 0; i < Wards.Count; i++)
                {
                    var Ward = Wards[i];
                    WardData.Add(new Object[]
                    {
                        Ward.Id,
                        Ward.Code,
                        Ward.Name,
                        Ward.Priority,
                        Ward.DistrictId,
                        Ward.StatusId,
                    });
                }
                excel.GenerateWorksheet("Ward", WardHeaders, WardData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "StoreScouting.xlsx");
        }

        [Route(StoreScoutingRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] StoreScouting_StoreScoutingFilterDTO StoreScouting_StoreScoutingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region StoreScouting
                var StoreScoutingHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "OwnerPhone",
                        "ProvinceId",
                        "DistrictId",
                        "WardId",
                        "OrganizationId",
                        "Address",
                        "Latitude",
                        "Longitude",
                        "StoreId",
                        "CreatorId",
                        "StoreScoutingStatusId",
                    }
                };
                List<object[]> StoreScoutingData = new List<object[]>();
                excel.GenerateWorksheet("StoreScouting", StoreScoutingHeaders, StoreScoutingData);
                #endregion
                
                #region AppUser
                var AppUserFilter = new AppUserFilter();
                AppUserFilter.Selects = AppUserSelect.ALL;
                AppUserFilter.OrderBy = AppUserOrder.Id;
                AppUserFilter.OrderType = OrderType.ASC;
                AppUserFilter.Skip = 0;
                AppUserFilter.Take = int.MaxValue;
                List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

                var AppUserHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Username",
                        "DisplayName",
                        "Address",
                        "Email",
                        "Phone",
                        "PositionId",
                        "Department",
                        "OrganizationId",
                        "StatusId",
                        "Avatar",
                        "ProvinceId",
                        "SexId",
                        "Birthday",
                    }
                };
                List<object[]> AppUserData = new List<object[]>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    var AppUser = AppUsers[i];
                    AppUserData.Add(new Object[]
                    {
                        AppUser.Id,
                        AppUser.Username,
                        AppUser.DisplayName,
                        AppUser.Address,
                        AppUser.Email,
                        AppUser.Phone,
                        AppUser.PositionId,
                        AppUser.Department,
                        AppUser.OrganizationId,
                        AppUser.StatusId,
                        AppUser.Avatar,
                        AppUser.ProvinceId,
                        AppUser.SexId,
                        AppUser.Birthday,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                #region District
                var DistrictFilter = new DistrictFilter();
                DistrictFilter.Selects = DistrictSelect.ALL;
                DistrictFilter.OrderBy = DistrictOrder.Id;
                DistrictFilter.OrderType = OrderType.ASC;
                DistrictFilter.Skip = 0;
                DistrictFilter.Take = int.MaxValue;
                List<District> Districts = await DistrictService.List(DistrictFilter);

                var DistrictHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Priority",
                        "ProvinceId",
                        "StatusId",
                    }
                };
                List<object[]> DistrictData = new List<object[]>();
                for (int i = 0; i < Districts.Count; i++)
                {
                    var District = Districts[i];
                    DistrictData.Add(new Object[]
                    {
                        District.Id,
                        District.Code,
                        District.Name,
                        District.Priority,
                        District.ProvinceId,
                        District.StatusId,
                    });
                }
                excel.GenerateWorksheet("District", DistrictHeaders, DistrictData);
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
                #region Province
                var ProvinceFilter = new ProvinceFilter();
                ProvinceFilter.Selects = ProvinceSelect.ALL;
                ProvinceFilter.OrderBy = ProvinceOrder.Id;
                ProvinceFilter.OrderType = OrderType.ASC;
                ProvinceFilter.Skip = 0;
                ProvinceFilter.Take = int.MaxValue;
                List<Province> Provinces = await ProvinceService.List(ProvinceFilter);

                var ProvinceHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Priority",
                        "StatusId",
                    }
                };
                List<object[]> ProvinceData = new List<object[]>();
                for (int i = 0; i < Provinces.Count; i++)
                {
                    var Province = Provinces[i];
                    ProvinceData.Add(new Object[]
                    {
                        Province.Id,
                        Province.Code,
                        Province.Name,
                        Province.Priority,
                        Province.StatusId,
                    });
                }
                excel.GenerateWorksheet("Province", ProvinceHeaders, ProvinceData);
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
                    });
                }
                excel.GenerateWorksheet("Store", StoreHeaders, StoreData);
                #endregion
                #region StoreScoutingStatus
                var StoreScoutingStatusFilter = new StoreScoutingStatusFilter();
                StoreScoutingStatusFilter.Selects = StoreScoutingStatusSelect.ALL;
                StoreScoutingStatusFilter.OrderBy = StoreScoutingStatusOrder.Id;
                StoreScoutingStatusFilter.OrderType = OrderType.ASC;
                StoreScoutingStatusFilter.Skip = 0;
                StoreScoutingStatusFilter.Take = int.MaxValue;
                List<StoreScoutingStatus> StoreScoutingStatuses = await StoreScoutingStatusService.List(StoreScoutingStatusFilter);

                var StoreScoutingStatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> StoreScoutingStatusData = new List<object[]>();
                for (int i = 0; i < StoreScoutingStatuses.Count; i++)
                {
                    var StoreScoutingStatus = StoreScoutingStatuses[i];
                    StoreScoutingStatusData.Add(new Object[]
                    {
                        StoreScoutingStatus.Id,
                        StoreScoutingStatus.Code,
                        StoreScoutingStatus.Name,
                    });
                }
                excel.GenerateWorksheet("StoreScoutingStatus", StoreScoutingStatusHeaders, StoreScoutingStatusData);
                #endregion
                #region Ward
                var WardFilter = new WardFilter();
                WardFilter.Selects = WardSelect.ALL;
                WardFilter.OrderBy = WardOrder.Id;
                WardFilter.OrderType = OrderType.ASC;
                WardFilter.Skip = 0;
                WardFilter.Take = int.MaxValue;
                List<Ward> Wards = await WardService.List(WardFilter);

                var WardHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Priority",
                        "DistrictId",
                        "StatusId",
                    }
                };
                List<object[]> WardData = new List<object[]>();
                for (int i = 0; i < Wards.Count; i++)
                {
                    var Ward = Wards[i];
                    WardData.Add(new Object[]
                    {
                        Ward.Id,
                        Ward.Code,
                        Ward.Name,
                        Ward.Priority,
                        Ward.DistrictId,
                        Ward.StatusId,
                    });
                }
                excel.GenerateWorksheet("Ward", WardHeaders, WardData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "StoreScouting.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            StoreScoutingFilter StoreScoutingFilter = new StoreScoutingFilter();
            StoreScoutingFilter = StoreScoutingService.ToFilter(StoreScoutingFilter);
            if (Id == 0)
            {

            }
            else
            {
                StoreScoutingFilter.Id = new IdFilter { Equal = Id };
                int count = await StoreScoutingService.Count(StoreScoutingFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private StoreScouting ConvertDTOToEntity(StoreScouting_StoreScoutingDTO StoreScouting_StoreScoutingDTO)
        {
            StoreScouting StoreScouting = new StoreScouting();
            StoreScouting.Id = StoreScouting_StoreScoutingDTO.Id;
            StoreScouting.Code = StoreScouting_StoreScoutingDTO.Code;
            StoreScouting.Name = StoreScouting_StoreScoutingDTO.Name;
            StoreScouting.OwnerPhone = StoreScouting_StoreScoutingDTO.OwnerPhone;
            StoreScouting.ProvinceId = StoreScouting_StoreScoutingDTO.ProvinceId;
            StoreScouting.DistrictId = StoreScouting_StoreScoutingDTO.DistrictId;
            StoreScouting.WardId = StoreScouting_StoreScoutingDTO.WardId;
            StoreScouting.OrganizationId = StoreScouting_StoreScoutingDTO.OrganizationId;
            StoreScouting.Address = StoreScouting_StoreScoutingDTO.Address;
            StoreScouting.Latitude = StoreScouting_StoreScoutingDTO.Latitude;
            StoreScouting.Longitude = StoreScouting_StoreScoutingDTO.Longitude;
            StoreScouting.StoreId = StoreScouting_StoreScoutingDTO.StoreId;
            StoreScouting.CreatorId = StoreScouting_StoreScoutingDTO.CreatorId;
            StoreScouting.StoreScoutingStatusId = StoreScouting_StoreScoutingDTO.StoreScoutingStatusId;
            StoreScouting.Creator = StoreScouting_StoreScoutingDTO.Creator == null ? null : new AppUser
            {
                Id = StoreScouting_StoreScoutingDTO.Creator.Id,
                Username = StoreScouting_StoreScoutingDTO.Creator.Username,
                DisplayName = StoreScouting_StoreScoutingDTO.Creator.DisplayName,
                Address = StoreScouting_StoreScoutingDTO.Creator.Address,
                Email = StoreScouting_StoreScoutingDTO.Creator.Email,
                Phone = StoreScouting_StoreScoutingDTO.Creator.Phone,
                PositionId = StoreScouting_StoreScoutingDTO.Creator.PositionId,
                Department = StoreScouting_StoreScoutingDTO.Creator.Department,
                OrganizationId = StoreScouting_StoreScoutingDTO.Creator.OrganizationId,
                StatusId = StoreScouting_StoreScoutingDTO.Creator.StatusId,
                Avatar = StoreScouting_StoreScoutingDTO.Creator.Avatar,
                ProvinceId = StoreScouting_StoreScoutingDTO.Creator.ProvinceId,
                SexId = StoreScouting_StoreScoutingDTO.Creator.SexId,
                Birthday = StoreScouting_StoreScoutingDTO.Creator.Birthday,
            };
            StoreScouting.District = StoreScouting_StoreScoutingDTO.District == null ? null : new District
            {
                Id = StoreScouting_StoreScoutingDTO.District.Id,
                Code = StoreScouting_StoreScoutingDTO.District.Code,
                Name = StoreScouting_StoreScoutingDTO.District.Name,
                Priority = StoreScouting_StoreScoutingDTO.District.Priority,
                ProvinceId = StoreScouting_StoreScoutingDTO.District.ProvinceId,
                StatusId = StoreScouting_StoreScoutingDTO.District.StatusId,
            };
            StoreScouting.Organization = StoreScouting_StoreScoutingDTO.Organization == null ? null : new Organization
            {
                Id = StoreScouting_StoreScoutingDTO.Organization.Id,
                Code = StoreScouting_StoreScoutingDTO.Organization.Code,
                Name = StoreScouting_StoreScoutingDTO.Organization.Name,
                ParentId = StoreScouting_StoreScoutingDTO.Organization.ParentId,
                Path = StoreScouting_StoreScoutingDTO.Organization.Path,
                Level = StoreScouting_StoreScoutingDTO.Organization.Level,
                StatusId = StoreScouting_StoreScoutingDTO.Organization.StatusId,
                Phone = StoreScouting_StoreScoutingDTO.Organization.Phone,
                Email = StoreScouting_StoreScoutingDTO.Organization.Email,
                Address = StoreScouting_StoreScoutingDTO.Organization.Address,
            };
            StoreScouting.Province = StoreScouting_StoreScoutingDTO.Province == null ? null : new Province
            {
                Id = StoreScouting_StoreScoutingDTO.Province.Id,
                Code = StoreScouting_StoreScoutingDTO.Province.Code,
                Name = StoreScouting_StoreScoutingDTO.Province.Name,
                Priority = StoreScouting_StoreScoutingDTO.Province.Priority,
                StatusId = StoreScouting_StoreScoutingDTO.Province.StatusId,
            };
            StoreScouting.Store = StoreScouting_StoreScoutingDTO.Store == null ? null : new Store
            {
                Id = StoreScouting_StoreScoutingDTO.Store.Id,
                Code = StoreScouting_StoreScoutingDTO.Store.Code,
                Name = StoreScouting_StoreScoutingDTO.Store.Name,
                ParentStoreId = StoreScouting_StoreScoutingDTO.Store.ParentStoreId,
                OrganizationId = StoreScouting_StoreScoutingDTO.Store.OrganizationId,
                StoreTypeId = StoreScouting_StoreScoutingDTO.Store.StoreTypeId,
                StoreGroupingId = StoreScouting_StoreScoutingDTO.Store.StoreGroupingId,
                ResellerId = StoreScouting_StoreScoutingDTO.Store.ResellerId,
                Telephone = StoreScouting_StoreScoutingDTO.Store.Telephone,
                ProvinceId = StoreScouting_StoreScoutingDTO.Store.ProvinceId,
                DistrictId = StoreScouting_StoreScoutingDTO.Store.DistrictId,
                WardId = StoreScouting_StoreScoutingDTO.Store.WardId,
                Address = StoreScouting_StoreScoutingDTO.Store.Address,
                DeliveryAddress = StoreScouting_StoreScoutingDTO.Store.DeliveryAddress,
                Latitude = StoreScouting_StoreScoutingDTO.Store.Latitude,
                Longitude = StoreScouting_StoreScoutingDTO.Store.Longitude,
                DeliveryLatitude = StoreScouting_StoreScoutingDTO.Store.DeliveryLatitude,
                DeliveryLongitude = StoreScouting_StoreScoutingDTO.Store.DeliveryLongitude,
                OwnerName = StoreScouting_StoreScoutingDTO.Store.OwnerName,
                OwnerPhone = StoreScouting_StoreScoutingDTO.Store.OwnerPhone,
                OwnerEmail = StoreScouting_StoreScoutingDTO.Store.OwnerEmail,
                TaxCode = StoreScouting_StoreScoutingDTO.Store.TaxCode,
                LegalEntity = StoreScouting_StoreScoutingDTO.Store.LegalEntity,
                StatusId = StoreScouting_StoreScoutingDTO.Store.StatusId,
                Used = StoreScouting_StoreScoutingDTO.Store.Used,
            };
            StoreScouting.StoreScoutingStatus = StoreScouting_StoreScoutingDTO.StoreScoutingStatus == null ? null : new StoreScoutingStatus
            {
                Id = StoreScouting_StoreScoutingDTO.StoreScoutingStatus.Id,
                Code = StoreScouting_StoreScoutingDTO.StoreScoutingStatus.Code,
                Name = StoreScouting_StoreScoutingDTO.StoreScoutingStatus.Name,
            };
            StoreScouting.Ward = StoreScouting_StoreScoutingDTO.Ward == null ? null : new Ward
            {
                Id = StoreScouting_StoreScoutingDTO.Ward.Id,
                Code = StoreScouting_StoreScoutingDTO.Ward.Code,
                Name = StoreScouting_StoreScoutingDTO.Ward.Name,
                Priority = StoreScouting_StoreScoutingDTO.Ward.Priority,
                DistrictId = StoreScouting_StoreScoutingDTO.Ward.DistrictId,
                StatusId = StoreScouting_StoreScoutingDTO.Ward.StatusId,
            };
            StoreScouting.BaseLanguage = CurrentContext.Language;
            return StoreScouting;
        }

        private StoreScoutingFilter ConvertFilterDTOToFilterEntity(StoreScouting_StoreScoutingFilterDTO StoreScouting_StoreScoutingFilterDTO)
        {
            StoreScoutingFilter StoreScoutingFilter = new StoreScoutingFilter();
            StoreScoutingFilter.Selects = StoreScoutingSelect.ALL;
            StoreScoutingFilter.Skip = StoreScouting_StoreScoutingFilterDTO.Skip;
            StoreScoutingFilter.Take = StoreScouting_StoreScoutingFilterDTO.Take;
            StoreScoutingFilter.OrderBy = StoreScouting_StoreScoutingFilterDTO.OrderBy;
            StoreScoutingFilter.OrderType = StoreScouting_StoreScoutingFilterDTO.OrderType;

            StoreScoutingFilter.Id = StoreScouting_StoreScoutingFilterDTO.Id;
            StoreScoutingFilter.Code = StoreScouting_StoreScoutingFilterDTO.Code;
            StoreScoutingFilter.Name = StoreScouting_StoreScoutingFilterDTO.Name;
            StoreScoutingFilter.OwnerPhone = StoreScouting_StoreScoutingFilterDTO.OwnerPhone;
            StoreScoutingFilter.ProvinceId = StoreScouting_StoreScoutingFilterDTO.ProvinceId;
            StoreScoutingFilter.DistrictId = StoreScouting_StoreScoutingFilterDTO.DistrictId;
            StoreScoutingFilter.WardId = StoreScouting_StoreScoutingFilterDTO.WardId;
            StoreScoutingFilter.OrganizationId = StoreScouting_StoreScoutingFilterDTO.OrganizationId;
            StoreScoutingFilter.Address = StoreScouting_StoreScoutingFilterDTO.Address;
            StoreScoutingFilter.Latitude = StoreScouting_StoreScoutingFilterDTO.Latitude;
            StoreScoutingFilter.Longitude = StoreScouting_StoreScoutingFilterDTO.Longitude;
            StoreScoutingFilter.StoreId = StoreScouting_StoreScoutingFilterDTO.StoreId;
            StoreScoutingFilter.CreatorId = StoreScouting_StoreScoutingFilterDTO.CreatorId;
            StoreScoutingFilter.StoreScoutingStatusId = StoreScouting_StoreScoutingFilterDTO.StoreScoutingStatusId;
            StoreScoutingFilter.CreatedAt = StoreScouting_StoreScoutingFilterDTO.CreatedAt;
            StoreScoutingFilter.UpdatedAt = StoreScouting_StoreScoutingFilterDTO.UpdatedAt;
            return StoreScoutingFilter;
        }

        [Route(StoreScoutingRoute.FilterListAppUser), HttpPost]
        public async Task<List<StoreScouting_AppUserDTO>> FilterListAppUser([FromBody] StoreScouting_AppUserFilterDTO StoreScouting_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = StoreScouting_AppUserFilterDTO.Id;
            AppUserFilter.Username = StoreScouting_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = StoreScouting_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = StoreScouting_AppUserFilterDTO.Address;
            AppUserFilter.Email = StoreScouting_AppUserFilterDTO.Email;
            AppUserFilter.Phone = StoreScouting_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = StoreScouting_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = StoreScouting_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = StoreScouting_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = StoreScouting_AppUserFilterDTO.StatusId;
            AppUserFilter.ProvinceId = StoreScouting_AppUserFilterDTO.ProvinceId;
            AppUserFilter.SexId = StoreScouting_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = StoreScouting_AppUserFilterDTO.Birthday;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<StoreScouting_AppUserDTO> StoreScouting_AppUserDTOs = AppUsers
                .Select(x => new StoreScouting_AppUserDTO(x)).ToList();
            return StoreScouting_AppUserDTOs;
        }
        [Route(StoreScoutingRoute.FilterListDistrict), HttpPost]
        public async Task<List<StoreScouting_DistrictDTO>> FilterListDistrict([FromBody] StoreScouting_DistrictFilterDTO StoreScouting_DistrictFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Id;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = StoreScouting_DistrictFilterDTO.Id;
            DistrictFilter.Code = StoreScouting_DistrictFilterDTO.Code;
            DistrictFilter.Name = StoreScouting_DistrictFilterDTO.Name;
            DistrictFilter.Priority = StoreScouting_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = StoreScouting_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = StoreScouting_DistrictFilterDTO.StatusId;

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<StoreScouting_DistrictDTO> StoreScouting_DistrictDTOs = Districts
                .Select(x => new StoreScouting_DistrictDTO(x)).ToList();
            return StoreScouting_DistrictDTOs;
        }
        [Route(StoreScoutingRoute.FilterListOrganization), HttpPost]
        public async Task<List<StoreScouting_OrganizationDTO>> FilterListOrganization([FromBody] StoreScouting_OrganizationFilterDTO StoreScouting_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = StoreScouting_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = StoreScouting_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = StoreScouting_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = StoreScouting_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = StoreScouting_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = StoreScouting_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = StoreScouting_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = StoreScouting_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = StoreScouting_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = StoreScouting_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<StoreScouting_OrganizationDTO> StoreScouting_OrganizationDTOs = Organizations
                .Select(x => new StoreScouting_OrganizationDTO(x)).ToList();
            return StoreScouting_OrganizationDTOs;
        }
        [Route(StoreScoutingRoute.FilterListProvince), HttpPost]
        public async Task<List<StoreScouting_ProvinceDTO>> FilterListProvince([FromBody] StoreScouting_ProvinceFilterDTO StoreScouting_ProvinceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Id;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = StoreScouting_ProvinceFilterDTO.Id;
            ProvinceFilter.Code = StoreScouting_ProvinceFilterDTO.Code;
            ProvinceFilter.Name = StoreScouting_ProvinceFilterDTO.Name;
            ProvinceFilter.Priority = StoreScouting_ProvinceFilterDTO.Priority;
            ProvinceFilter.StatusId = StoreScouting_ProvinceFilterDTO.StatusId;

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<StoreScouting_ProvinceDTO> StoreScouting_ProvinceDTOs = Provinces
                .Select(x => new StoreScouting_ProvinceDTO(x)).ToList();
            return StoreScouting_ProvinceDTOs;
        }
        [Route(StoreScoutingRoute.FilterListStore), HttpPost]
        public async Task<List<StoreScouting_StoreDTO>> FilterListStore([FromBody] StoreScouting_StoreFilterDTO StoreScouting_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = StoreScouting_StoreFilterDTO.Id;
            StoreFilter.Code = StoreScouting_StoreFilterDTO.Code;
            StoreFilter.Name = StoreScouting_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = StoreScouting_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = StoreScouting_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = StoreScouting_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = StoreScouting_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = StoreScouting_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = StoreScouting_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = StoreScouting_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = StoreScouting_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = StoreScouting_StoreFilterDTO.WardId;
            StoreFilter.Address = StoreScouting_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = StoreScouting_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreScouting_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreScouting_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = StoreScouting_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = StoreScouting_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = StoreScouting_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreScouting_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreScouting_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = StoreScouting_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<StoreScouting_StoreDTO> StoreScouting_StoreDTOs = Stores
                .Select(x => new StoreScouting_StoreDTO(x)).ToList();
            return StoreScouting_StoreDTOs;
        }
        [Route(StoreScoutingRoute.FilterListStoreScoutingStatus), HttpPost]
        public async Task<List<StoreScouting_StoreScoutingStatusDTO>> FilterListStoreScoutingStatus([FromBody] StoreScouting_StoreScoutingStatusFilterDTO StoreScouting_StoreScoutingStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScoutingStatusFilter StoreScoutingStatusFilter = new StoreScoutingStatusFilter();
            StoreScoutingStatusFilter.Skip = 0;
            StoreScoutingStatusFilter.Take = 20;
            StoreScoutingStatusFilter.OrderBy = StoreScoutingStatusOrder.Id;
            StoreScoutingStatusFilter.OrderType = OrderType.ASC;
            StoreScoutingStatusFilter.Selects = StoreScoutingStatusSelect.ALL;
            StoreScoutingStatusFilter.Id = StoreScouting_StoreScoutingStatusFilterDTO.Id;
            StoreScoutingStatusFilter.Code = StoreScouting_StoreScoutingStatusFilterDTO.Code;
            StoreScoutingStatusFilter.Name = StoreScouting_StoreScoutingStatusFilterDTO.Name;

            List<StoreScoutingStatus> StoreScoutingStatuses = await StoreScoutingStatusService.List(StoreScoutingStatusFilter);
            List<StoreScouting_StoreScoutingStatusDTO> StoreScouting_StoreScoutingStatusDTOs = StoreScoutingStatuses
                .Select(x => new StoreScouting_StoreScoutingStatusDTO(x)).ToList();
            return StoreScouting_StoreScoutingStatusDTOs;
        }
        [Route(StoreScoutingRoute.FilterListWard), HttpPost]
        public async Task<List<StoreScouting_WardDTO>> FilterListWard([FromBody] StoreScouting_WardFilterDTO StoreScouting_WardFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Id;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = StoreScouting_WardFilterDTO.Id;
            WardFilter.Code = StoreScouting_WardFilterDTO.Code;
            WardFilter.Name = StoreScouting_WardFilterDTO.Name;
            WardFilter.Priority = StoreScouting_WardFilterDTO.Priority;
            WardFilter.DistrictId = StoreScouting_WardFilterDTO.DistrictId;
            WardFilter.StatusId = StoreScouting_WardFilterDTO.StatusId;

            List<Ward> Wards = await WardService.List(WardFilter);
            List<StoreScouting_WardDTO> StoreScouting_WardDTOs = Wards
                .Select(x => new StoreScouting_WardDTO(x)).ToList();
            return StoreScouting_WardDTOs;
        }

        [Route(StoreScoutingRoute.SingleListAppUser), HttpPost]
        public async Task<List<StoreScouting_AppUserDTO>> SingleListAppUser([FromBody] StoreScouting_AppUserFilterDTO StoreScouting_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = StoreScouting_AppUserFilterDTO.Id;
            AppUserFilter.Username = StoreScouting_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = StoreScouting_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = StoreScouting_AppUserFilterDTO.Address;
            AppUserFilter.Email = StoreScouting_AppUserFilterDTO.Email;
            AppUserFilter.Phone = StoreScouting_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = StoreScouting_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = StoreScouting_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = StoreScouting_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = StoreScouting_AppUserFilterDTO.StatusId;
            AppUserFilter.ProvinceId = StoreScouting_AppUserFilterDTO.ProvinceId;
            AppUserFilter.SexId = StoreScouting_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = StoreScouting_AppUserFilterDTO.Birthday;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<StoreScouting_AppUserDTO> StoreScouting_AppUserDTOs = AppUsers
                .Select(x => new StoreScouting_AppUserDTO(x)).ToList();
            return StoreScouting_AppUserDTOs;
        }
        [Route(StoreScoutingRoute.SingleListDistrict), HttpPost]
        public async Task<List<StoreScouting_DistrictDTO>> SingleListDistrict([FromBody] StoreScouting_DistrictFilterDTO StoreScouting_DistrictFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Id;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = StoreScouting_DistrictFilterDTO.Id;
            DistrictFilter.Code = StoreScouting_DistrictFilterDTO.Code;
            DistrictFilter.Name = StoreScouting_DistrictFilterDTO.Name;
            DistrictFilter.Priority = StoreScouting_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = StoreScouting_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = StoreScouting_DistrictFilterDTO.StatusId;

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<StoreScouting_DistrictDTO> StoreScouting_DistrictDTOs = Districts
                .Select(x => new StoreScouting_DistrictDTO(x)).ToList();
            return StoreScouting_DistrictDTOs;
        }
        [Route(StoreScoutingRoute.SingleListOrganization), HttpPost]
        public async Task<List<StoreScouting_OrganizationDTO>> SingleListOrganization([FromBody] StoreScouting_OrganizationFilterDTO StoreScouting_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = StoreScouting_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = StoreScouting_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = StoreScouting_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = StoreScouting_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = StoreScouting_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = StoreScouting_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = StoreScouting_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = StoreScouting_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = StoreScouting_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = StoreScouting_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<StoreScouting_OrganizationDTO> StoreScouting_OrganizationDTOs = Organizations
                .Select(x => new StoreScouting_OrganizationDTO(x)).ToList();
            return StoreScouting_OrganizationDTOs;
        }
        [Route(StoreScoutingRoute.SingleListProvince), HttpPost]
        public async Task<List<StoreScouting_ProvinceDTO>> SingleListProvince([FromBody] StoreScouting_ProvinceFilterDTO StoreScouting_ProvinceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Id;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = StoreScouting_ProvinceFilterDTO.Id;
            ProvinceFilter.Code = StoreScouting_ProvinceFilterDTO.Code;
            ProvinceFilter.Name = StoreScouting_ProvinceFilterDTO.Name;
            ProvinceFilter.Priority = StoreScouting_ProvinceFilterDTO.Priority;
            ProvinceFilter.StatusId = StoreScouting_ProvinceFilterDTO.StatusId;

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<StoreScouting_ProvinceDTO> StoreScouting_ProvinceDTOs = Provinces
                .Select(x => new StoreScouting_ProvinceDTO(x)).ToList();
            return StoreScouting_ProvinceDTOs;
        }
        [Route(StoreScoutingRoute.SingleListStore), HttpPost]
        public async Task<List<StoreScouting_StoreDTO>> SingleListStore([FromBody] StoreScouting_StoreFilterDTO StoreScouting_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = StoreScouting_StoreFilterDTO.Id;
            StoreFilter.Code = StoreScouting_StoreFilterDTO.Code;
            StoreFilter.Name = StoreScouting_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = StoreScouting_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = StoreScouting_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = StoreScouting_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = StoreScouting_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = StoreScouting_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = StoreScouting_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = StoreScouting_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = StoreScouting_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = StoreScouting_StoreFilterDTO.WardId;
            StoreFilter.Address = StoreScouting_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = StoreScouting_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreScouting_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreScouting_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = StoreScouting_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = StoreScouting_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = StoreScouting_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreScouting_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreScouting_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = StoreScouting_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<StoreScouting_StoreDTO> StoreScouting_StoreDTOs = Stores
                .Select(x => new StoreScouting_StoreDTO(x)).ToList();
            return StoreScouting_StoreDTOs;
        }
        [Route(StoreScoutingRoute.SingleListStoreScoutingStatus), HttpPost]
        public async Task<List<StoreScouting_StoreScoutingStatusDTO>> SingleListStoreScoutingStatus([FromBody] StoreScouting_StoreScoutingStatusFilterDTO StoreScouting_StoreScoutingStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScoutingStatusFilter StoreScoutingStatusFilter = new StoreScoutingStatusFilter();
            StoreScoutingStatusFilter.Skip = 0;
            StoreScoutingStatusFilter.Take = 20;
            StoreScoutingStatusFilter.OrderBy = StoreScoutingStatusOrder.Id;
            StoreScoutingStatusFilter.OrderType = OrderType.ASC;
            StoreScoutingStatusFilter.Selects = StoreScoutingStatusSelect.ALL;
            StoreScoutingStatusFilter.Id = StoreScouting_StoreScoutingStatusFilterDTO.Id;
            StoreScoutingStatusFilter.Code = StoreScouting_StoreScoutingStatusFilterDTO.Code;
            StoreScoutingStatusFilter.Name = StoreScouting_StoreScoutingStatusFilterDTO.Name;

            List<StoreScoutingStatus> StoreScoutingStatuses = await StoreScoutingStatusService.List(StoreScoutingStatusFilter);
            List<StoreScouting_StoreScoutingStatusDTO> StoreScouting_StoreScoutingStatusDTOs = StoreScoutingStatuses
                .Select(x => new StoreScouting_StoreScoutingStatusDTO(x)).ToList();
            return StoreScouting_StoreScoutingStatusDTOs;
        }
        [Route(StoreScoutingRoute.SingleListWard), HttpPost]
        public async Task<List<StoreScouting_WardDTO>> SingleListWard([FromBody] StoreScouting_WardFilterDTO StoreScouting_WardFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Id;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = StoreScouting_WardFilterDTO.Id;
            WardFilter.Code = StoreScouting_WardFilterDTO.Code;
            WardFilter.Name = StoreScouting_WardFilterDTO.Name;
            WardFilter.Priority = StoreScouting_WardFilterDTO.Priority;
            WardFilter.DistrictId = StoreScouting_WardFilterDTO.DistrictId;
            WardFilter.StatusId = StoreScouting_WardFilterDTO.StatusId;

            List<Ward> Wards = await WardService.List(WardFilter);
            List<StoreScouting_WardDTO> StoreScouting_WardDTOs = Wards
                .Select(x => new StoreScouting_WardDTO(x)).ToList();
            return StoreScouting_WardDTOs;
        }

    }
}

