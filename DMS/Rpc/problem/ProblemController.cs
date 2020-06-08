using Common;
using DMS.Entities;
using DMS.Services.MAppUser;
using DMS.Services.MImage;
using DMS.Services.MProblem;
using DMS.Services.MProblemStatus;
using DMS.Services.MProblemType;
using DMS.Services.MStore;
using DMS.Services.MStoreChecking;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.problem
{
    public class ProblemController : RpcController
    {
        private IAppUserService AppUserService;
        private IProblemStatusService ProblemStatusService;
        private IProblemTypeService ProblemTypeService;
        private IStoreService StoreService;
        private IStoreCheckingService StoreCheckingService;
        private IImageService ImageService;
        private IProblemService ProblemService;
        private ICurrentContext CurrentContext;
        public ProblemController(
            IAppUserService AppUserService,
            IProblemStatusService ProblemStatusService,
            IProblemTypeService ProblemTypeService,
            IStoreService StoreService,
            IStoreCheckingService StoreCheckingService,
            IImageService ImageService,
            IProblemService ProblemService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.ProblemStatusService = ProblemStatusService;
            this.ProblemTypeService = ProblemTypeService;
            this.StoreService = StoreService;
            this.StoreCheckingService = StoreCheckingService;
            this.ImageService = ImageService;
            this.ProblemService = ProblemService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ProblemRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Problem_ProblemFilterDTO Problem_ProblemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProblemFilter ProblemFilter = ConvertFilterDTOToFilterEntity(Problem_ProblemFilterDTO);
            ProblemFilter = ProblemService.ToFilter(ProblemFilter);
            int count = await ProblemService.Count(ProblemFilter);
            return count;
        }

        [Route(ProblemRoute.List), HttpPost]
        public async Task<ActionResult<List<Problem_ProblemDTO>>> List([FromBody] Problem_ProblemFilterDTO Problem_ProblemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProblemFilter ProblemFilter = ConvertFilterDTOToFilterEntity(Problem_ProblemFilterDTO);
            ProblemFilter = ProblemService.ToFilter(ProblemFilter);
            List<Problem> Problems = await ProblemService.List(ProblemFilter);
            List<Problem_ProblemDTO> Problem_ProblemDTOs = Problems
                .Select(c => new Problem_ProblemDTO(c)).ToList();
            return Problem_ProblemDTOs;
        }

        [Route(ProblemRoute.Get), HttpPost]
        public async Task<ActionResult<Problem_ProblemDTO>> Get([FromBody]Problem_ProblemDTO Problem_ProblemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Problem_ProblemDTO.Id))
                return Forbid();

            Problem Problem = await ProblemService.Get(Problem_ProblemDTO.Id);
            return new Problem_ProblemDTO(Problem);
        }

        [Route(ProblemRoute.Create), HttpPost]
        public async Task<ActionResult<Problem_ProblemDTO>> Create([FromBody] Problem_ProblemDTO Problem_ProblemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Problem_ProblemDTO.Id))
                return Forbid();

            Problem Problem = ConvertDTOToEntity(Problem_ProblemDTO);
            Problem = await ProblemService.Create(Problem);
            Problem_ProblemDTO = new Problem_ProblemDTO(Problem);
            if (Problem.IsValidated)
                return Problem_ProblemDTO;
            else
                return BadRequest(Problem_ProblemDTO);
        }

        [Route(ProblemRoute.Update), HttpPost]
        public async Task<ActionResult<Problem_ProblemDTO>> Update([FromBody] Problem_ProblemDTO Problem_ProblemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Problem_ProblemDTO.Id))
                return Forbid();

            Problem Problem = ConvertDTOToEntity(Problem_ProblemDTO);
            Problem = await ProblemService.Update(Problem);
            Problem_ProblemDTO = new Problem_ProblemDTO(Problem);
            if (Problem.IsValidated)
                return Problem_ProblemDTO;
            else
                return BadRequest(Problem_ProblemDTO);
        }

        [Route(ProblemRoute.Delete), HttpPost]
        public async Task<ActionResult<Problem_ProblemDTO>> Delete([FromBody] Problem_ProblemDTO Problem_ProblemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Problem_ProblemDTO.Id))
                return Forbid();

            Problem Problem = ConvertDTOToEntity(Problem_ProblemDTO);
            Problem = await ProblemService.Delete(Problem);
            Problem_ProblemDTO = new Problem_ProblemDTO(Problem);
            if (Problem.IsValidated)
                return Problem_ProblemDTO;
            else
                return BadRequest(Problem_ProblemDTO);
        }

        [Route(ProblemRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProblemFilter ProblemFilter = new ProblemFilter();
            ProblemFilter = ProblemService.ToFilter(ProblemFilter);
            ProblemFilter.Id = new IdFilter { In = Ids };
            ProblemFilter.Selects = ProblemSelect.Id;
            ProblemFilter.Skip = 0;
            ProblemFilter.Take = int.MaxValue;

            List<Problem> Problems = await ProblemService.List(ProblemFilter);
            Problems = await ProblemService.BulkDelete(Problems);
            return true;
        }

        [Route(ProblemRoute.Import), HttpPost]
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
            ProblemStatusFilter ProblemStatusFilter = new ProblemStatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProblemStatusSelect.ALL
            };
            List<ProblemStatus> ProblemStatuses = await ProblemStatusService.List(ProblemStatusFilter);
            ProblemTypeFilter ProblemTypeFilter = new ProblemTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProblemTypeSelect.ALL
            };
            List<ProblemType> ProblemTypes = await ProblemTypeService.List(ProblemTypeFilter);
            StoreFilter StoreFilter = new StoreFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreSelect.ALL
            };
            List<Store> Stores = await StoreService.List(StoreFilter);
            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreCheckingSelect.ALL
            };
            List<StoreChecking> StoreCheckings = await StoreCheckingService.List(StoreCheckingFilter);
            List<Problem> Problems = new List<Problem>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Problems);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int StoreCheckingIdColumn = 1 + StartColumn;
                int StoreIdColumn = 2 + StartColumn;
                int CreatorIdColumn = 3 + StartColumn;
                int ProblemTypeIdColumn = 4 + StartColumn;
                int NoteAtColumn = 5 + StartColumn;
                int CompletedAtColumn = 6 + StartColumn;
                int ContentColumn = 7 + StartColumn;
                int ProblemStatusIdColumn = 8 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string StoreCheckingIdValue = worksheet.Cells[i + StartRow, StoreCheckingIdColumn].Value?.ToString();
                    string StoreIdValue = worksheet.Cells[i + StartRow, StoreIdColumn].Value?.ToString();
                    string CreatorIdValue = worksheet.Cells[i + StartRow, CreatorIdColumn].Value?.ToString();
                    string ProblemTypeIdValue = worksheet.Cells[i + StartRow, ProblemTypeIdColumn].Value?.ToString();
                    string NoteAtValue = worksheet.Cells[i + StartRow, NoteAtColumn].Value?.ToString();
                    string CompletedAtValue = worksheet.Cells[i + StartRow, CompletedAtColumn].Value?.ToString();
                    string ContentValue = worksheet.Cells[i + StartRow, ContentColumn].Value?.ToString();
                    string ProblemStatusIdValue = worksheet.Cells[i + StartRow, ProblemStatusIdColumn].Value?.ToString();

                    Problem Problem = new Problem();
                    Problem.NoteAt = DateTime.TryParse(NoteAtValue, out DateTime NoteAt) ? NoteAt : DateTime.Now;
                    Problem.CompletedAt = DateTime.TryParse(CompletedAtValue, out DateTime CompletedAt) ? CompletedAt : DateTime.Now;
                    Problem.Content = ContentValue;
                    AppUser Creator = Creators.Where(x => x.Id.ToString() == CreatorIdValue).FirstOrDefault();
                    Problem.CreatorId = Creator == null ? 0 : Creator.Id;
                    Problem.Creator = Creator;
                    ProblemStatus ProblemStatus = ProblemStatuses.Where(x => x.Id.ToString() == ProblemStatusIdValue).FirstOrDefault();
                    Problem.ProblemStatusId = ProblemStatus == null ? 0 : ProblemStatus.Id;
                    Problem.ProblemStatus = ProblemStatus;
                    ProblemType ProblemType = ProblemTypes.Where(x => x.Id.ToString() == ProblemTypeIdValue).FirstOrDefault();
                    Problem.ProblemTypeId = ProblemType == null ? 0 : ProblemType.Id;
                    Problem.ProblemType = ProblemType;
                    Store Store = Stores.Where(x => x.Id.ToString() == StoreIdValue).FirstOrDefault();
                    Problem.StoreId = Store == null ? 0 : Store.Id;
                    Problem.Store = Store;
                    StoreChecking StoreChecking = StoreCheckings.Where(x => x.Id.ToString() == StoreCheckingIdValue).FirstOrDefault();
                    Problem.StoreCheckingId = StoreChecking == null ? 0 : StoreChecking.Id;
                    Problem.StoreChecking = StoreChecking;

                    Problems.Add(Problem);
                }
            }
            Problems = await ProblemService.Import(Problems);
            if (Problems.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Problems.Count; i++)
                {
                    Problem Problem = Problems[i];
                    if (!Problem.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Problem.Errors.ContainsKey(nameof(Problem.Id)))
                            Error += Problem.Errors[nameof(Problem.Id)];
                        if (Problem.Errors.ContainsKey(nameof(Problem.StoreCheckingId)))
                            Error += Problem.Errors[nameof(Problem.StoreCheckingId)];
                        if (Problem.Errors.ContainsKey(nameof(Problem.StoreId)))
                            Error += Problem.Errors[nameof(Problem.StoreId)];
                        if (Problem.Errors.ContainsKey(nameof(Problem.CreatorId)))
                            Error += Problem.Errors[nameof(Problem.CreatorId)];
                        if (Problem.Errors.ContainsKey(nameof(Problem.ProblemTypeId)))
                            Error += Problem.Errors[nameof(Problem.ProblemTypeId)];
                        if (Problem.Errors.ContainsKey(nameof(Problem.NoteAt)))
                            Error += Problem.Errors[nameof(Problem.NoteAt)];
                        if (Problem.Errors.ContainsKey(nameof(Problem.CompletedAt)))
                            Error += Problem.Errors[nameof(Problem.CompletedAt)];
                        if (Problem.Errors.ContainsKey(nameof(Problem.Content)))
                            Error += Problem.Errors[nameof(Problem.Content)];
                        if (Problem.Errors.ContainsKey(nameof(Problem.ProblemStatusId)))
                            Error += Problem.Errors[nameof(Problem.ProblemStatusId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }

        [Route(ProblemRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] Problem_ProblemFilterDTO Problem_ProblemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Problem
                var ProblemFilter = ConvertFilterDTOToFilterEntity(Problem_ProblemFilterDTO);
                ProblemFilter.Skip = 0;
                ProblemFilter.Take = int.MaxValue;
                ProblemFilter = ProblemService.ToFilter(ProblemFilter);
                List<Problem> Problems = await ProblemService.List(ProblemFilter);

                var ProblemHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "StoreCheckingId",
                        "StoreId",
                        "CreatorId",
                        "ProblemTypeId",
                        "NoteAt",
                        "CompletedAt",
                        "Content",
                        "ProblemStatusId",
                    }
                };
                List<object[]> ProblemData = new List<object[]>();
                for (int i = 0; i < Problems.Count; i++)
                {
                    var Problem = Problems[i];
                    ProblemData.Add(new Object[]
                    {
                        Problem.Id,
                        Problem.StoreCheckingId,
                        Problem.StoreId,
                        Problem.CreatorId,
                        Problem.ProblemTypeId,
                        Problem.NoteAt,
                        Problem.CompletedAt,
                        Problem.Content,
                        Problem.ProblemStatusId,
                    });
                }
                excel.GenerateWorksheet("Problem", ProblemHeaders, ProblemData);
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
                #region ProblemStatus
                var ProblemStatusFilter = new ProblemStatusFilter();
                ProblemStatusFilter.Selects = ProblemStatusSelect.ALL;
                ProblemStatusFilter.OrderBy = ProblemStatusOrder.Id;
                ProblemStatusFilter.OrderType = OrderType.ASC;
                ProblemStatusFilter.Skip = 0;
                ProblemStatusFilter.Take = int.MaxValue;
                List<ProblemStatus> ProblemStatuses = await ProblemStatusService.List(ProblemStatusFilter);

                var ProblemStatusHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> ProblemStatusData = new List<object[]>();
                for (int i = 0; i < ProblemStatuses.Count; i++)
                {
                    var ProblemStatus = ProblemStatuses[i];
                    ProblemStatusData.Add(new Object[]
                    {
                        ProblemStatus.Id,
                        ProblemStatus.Code,
                        ProblemStatus.Name,
                    });
                }
                excel.GenerateWorksheet("ProblemStatus", ProblemStatusHeaders, ProblemStatusData);
                #endregion
                #region ProblemType
                var ProblemTypeFilter = new ProblemTypeFilter();
                ProblemTypeFilter.Selects = ProblemTypeSelect.ALL;
                ProblemTypeFilter.OrderBy = ProblemTypeOrder.Id;
                ProblemTypeFilter.OrderType = OrderType.ASC;
                ProblemTypeFilter.Skip = 0;
                ProblemTypeFilter.Take = int.MaxValue;
                List<ProblemType> ProblemTypes = await ProblemTypeService.List(ProblemTypeFilter);

                var ProblemTypeHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> ProblemTypeData = new List<object[]>();
                for (int i = 0; i < ProblemTypes.Count; i++)
                {
                    var ProblemType = ProblemTypes[i];
                    ProblemTypeData.Add(new Object[]
                    {
                        ProblemType.Id,
                        ProblemType.Code,
                        ProblemType.Name,
                    });
                }
                excel.GenerateWorksheet("ProblemType", ProblemTypeHeaders, ProblemTypeData);
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
                    });
                }
                excel.GenerateWorksheet("Store", StoreHeaders, StoreData);
                #endregion
                #region StoreChecking
                var StoreCheckingFilter = new StoreCheckingFilter();
                StoreCheckingFilter.Selects = StoreCheckingSelect.ALL;
                StoreCheckingFilter.OrderBy = StoreCheckingOrder.Id;
                StoreCheckingFilter.OrderType = OrderType.ASC;
                StoreCheckingFilter.Skip = 0;
                StoreCheckingFilter.Take = int.MaxValue;
                List<StoreChecking> StoreCheckings = await StoreCheckingService.List(StoreCheckingFilter);

                var StoreCheckingHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "StoreId",
                        "SaleEmployeeId",
                        "Longtitude",
                        "Latitude",
                        "CheckInAt",
                        "CheckOutAt",
                        "IndirectSalesOrderCounter",
                        "ImageCounter",
                    }
                };
                List<object[]> StoreCheckingData = new List<object[]>();
                for (int i = 0; i < StoreCheckings.Count; i++)
                {
                    var StoreChecking = StoreCheckings[i];
                    StoreCheckingData.Add(new Object[]
                    {
                        StoreChecking.Id,
                        StoreChecking.StoreId,
                        StoreChecking.SaleEmployeeId,
                        StoreChecking.Longtitude,
                        StoreChecking.Latitude,
                        StoreChecking.CheckInAt,
                        StoreChecking.CheckOutAt,
                    });
                }
                excel.GenerateWorksheet("StoreChecking", StoreCheckingHeaders, StoreCheckingData);
                #endregion
                #region Image
                var ImageFilter = new ImageFilter();
                ImageFilter.Selects = ImageSelect.ALL;
                ImageFilter.OrderBy = ImageOrder.Id;
                ImageFilter.OrderType = OrderType.ASC;
                ImageFilter.Skip = 0;
                ImageFilter.Take = int.MaxValue;
                List<Image> Images = await ImageService.List(ImageFilter);

                var ImageHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Name",
                        "Url",
                    }
                };
                List<object[]> ImageData = new List<object[]>();
                for (int i = 0; i < Images.Count; i++)
                {
                    var Image = Images[i];
                    ImageData.Add(new Object[]
                    {
                        Image.Id,
                        Image.Name,
                        Image.Url,
                    });
                }
                excel.GenerateWorksheet("Image", ImageHeaders, ImageData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Problem.xlsx");
        }

        [Route(ProblemRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] Problem_ProblemFilterDTO Problem_ProblemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Problem
                var ProblemHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "StoreCheckingId",
                        "StoreId",
                        "CreatorId",
                        "ProblemTypeId",
                        "NoteAt",
                        "CompletedAt",
                        "Content",
                        "ProblemStatusId",
                    }
                };
                List<object[]> ProblemData = new List<object[]>();
                excel.GenerateWorksheet("Problem", ProblemHeaders, ProblemData);
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
                #region ProblemStatus
                var ProblemStatusFilter = new ProblemStatusFilter();
                ProblemStatusFilter.Selects = ProblemStatusSelect.ALL;
                ProblemStatusFilter.OrderBy = ProblemStatusOrder.Id;
                ProblemStatusFilter.OrderType = OrderType.ASC;
                ProblemStatusFilter.Skip = 0;
                ProblemStatusFilter.Take = int.MaxValue;
                List<ProblemStatus> ProblemStatuses = await ProblemStatusService.List(ProblemStatusFilter);

                var ProblemStatusHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> ProblemStatusData = new List<object[]>();
                for (int i = 0; i < ProblemStatuses.Count; i++)
                {
                    var ProblemStatus = ProblemStatuses[i];
                    ProblemStatusData.Add(new Object[]
                    {
                        ProblemStatus.Id,
                        ProblemStatus.Code,
                        ProblemStatus.Name,
                    });
                }
                excel.GenerateWorksheet("ProblemStatus", ProblemStatusHeaders, ProblemStatusData);
                #endregion
                #region ProblemType
                var ProblemTypeFilter = new ProblemTypeFilter();
                ProblemTypeFilter.Selects = ProblemTypeSelect.ALL;
                ProblemTypeFilter.OrderBy = ProblemTypeOrder.Id;
                ProblemTypeFilter.OrderType = OrderType.ASC;
                ProblemTypeFilter.Skip = 0;
                ProblemTypeFilter.Take = int.MaxValue;
                List<ProblemType> ProblemTypes = await ProblemTypeService.List(ProblemTypeFilter);

                var ProblemTypeHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> ProblemTypeData = new List<object[]>();
                for (int i = 0; i < ProblemTypes.Count; i++)
                {
                    var ProblemType = ProblemTypes[i];
                    ProblemTypeData.Add(new Object[]
                    {
                        ProblemType.Id,
                        ProblemType.Code,
                        ProblemType.Name,
                    });
                }
                excel.GenerateWorksheet("ProblemType", ProblemTypeHeaders, ProblemTypeData);
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
                    });
                }
                excel.GenerateWorksheet("Store", StoreHeaders, StoreData);
                #endregion
                #region StoreChecking
                var StoreCheckingFilter = new StoreCheckingFilter();
                StoreCheckingFilter.Selects = StoreCheckingSelect.ALL;
                StoreCheckingFilter.OrderBy = StoreCheckingOrder.Id;
                StoreCheckingFilter.OrderType = OrderType.ASC;
                StoreCheckingFilter.Skip = 0;
                StoreCheckingFilter.Take = int.MaxValue;
                List<StoreChecking> StoreCheckings = await StoreCheckingService.List(StoreCheckingFilter);

                var StoreCheckingHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "StoreId",
                        "SaleEmployeeId",
                        "Longtitude",
                        "Latitude",
                        "CheckInAt",
                        "CheckOutAt",
                        "IndirectSalesOrderCounter",
                        "ImageCounter",
                    }
                };
                List<object[]> StoreCheckingData = new List<object[]>();
                for (int i = 0; i < StoreCheckings.Count; i++)
                {
                    var StoreChecking = StoreCheckings[i];
                    StoreCheckingData.Add(new Object[]
                    {
                        StoreChecking.Id,
                        StoreChecking.StoreId,
                        StoreChecking.SaleEmployeeId,
                        StoreChecking.Longtitude,
                        StoreChecking.Latitude,
                        StoreChecking.CheckInAt,
                        StoreChecking.CheckOutAt,
                    });
                }
                excel.GenerateWorksheet("StoreChecking", StoreCheckingHeaders, StoreCheckingData);
                #endregion
                #region Image
                var ImageFilter = new ImageFilter();
                ImageFilter.Selects = ImageSelect.ALL;
                ImageFilter.OrderBy = ImageOrder.Id;
                ImageFilter.OrderType = OrderType.ASC;
                ImageFilter.Skip = 0;
                ImageFilter.Take = int.MaxValue;
                List<Image> Images = await ImageService.List(ImageFilter);

                var ImageHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Name",
                        "Url",
                    }
                };
                List<object[]> ImageData = new List<object[]>();
                for (int i = 0; i < Images.Count; i++)
                {
                    var Image = Images[i];
                    ImageData.Add(new Object[]
                    {
                        Image.Id,
                        Image.Name,
                        Image.Url,
                    });
                }
                excel.GenerateWorksheet("Image", ImageHeaders, ImageData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Problem.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ProblemFilter ProblemFilter = new ProblemFilter();
            ProblemFilter = ProblemService.ToFilter(ProblemFilter);
            if (Id == 0)
            {

            }
            else
            {
                ProblemFilter.Id = new IdFilter { Equal = Id };
                int count = await ProblemService.Count(ProblemFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Problem ConvertDTOToEntity(Problem_ProblemDTO Problem_ProblemDTO)
        {
            Problem Problem = new Problem();
            Problem.Id = Problem_ProblemDTO.Id;
            Problem.StoreCheckingId = Problem_ProblemDTO.StoreCheckingId;
            Problem.StoreId = Problem_ProblemDTO.StoreId;
            Problem.CreatorId = Problem_ProblemDTO.CreatorId;
            Problem.ProblemTypeId = Problem_ProblemDTO.ProblemTypeId;
            Problem.NoteAt = Problem_ProblemDTO.NoteAt;
            Problem.CompletedAt = Problem_ProblemDTO.CompletedAt;
            Problem.Content = Problem_ProblemDTO.Content;
            Problem.ProblemStatusId = Problem_ProblemDTO.ProblemStatusId;
            Problem.Creator = Problem_ProblemDTO.Creator == null ? null : new AppUser
            {
                Id = Problem_ProblemDTO.Creator.Id,
                Username = Problem_ProblemDTO.Creator.Username,
                DisplayName = Problem_ProblemDTO.Creator.DisplayName,
                Address = Problem_ProblemDTO.Creator.Address,
                Email = Problem_ProblemDTO.Creator.Email,
                Phone = Problem_ProblemDTO.Creator.Phone,
                PositionId = Problem_ProblemDTO.Creator.PositionId,
                Department = Problem_ProblemDTO.Creator.Department,
                OrganizationId = Problem_ProblemDTO.Creator.OrganizationId,
                StatusId = Problem_ProblemDTO.Creator.StatusId,
                Avatar = Problem_ProblemDTO.Creator.Avatar,
                ProvinceId = Problem_ProblemDTO.Creator.ProvinceId,
                SexId = Problem_ProblemDTO.Creator.SexId,
                Birthday = Problem_ProblemDTO.Creator.Birthday,
            };
            Problem.ProblemStatus = Problem_ProblemDTO.ProblemStatus == null ? null : new ProblemStatus
            {
                Id = Problem_ProblemDTO.ProblemStatus.Id,
                Code = Problem_ProblemDTO.ProblemStatus.Code,
                Name = Problem_ProblemDTO.ProblemStatus.Name,
            };
            Problem.ProblemType = Problem_ProblemDTO.ProblemType == null ? null : new ProblemType
            {
                Id = Problem_ProblemDTO.ProblemType.Id,
                Code = Problem_ProblemDTO.ProblemType.Code,
                Name = Problem_ProblemDTO.ProblemType.Name,
            };
            Problem.Store = Problem_ProblemDTO.Store == null ? null : new Store
            {
                Id = Problem_ProblemDTO.Store.Id,
                Code = Problem_ProblemDTO.Store.Code,
                Name = Problem_ProblemDTO.Store.Name,
                ParentStoreId = Problem_ProblemDTO.Store.ParentStoreId,
                OrganizationId = Problem_ProblemDTO.Store.OrganizationId,
                StoreTypeId = Problem_ProblemDTO.Store.StoreTypeId,
                StoreGroupingId = Problem_ProblemDTO.Store.StoreGroupingId,
                ResellerId = Problem_ProblemDTO.Store.ResellerId,
                Telephone = Problem_ProblemDTO.Store.Telephone,
                ProvinceId = Problem_ProblemDTO.Store.ProvinceId,
                DistrictId = Problem_ProblemDTO.Store.DistrictId,
                WardId = Problem_ProblemDTO.Store.WardId,
                Address = Problem_ProblemDTO.Store.Address,
                DeliveryAddress = Problem_ProblemDTO.Store.DeliveryAddress,
                Latitude = Problem_ProblemDTO.Store.Latitude,
                Longitude = Problem_ProblemDTO.Store.Longitude,
                DeliveryLatitude = Problem_ProblemDTO.Store.DeliveryLatitude,
                DeliveryLongitude = Problem_ProblemDTO.Store.DeliveryLongitude,
                OwnerName = Problem_ProblemDTO.Store.OwnerName,
                OwnerPhone = Problem_ProblemDTO.Store.OwnerPhone,
                OwnerEmail = Problem_ProblemDTO.Store.OwnerEmail,
                TaxCode = Problem_ProblemDTO.Store.TaxCode,
                LegalEntity = Problem_ProblemDTO.Store.LegalEntity,
                StatusId = Problem_ProblemDTO.Store.StatusId,
            };
            Problem.StoreChecking = Problem_ProblemDTO.StoreChecking == null ? null : new StoreChecking
            {
                Id = Problem_ProblemDTO.StoreChecking.Id,
                StoreId = Problem_ProblemDTO.StoreChecking.StoreId,
                SaleEmployeeId = Problem_ProblemDTO.StoreChecking.SaleEmployeeId,
                Longtitude = Problem_ProblemDTO.StoreChecking.Longtitude,
                Latitude = Problem_ProblemDTO.StoreChecking.Latitude,
                CheckInAt = Problem_ProblemDTO.StoreChecking.CheckInAt,
                CheckOutAt = Problem_ProblemDTO.StoreChecking.CheckOutAt,
            };
            Problem.ProblemImageMappings = Problem_ProblemDTO.ProblemImageMappings?
                .Select(x => new ProblemImageMapping
                {
                    ImageId = x.ImageId,
                    Image = x.Image == null ? null : new Image
                    {
                        Id = x.Image.Id,
                        Name = x.Image.Name,
                        Url = x.Image.Url,
                    },
                }).ToList();
            Problem.BaseLanguage = CurrentContext.Language;
            return Problem;
        }

        private ProblemFilter ConvertFilterDTOToFilterEntity(Problem_ProblemFilterDTO Problem_ProblemFilterDTO)
        {
            ProblemFilter ProblemFilter = new ProblemFilter();
            ProblemFilter.Selects = ProblemSelect.ALL;
            ProblemFilter.Skip = Problem_ProblemFilterDTO.Skip;
            ProblemFilter.Take = Problem_ProblemFilterDTO.Take;
            ProblemFilter.OrderBy = Problem_ProblemFilterDTO.OrderBy;
            ProblemFilter.OrderType = Problem_ProblemFilterDTO.OrderType;

            ProblemFilter.Id = Problem_ProblemFilterDTO.Id;
            ProblemFilter.StoreCheckingId = Problem_ProblemFilterDTO.StoreCheckingId;
            ProblemFilter.StoreId = Problem_ProblemFilterDTO.StoreId;
            ProblemFilter.CreatorId = Problem_ProblemFilterDTO.CreatorId;
            ProblemFilter.ProblemTypeId = Problem_ProblemFilterDTO.ProblemTypeId;
            ProblemFilter.NoteAt = Problem_ProblemFilterDTO.NoteAt;
            ProblemFilter.CompletedAt = Problem_ProblemFilterDTO.CompletedAt;
            ProblemFilter.Content = Problem_ProblemFilterDTO.Content;
            ProblemFilter.ProblemStatusId = Problem_ProblemFilterDTO.ProblemStatusId;
            return ProblemFilter;
        }

        [Route(ProblemRoute.FilterListAppUser), HttpPost]
        public async Task<List<Problem_AppUserDTO>> FilterListAppUser([FromBody] Problem_AppUserFilterDTO Problem_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Problem_AppUserFilterDTO.Id;
            AppUserFilter.Username = Problem_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = Problem_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = Problem_AppUserFilterDTO.Address;
            AppUserFilter.Email = Problem_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Problem_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = Problem_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = Problem_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = Problem_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = Problem_AppUserFilterDTO.StatusId;
            AppUserFilter.ProvinceId = Problem_AppUserFilterDTO.ProvinceId;
            AppUserFilter.SexId = Problem_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = Problem_AppUserFilterDTO.Birthday;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Problem_AppUserDTO> Problem_AppUserDTOs = AppUsers
                .Select(x => new Problem_AppUserDTO(x)).ToList();
            return Problem_AppUserDTOs;
        }
        [Route(ProblemRoute.FilterListProblemStatus), HttpPost]
        public async Task<List<Problem_ProblemStatusDTO>> FilterListProblemStatus([FromBody] Problem_ProblemStatusFilterDTO Problem_ProblemStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProblemStatusFilter ProblemStatusFilter = new ProblemStatusFilter();
            ProblemStatusFilter.Skip = 0;
            ProblemStatusFilter.Take = 20;
            ProblemStatusFilter.OrderBy = ProblemStatusOrder.Id;
            ProblemStatusFilter.OrderType = OrderType.ASC;
            ProblemStatusFilter.Selects = ProblemStatusSelect.ALL;
            ProblemStatusFilter.Id = Problem_ProblemStatusFilterDTO.Id;
            ProblemStatusFilter.Code = Problem_ProblemStatusFilterDTO.Code;
            ProblemStatusFilter.Name = Problem_ProblemStatusFilterDTO.Name;

            List<ProblemStatus> ProblemStatuses = await ProblemStatusService.List(ProblemStatusFilter);
            List<Problem_ProblemStatusDTO> Problem_ProblemStatusDTOs = ProblemStatuses
                .Select(x => new Problem_ProblemStatusDTO(x)).ToList();
            return Problem_ProblemStatusDTOs;
        }
        [Route(ProblemRoute.FilterListProblemType), HttpPost]
        public async Task<List<Problem_ProblemTypeDTO>> FilterListProblemType([FromBody] Problem_ProblemTypeFilterDTO Problem_ProblemTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProblemTypeFilter ProblemTypeFilter = new ProblemTypeFilter();
            ProblemTypeFilter.Skip = 0;
            ProblemTypeFilter.Take = int.MaxValue;
            ProblemTypeFilter.Take = 20;
            ProblemTypeFilter.OrderBy = ProblemTypeOrder.Id;
            ProblemTypeFilter.OrderType = OrderType.ASC;
            ProblemTypeFilter.Selects = ProblemTypeSelect.ALL;

            List<ProblemType> ProblemTypes = await ProblemTypeService.List(ProblemTypeFilter);
            List<Problem_ProblemTypeDTO> Problem_ProblemTypeDTOs = ProblemTypes
                .Select(x => new Problem_ProblemTypeDTO(x)).ToList();
            return Problem_ProblemTypeDTOs;
        }
        [Route(ProblemRoute.FilterListStore), HttpPost]
        public async Task<List<Problem_StoreDTO>> FilterListStore([FromBody] Problem_StoreFilterDTO Problem_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Problem_StoreFilterDTO.Id;
            StoreFilter.Code = Problem_StoreFilterDTO.Code;
            StoreFilter.Name = Problem_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Problem_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = Problem_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = Problem_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Problem_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = Problem_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = Problem_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = Problem_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Problem_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Problem_StoreFilterDTO.WardId;
            StoreFilter.Address = Problem_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Problem_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Problem_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Problem_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Problem_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Problem_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Problem_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Problem_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Problem_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = Problem_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<Problem_StoreDTO> Problem_StoreDTOs = Stores
                .Select(x => new Problem_StoreDTO(x)).ToList();
            return Problem_StoreDTOs;
        }
        [Route(ProblemRoute.FilterListStoreChecking), HttpPost]
        public async Task<List<Problem_StoreCheckingDTO>> FilterListStoreChecking([FromBody] Problem_StoreCheckingFilterDTO Problem_StoreCheckingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter();
            StoreCheckingFilter.Skip = 0;
            StoreCheckingFilter.Take = 20;
            StoreCheckingFilter.OrderBy = StoreCheckingOrder.Id;
            StoreCheckingFilter.OrderType = OrderType.ASC;
            StoreCheckingFilter.Selects = StoreCheckingSelect.ALL;
            StoreCheckingFilter.Id = Problem_StoreCheckingFilterDTO.Id;
            StoreCheckingFilter.StoreId = Problem_StoreCheckingFilterDTO.StoreId;
            StoreCheckingFilter.SaleEmployeeId = Problem_StoreCheckingFilterDTO.SaleEmployeeId;
            StoreCheckingFilter.Longtitude = Problem_StoreCheckingFilterDTO.Longtitude;
            StoreCheckingFilter.Latitude = Problem_StoreCheckingFilterDTO.Latitude;
            StoreCheckingFilter.CheckInAt = Problem_StoreCheckingFilterDTO.CheckInAt;
            StoreCheckingFilter.CheckOutAt = Problem_StoreCheckingFilterDTO.CheckOutAt;

            List<StoreChecking> StoreCheckings = await StoreCheckingService.List(StoreCheckingFilter);
            List<Problem_StoreCheckingDTO> Problem_StoreCheckingDTOs = StoreCheckings
                .Select(x => new Problem_StoreCheckingDTO(x)).ToList();
            return Problem_StoreCheckingDTOs;
        }

        [Route(ProblemRoute.SingleListAppUser), HttpPost]
        public async Task<List<Problem_AppUserDTO>> SingleListAppUser([FromBody] Problem_AppUserFilterDTO Problem_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Problem_AppUserFilterDTO.Id;
            AppUserFilter.Username = Problem_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = Problem_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = Problem_AppUserFilterDTO.Address;
            AppUserFilter.Email = Problem_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Problem_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = Problem_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = Problem_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = Problem_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = Problem_AppUserFilterDTO.StatusId;
            AppUserFilter.ProvinceId = Problem_AppUserFilterDTO.ProvinceId;
            AppUserFilter.SexId = Problem_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = Problem_AppUserFilterDTO.Birthday;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Problem_AppUserDTO> Problem_AppUserDTOs = AppUsers
                .Select(x => new Problem_AppUserDTO(x)).ToList();
            return Problem_AppUserDTOs;
        }
        [Route(ProblemRoute.SingleListProblemStatus), HttpPost]
        public async Task<List<Problem_ProblemStatusDTO>> SingleListProblemStatus([FromBody] Problem_ProblemStatusFilterDTO Problem_ProblemStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProblemStatusFilter ProblemStatusFilter = new ProblemStatusFilter();
            ProblemStatusFilter.Skip = 0;
            ProblemStatusFilter.Take = 20;
            ProblemStatusFilter.OrderBy = ProblemStatusOrder.Id;
            ProblemStatusFilter.OrderType = OrderType.ASC;
            ProblemStatusFilter.Selects = ProblemStatusSelect.ALL;
            ProblemStatusFilter.Id = Problem_ProblemStatusFilterDTO.Id;
            ProblemStatusFilter.Code = Problem_ProblemStatusFilterDTO.Code;
            ProblemStatusFilter.Name = Problem_ProblemStatusFilterDTO.Name;

            List<ProblemStatus> ProblemStatuses = await ProblemStatusService.List(ProblemStatusFilter);
            List<Problem_ProblemStatusDTO> Problem_ProblemStatusDTOs = ProblemStatuses
                .Select(x => new Problem_ProblemStatusDTO(x)).ToList();
            return Problem_ProblemStatusDTOs;
        }
        [Route(ProblemRoute.SingleListProblemType), HttpPost]
        public async Task<List<Problem_ProblemTypeDTO>> SingleListProblemType([FromBody] Problem_ProblemTypeFilterDTO Problem_ProblemTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProblemTypeFilter ProblemTypeFilter = new ProblemTypeFilter();
            ProblemTypeFilter.Skip = 0;
            ProblemTypeFilter.Take = int.MaxValue;
            ProblemTypeFilter.Take = 20;
            ProblemTypeFilter.OrderBy = ProblemTypeOrder.Id;
            ProblemTypeFilter.OrderType = OrderType.ASC;
            ProblemTypeFilter.Selects = ProblemTypeSelect.ALL;

            List<ProblemType> ProblemTypes = await ProblemTypeService.List(ProblemTypeFilter);
            List<Problem_ProblemTypeDTO> Problem_ProblemTypeDTOs = ProblemTypes
                .Select(x => new Problem_ProblemTypeDTO(x)).ToList();
            return Problem_ProblemTypeDTOs;
        }
        [Route(ProblemRoute.SingleListStore), HttpPost]
        public async Task<List<Problem_StoreDTO>> SingleListStore([FromBody] Problem_StoreFilterDTO Problem_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Problem_StoreFilterDTO.Id;
            StoreFilter.Code = Problem_StoreFilterDTO.Code;
            StoreFilter.Name = Problem_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Problem_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = Problem_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = Problem_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Problem_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = Problem_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = Problem_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = Problem_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Problem_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Problem_StoreFilterDTO.WardId;
            StoreFilter.Address = Problem_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Problem_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Problem_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Problem_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Problem_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Problem_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Problem_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Problem_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Problem_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = Problem_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<Problem_StoreDTO> Problem_StoreDTOs = Stores
                .Select(x => new Problem_StoreDTO(x)).ToList();
            return Problem_StoreDTOs;
        }
        [Route(ProblemRoute.SingleListStoreChecking), HttpPost]
        public async Task<List<Problem_StoreCheckingDTO>> SingleListStoreChecking([FromBody] Problem_StoreCheckingFilterDTO Problem_StoreCheckingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter();
            StoreCheckingFilter.Skip = 0;
            StoreCheckingFilter.Take = 20;
            StoreCheckingFilter.OrderBy = StoreCheckingOrder.Id;
            StoreCheckingFilter.OrderType = OrderType.ASC;
            StoreCheckingFilter.Selects = StoreCheckingSelect.ALL;
            StoreCheckingFilter.Id = Problem_StoreCheckingFilterDTO.Id;
            StoreCheckingFilter.StoreId = Problem_StoreCheckingFilterDTO.StoreId;
            StoreCheckingFilter.SaleEmployeeId = Problem_StoreCheckingFilterDTO.SaleEmployeeId;
            StoreCheckingFilter.Longtitude = Problem_StoreCheckingFilterDTO.Longtitude;
            StoreCheckingFilter.Latitude = Problem_StoreCheckingFilterDTO.Latitude;
            StoreCheckingFilter.CheckInAt = Problem_StoreCheckingFilterDTO.CheckInAt;
            StoreCheckingFilter.CheckOutAt = Problem_StoreCheckingFilterDTO.CheckOutAt;

            List<StoreChecking> StoreCheckings = await StoreCheckingService.List(StoreCheckingFilter);
            List<Problem_StoreCheckingDTO> Problem_StoreCheckingDTOs = StoreCheckings
                .Select(x => new Problem_StoreCheckingDTO(x)).ToList();
            return Problem_StoreCheckingDTOs;
        }
    }
}

