﻿using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MDistrict;
using DMS.Services.MOrganization;
using DMS.Services.MProvince;
using DMS.Services.MReseller;
using DMS.Services.MStatus;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreScouting;
using DMS.Services.MStoreType;
using DMS.Services.MWard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.store
{
    public class StoreController : RpcController
    {
        private IDistrictService DistrictService;
        private IOrganizationService OrganizationService;
        private IProvinceService ProvinceService;
        private IResellerService ResellerService;
        private IStatusService StatusService;
        private IStoreScoutingService StoreScoutingService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private IWardService WardService;
        private IStoreService StoreService;
        private ICurrentContext CurrentContext;
        public StoreController(
            IDistrictService DistrictService,
            IOrganizationService OrganizationService,
            IProvinceService ProvinceService,
            IResellerService ResellerService,
            IStatusService StatusService,
            IStoreScoutingService StoreScoutingService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            IWardService WardService,
            IStoreService StoreService,
            ICurrentContext CurrentContext
        )
        {
            this.DistrictService = DistrictService;
            this.OrganizationService = OrganizationService;
            this.ProvinceService = ProvinceService;
            this.ResellerService = ResellerService;
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
        public async Task<ActionResult<Store_StoreDTO>> Get([FromBody]Store_StoreDTO Store_StoreDTO)
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
                DistrictId = StoreScouting.DistrictId,
                Latitude = StoreScouting.Latitude,
                Longitude = StoreScouting.Longitude,
                Name = StoreScouting.Name,
                OrganizationId = StoreScouting.OrganizationId.HasValue ? StoreScouting.OrganizationId.Value : 0,
                OwnerPhone = StoreScouting.OwnerPhone,
                ProvinceId = StoreScouting.ProvinceId,
                StatusId = StatusEnum.INACTIVE.Id,
                WardId = StoreScouting.WardId,
                District = StoreScouting.District == null ? null : new Store_DistrictDTO(StoreScouting.District),
                Organization = StoreScouting.Organization == null ? null : new Store_OrganizationDTO(StoreScouting.Organization),
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
            Store = await StoreService.Approve(Store);
            Store_StoreDTO = new Store_StoreDTO(Store);
            if (Store.IsValidated)
                return Store_StoreDTO;
            else
                return BadRequest(Store_StoreDTO);
        }

        [Route(StoreRoute.Reject), HttpPost]
        public async Task<ActionResult<Store_StoreDTO>> Reject([FromBody] Store_StoreDTO Store_StoreDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Store_StoreDTO.Id))
                return Forbid();

            Store Store = ConvertDTOToEntity(Store_StoreDTO);
            Store = await StoreService.Reject(Store);
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

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };
            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Id | OrganizationSelect.Code
            });

            List<Reseller> Resellers = await ResellerService.List(new ResellerFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ResellerSelect.Id | ResellerSelect.Code
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
                Selects = StatusSelect.Id | StatusSelect.Code
            });

            List<Store> Stores = new List<Store>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Store"];
                if (worksheet == null)
                    return null;
                int StartColumn = 1;
                int StartRow = 1;

                int CodeColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;

                int ParentStoreCodeColumn = 2 + StartColumn;
                int OrganizationCodeColumn = 3 + StartColumn;
                int StoreTypeCodeColumn = 4 + StartColumn;
                int StoreGroupingCodeColumn = 5 + StartColumn;

                int TelephoneColumn = 6 + StartColumn;
                int ResellerCodeColumn = 7 + StartColumn;

                int ProvinceCodeColumn = 8 + StartColumn;
                int DistrictCodeColumn = 9 + StartColumn;
                int WardCodeColumn = 10 + StartColumn;

                int AddressColumn = 11 + StartColumn;
                int DeliveryAddressColumn = 12 + StartColumn;

                int LatitudeColumn = 13 + StartColumn;
                int LongitudeColumn = 14 + StartColumn;
                int DeliveryLatitudeColumn = 15 + StartColumn;
                int DeliveryLongitudeColumn = 16 + StartColumn;

                int OwnerNameColumn = 17 + StartColumn;
                int OwnerPhoneColumn = 18 + StartColumn;
                int OwnerEmailColumn = 19 + StartColumn;

                int StatusCodeColumn = 20 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    // Lấy thông tin từng dòng
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string ParentStoreCodeValue = worksheet.Cells[i + StartRow, ParentStoreCodeColumn].Value?.ToString();
                    string OrganizationCodeValue = worksheet.Cells[i + StartRow, OrganizationCodeColumn].Value?.ToString();
                    string StoreTypeCodeValue = worksheet.Cells[i + StartRow, StoreTypeCodeColumn].Value?.ToString();
                    string StoreGroupingCodeValue = worksheet.Cells[i + StartRow, StoreGroupingCodeColumn].Value?.ToString();
                    string TelephoneValue = worksheet.Cells[i + StartRow, TelephoneColumn].Value?.ToString();
                    string ResellerCodeValue = worksheet.Cells[i + StartRow, ResellerCodeColumn].Value?.ToString();
                    string ProvinceCodeValue = worksheet.Cells[i + StartRow, ProvinceCodeColumn].Value?.ToString();
                    string DistrictCodeValue = worksheet.Cells[i + StartRow, DistrictCodeColumn].Value?.ToString();
                    string WardCodeValue = worksheet.Cells[i + StartRow, WardCodeColumn].Value?.ToString();
                    string AddressValue = worksheet.Cells[i + StartRow, AddressColumn].Value?.ToString();
                    string DeliveryAddressValue = worksheet.Cells[i + StartRow, DeliveryAddressColumn].Value?.ToString();
                    string LatitudeValue = worksheet.Cells[i + StartRow, LatitudeColumn].Value?.ToString();
                    string LongitudeValue = worksheet.Cells[i + StartRow, LongitudeColumn].Value?.ToString();
                    string DeliveryLatitudeValue = worksheet.Cells[i + StartRow, DeliveryLatitudeColumn].Value?.ToString();
                    string DeliveryLongitudeValue = worksheet.Cells[i + StartRow, DeliveryLongitudeColumn].Value?.ToString();
                    string OwnerNameValue = worksheet.Cells[i + StartRow, OwnerNameColumn].Value?.ToString();
                    string OwnerPhoneValue = worksheet.Cells[i + StartRow, OwnerPhoneColumn].Value?.ToString();
                    string OwnerEmailValue = worksheet.Cells[i + StartRow, OwnerEmailColumn].Value?.ToString();
                    string StatusCodeValue = worksheet.Cells[i + StartRow, StatusCodeColumn].Value?.ToString();
                    if (string.IsNullOrEmpty(CodeValue))
                        continue;

                    Store Store = new Store();
                    Store.Code = CodeValue;
                    Store.Name = NameValue;
                    Store.Telephone = TelephoneValue;
                    Store.Address = AddressValue;
                    Store.DeliveryAddress = DeliveryAddressValue;
                    Store.Latitude = decimal.TryParse(LatitudeValue, out decimal Latitude) ? Latitude : 0;
                    Store.Longitude = decimal.TryParse(LongitudeValue, out decimal Longitude) ? Longitude : 0;
                    Store.DeliveryLatitude = decimal.TryParse(DeliveryLatitudeValue, out decimal DeliveryLatitude) ? DeliveryLatitude : 0;
                    Store.DeliveryLongitude = decimal.TryParse(DeliveryLongitudeValue, out decimal DeliveryLongitude) ? DeliveryLongitude : 0;
                    Store.OwnerName = OwnerNameValue;
                    Store.OwnerPhone = OwnerPhoneValue;
                    Store.OwnerEmail = OwnerEmailValue;

                    if (!string.IsNullOrEmpty(ParentStoreCodeValue))
                    {
                        Store.ParentStore = new Store
                        {
                            Code = ParentStoreCodeValue
                        };
                    }
                    Store.Organization = Organizations.Where(x => x.Code == OrganizationCodeValue).FirstOrDefault() ?? new Organization { Code = OrganizationCodeValue };
                    Store.Reseller = Resellers.Where(x => x.Code == ResellerCodeValue).FirstOrDefault() ?? new Reseller { Code = ResellerCodeValue };
                    Store.StoreType = StoreTypes.Where(x => x.Code == StoreTypeCodeValue).FirstOrDefault() ?? new StoreType { Code = StoreTypeCodeValue };
                    Store.StoreGrouping = StoreGroupings.Where(x => x.Code == StoreGroupingCodeValue).FirstOrDefault() ?? new StoreGrouping { Code = StoreGroupingCodeValue };
                    Store.Province = Provinces.Where(x => x.Code == ProvinceCodeValue).FirstOrDefault() ?? new Province { Code = ProvinceCodeValue };
                    Store.District = Districts.Where(x => x.Code == DistrictCodeValue).FirstOrDefault() ?? new District { Code = DistrictCodeValue };
                    Store.Ward = Wards.Where(x => x.Code == WardCodeValue).FirstOrDefault() ?? new Ward { Code = WardCodeValue };
                    Store.Status = Statuses.Where(x => x.Code == StatusCodeValue).FirstOrDefault();
                    Stores.Add(Store);
                }
            }

            Stores = await StoreService.Import(Stores);
            List<Store_StoreDTO> Store_StoreDTOs = Stores
                .Select(c => new Store_StoreDTO(c)).ToList();
            if (Stores.Any(s => !s.IsValidated))
                return BadRequest(Store_StoreDTOs);
            return Ok(Store_StoreDTOs);
        }

        [Route(StoreRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Store_StoreFilterDTO Store_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            List<Store> ParentStores = await StoreService.List(new StoreFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreSelect.Code | StoreSelect.Name
            });

            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Code | OrganizationSelect.Name | OrganizationSelect.Path
            });

            List<Reseller> Resellers = await ResellerService.List(new ResellerFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ResellerSelect.Id | ResellerSelect.Code
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
                        "Mã Cửa Hàng",
                        "Tên Cửa Hàng",
                        "Đơn Vị Quản Lý",
                        "Cửa Hàng Cha",
                        "Câp Cửa Hàng" ,
                        "Nhóm Cửa Hàng",
                        "Mã Khách Hàng",
                        "Tỉnh/Thành phố",
                        "Quận/Huyện",
                        "Phường/Xã",
                        "Địa Chỉ Cửa Hàng",
                        "Địa Chỉ Giao Hàng",
                        "Kinh Độ",
                        "Vĩ Độ",
                        "Kinh Độ Giao Hàng",
                        "Vĩ Độ Giao Hàng",
                        "Điện Thoại",
                        "Tên Chủ Cửa Hàng",
                        "Điện Thoại Liên Hệ",
                        "Email"
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
                        Store.Name,
                        Store.Organization == null ? null : Store.Organization.Code,
                        Store.ParentStore == null ? null : Store.ParentStore.Code,
                        Store.StoreType == null ? null : Store.StoreType.Code,
                        Store.StoreGrouping== null ? null : Store.StoreGrouping.Code,
                        Store.Reseller== null ? null : Store.Reseller.Code,
                        Store.Province == null ? null : Store.Province.Code,
                        Store.District == null ? null : Store.District.Code,
                        Store.Ward.Code == null ? null : Store.Ward.Code,
                        Store.Address,
                        Store.DeliveryAddress,
                        Store.Latitude,
                        Store.Longitude,
                        Store.DeliveryLatitude,
                        Store.DeliveryLongitude,
                        Store.Telephone,
                        Store.OwnerName,
                        Store.OwnerPhone,
                        Store.OwnerEmail
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

                #region ParentStore
                var ParentStoreHeaders = new List<string[]>()
                {
                    new string[]
                    {
                        "Mã",
                        "Tên",
                    }
                };
                data = new List<object[]>();
                for (int i = 0; i < ParentStores.Count; i++)
                {
                    var ParentStore = ParentStores[i];
                    data.Add(new Object[]
                    {
                        ParentStore.Code,
                        ParentStore.Name,
                    });
                }
                excel.GenerateWorksheet("StoreParent", ParentStoreHeaders, data);
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

                #region Reseller
                var ResellerHeaders = new List<string[]>()
                {
                    new string[]
                    {
                        "Mã",
                        "Tên",
                    }
                };
                data = new List<object[]>();
                for (int i = 0; i < Resellers.Count; i++)
                {
                    var Reseller = Resellers[i];
                    data.Add(new Object[]
                    {
                        Reseller.Code,
                        Reseller.Name,
                    });
                }
                excel.GenerateWorksheet("Reseller", ResellerHeaders, data);
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
                    });
                }
                excel.GenerateWorksheet("Ward", WardHeaders, data);
                #endregion
                excel.Save();
            }

            return File(memoryStream.ToArray(), "application/octet-stream", "Store.xlsx");
        }

        [Route(StoreRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            MemoryStream MemoryStream = new MemoryStream();
            string tempPath = "Templates/Store_Template.xlsx";
            using (var xlPackage = new ExcelPackage(new FileInfo(tempPath)))
            {
                var nameexcel = "Export cửa hàng" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                xlPackage.Workbook.Properties.Title = string.Format("{0}", nameexcel);
                xlPackage.Workbook.Properties.Author = "Sonhx5";
                xlPackage.Workbook.Properties.Subject = string.Format("{0}", "RD-DMS");
                xlPackage.Workbook.Properties.Category = "RD-DMS";
                xlPackage.Workbook.Properties.Company = "FPT-FIS-ERP-ESC";
                xlPackage.SaveAs(MemoryStream);
            }

            return File(MemoryStream.ToArray(), "application/octet-stream", "Store" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
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
                Content = memoryStream.ToArray()
            };
            Image = await StoreService.SaveImage(Image);
            if (Image == null)
                return BadRequest();
            Store_ImageDTO Store_ImageDTO = new Store_ImageDTO
            {
                Id = Image.Id,
                Name = Image.Name,
                Url = Image.Url,
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
            Store.Name = Store_StoreDTO.Name;
            Store.ParentStoreId = Store_StoreDTO.ParentStoreId;
            Store.OrganizationId = Store_StoreDTO.OrganizationId;
            Store.StoreTypeId = Store_StoreDTO.StoreTypeId;
            Store.StoreGroupingId = Store_StoreDTO.StoreGroupingId;
            Store.Telephone = Store_StoreDTO.Telephone;
            Store.ResellerId = Store_StoreDTO.ResellerId;
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
                ResellerId = Store_StoreDTO.ParentStore.ResellerId,
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
                OrganizationId = Store_StoreDTO.StoreScouting.OrganizationId,
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
            StoreFilter.Name = Store_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Store_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = Store_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = Store_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Store_StoreFilterDTO.StoreGroupingId;
            StoreFilter.Telephone = Store_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = Store_StoreFilterDTO.ResellerId;
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
            StoreFilter.StatusId = Store_StoreFilterDTO.StatusId;
            return StoreFilter;
        }

        [Route(StoreRoute.FilterListDistrict), HttpPost]
        public async Task<List<Store_DistrictDTO>> FilterListDistrict([FromBody] Store_DistrictFilterDTO Store_DistrictFilterDTO)
        {
            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Priority;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = Store_DistrictFilterDTO.Id;
            DistrictFilter.Name = Store_DistrictFilterDTO.Name;
            DistrictFilter.Priority = Store_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = Store_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = Store_DistrictFilterDTO.StatusId;

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<Store_DistrictDTO> Store_DistrictDTOs = Districts
                .Select(x => new Store_DistrictDTO(x)).ToList();
            return Store_DistrictDTOs;
        }

        [Route(StoreRoute.FilterListOrganization), HttpPost]
        public async Task<List<Store_OrganizationDTO>> FilterListOrganization([FromBody] Store_OrganizationFilterDTO Store_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            if (OrganizationFilter.OrFilter == null) OrganizationFilter.OrFilter = new List<OrganizationFilter>();
            if (CurrentContext.Filters != null)
            {
                foreach (var currentFilter in CurrentContext.Filters)
                {
                    OrganizationFilter subFilter = new OrganizationFilter();
                    OrganizationFilter.OrFilter.Add(subFilter);
                    List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                    foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                    {
                        if (FilterPermissionDefinition.Name == nameof(StoreFilter.OrganizationId))
                            subFilter.Id = FilterPermissionDefinition.IdFilter;
                    }
                }
            }
            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<Store_OrganizationDTO> Store_OrganizationDTOs = Organizations
                .Select(x => new Store_OrganizationDTO(x)).ToList();
            return Store_OrganizationDTOs;
        }

        [Route(StoreRoute.FilterListProvince), HttpPost]
        public async Task<List<Store_ProvinceDTO>> FilterListProvince([FromBody] Store_ProvinceFilterDTO Store_ProvinceFilterDTO)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Priority;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = Store_ProvinceFilterDTO.Id;
            ProvinceFilter.Name = Store_ProvinceFilterDTO.Name;
            ProvinceFilter.StatusId = Store_ProvinceFilterDTO.StatusId;

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<Store_ProvinceDTO> Store_ProvinceDTOs = Provinces
                .Select(x => new Store_ProvinceDTO(x)).ToList();
            return Store_ProvinceDTOs;
        }

        [Route(StoreRoute.FilterListParentStore), HttpPost]
        public async Task<List<Store_StoreDTO>> FilterListParentStore([FromBody] Store_StoreFilterDTO Store_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Store_StoreFilterDTO.Id;
            StoreFilter.Code = Store_StoreFilterDTO.Code;
            StoreFilter.Name = Store_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Store_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = Store_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = Store_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Store_StoreFilterDTO.StoreGroupingId;
            StoreFilter.Telephone = Store_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = Store_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = Store_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Store_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Store_StoreFilterDTO.WardId;
            StoreFilter.Address = Store_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Store_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Store_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Store_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = Store_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Store_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Store_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = Store_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<Store_StoreDTO> Store_StoreDTOs = Stores
                .Select(x => new Store_StoreDTO(x)).ToList();
            return Store_StoreDTOs;
        }

        [Route(StoreRoute.FilterListWard), HttpPost]
        public async Task<List<Store_WardDTO>> FilterListWard([FromBody] Store_WardFilterDTO Store_WardFilterDTO)
        {
            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Priority;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = Store_WardFilterDTO.Id;
            WardFilter.Name = Store_WardFilterDTO.Name;
            WardFilter.DistrictId = Store_WardFilterDTO.DistrictId;
            WardFilter.StatusId = Store_WardFilterDTO.StatusId;
            List<Ward> Wards = await WardService.List(WardFilter);
            List<Store_WardDTO> Store_WardDTOs = Wards
                .Select(x => new Store_WardDTO(x)).ToList();
            return Store_WardDTOs;
        }
        [Route(StoreRoute.FilterListStatus), HttpPost]
        public async Task<List<Store_StatusDTO>> FilterListStatus([FromBody] Store_StatusFilterDTO Store_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Store_StatusDTO> Store_StatusDTOs = Statuses
                .Select(x => new Store_StatusDTO(x)).ToList();
            return Store_StatusDTOs;
        }

        [Route(StoreRoute.SingleListDistrict), HttpPost]
        public async Task<List<Store_DistrictDTO>> SingleListDistrict([FromBody] Store_DistrictFilterDTO Store_DistrictFilterDTO)
        {
            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Priority;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = Store_DistrictFilterDTO.Id;
            DistrictFilter.Name = Store_DistrictFilterDTO.Name;
            DistrictFilter.Priority = Store_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = Store_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<Store_DistrictDTO> Store_DistrictDTOs = Districts
                .Select(x => new Store_DistrictDTO(x)).ToList();
            return Store_DistrictDTOs;
        }
        [Route(StoreRoute.SingleListOrganization), HttpPost]
        public async Task<List<Store_OrganizationDTO>> SingleListOrganization([FromBody] Store_OrganizationFilterDTO Store_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            if (OrganizationFilter.OrFilter == null) OrganizationFilter.OrFilter = new List<OrganizationFilter>();
            if (CurrentContext.Filters != null)
            {
                foreach (var currentFilter in CurrentContext.Filters)
                {
                    OrganizationFilter subFilter = new OrganizationFilter();
                    OrganizationFilter.OrFilter.Add(subFilter);
                    List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                    foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                    {
                        if (FilterPermissionDefinition.Name == nameof(StoreFilter.OrganizationId))
                            subFilter.Id = FilterPermissionDefinition.IdFilter;
                    }
                }
            }
            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<Store_OrganizationDTO> Store_OrganizationDTOs = Organizations
                .Select(x => new Store_OrganizationDTO(x)).ToList();
            return Store_OrganizationDTOs;
        }
        [Route(StoreRoute.SingleListParentStore), HttpPost]
        public async Task<List<Store_StoreDTO>> SingleListParentStore([FromBody] Store_StoreFilterDTO Store_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Store_StoreFilterDTO.Id;
            StoreFilter.Code = Store_StoreFilterDTO.Code;
            StoreFilter.Name = Store_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Store_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = Store_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = Store_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Store_StoreFilterDTO.StoreGroupingId;
            StoreFilter.Telephone = Store_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = Store_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = Store_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Store_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Store_StoreFilterDTO.WardId;
            StoreFilter.Address = Store_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Store_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Store_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Store_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = Store_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Store_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Store_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<Store_StoreDTO> Store_StoreDTOs = Stores
                .Select(x => new Store_StoreDTO(x)).ToList();
            return Store_StoreDTOs;
        }
        [Route(StoreRoute.SingleListProvince), HttpPost]
        public async Task<List<Store_ProvinceDTO>> SingleListProvince([FromBody] Store_ProvinceFilterDTO Store_ProvinceFilterDTO)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Priority;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = Store_ProvinceFilterDTO.Id;
            ProvinceFilter.Name = Store_ProvinceFilterDTO.Name;
            ProvinceFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<Store_ProvinceDTO> Store_ProvinceDTOs = Provinces
                .Select(x => new Store_ProvinceDTO(x)).ToList();
            return Store_ProvinceDTOs;
        }
        [Route(StoreRoute.SingleListStatus), HttpPost]
        public async Task<List<Store_StatusDTO>> SingleListStatus([FromBody] Store_StatusFilterDTO Store_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Store_StatusDTO> Store_StatusDTOs = Statuses
                .Select(x => new Store_StatusDTO(x)).ToList();
            return Store_StatusDTOs;
        }
        [Route(StoreRoute.SingleListStoreGrouping), HttpPost]
        public async Task<List<Store_StoreGroupingDTO>> SingleListStoreGrouping([FromBody] Store_StoreGroupingFilterDTO Store_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Code = Store_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = Store_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.Level = Store_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.Path = Store_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<Store_StoreGroupingDTO> Store_StoreGroupingDTOs = StoreGroupings
                .Select(x => new Store_StoreGroupingDTO(x)).ToList();
            return Store_StoreGroupingDTOs;
        }
        [Route(StoreRoute.SingleListStoreType), HttpPost]
        public async Task<List<Store_StoreTypeDTO>> SingleListStoreType([FromBody] Store_StoreTypeFilterDTO Store_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = Store_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = Store_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = Store_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<Store_StoreTypeDTO> Store_StoreTypeDTOs = StoreTypes
                .Select(x => new Store_StoreTypeDTO(x)).ToList();
            return Store_StoreTypeDTOs;
        }
        [Route(StoreRoute.SingleListWard), HttpPost]
        public async Task<List<Store_WardDTO>> SingleListWard([FromBody] Store_WardFilterDTO Store_WardFilterDTO)
        {
            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Priority;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = Store_WardFilterDTO.Id;
            WardFilter.Name = Store_WardFilterDTO.Name;
            WardFilter.DistrictId = Store_WardFilterDTO.DistrictId;
            WardFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            List<Ward> Wards = await WardService.List(WardFilter);
            List<Store_WardDTO> Store_WardDTOs = Wards
                .Select(x => new Store_WardDTO(x)).ToList();
            return Store_WardDTOs;
        }

        [Route(StoreRoute.CountReseller), HttpPost]
        public async Task<long> CountReseller([FromBody] Store_ResellerFilterDTO Store_ResellerFilterDTO)
        {
            ResellerFilter ResellerFilter = new ResellerFilter();
            ResellerFilter.Id = Store_ResellerFilterDTO.Id;
            ResellerFilter.Code = Store_ResellerFilterDTO.Code;
            ResellerFilter.Name = Store_ResellerFilterDTO.Name;
            ResellerFilter.OrganizationId = Store_ResellerFilterDTO.OrganizationId;
            ResellerFilter.ResellerTypeId = Store_ResellerFilterDTO.ResellerTypeId;
            ResellerFilter.ResellerStatusId = Store_ResellerFilterDTO.ResellerStatusId;
            ResellerFilter.Phone = Store_ResellerFilterDTO.Phone;
            ResellerFilter.Email = Store_ResellerFilterDTO.Email;
            ResellerFilter.CompanyName = Store_ResellerFilterDTO.CompanyName;
            ResellerFilter.DeputyName = Store_ResellerFilterDTO.DeputyName;
            ResellerFilter.Address = Store_ResellerFilterDTO.Address;
            ResellerFilter.Description = Store_ResellerFilterDTO.Description;
            ResellerFilter.StaffId = Store_ResellerFilterDTO.StaffId;
            ResellerFilter.TaxCode = Store_ResellerFilterDTO.TaxCode;
            ResellerFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            return await ResellerService.Count(ResellerFilter);
        }

        [Route(StoreRoute.ListReseller), HttpPost]
        public async Task<List<Store_ResellerDTO>> ListReseller([FromBody] Store_ResellerFilterDTO Store_ResellerFilterDTO)
        {
            ResellerFilter ResellerFilter = new ResellerFilter();
            ResellerFilter.Skip = Store_ResellerFilterDTO.Skip;
            ResellerFilter.Take = Store_ResellerFilterDTO.Take;
            ResellerFilter.OrderBy = ResellerOrder.Id;
            ResellerFilter.OrderType = OrderType.ASC;
            ResellerFilter.Selects = ResellerSelect.ALL;
            ResellerFilter.Id = Store_ResellerFilterDTO.Id;
            ResellerFilter.Code = Store_ResellerFilterDTO.Code;
            ResellerFilter.Name = Store_ResellerFilterDTO.Name;
            ResellerFilter.OrganizationId = Store_ResellerFilterDTO.OrganizationId;
            ResellerFilter.ResellerTypeId = Store_ResellerFilterDTO.ResellerTypeId;
            ResellerFilter.ResellerStatusId = Store_ResellerFilterDTO.ResellerStatusId;
            ResellerFilter.Phone = Store_ResellerFilterDTO.Phone;
            ResellerFilter.Email = Store_ResellerFilterDTO.Email;
            ResellerFilter.CompanyName = Store_ResellerFilterDTO.CompanyName;
            ResellerFilter.DeputyName = Store_ResellerFilterDTO.DeputyName;
            ResellerFilter.Address = Store_ResellerFilterDTO.Address;
            ResellerFilter.Description = Store_ResellerFilterDTO.Description;
            ResellerFilter.StaffId = Store_ResellerFilterDTO.StaffId;
            ResellerFilter.TaxCode = Store_ResellerFilterDTO.TaxCode;
            ResellerFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Reseller> Resellers = await ResellerService.List(ResellerFilter);
            List<Store_ResellerDTO> Store_ResellerDTOs = Resellers
                .Select(x => new Store_ResellerDTO(x)).ToList();
            return Store_ResellerDTOs;
        }

    }
}

