using Common;
using DMS.Entities;
using DMS.Services.MAppUser;
using DMS.Services.MImage;
using DMS.Services.MProblem;
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
            ProblemFilter = await ProblemService.ToFilter(ProblemFilter);
            int count = await ProblemService.Count(ProblemFilter);
            return count;
        }

        [Route(ProblemRoute.List), HttpPost]
        public async Task<ActionResult<List<Problem_ProblemDTO>>> List([FromBody] Problem_ProblemFilterDTO Problem_ProblemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProblemFilter ProblemFilter = ConvertFilterDTOToFilterEntity(Problem_ProblemFilterDTO);
            ProblemFilter = await ProblemService.ToFilter(ProblemFilter);
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

        [HttpPost]
        [Route(ProblemRoute.SaveImage)]
        public async Task<ActionResult<Problem_ImageDTO>> SaveImage(IFormFile file)
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
            Image = await ProblemService.SaveImage(Image);
            if (Image == null)
                return BadRequest();
            Problem_ImageDTO Problem_ImageDTO = new Problem_ImageDTO
            {
                Id = Image.Id,
                Name = Image.Name,
                Url = Image.Url,
            };
            return Ok(Problem_ImageDTO);
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

        private async Task<bool> HasPermission(long Id)
        {
            ProblemFilter ProblemFilter = new ProblemFilter();
            ProblemFilter = await ProblemService.ToFilter(ProblemFilter);
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
            Problem.Code = Problem_ProblemDTO.Code;
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
                Longitude = Problem_ProblemDTO.StoreChecking.Longitude,
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
            ProblemFilter.Code = Problem_ProblemFilterDTO.Code;
            ProblemFilter.StoreCheckingId = Problem_ProblemFilterDTO.StoreCheckingId;
            ProblemFilter.StoreId = Problem_ProblemFilterDTO.StoreId;
            ProblemFilter.AppUserId = Problem_ProblemFilterDTO.CreatorId;
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
            StoreCheckingFilter.Longitude = Problem_StoreCheckingFilterDTO.Longitude;
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
            StoreCheckingFilter.Longitude = Problem_StoreCheckingFilterDTO.Longitude;
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

