using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MMenu;
using DMS.Services.MRole;
using DMS.Services.MStatus;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using DMS.Services.MBrand;
using DMS.Services.MOrganization;
using DMS.Services.MProduct;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;
using DMS.Services.MReseller;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MSupplier;
using DMS.Services.MWarehouse;

namespace DMS.Rpc.role
{
    public class RoleController : RpcController
    {
        private IAppUserService AppUserService;
        private IMenuService MenuService;
        private IRoleService RoleService;
        private IPermissionService PermissionService;
        private IStatusService StatusService;
        private IBrandService BrandService;
        private IOrganizationService OrganizationService;
        private IProductService ProductService;
        private IProductGroupingService ProductGroupingService;
        private IProductTypeService ProductTypeService;
        private IResellerService ResellerService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private ISupplierService SupplierService;
        private IWarehouseService WarehouseService;

        public RoleController(
            IAppUserService AppUserService,
            IMenuService MenuService,
            IRoleService RoleService,
            IPermissionService PermissionService,
            IBrandService BrandService,
            IOrganizationService OrganizationService,
            IProductService ProductService,
            IProductGroupingService ProductGroupingService,
            IProductTypeService ProductTypeService,
            IResellerService ResellerService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            ISupplierService SupplierService,
            IWarehouseService WarehouseService,
            IStatusService StatusService
        )
        {
            this.AppUserService = AppUserService;
            this.MenuService = MenuService;
            this.RoleService = RoleService;
            this.PermissionService = PermissionService;
            this.BrandService = BrandService;
            this.OrganizationService = OrganizationService;
            this.ProductService = ProductService;
            this.ProductGroupingService = ProductGroupingService;
            this.ProductTypeService = ProductTypeService;
            this.ResellerService = ResellerService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.SupplierService = SupplierService;
            this.WarehouseService = WarehouseService;
            this.StatusService = StatusService;
        }

        [Route(RoleRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Role_RoleFilterDTO Role_RoleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RoleFilter RoleFilter = ConvertFilterDTOToFilterEntity(Role_RoleFilterDTO);
            RoleFilter = RoleService.ToFilter(RoleFilter);
            int count = await RoleService.Count(RoleFilter);
            return count;
        }

        [Route(RoleRoute.List), HttpPost]
        public async Task<ActionResult<List<Role_RoleDTO>>> List([FromBody] Role_RoleFilterDTO Role_RoleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RoleFilter RoleFilter = ConvertFilterDTOToFilterEntity(Role_RoleFilterDTO);
            RoleFilter = RoleService.ToFilter(RoleFilter);
            List<Role> Roles = await RoleService.List(RoleFilter);
            List<Role_RoleDTO> Role_RoleDTOs = Roles
                .Select(c => new Role_RoleDTO(c)).ToList();
            return Role_RoleDTOs;
        }

        [Route(RoleRoute.Get), HttpPost]
        public async Task<ActionResult<Role_RoleDTO>> Get([FromBody]Role_RoleDTO Role_RoleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Role_RoleDTO.Id))
                return Forbid();

            Role Role = await RoleService.Get(Role_RoleDTO.Id);
            return new Role_RoleDTO(Role);
        }

        [Route(RoleRoute.Create), HttpPost]
        public async Task<ActionResult<Role_RoleDTO>> Create([FromBody] Role_RoleDTO Role_RoleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Role_RoleDTO.Id))
                return Forbid();

            Role Role = ConvertDTOToEntity(Role_RoleDTO);
            Role = await RoleService.Create(Role);
            Role_RoleDTO = new Role_RoleDTO(Role);
            if (Role.IsValidated)
                return Role_RoleDTO;
            else
                return BadRequest(Role_RoleDTO);
        }

        [Route(RoleRoute.Update), HttpPost]
        public async Task<ActionResult<Role_RoleDTO>> Update([FromBody] Role_RoleDTO Role_RoleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Role_RoleDTO.Id))
                return Forbid();

            Role Role = ConvertDTOToEntity(Role_RoleDTO);
            Role = await RoleService.Update(Role);
            Role_RoleDTO = new Role_RoleDTO(Role);
            if (Role.IsValidated)
                return Role_RoleDTO;
            else
                return BadRequest(Role_RoleDTO);
        }

        [Route(RoleRoute.Delete), HttpPost]
        public async Task<ActionResult<Role_RoleDTO>> Delete([FromBody] Role_RoleDTO Role_RoleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Role_RoleDTO.Id))
                return Forbid();

            Role Role = ConvertDTOToEntity(Role_RoleDTO);
            Role = await RoleService.Delete(Role);
            Role_RoleDTO = new Role_RoleDTO(Role);
            if (Role.IsValidated)
                return Role_RoleDTO;
            else
                return BadRequest(Role_RoleDTO);
        }

        [Route(RoleRoute.AssignAppUser), HttpPost]
        public async Task<ActionResult<Role_RoleDTO>> AssignAppUser([FromBody] Role_RoleDTO Role_RoleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Role_RoleDTO.Id))
                return Forbid();

            Role Role = ConvertDTOToEntity(Role_RoleDTO);
            Role = await RoleService.AssignAppUser(Role);
            Role_RoleDTO = new Role_RoleDTO(Role);
            if (Role.IsValidated)
                return Role_RoleDTO;
            else
                return BadRequest(Role_RoleDTO);
        }

        private async Task<bool> HasPermission(long Id)
        {
            RoleFilter RoleFilter = new RoleFilter();
            if (Id == 0)
            {

            }
            else
            {
                RoleFilter.Id = new IdFilter { Equal = Id };
                RoleFilter = RoleService.ToFilter(RoleFilter);
                int count = await RoleService.Count(RoleFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        public Role ConvertDTOToEntity(Role_RoleDTO Role_RoleDTO)
        {
            Role Role = new Role();
            Role.Id = Role_RoleDTO.Id;
            Role.Code = Role_RoleDTO.Code;
            Role.Name = Role_RoleDTO.Name;
            Role.StatusId = Role_RoleDTO.StatusId;
            Role.AppUserRoleMappings = Role_RoleDTO.AppUserRoleMappings?
                .Select(x => new AppUserRoleMapping
                {
                    AppUserId = x.AppUserId,
                    RoleId = x.RoleId
                }).ToList();
            return Role;
        }

        public RoleFilter ConvertFilterDTOToFilterEntity(Role_RoleFilterDTO Role_RoleFilterDTO)
        {
            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Selects = RoleSelect.ALL;
            RoleFilter.Skip = Role_RoleFilterDTO.Skip;
            RoleFilter.Take = Role_RoleFilterDTO.Take;
            RoleFilter.OrderBy = Role_RoleFilterDTO.OrderBy;
            RoleFilter.OrderType = Role_RoleFilterDTO.OrderType;

            RoleFilter.Id = Role_RoleFilterDTO.Id;
            RoleFilter.Code = Role_RoleFilterDTO.Code;
            RoleFilter.Name = Role_RoleFilterDTO.Name;
            RoleFilter.StatusId = Role_RoleFilterDTO.StatusId;
            return RoleFilter;
        }

        [Route(RoleRoute.GetMenu), HttpPost]
        public async Task<ActionResult<Role_MenuDTO>> GetMenu([FromBody]Role_MenuDTO Role_MenuDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Menu Menu = await MenuService.Get(Role_MenuDTO.Id);
            return new Role_MenuDTO(Menu);
        }



        [Route(RoleRoute.CreatePermission), HttpPost]
        public async Task<ActionResult<Role_PermissionDTO>> CreatePermission([FromBody]Role_PermissionDTO Role_PermissionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Permission Permission = ConvertPermissionDTOToBO(Role_PermissionDTO);
            Permission = await PermissionService.Create(Permission);
            Role_PermissionDTO = new Role_PermissionDTO(Permission);
            if (Permission.IsValidated)
                return Role_PermissionDTO;
            else
                return BadRequest(Role_PermissionDTO);
        }
        [Route(RoleRoute.UpdatePermission), HttpPost]
        public async Task<ActionResult<Role_PermissionDTO>> UpdatePermission([FromBody]Role_PermissionDTO Role_PermissionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Permission Permission = ConvertPermissionDTOToBO(Role_PermissionDTO);
            Permission = await PermissionService.Create(Permission);
            Role_PermissionDTO = new Role_PermissionDTO(Permission);
            if (Permission.IsValidated)
                return Role_PermissionDTO;
            else
                return BadRequest(Role_PermissionDTO);
        }

        [Route(RoleRoute.DeletePermission), HttpPost]
        public async Task<ActionResult<Role_PermissionDTO>> DeletePermission([FromBody]Role_PermissionDTO Role_PermissionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Permission Permission = ConvertPermissionDTOToBO(Role_PermissionDTO);
            Permission = await PermissionService.Create(Permission);
            Role_PermissionDTO = new Role_PermissionDTO(Permission);
            if (Permission.IsValidated)
                return Role_PermissionDTO;
            else
                return BadRequest(Role_PermissionDTO);
        }

        public Permission ConvertPermissionDTOToBO(Role_PermissionDTO Role_PermissionDTO)
        {
            Permission Permission = new Permission
            {
                Id = Role_PermissionDTO.Id,
                Code = Role_PermissionDTO.Code,
                Name = Role_PermissionDTO.Name,
                RoleId = Role_PermissionDTO.RoleId,
                MenuId = Role_PermissionDTO.MenuId,
                StatusId = Role_PermissionDTO.StatusId,
                Menu = new Menu
                {
                    Id = Role_PermissionDTO.Menu.Id,
                    Code = Role_PermissionDTO.Menu.Code,
                    Name = Role_PermissionDTO.Menu.Name,
                    Path = Role_PermissionDTO.Menu.Path,
                    IsDeleted = Role_PermissionDTO.Menu.IsDeleted,
                    Fields = Role_PermissionDTO.Menu.Fields?.Select(f => new Field
                    {
                        Id = f.Id,
                        Name = f.Name,
                        FieldTypeId = f.FieldTypeId,
                    }).ToList(),
                    Actions = Role_PermissionDTO.Menu.Actions?.Select(p => new Entities.Action
                    {
                        Id = p.Id,
                        Name = p.Name,
                    }).ToList(),
                },
                PermissionContents = Role_PermissionDTO.PermissionContents?.Select(pf => new PermissionContent
                {
                    Id = pf.Id,
                    PermissionOperatorId = pf.PermissionOperatorId,
                    FieldId = pf.FieldId,
                    Value = pf.Value,
                }).ToList(),
                PermissionActionMappings = Role_PermissionDTO.PermissionActionMappings?.Select(pp => new PermissionActionMapping
                {
                    ActionId = pp.ActionId,
                }).ToList(),
            };
            return Permission;
        }

        [Route(RoleRoute.SingleListAppUser), HttpPost]
        public async Task<List<Role_AppUserDTO>> SingleListAppUser([FromBody] Role_AppUserFilterDTO Role_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName | AppUserSelect.Email | AppUserSelect.Phone;
            AppUserFilter.Id = Role_AppUserFilterDTO.Id;
            AppUserFilter.Username = Role_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = Role_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = Role_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Role_AppUserFilterDTO.Phone;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Role_AppUserDTO> Role_AppUserDTOs = AppUsers
                .Select(x => new Role_AppUserDTO(x)).ToList();
            return Role_AppUserDTOs;
        }
        [Route(RoleRoute.SingleListMenu), HttpPost]
        public async Task<List<Role_MenuDTO>> SingleListMenu([FromBody] Role_MenuFilterDTO Role_MenuFilterDTO)
        {
            MenuFilter MenuFilter = new MenuFilter();
            MenuFilter.Skip = 0;
            MenuFilter.Take = 20;
            MenuFilter.OrderBy = MenuOrder.Id;
            MenuFilter.OrderType = OrderType.ASC;
            MenuFilter.Selects = MenuSelect.ALL;
            MenuFilter.Id = Role_MenuFilterDTO.Id;
            MenuFilter.Code = Role_MenuFilterDTO.Code;
            MenuFilter.Name = Role_MenuFilterDTO.Name;
            MenuFilter.Path = Role_MenuFilterDTO.Path;

            List<Menu> Menus = await MenuService.List(MenuFilter);
            List<Role_MenuDTO> Role_MenuDTOs = Menus
                .Select(x => new Role_MenuDTO(x)).ToList();
            return Role_MenuDTOs;
        }

        [Route(RoleRoute.SingleListStatus), HttpPost]
        public async Task<List<Role_StatusDTO>> SingleListStatus([FromBody] Role_StatusFilterDTO Role_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = Role_StatusFilterDTO.Id;
            StatusFilter.Code = Role_StatusFilterDTO.Code;
            StatusFilter.Name = Role_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Role_StatusDTO> Role_StatusDTOs = Statuses
                .Select(x => new Role_StatusDTO(x)).ToList();
            return Role_StatusDTOs;
        }

        [Route(RoleRoute.SingleListBrand), HttpPost]
        public async Task<List<Role_BrandDTO>> SingleListBrand([FromBody] Role_BrandFilterDTO Role_BrandFilterDTO)
        {
            BrandFilter BrandFilter = new BrandFilter();
            BrandFilter.Skip = 0;
            BrandFilter.Take = 20;
            BrandFilter.OrderBy = BrandOrder.Id;
            BrandFilter.OrderType = OrderType.ASC;
            BrandFilter.Selects = BrandSelect.Id | BrandSelect.Code | BrandSelect.Name;
            BrandFilter.Id = Role_BrandFilterDTO.Id;
            BrandFilter.Code = Role_BrandFilterDTO.Code;
            BrandFilter.Name = Role_BrandFilterDTO.Name;

            List<Brand> Brandes = await BrandService.List(BrandFilter);
            List<Role_BrandDTO> Role_BrandDTOs = Brandes
                .Select(x => new Role_BrandDTO(x)).ToList();
            return Role_BrandDTOs;
        }

        [Route(RoleRoute.SingleListOrganization), HttpPost]
        public async Task<List<Role_OrganizationDTO>> SingleListOrganization([FromBody] Role_OrganizationFilterDTO Role_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.Id | OrganizationSelect.Code | OrganizationSelect.Name | OrganizationSelect.Parent;
            OrganizationFilter.Id = Role_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = Role_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = Role_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = Role_OrganizationFilterDTO.ParentId;

            List<Organization> Organizationes = await OrganizationService.List(OrganizationFilter);
            List<Role_OrganizationDTO> Role_OrganizationDTOs = Organizationes
                .Select(x => new Role_OrganizationDTO(x)).ToList();
            return Role_OrganizationDTOs;
        }

        [Route(RoleRoute.SingleListProduct), HttpPost]
        public async Task<List<Role_ProductDTO>> SingleListProduct([FromBody] Role_ProductFilterDTO Role_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Skip = 0;
            ProductFilter.Take = 20;
            ProductFilter.OrderBy = ProductOrder.Id;
            ProductFilter.OrderType = OrderType.ASC;
            ProductFilter.Selects = ProductSelect.Id | ProductSelect.Code | ProductSelect.Name;
            ProductFilter.Id = Role_ProductFilterDTO.Id;
            ProductFilter.Code = Role_ProductFilterDTO.Code;
            ProductFilter.Name = Role_ProductFilterDTO.Name;

            List<Product> Productes = await ProductService.List(ProductFilter);
            List<Role_ProductDTO> Role_ProductDTOs = Productes
                .Select(x => new Role_ProductDTO(x)).ToList();
            return Role_ProductDTOs;
        }

        [Route(RoleRoute.SingleListProductGrouping), HttpPost]
        public async Task<List<Role_ProductGroupingDTO>> SingleListProductGrouping([FromBody] Role_ProductGroupingFilterDTO Role_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;
            ProductGroupingFilter.Id = Role_ProductGroupingFilterDTO.Id;
            ProductGroupingFilter.Code = Role_ProductGroupingFilterDTO.Code;
            ProductGroupingFilter.Name = Role_ProductGroupingFilterDTO.Name;

            List<ProductGrouping> ProductGroupinges = await ProductGroupingService.List(ProductGroupingFilter);
            List<Role_ProductGroupingDTO> Role_ProductGroupingDTOs = ProductGroupinges
                .Select(x => new Role_ProductGroupingDTO(x)).ToList();
            return Role_ProductGroupingDTOs;
        }

        [Route(RoleRoute.SingleListProductType), HttpPost]
        public async Task<List<Role_ProductTypeDTO>> SingleListProductType([FromBody] Role_ProductTypeFilterDTO Role_ProductTypeFilterDTO)
        {
            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Skip = 0;
            ProductTypeFilter.Take = 20;
            ProductTypeFilter.OrderBy = ProductTypeOrder.Id;
            ProductTypeFilter.OrderType = OrderType.ASC;
            ProductTypeFilter.Selects = ProductTypeSelect.Id | ProductTypeSelect.Code | ProductTypeSelect.Name;
            ProductTypeFilter.Id = Role_ProductTypeFilterDTO.Id;
            ProductTypeFilter.Code = Role_ProductTypeFilterDTO.Code;
            ProductTypeFilter.Name = Role_ProductTypeFilterDTO.Name;

            List<ProductType> ProductTypees = await ProductTypeService.List(ProductTypeFilter);
            List<Role_ProductTypeDTO> Role_ProductTypeDTOs = ProductTypees
                .Select(x => new Role_ProductTypeDTO(x)).ToList();
            return Role_ProductTypeDTOs;
        }

        [Route(RoleRoute.SingleListReseller), HttpPost]
        public async Task<List<Role_ResellerDTO>> SingleListReseller([FromBody] Role_ResellerFilterDTO Role_ResellerFilterDTO)
        {
            ResellerFilter ResellerFilter = new ResellerFilter();
            ResellerFilter.Skip = 0;
            ResellerFilter.Take = 20;
            ResellerFilter.OrderBy = ResellerOrder.Id;
            ResellerFilter.OrderType = OrderType.ASC;
            ResellerFilter.Selects = ResellerSelect.Id | ResellerSelect.Code | ResellerSelect.Name;
            ResellerFilter.Id = Role_ResellerFilterDTO.Id;
            ResellerFilter.Code = Role_ResellerFilterDTO.Code;
            ResellerFilter.Name = Role_ResellerFilterDTO.Name;

            List<Reseller> Reselleres = await ResellerService.List(ResellerFilter);
            List<Role_ResellerDTO> Role_ResellerDTOs = Reselleres
                .Select(x => new Role_ResellerDTO(x)).ToList();
            return Role_ResellerDTOs;
        }

        [Route(RoleRoute.SingleListStore), HttpPost]
        public async Task<List<Role_StoreDTO>> SingleListStore([FromBody] Role_StoreFilterDTO Role_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.Id | StoreSelect.Code | StoreSelect.Name;
            StoreFilter.Id = Role_StoreFilterDTO.Id;
            StoreFilter.Code = Role_StoreFilterDTO.Code;
            StoreFilter.Name = Role_StoreFilterDTO.Name;

            List<Store> Storees = await StoreService.List(StoreFilter);
            List<Role_StoreDTO> Role_StoreDTOs = Storees
                .Select(x => new Role_StoreDTO(x)).ToList();
            return Role_StoreDTOs;
        }

        [Route(RoleRoute.SingleListStoreGrouping), HttpPost]
        public async Task<List<Role_StoreGroupingDTO>> SingleListStoreGrouping([FromBody] Role_StoreGroupingFilterDTO Role_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 20;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.Id | StoreGroupingSelect.Code | StoreGroupingSelect.Name;
            StoreGroupingFilter.Id = Role_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = Role_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = Role_StoreGroupingFilterDTO.Name;

            List<StoreGrouping> StoreGroupinges = await StoreGroupingService.List(StoreGroupingFilter);
            List<Role_StoreGroupingDTO> Role_StoreGroupingDTOs = StoreGroupinges
                .Select(x => new Role_StoreGroupingDTO(x)).ToList();
            return Role_StoreGroupingDTOs;
        }


        [Route(RoleRoute.SingleListStoreType), HttpPost]
        public async Task<List<Role_StoreTypeDTO>> SingleListStoreType([FromBody] Role_StoreTypeFilterDTO Role_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.Id | StoreTypeSelect.Code | StoreTypeSelect.Name;
            StoreTypeFilter.Id = Role_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = Role_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = Role_StoreTypeFilterDTO.Name;

            List<StoreType> StoreTypees = await StoreTypeService.List(StoreTypeFilter);
            List<Role_StoreTypeDTO> Role_StoreTypeDTOs = StoreTypees
                .Select(x => new Role_StoreTypeDTO(x)).ToList();
            return Role_StoreTypeDTOs;
        }

        [Route(RoleRoute.SingleListSupplier), HttpPost]
        public async Task<List<Role_SupplierDTO>> SingleListSupplier([FromBody] Role_SupplierFilterDTO Role_SupplierFilterDTO)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Skip = 0;
            SupplierFilter.Take = 20;
            SupplierFilter.OrderBy = SupplierOrder.Id;
            SupplierFilter.OrderType = OrderType.ASC;
            SupplierFilter.Selects = SupplierSelect.Id | SupplierSelect.Code | SupplierSelect.Name;
            SupplierFilter.Id = Role_SupplierFilterDTO.Id;
            SupplierFilter.Code = Role_SupplierFilterDTO.Code;
            SupplierFilter.Name = Role_SupplierFilterDTO.Name;

            List<Supplier> Supplieres = await SupplierService.List(SupplierFilter);
            List<Role_SupplierDTO> Role_SupplierDTOs = Supplieres
                .Select(x => new Role_SupplierDTO(x)).ToList();
            return Role_SupplierDTOs;
        }

        [Route(RoleRoute.SingleListWarehouse), HttpPost]
        public async Task<List<Role_WarehouseDTO>> SingleListWarehouse([FromBody] Role_WarehouseFilterDTO Role_WarehouseFilterDTO)
        {
            WarehouseFilter WarehouseFilter = new WarehouseFilter();
            WarehouseFilter.Skip = 0;
            WarehouseFilter.Take = 20;
            WarehouseFilter.OrderBy = WarehouseOrder.Id;
            WarehouseFilter.OrderType = OrderType.ASC;
            WarehouseFilter.Selects = WarehouseSelect.Id | WarehouseSelect.Code | WarehouseSelect.Name;
            WarehouseFilter.Id = Role_WarehouseFilterDTO.Id;
            WarehouseFilter.Code = Role_WarehouseFilterDTO.Code;
            WarehouseFilter.Name = Role_WarehouseFilterDTO.Name;

            List<Warehouse> Warehousees = await WarehouseService.List(WarehouseFilter);
            List<Role_WarehouseDTO> Role_WarehouseDTOs = Warehousees
                .Select(x => new Role_WarehouseDTO(x)).ToList();
            return Role_WarehouseDTOs;
        }

        [Route(RoleRoute.CountAppUser), HttpPost]
        public async Task<long> CountAppUser([FromBody] Role_AppUserFilterDTO Role_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Id = Role_AppUserFilterDTO.Id;
            AppUserFilter.Username = Role_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = Role_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = Role_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Role_AppUserFilterDTO.Phone;
            AppUserFilter.OrganizationId = Role_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await AppUserService.Count(AppUserFilter);
        }
        [Route(RoleRoute.ListAppUser), HttpPost]
        public async Task<List<Role_AppUserDTO>> ListAppUser([FromBody] Role_AppUserFilterDTO Role_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = Role_AppUserFilterDTO.Skip;
            AppUserFilter.Take = Role_AppUserFilterDTO.Take;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Role_AppUserFilterDTO.Id;
            AppUserFilter.Username = Role_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = Role_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = Role_AppUserFilterDTO.Email;
            AppUserFilter.OrganizationId = Role_AppUserFilterDTO.OrganizationId;
            AppUserFilter.Phone = Role_AppUserFilterDTO.Phone;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Role_AppUserDTO> Role_AppUserDTOs = AppUsers
                .Select(x => new Role_AppUserDTO(x)).ToList();
            return Role_AppUserDTOs;
        }
    }
}

