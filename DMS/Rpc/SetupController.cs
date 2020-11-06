using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Helpers;
using DMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DMS.Rpc
{
    public class SetupController : ControllerBase
    {
        private DataContext DataContext;
        public SetupController(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        [HttpGet, Route("rpc/dms/setup/set-org")]
        public async Task Count()
        {
            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking.ToListAsync();
            var AppUserIds = StoreCheckingDAOs.Select(x => x.SaleEmployeeId).Distinct().ToList();
            var AppUserDAOs = await DataContext.AppUser.Where(x => AppUserIds.Contains(x.Id)).ToListAsync();
            foreach (var StoreCheckingDAO in StoreCheckingDAOs)
            {
                StoreCheckingDAO.OrganizationId = AppUserDAOs.Where(x => x.Id == StoreCheckingDAO.SaleEmployeeId).AsParallel().Select(x => x.OrganizationId).FirstOrDefault();
            }
            await DataContext.SaveChangesAsync();
        }

        [HttpGet, Route("rpc/dms/setup/store-gen-code")]
        public async Task StoreGenCode()
        {
            List<StoreDAO> Stores = await DataContext.Store.OrderByDescending(x => x.CreatedAt).ToListAsync();
            List<OrganizationDAO> Organizations = await DataContext.Organization.OrderByDescending(x => x.CreatedAt).ToListAsync();
            List<StoreTypeDAO> StoreTypes = await DataContext.StoreType.OrderByDescending(x => x.CreatedAt).ToListAsync();
            var counter = Stores.Count();
            foreach (var Store in Stores)
            {
                var Organization = Organizations.Where(x => x.Id == Store.OrganizationId).Select(x => x.Code).FirstOrDefault();
                var StoreType = StoreTypes.Where(x => x.Id == Store.StoreTypeId).Select(x => x.Code).FirstOrDefault();
                Store.Code = $"{Organization}.{StoreType}.{(10000000 + counter--).ToString().Substring(1)}";
            }
            await DataContext.SaveChangesAsync();
        }

        [HttpGet, Route("rpc/dms/setup/year/{year}")]
        public bool ChangeYear(int year)
        {
            StaticParams.ChangeYear = year;
            return true;
        }

        [HttpGet, Route("rpc/dms/setup/init")]
        public ActionResult Init()
        {
            InitEnum();
            InitData();
            InitRoute();
            InitAdmin();
            return Ok();
        }

        #region sync
        public ActionResult InitData()
        {
            RestClient RestClient = new RestClient(InternalServices.ES);
            InitPosition(RestClient);
            InitProvince(RestClient);
            InitDistrict(RestClient);
            InitWard(RestClient);
            InitOrganization(RestClient);
            InitAppUser(RestClient);
            return Ok();
        }

        private void InitPosition(RestClient RestClient)
        {
            IRestRequest RestRequest = new RestRequest("rpc/es/position/list");
            RestRequest.Method = Method.POST;
            IRestResponse<List<Position>> RestResponse = RestClient.Post<List<Position>>(RestRequest);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<PositionDAO> PositionInDB = DataContext.Position.AsNoTracking().ToList();
                List<Position> Positions = RestResponse.Data;
                foreach (Position Position in Positions)
                {
                    PositionDAO PositionDAO = PositionInDB.Where(x => x.Id == Position.Id).FirstOrDefault();
                    if (PositionDAO == null)
                    {
                        PositionDAO = new PositionDAO
                        {
                            Id = Position.Id,
                        };
                        PositionInDB.Add(PositionDAO);
                    }
                    PositionDAO.Code = Position.Code;
                    PositionDAO.Name = Position.Name;
                    PositionDAO.StatusId = Position.StatusId;
                    PositionDAO.CreatedAt = Position.CreatedAt;
                    PositionDAO.UpdatedAt = Position.UpdatedAt;
                    PositionDAO.DeletedAt = Position.DeletedAt;
                    PositionDAO.RowId = Position.RowId;
                }
                DataContext.Position.BulkMerge(PositionInDB);
            }
        }

        private void InitOrganization(RestClient RestClient)
        {
            IRestRequest RestRequest = new RestRequest("rpc/es/organization/list");
            RestRequest.Method = Method.POST;
            IRestResponse<List<Organization>> RestResponse = RestClient.Post<List<Organization>>(RestRequest);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<OrganizationDAO> OrganizationInDB = DataContext.Organization.AsNoTracking().ToList();
                List<Organization> Organizations = RestResponse.Data;
                foreach (Organization Organization in Organizations)
                {
                    OrganizationDAO OrganizationDAO = OrganizationInDB.Where(x => x.Id == Organization.Id).FirstOrDefault();
                    if (OrganizationDAO == null)
                    {
                        OrganizationDAO = new OrganizationDAO
                        {
                            Id = Organization.Id,
                        };
                        OrganizationInDB.Add(OrganizationDAO);
                    }
                    OrganizationDAO.Code = Organization.Code;
                    OrganizationDAO.Name = Organization.Name;
                    OrganizationDAO.Path = Organization.Path;
                    OrganizationDAO.Level = Organization.Level;
                    OrganizationDAO.Address = Organization.Address;
                    OrganizationDAO.Email = Organization.Email;
                    OrganizationDAO.ParentId = Organization.ParentId;
                    OrganizationDAO.StatusId = Organization.StatusId;
                    OrganizationDAO.CreatedAt = Organization.CreatedAt;
                    OrganizationDAO.UpdatedAt = Organization.UpdatedAt;
                    OrganizationDAO.DeletedAt = Organization.DeletedAt;
                    OrganizationDAO.RowId = Organization.RowId;

                }
                DataContext.Organization.BulkMerge(OrganizationInDB);
            }

        }

        private void InitAppUser(RestClient RestClient)
        {
            IRestRequest RestRequest = new RestRequest("rpc/es/app-user/list");
            RestRequest.Method = Method.POST;
            IRestResponse<List<AppUser>> RestResponse = RestClient.Post<List<AppUser>>(RestRequest);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<AppUserDAO> AppUserInDB = DataContext.AppUser.AsNoTracking().ToList();
                List<AppUser> AppUsers = RestResponse.Data;
                foreach (AppUser AppUser in AppUsers)
                {
                    AppUserDAO AppUserDAO = AppUserInDB.Where(x => x.Id == AppUser.Id).FirstOrDefault();
                    if (AppUserDAO == null)
                    {
                        AppUserDAO = new AppUserDAO
                        {
                            Id = AppUser.Id,

                        };
                        AppUserInDB.Add(AppUserDAO);
                    }
                    AppUserDAO.Username = AppUser.Username;
                    AppUserDAO.DisplayName = AppUser.DisplayName;
                    AppUserDAO.Address = AppUser.Address;
                    AppUserDAO.Email = AppUser.Email;
                    AppUserDAO.Phone = AppUser.Phone;
                    AppUserDAO.ProvinceId = AppUser.ProvinceId;
                    AppUserDAO.PositionId = AppUser.PositionId;
                    AppUserDAO.Department = AppUser.Department;
                    AppUserDAO.OrganizationId = AppUser.OrganizationId;
                    AppUserDAO.SexId = AppUser.SexId;
                    AppUserDAO.StatusId = AppUser.StatusId;
                    AppUserDAO.CreatedAt = AppUser.CreatedAt;
                    AppUserDAO.UpdatedAt = AppUser.UpdatedAt;
                    AppUserDAO.DeletedAt = AppUser.DeletedAt;
                    AppUserDAO.Avatar = AppUser.Avatar;
                    AppUserDAO.Birthday = AppUser.Birthday;
                    AppUserDAO.RowId = AppUser.RowId;
                }
                DataContext.AppUser.BulkMerge(AppUserInDB);
            }
        }

        private void InitProvince(RestClient RestClient)
        {
            IRestRequest RestRequest = new RestRequest("rpc/es/province/list");
            RestRequest.Method = Method.POST;
            IRestResponse<List<Province>> RestResponse = RestClient.Post<List<Province>>(RestRequest);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<ProvinceDAO> ProvinceInDB = DataContext.Province.AsNoTracking().ToList();
                List<Province> Provinces = RestResponse.Data;
                foreach (Province Province in Provinces)
                {
                    ProvinceDAO ProvinceDAO = ProvinceInDB.Where(x => x.Id == Province.Id).FirstOrDefault();
                    if (ProvinceDAO == null)
                    {
                        ProvinceDAO = new ProvinceDAO
                        {
                            Id = Province.Id,
                        };
                        ProvinceInDB.Add(ProvinceDAO);
                    }
                    ProvinceDAO.Code = Province.Code;
                    ProvinceDAO.Name = Province.Name;
                    ProvinceDAO.StatusId = Province.StatusId;
                    ProvinceDAO.CreatedAt = Province.CreatedAt;
                    ProvinceDAO.UpdatedAt = Province.UpdatedAt;
                    ProvinceDAO.DeletedAt = Province.DeletedAt;
                    ProvinceDAO.RowId = Province.RowId;
                }
                DataContext.Province.BulkMerge(ProvinceInDB);
            }
        }

        private void InitDistrict(RestClient RestClient)
        {
            IRestRequest RestRequest = new RestRequest("rpc/es/district/list");
            RestRequest.Method = Method.POST;
            IRestResponse<List<District>> RestResponse = RestClient.Post<List<District>>(RestRequest);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<DistrictDAO> DistrictInDB = DataContext.District.AsNoTracking().ToList();
                List<District> Districts = RestResponse.Data;
                foreach (District District in Districts)
                {
                    DistrictDAO DistrictDAO = DistrictInDB.Where(x => x.Id == District.Id).FirstOrDefault();
                    if (DistrictDAO == null)
                    {
                        DistrictDAO = new DistrictDAO
                        {
                            Id = District.Id,
                        };
                        DistrictInDB.Add(DistrictDAO);
                    }
                    DistrictDAO.Code = District.Code;
                    DistrictDAO.Name = District.Name;
                    DistrictDAO.ProvinceId = District.ProvinceId;
                    DistrictDAO.StatusId = District.StatusId;
                    DistrictDAO.CreatedAt = District.CreatedAt;
                    DistrictDAO.UpdatedAt = District.UpdatedAt;
                    DistrictDAO.DeletedAt = District.DeletedAt;
                    DistrictDAO.RowId = District.RowId;
                }
                DataContext.District.BulkMerge(DistrictInDB);
            }
        }

        private void InitWard(RestClient RestClient)
        {
            IRestRequest RestRequest = new RestRequest("rpc/es/ward/list");
            RestRequest.Method = Method.POST;
            IRestResponse<List<Ward>> RestResponse = RestClient.Post<List<Ward>>(RestRequest);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<WardDAO> WardInDB = DataContext.Ward.AsNoTracking().ToList();
                List<Ward> Wards = RestResponse.Data;
                foreach (Ward Ward in Wards)
                {
                    WardDAO WardDAO = WardInDB.Where(x => x.Id == Ward.Id).FirstOrDefault();
                    if (WardDAO == null)
                    {
                        WardDAO = new WardDAO
                        {
                            Id = Ward.Id,
                        };
                        WardInDB.Add(WardDAO);
                    }
                    WardDAO.Code = Ward.Code;
                    WardDAO.Name = Ward.Name;
                    WardDAO.DistrictId = Ward.DistrictId;
                    WardDAO.StatusId = Ward.StatusId;
                    WardDAO.CreatedAt = Ward.CreatedAt;
                    WardDAO.UpdatedAt = Ward.UpdatedAt;
                    WardDAO.DeletedAt = Ward.DeletedAt;
                    WardDAO.RowId = Ward.RowId;
                }
                DataContext.Ward.BulkMerge(WardInDB);
            }
        }
        #endregion

        #region permission
        [HttpGet, Route("rpc/dms/setup/init-route")]
        public ActionResult InitRoute()
        {
            List<Type> routeTypes = typeof(SetupController).Assembly.GetTypes()
               .Where(x => typeof(Root).IsAssignableFrom(x) && x.IsClass)
               .ToList();

            InitMenu(routeTypes);
            InitPage(routeTypes);
            InitField(routeTypes);
            InitAction(routeTypes);

            DataContext.ActionPageMapping.Where(ap => ap.Action.IsDeleted || ap.Page.IsDeleted).DeleteFromQuery();
            DataContext.PermissionActionMapping.Where(ap => ap.Action.IsDeleted).DeleteFromQuery();
            DataContext.Action.Where(p => p.IsDeleted || p.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Page.Where(p => p.IsDeleted).DeleteFromQuery();
            DataContext.PermissionContent.Where(f => f.Field.IsDeleted == true || f.Field.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Field.Where(pf => pf.IsDeleted || pf.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Permission.Where(p => p.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Menu.Where(v => v.IsDeleted).DeleteFromQuery();
            return Ok();
        }
        [HttpGet, Route("rpc/dms/setup/init-admin")]
        public ActionResult InitAdmin()
        {
            RoleDAO Admin = DataContext.Role
               .Where(r => r.Name == "ADMIN")
               .FirstOrDefault();
            if (Admin == null)
            {
                Admin = new RoleDAO
                {
                    Name = "ADMIN",
                    Code = "ADMIN",
                    StatusId = StatusEnum.ACTIVE.Id,
                };
                DataContext.Role.Add(Admin);
                DataContext.SaveChanges();
            }

            AppUserDAO AppUser = DataContext.AppUser
                .Where(au => au.Username.ToLower() == "Administrator".ToLower())
                .FirstOrDefault();
            if (AppUser == null)
            {
                return Ok();
            }

            AppUserRoleMappingDAO AppUserRoleMappingDAO = DataContext.AppUserRoleMapping
                .Where(ur => ur.RoleId == Admin.Id && ur.AppUserId == AppUser.Id)
                .FirstOrDefault();
            if (AppUserRoleMappingDAO == null)
            {
                AppUserRoleMappingDAO = new AppUserRoleMappingDAO
                {
                    AppUserId = AppUser.Id,
                    RoleId = Admin.Id,
                };
                DataContext.AppUserRoleMapping.Add(AppUserRoleMappingDAO);
                DataContext.SaveChanges();
            }

            List<MenuDAO> Menus = DataContext.Menu.AsNoTracking()
                .Include(v => v.Actions)
                .ToList();
            List<PermissionDAO> permissions = DataContext.Permission.AsNoTracking()
                .Include(p => p.PermissionActionMappings)
                .ToList();
            foreach (MenuDAO Menu in Menus)
            {
                PermissionDAO permission = permissions
                    .Where(p => p.MenuId == Menu.Id && p.RoleId == Admin.Id)
                    .FirstOrDefault();
                if (permission == null)
                {
                    permission = new PermissionDAO
                    {
                        Code = Admin + "_" + Menu.Name,
                        Name = Admin + "_" + Menu.Name,
                        MenuId = Menu.Id,
                        RoleId = Admin.Id,
                        StatusId = StatusEnum.ACTIVE.Id,
                        PermissionActionMappings = new List<PermissionActionMappingDAO>(),
                    };
                    permissions.Add(permission);
                }
                else
                {
                    permission.StatusId = StatusEnum.ACTIVE.Id;
                    if (permission.PermissionActionMappings == null)
                        permission.PermissionActionMappings = new List<PermissionActionMappingDAO>();
                }
                foreach (ActionDAO action in Menu.Actions)
                {
                    PermissionActionMappingDAO PermissionActionMappingDAO = permission.PermissionActionMappings
                        .Where(ppm => ppm.ActionId == action.Id).FirstOrDefault();
                    if (PermissionActionMappingDAO == null)
                    {
                        PermissionActionMappingDAO = new PermissionActionMappingDAO
                        {
                            ActionId = action.Id
                        };
                        permission.PermissionActionMappings.Add(PermissionActionMappingDAO);
                    }
                }

            }
            DataContext.Permission.BulkMerge(permissions);
            permissions.ForEach(p =>
            {
                foreach (var action in p.PermissionActionMappings)
                {
                    action.PermissionId = p.Id;
                }
            });

            List<PermissionActionMappingDAO> PermissionActionMappingDAOs = permissions
                .SelectMany(p => p.PermissionActionMappings).ToList();
            DataContext.PermissionContent.Where(pf => pf.Permission.RoleId == Admin.Id).DeleteFromQuery();
            DataContext.PermissionActionMapping.Where(pf => pf.Permission.RoleId == Admin.Id).DeleteFromQuery();
            DataContext.PermissionActionMapping.BulkMerge(PermissionActionMappingDAOs);
            return Ok();
        }
        private void InitMenu(List<Type> routeTypes)
        {
            List<MenuDAO> Menus = DataContext.Menu.AsNoTracking().ToList();
            Menus.ForEach(m => m.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                MenuDAO Menu = Menus.Where(m => m.Name == type.Name).FirstOrDefault();
                if (Menu == null)
                {
                    Menu = new MenuDAO
                    {
                        Code = type.Name,
                        Name = type.Name,
                        IsDeleted = false,
                    };
                    Menus.Add(Menu);
                }
                else
                {
                    Menu.IsDeleted = false;
                }
            }
            DataContext.BulkMerge(Menus);
        }

        private void InitPage(List<Type> routeTypes)
        {
            List<PageDAO> pages = DataContext.Page.AsNoTracking().OrderBy(p => p.Path).ToList();
            pages.ForEach(p => p.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                var values = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(x => (string)x.GetRawConstantValue())
                .ToList();
                foreach (string value in values)
                {
                    PageDAO page = pages.Where(p => p.Path == value).FirstOrDefault();
                    if (page == null)
                    {
                        page = new PageDAO
                        {
                            Name = value,
                            Path = value,
                            IsDeleted = false,
                        };
                        pages.Add(page);
                    }
                    else
                    {
                        page.IsDeleted = false;
                    }
                }
            }
            DataContext.BulkMerge(pages);
        }

        private void InitField(List<Type> routeTypes)
        {
            List<MenuDAO> Menus = DataContext.Menu.AsNoTracking().ToList();
            List<FieldDAO> fields = DataContext.Field.AsNoTracking().ToList();
            fields.ForEach(p => p.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                MenuDAO Menu = Menus.Where(m => m.Code == type.Name).FirstOrDefault();
                var value = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => !fi.IsInitOnly && fi.FieldType == typeof(Dictionary<string, long>))
                .Select(x => (Dictionary<string, long>)x.GetValue(x))
                .FirstOrDefault();
                if (value == null)
                    continue;
                foreach (var pair in value)
                {
                    FieldDAO field = fields
                        .Where(p => p.MenuId == Menu.Id && p.Name == pair.Key)
                        .FirstOrDefault();
                    if (field == null)
                    {
                        field = new FieldDAO
                        {
                            MenuId = Menu.Id,
                            Name = pair.Key,
                            FieldTypeId = pair.Value,
                            IsDeleted = false,
                        };
                        fields.Add(field);
                    }
                    else
                    {
                        field.FieldTypeId = pair.Value;
                        field.IsDeleted = false;
                    }
                }
            }
            DataContext.BulkMerge(fields);
        }
        private void InitAction(List<Type> routeTypes)
        {
            List<MenuDAO> Menus = DataContext.Menu.AsNoTracking().ToList();
            List<ActionDAO> actions = DataContext.Action.AsNoTracking().ToList();
            actions.ForEach(p => p.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                MenuDAO Menu = Menus.Where(m => m.Code == type.Name).FirstOrDefault();
                var value = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
               .Where(fi => !fi.IsInitOnly && fi.FieldType == typeof(Dictionary<string, List<string>>))
               .Select(x => (Dictionary<string, List<string>>)x.GetValue(x))
               .FirstOrDefault();
                if (value == null)
                    continue;
                foreach (var pair in value)
                {
                    ActionDAO action = actions
                        .Where(p => p.MenuId == Menu.Id && p.Name == pair.Key)
                        .FirstOrDefault();
                    if (action == null)
                    {
                        action = new ActionDAO
                        {
                            MenuId = Menu.Id,
                            Name = pair.Key,
                            IsDeleted = false,
                        };
                        actions.Add(action);
                    }
                    else
                    {
                        action.IsDeleted = false;
                    }
                }
            }
            DataContext.BulkMerge(actions);

            actions = DataContext.Action.Where(a => a.IsDeleted == false).AsNoTracking().ToList();
            List<PageDAO> PageDAOs = DataContext.Page.AsNoTracking().ToList();
            List<ActionPageMappingDAO> ActionPageMappingDAOs = new List<ActionPageMappingDAO>();
            foreach (Type type in routeTypes)
            {
                MenuDAO Menu = Menus.Where(m => m.Code == type.Name).FirstOrDefault();
                var value = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
               .Where(fi => !fi.IsInitOnly && fi.FieldType == typeof(Dictionary<string, List<string>>))
               .Select(x => (Dictionary<string, List<string>>)x.GetValue(x))
               .FirstOrDefault();
                if (value == null)
                    continue;

                foreach (var pair in value)
                {
                    ActionDAO action = actions
                        .Where(p => p.MenuId == Menu.Id && p.Name == pair.Key)
                        .FirstOrDefault();
                    if (action == null)
                        continue;
                    List<string> pages = pair.Value;
                    foreach (string page in pages)
                    {
                        PageDAO PageDAO = PageDAOs.Where(p => p.Path == page).FirstOrDefault();
                        if (PageDAO != null)
                        {
                            if (!ActionPageMappingDAOs.Any(ap => ap.ActionId == action.Id && ap.PageId == PageDAO.Id))
                            {
                                ActionPageMappingDAOs.Add(new ActionPageMappingDAO
                                {
                                    ActionId = action.Id,
                                    PageId = PageDAO.Id
                                });
                            }
                        }
                    }
                }
            }
            ActionPageMappingDAOs = ActionPageMappingDAOs.Distinct().ToList();
            DataContext.ActionPageMapping.DeleteFromQuery();
            DataContext.BulkInsert(ActionPageMappingDAOs);
        }
        #endregion

        #region enum
        [HttpGet, Route("rpc/dms/setup/init-enum")]
        public ActionResult InitEnum()
        {
            InitPriceListTypeEnum();
            InitSalesOrderTypeEnum();
            InitEditedPriceStatusEnum();
            InitProblemStatusEnum();
            InitResellerStatusEnum();
            InitStatusEnum();
            InitStoreStatusEnum();
            InitERouteTypeEnum();
            InitNotificationStatusEnum();
            InitSurveyEnum();
            InitUsedVariationEnum();
            InitKpiEnum();
            InitPermissionEnum();
            InitStoreScoutingStatusEnum();
            InitSexEnum();
            InitSystemConfigurationEnum();
            InitWorkflowEnum();
            InitColorEnum();
            InitIdGenerate();
            InitPromotionTypeEnum();
            InitPromotionProductAppliedTypeEnum();
            InitPromotionPolicyEnum();
            InitPromotionDiscountTypeEnum();
            InitRewardStatusEnum();
            return Ok();
        }

        private void InitIdGenerate()
        {
            //Store
            var count = DataContext.IdGenerator.Where(x => x.IdGenerateTypeId == IdGenerateTypeEnum.STORE.Id).Count();
            if (count == 0)
            {
                List<IdGeneratorDAO> StoreIds = new List<IdGeneratorDAO>();
                for (int i = 1; i < 10000000; i++)
                {
                    IdGeneratorDAO IdGenerateDAO = new IdGeneratorDAO
                    {
                        IdGenerateTypeId = IdGenerateTypeEnum.STORE.Id,
                        Counter = i,
                        Used = false
                    };
                    StoreIds.Add(IdGenerateDAO);
                }
                DataContext.IdGenerator.BulkSynchronize(StoreIds);
            }
        }

        private void InitStatusEnum()
        {
            List<StatusDAO> StatusEnumList = StatusEnum.StatusEnumList.Select(item => new StatusDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.Status.BulkSynchronize(StatusEnumList);
        }

        private void InitStoreStatusEnum()
        {
            List<StoreStatusDAO> StoreStatusEnumList = StoreStatusEnum.StoreStatusEnumList.Select(item => new StoreStatusDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.StoreStatus.BulkSynchronize(StoreStatusEnumList);
        }

        private void InitERouteTypeEnum()
        {
            List<ERouteTypeDAO> ERouteTypeEnumList = ERouteTypeEnum.ERouteTypeEnumList.Select(item => new ERouteTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.ERouteType.BulkSynchronize(ERouteTypeEnumList);
        }

        private void InitRewardStatusEnum()
        {
            List<RewardStatusDAO> RewardStatusEnumList = RewardStatusEnum.RewardStatusEnumList.Select(item => new RewardStatusDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.RewardStatus.BulkSynchronize(RewardStatusEnumList);
        }

        private void InitStoreScoutingStatusEnum()
        {
            List<StoreScoutingStatusDAO> StoreScoutingStatusEnumList = StoreScoutingStatusEnum.StoreScoutingStatusEnumList.Select(item => new StoreScoutingStatusDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.StoreScoutingStatus.BulkSynchronize(StoreScoutingStatusEnumList);
        }

        private void InitNotificationStatusEnum()
        {
            List<NotificationStatusDAO> NotificationStatusEnumList = NotificationStatusEnum.NotificationStatusEnumList.Select(item => new NotificationStatusDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.NotificationStatus.BulkSynchronize(NotificationStatusEnumList);
        }

        private void InitKpiEnum()
        {
            List<KpiCriteriaGeneralDAO> KpiCriteriaGeneralDAOs = KpiCriteriaGeneralEnum.KpiCriteriaGeneralEnumList.Select(item => new KpiCriteriaGeneralDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.KpiCriteriaGeneral.BulkSynchronize(KpiCriteriaGeneralDAOs);

            List<KpiCriteriaItemDAO> KpiCriteriaItemDAOs = KpiCriteriaItemEnum.KpiCriteriaItemEnumList.Select(item => new KpiCriteriaItemDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.KpiCriteriaItem.BulkSynchronize(KpiCriteriaItemDAOs);

            List<KpiPeriodDAO> KpiPeriodDAOs = KpiPeriodEnum.KpiPeriodEnumList.Select(item => new KpiPeriodDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.KpiPeriod.BulkSynchronize(KpiPeriodDAOs);

            List<KpiYearDAO> KpiYearDAOs = KpiYearEnum.KpiYearEnumList.Select(item => new KpiYearDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.KpiYear.BulkSynchronize(KpiYearDAOs);

            List<KpiCriteriaTotalDAO> KpiCriteriaTotalDAOs = KpiCriteriaTotalEnum.KpiCriteriaTotalEnumList.Select(item => new KpiCriteriaTotalDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.KpiCriteriaTotal.BulkSynchronize(KpiCriteriaTotalDAOs);
        }

        private void InitUsedVariationEnum()
        {
            List<UsedVariationDAO> UsedVariationEnumList = UsedVariationEnum.UsedVariationEnumList.Select(item => new UsedVariationDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.UsedVariation.BulkSynchronize(UsedVariationEnumList);
        }
        private void InitEditedPriceStatusEnum()
        {
            List<EditedPriceStatusDAO> EditedPriceStatusEnumList = EditedPriceStatusEnum.EditedPriceStatusEnumList.Select(item => new EditedPriceStatusDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.EditedPriceStatus.BulkSynchronize(EditedPriceStatusEnumList);
        }
        private void InitResellerStatusEnum()
        {
            List<ResellerStatusDAO> ResellerStatusEnumList = ResellerStatusEnum.ResellerStatusEnumList.Select(item => new ResellerStatusDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.ResellerStatus.BulkSynchronize(ResellerStatusEnumList);
        }

        private void InitPriceListTypeEnum()
        {
            List<PriceListTypeDAO> PriceListTypeEnumList = PriceListTypeEnum.PriceListTypeEnumList.Select(item => new PriceListTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.PriceListType.BulkSynchronize(PriceListTypeEnumList);
        }

        private void InitPromotionTypeEnum()
        {
            List<PromotionTypeDAO> PromotionTypeEnumList = PromotionTypeEnum.PromotionTypeEnumList.Select(item => new PromotionTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.PromotionType.BulkSynchronize(PromotionTypeEnumList);
        }

        private void InitPromotionProductAppliedTypeEnum()
        {
            List<PromotionProductAppliedTypeDAO> PromotionProductAppliedTypeDAOs = PromotionProductAppliedTypeEnum.PromotionProductAppliedTypeEnumList.Select(item => new PromotionProductAppliedTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.PromotionProductAppliedType.BulkSynchronize(PromotionProductAppliedTypeDAOs);
        }

        private void InitPromotionPolicyEnum()
        {
            List<PromotionPolicyDAO> PromotionPolicyEnumList = PromotionPolicyEnum.PromotionPolicyEnumList.Select(item => new PromotionPolicyDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.PromotionPolicy.BulkSynchronize(PromotionPolicyEnumList);
        }

        private void InitPromotionDiscountTypeEnum()
        {
            List<PromotionDiscountTypeDAO> PromotionDiscountTypeEnumList = PromotionDiscountTypeEnum.PromotionDiscountTypeEnumList.Select(item => new PromotionDiscountTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.PromotionDiscountType.BulkSynchronize(PromotionDiscountTypeEnumList);
        }

        private void InitSalesOrderTypeEnum()
        {
            List<SalesOrderTypeDAO> SalesOrderTypeEnumList = SalesOrderTypeEnum.SalesOrderTypeEnumList.Select(item => new SalesOrderTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.SalesOrderType.BulkSynchronize(SalesOrderTypeEnumList);
        }

        private void InitSurveyEnum()
        {
            List<SurveyQuestionTypeDAO> SurveyQuestionTypeEnumList = SurveyQuestionTypeEnum.SurveyQuestionTypeEnumList.Select(item => new SurveyQuestionTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.SurveyQuestionType.BulkSynchronize(SurveyQuestionTypeEnumList);

            List<SurveyOptionTypeDAO> SurveyOptionTypeEnumList = SurveyOptionTypeEnum.SurveyOptionTypeEnumList.Select(item => new SurveyOptionTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.SurveyOptionType.BulkSynchronize(SurveyOptionTypeEnumList);

            List<SurveyRespondentTypeDAO> SurveyRespondentTypeEnumList = SurveyRespondentTypeEnum.SurveyRespondentTypeEnumList.Select(item => new SurveyRespondentTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.SurveyRespondentType.BulkSynchronize(SurveyRespondentTypeEnumList);
        }

        private void InitProblemStatusEnum()
        {
            List<ProblemStatusDAO> ProblemStatusEnumList = ProblemStatusEnum.ProblemStatusEnumList.Select(item => new ProblemStatusDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.ProblemStatus.BulkSynchronize(ProblemStatusEnumList);
        }

        private void InitPermissionEnum()
        {
            List<FieldTypeDAO> FieldTypeDAOs = FieldTypeEnum.List.Select(item => new FieldTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.FieldType.BulkSynchronize(FieldTypeDAOs);
            List<PermissionOperatorDAO> PermissionOperatorDAOs = new List<PermissionOperatorDAO>();
            List<PermissionOperatorDAO> ID = PermissionOperatorEnum.PermissionOperatorEnumForID.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.ID.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(ID);
            List<PermissionOperatorDAO> STRING = PermissionOperatorEnum.PermissionOperatorEnumForSTRING.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.STRING.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(STRING);

            List<PermissionOperatorDAO> LONG = PermissionOperatorEnum.PermissionOperatorEnumForLONG.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.LONG.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(LONG);

            List<PermissionOperatorDAO> DECIMAL = PermissionOperatorEnum.PermissionOperatorEnumForDECIMAL.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.DECIMAL.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(DECIMAL);

            List<PermissionOperatorDAO> DATE = PermissionOperatorEnum.PermissionOperatorEnumForDATE.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.DATE.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(DATE);

            DataContext.PermissionOperator.BulkSynchronize(PermissionOperatorDAOs);
        }
        private void InitSexEnum()
        {
            List<SexDAO> SexDAOs = SexEnum.SexEnumList.Select(item => new SexDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.Sex.BulkSynchronize(SexDAOs);
        }

        private void InitSystemConfigurationEnum()
        {
            List<SystemConfigurationDAO> SystemConfigurationDAOs = DataContext.SystemConfiguration.ToList();
            foreach (GenericEnum item in SystemConfigurationEnum.SystemConfigurationEnumList)
            {
                SystemConfigurationDAO SystemConfigurationDAO = SystemConfigurationDAOs.Where(sc => sc.Id == item.Id).FirstOrDefault();
                if (SystemConfigurationDAO == null)
                {
                    SystemConfigurationDAO = new SystemConfigurationDAO();
                    SystemConfigurationDAO.Id = item.Id;
                    SystemConfigurationDAO.Code = item.Code;
                    SystemConfigurationDAO.Name = item.Name;
                    SystemConfigurationDAO.Value = null;
                    SystemConfigurationDAOs.Add(SystemConfigurationDAO);
                }
            }
            DataContext.SystemConfiguration.BulkSynchronize(SystemConfigurationDAOs);
        }

        private void InitWorkflowEnum()
        {
            List<WorkflowTypeDAO> WorkflowTypeEnumList = WorkflowTypeEnum.WorkflowTypeEnumList.Select(item => new WorkflowTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.WorkflowType.BulkSynchronize(WorkflowTypeEnumList);
            List<WorkflowStateDAO> WorkflowStateEnumList = WorkflowStateEnum.WorkflowStateEnumList.Select(item => new WorkflowStateDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.WorkflowState.BulkSynchronize(WorkflowStateEnumList);
            List<RequestStateDAO> RequestStateEnumList = RequestStateEnum.RequestStateEnumList.Select(item => new RequestStateDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.RequestState.BulkSynchronize(RequestStateEnumList);
            List<WorkflowParameterTypeDAO> WorkflowParameterTypeDAOs = WorkflowParameterTypeEnum.List.Select(item => new WorkflowParameterTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.WorkflowParameterType.BulkSynchronize(WorkflowParameterTypeDAOs);

            List<WorkflowOperatorDAO> WorkflowOperatorDAOs = new List<WorkflowOperatorDAO>();
            List<WorkflowOperatorDAO> ID = WorkflowOperatorEnum.WorkflowOperatorEnumForID.Select(item => new WorkflowOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = WorkflowParameterTypeEnum.ID.Id,
            }).ToList();
            WorkflowOperatorDAOs.AddRange(ID);

            List<WorkflowOperatorDAO> STRING = WorkflowOperatorEnum.WorkflowOperatorEnumForSTRING.Select(item => new WorkflowOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = FieldTypeEnum.STRING.Id,
            }).ToList();
            WorkflowOperatorDAOs.AddRange(STRING);

            List<WorkflowOperatorDAO> LONG = WorkflowOperatorEnum.WorkflowOperatorEnumForLONG.Select(item => new WorkflowOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = FieldTypeEnum.LONG.Id,
            }).ToList();
            WorkflowOperatorDAOs.AddRange(LONG);

            List<WorkflowOperatorDAO> DECIMAL = WorkflowOperatorEnum.WorkflowOperatorEnumForDECIMAL.Select(item => new WorkflowOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = FieldTypeEnum.DECIMAL.Id,
            }).ToList();
            WorkflowOperatorDAOs.AddRange(DECIMAL);

            List<WorkflowOperatorDAO> DATE = WorkflowOperatorEnum.WorkflowOperatorEnumForDATE.Select(item => new WorkflowOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = FieldTypeEnum.DATE.Id,
            }).ToList();
            WorkflowOperatorDAOs.AddRange(DATE);

            DataContext.WorkflowOperator.BulkSynchronize(WorkflowOperatorDAOs);

            List<WorkflowParameterDAO> WorkflowParameterDAOs = new List<WorkflowParameterDAO>();

            List<WorkflowParameterDAO> EROUTE_PARAMETER = WorkflowParameterEnum.ERouteEnumList.Select(item => new WorkflowParameterDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = long.Parse(item.Value),
                WorkflowTypeId = WorkflowTypeEnum.EROUTE.Id,
            }).ToList();
            WorkflowParameterDAOs.AddRange(EROUTE_PARAMETER);

            List<WorkflowParameterDAO> INDIRECT_SALES_ORDER_PARAMETER = WorkflowParameterEnum.IndirectSalesOrderEnumList.Select(item => new WorkflowParameterDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = long.Parse(item.Value),
                WorkflowTypeId = WorkflowTypeEnum.INDIRECT_SALES_ORDER.Id,
            }).ToList();
            WorkflowParameterDAOs.AddRange(INDIRECT_SALES_ORDER_PARAMETER);

            List<WorkflowParameterDAO> DIRECT_SALES_ORDER_PARAMETER = WorkflowParameterEnum.DirectSalesOrderEnumList.Select(item => new WorkflowParameterDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = long.Parse(item.Value),
                WorkflowTypeId = WorkflowTypeEnum.DIRECT_SALES_ORDER.Id,
            }).ToList();
            WorkflowParameterDAOs.AddRange(DIRECT_SALES_ORDER_PARAMETER);

            List<WorkflowParameterDAO> PRICE_LIST_PARAMETER = WorkflowParameterEnum.PriceListEnumList.Select(item => new WorkflowParameterDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = long.Parse(item.Value),
                WorkflowTypeId = WorkflowTypeEnum.PRICE_LIST.Id,
            }).ToList();
            WorkflowParameterDAOs.AddRange(PRICE_LIST_PARAMETER);

            DataContext.WorkflowParameter.BulkMerge(WorkflowParameterDAOs);
        }

        private void InitColorEnum()
        {
            List<ColorDAO> ColorDAOs = ColorEnum.ColorEnumList.Select(item => new ColorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.Color.BulkSynchronize(ColorDAOs);
        }

        #endregion
    }
}