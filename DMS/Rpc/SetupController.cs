using DMS.Common;
using DMS.DWModels;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;
using DMS.Helpers;
using DMS.Models;
using DMS.Repositories;
using DMS.Services;
using DMS.Services.MProduct;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DMS.Rpc
{
    public class SetupController : ControllerBase
    {
        private DataContext DataContext;
        private DWContext DWContext;
        private IItemService ItemService;
        private IMaintenanceService MaintenanceService;
        private IRabbitManager RabbitManager;
        private IUOW UOW;
        public SetupController(DataContext DataContext, DWContext DWContext, IItemService ItemService, IMaintenanceService MaintenanceService, IRabbitManager RabbitManager, IUOW UOW)
        {
            this.ItemService = ItemService;
            this.MaintenanceService = MaintenanceService;
            this.DataContext = DataContext;
            this.DWContext = DWContext;
            this.RabbitManager = RabbitManager;
            this.UOW = UOW;
        }

        #region publish Data
        [HttpGet, Route("rpc/dms/setup/publish-data")]
        public async Task<ActionResult> PublishData()
        {
            await DirectSalesOrderPublish();
            await IndirectSalesOrderPublish();
            await ProblemPublish();
            await ProblemTypePublish();
            await ShowingOrderPublish();
            await ShowingOrderWithDrawPublish();
            await ShowingItemPublish();
            await StoreCheckingPublish();
            await StoreScoutingTypePublish();
            await StoreStatusHistoryPublish();
            await StoreUncheckingPublish();
            return Ok();
        }

        #region DirectSalesOrder
        public async Task DirectSalesOrderPublish()
        {
            List<long> DirectSalesOrderIds = await DataContext.DirectSalesOrder.Select(x => x.Id).ToListAsync();
            var DirectSalesOrders = await UOW.DirectSalesOrderRepository.List(DirectSalesOrderIds);
            var count = DirectSalesOrders.Count();
            var BatchCounter = (count / 1000) + 1;
            for (int i = 0; i < count; i += 1000)
            {
                int skip = i * 1000;
                int take = 1000;
                var Batch = DirectSalesOrders.Skip(skip).Take(take).ToList();
                RabbitManager.PublishList(Batch, RoutingKeyEnum.DirectSalesOrderSync);
            }
        }
        #endregion

        #region IndirectSalesOrder
        public async Task IndirectSalesOrderPublish()
        {
            List<long> IndirectSalesOrderIds = await DataContext.IndirectSalesOrder.Select(x => x.Id).ToListAsync();
            var IndirectSalesOrders = await UOW.IndirectSalesOrderRepository.List(IndirectSalesOrderIds);
            var count = IndirectSalesOrders.Count();
            var BatchCounter = (count / 1000) + 1;
            for (int i = 0; i < count; i += 1000)
            {
                int skip = i * 1000;
                int take = 1000;
                var Batch = IndirectSalesOrders.Skip(skip).Take(take).ToList();
                RabbitManager.PublishList(Batch, RoutingKeyEnum.IndirectSalesOrderSync);
            }
        }
        #endregion

        #region Problem
        public async Task ProblemPublish()
        {
            List<long> ProblemIds = await DataContext.Problem.Select(x => x.Id).ToListAsync();
            var Problems = await UOW.ProblemRepository.List(ProblemIds);
            var count = Problems.Count();
            var BatchCounter = (count / 1000) + 1;
            for (int i = 0; i < count; i += 1000)
            {
                int skip = i * 1000;
                int take = 1000;
                var Batch = Problems.Skip(skip).Take(take).ToList();
                RabbitManager.PublishList(Batch, RoutingKeyEnum.ProblemSync);
            }
        }
        #endregion

        public async Task ProblemTypePublish()
        {
            List<long> ProblemTypeIds = await DataContext.ProblemType.Select(x => x.Id).ToListAsync();
            var ProblemTypes = await UOW.ProblemTypeRepository.List(ProblemTypeIds);
            RabbitManager.PublishList(ProblemTypes, RoutingKeyEnum.ProblemTypeSync);
        }

        #region Store
        public async Task PublishStore()
        {
            List<long> StoreIds = await DataContext.Store.Select(x => x.Id).ToListAsync();
            var Stores = await UOW.StoreRepository.List(StoreIds);

            var count = Stores.Count();
            var BatchCounter = (count / 1000) + 1;
            for (int i = 0; i < count; i += 1000)
            {
                int skip = i * 1000;
                int take = 1000;
                var Batch = Stores.Skip(skip).Take(take).ToList();
                RabbitManager.PublishList(Batch, RoutingKeyEnum.StoreSync);
            }
        }
        #endregion

        #region StoreGrouping
        public async Task PublishStoreGrouping()
        {
            List<long> StoreGroupingIds = await DataContext.StoreGrouping.Select(x => x.Id).ToListAsync();
            var StoreGroupinges = await UOW.StoreGroupingRepository.List(StoreGroupingIds);
            RabbitManager.PublishList(StoreGroupinges, RoutingKeyEnum.StoreGroupingSync);
        }
        #endregion

        #region StoreType
        public async Task PublishStoreType()
        {
            List<long> StoreTypeIds = await DataContext.StoreType.Select(x => x.Id).ToListAsync();
            var StoreTypes = await UOW.StoreTypeRepository.List(StoreTypeIds);
            RabbitManager.PublishList(StoreTypes, RoutingKeyEnum.StoreTypeSync);
        }
        #endregion

        public async Task ShowingItemPublish()
        {
            List<long> ShowingItemIds = await DataContext.ShowingItem.Select(x => x.Id).ToListAsync();
            var ShowingItems = await UOW.ShowingItemRepository.List(ShowingItemIds);
            RabbitManager.PublishList(ShowingItems, RoutingKeyEnum.ShowingItemSync);
        }

        public async Task ShowingOrderPublish()
        {
            List<long> ShowingOrderIds = await DataContext.ShowingOrder.Select(x => x.Id).ToListAsync();
            var ShowingOrders = await UOW.ShowingOrderRepository.List(ShowingOrderIds);

            var count = ShowingOrders.Count();
            var BatchCounter = (count / 1000) + 1;
            for (int i = 0; i < count; i += 1000)
            {
                int skip = i * 1000;
                int take = 1000;
                var Batch = ShowingOrders.Skip(skip).Take(take).ToList();
                RabbitManager.PublishList(Batch, RoutingKeyEnum.ShowingOrderSync);
            }
        }

        public async Task ShowingOrderWithDrawPublish()
        {
            List<long> ShowingOrderWithDrawIds = await DataContext.ShowingOrderWithDraw.Select(x => x.Id).ToListAsync();
            var ShowingOrderWithDraws = await UOW.ShowingOrderWithDrawRepository.List(ShowingOrderWithDrawIds);

            var count = ShowingOrderWithDraws.Count();
            var BatchCounter = (count / 1000) + 1;
            for (int i = 0; i < count; i += 1000)
            {
                int skip = i * 1000;
                int take = 1000;
                var Batch = ShowingOrderWithDraws.Skip(skip).Take(take).ToList();
                RabbitManager.PublishList(Batch, RoutingKeyEnum.ShowingOrderWithDrawSync);
            }
        }

        public async Task StoreCheckingPublish()
        {
            List<long> StoreCheckingIds = await DataContext.StoreChecking.Select(x => x.Id).ToListAsync();
            var StoreCheckings = await UOW.StoreCheckingRepository.List(StoreCheckingIds);

            var count = StoreCheckings.Count();
            var BatchCounter = (count / 1000) + 1;
            for (int i = 0; i < count; i += 1000)
            {
                int skip = i * 1000;
                int take = 1000;
                var Batch = StoreCheckings.Skip(skip).Take(take).ToList();
                RabbitManager.PublishList(Batch, RoutingKeyEnum.StoreCheckingSync);
            }
        }

        public async Task StoreScoutingPublish()
        {
            List<long> StoreScoutingIds = await DataContext.StoreScouting.Select(x => x.Id).ToListAsync();
            var StoreScoutings = await UOW.StoreScoutingRepository.List(StoreScoutingIds);

            var count = StoreScoutings.Count();
            var BatchCounter = (count / 1000) + 1;
            for (int i = 0; i < count; i += 1000)
            {
                int skip = i * 1000;
                int take = 1000;
                var Batch = StoreScoutings.Skip(skip).Take(take).ToList();
                RabbitManager.PublishList(Batch, RoutingKeyEnum.StoreScoutingSync);
            }
        }

        public async Task StoreScoutingTypePublish()
        {
            List<long> StoreScoutingTypeIds = await DataContext.StoreScoutingType.Select(x => x.Id).ToListAsync();
            var StoreScoutingTypes = await UOW.StoreScoutingTypeRepository.List(StoreScoutingTypeIds);
            RabbitManager.PublishList(StoreScoutingTypes, RoutingKeyEnum.StoreScoutingTypeSync);
        }

        public async Task StoreStatusHistoryPublish()
        {
            List<long> StoreStatusHistoryIds = await DataContext.StoreStatusHistory.Select(x => x.Id).ToListAsync();
            var StoreStatusHistories = await UOW.StoreStatusHistoryRepository.List(StoreStatusHistoryIds);
            RabbitManager.PublishList(StoreStatusHistories, RoutingKeyEnum.StoreStatusHistorySync);
        }

        public async Task StoreUncheckingPublish()
        {
            List<StoreUnchecking> StoreUncheckings = await DataContext.StoreUnchecking.Select(x => new StoreUnchecking
            {
                Id = x.Id,
                AppUserId = x.AppUserId,
                OrganizationId = x.OrganizationId,
                StoreId = x.StoreId,
                Date = x.Date,
                Store = x.Store == null ? null : new Store
                {
                    Id = x.Store.Id,
                    Code = x.Store.Code,
                    CodeDraft = x.Store.CodeDraft,
                    Name = x.Store.Name,
                    ParentStoreId = x.Store.ParentStoreId,
                    OrganizationId = x.Store.OrganizationId,
                    StoreTypeId = x.Store.StoreTypeId,
                    StoreGroupingId = x.Store.StoreGroupingId,
                    Telephone = x.Store.Telephone,
                    ProvinceId = x.Store.ProvinceId,
                    DistrictId = x.Store.DistrictId,
                    WardId = x.Store.WardId,
                    Address = x.Store.Address,
                    DeliveryAddress = x.Store.DeliveryAddress,
                    Latitude = x.Store.Latitude,
                    Longitude = x.Store.Longitude,
                    OwnerName = x.Store.OwnerName,
                    OwnerPhone = x.Store.OwnerPhone,
                    OwnerEmail = x.Store.OwnerEmail,
                    TaxCode = x.Store.TaxCode,
                    LegalEntity = x.Store.LegalEntity,
                    StatusId = x.Store.StatusId,
                },
                AppUser = x.AppUser == null ? null : new AppUser
                {
                    Id = x.AppUser.Id,
                    Username = x.AppUser.Username,
                    DisplayName = x.AppUser.DisplayName,
                    Email = x.AppUser.Email,
                    Phone = x.AppUser.Phone,
                    Address = x.AppUser.Address,
                    Department = x.AppUser.Department,
                    PositionId = x.AppUser.PositionId,
                    RowId = x.AppUser.RowId,
                    SexId = x.AppUser.SexId,
                    StatusId = x.AppUser.StatusId,
                    OrganizationId = x.AppUser.OrganizationId,
                    Organization = x.AppUser.Organization == null ? null : new Organization
                    {
                        Id = x.AppUser.Organization.Id,
                        Code = x.AppUser.Organization.Code,
                        Name = x.AppUser.Organization.Name,
                    },
                },
            }).ToListAsync();

            var count = StoreUncheckings.Count();
            var BatchCounter = (count / 1000) + 1;
            for (int i = 0; i < count; i += 1000)
            {
                int skip = i * 1000;
                int take = 1000;
                var Batch = StoreUncheckings.Skip(skip).Take(take).ToList();
                RabbitManager.PublishList(Batch, RoutingKeyEnum.StoreUncheckingSync);
            }
        }

        #endregion

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
            InitRoute();
            InitAdmin();
            return Ok();
        }

        #region permission
        private ActionResult InitRoute()
        {
            List<Type> routeTypes = typeof(SetupController).Assembly.GetTypes()
               .Where(x => typeof(Root).IsAssignableFrom(x) && x.IsClass && x.Name != "Root")
               .ToList();

            InitMenu(routeTypes);
            InitPage(routeTypes);
            InitField(routeTypes);
            InitAction(routeTypes);

            DataContext.ActionPageMapping.Where(ap => ap.Action.IsDeleted || ap.Page.IsDeleted || ap.Action.Menu.IsDeleted).DeleteFromQuery();
            DataContext.PermissionActionMapping.Where(ap => ap.Action.IsDeleted || ap.Action.Menu.IsDeleted || ap.Permission.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Action.Where(p => p.IsDeleted || p.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Page.Where(p => p.IsDeleted).DeleteFromQuery();
            DataContext.PermissionContent.Where(f => f.Field.IsDeleted == true || f.Field.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Field.Where(pf => pf.IsDeleted || pf.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Permission.Where(p => p.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Menu.Where(v => v.IsDeleted).DeleteFromQuery();
            return Ok();
        }

        private ActionResult InitAdmin()
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
                MenuDAO Menu = Menus.Where(m => m.Code == type.Name && m.Name != "Root").FirstOrDefault();
                var DisplayName = type.GetCustomAttributes(typeof(DisplayNameAttribute), true)
               .Select(x => ((DisplayNameAttribute)x).DisplayName)
               .DefaultIfEmpty(type.Name)
               .FirstOrDefault();

                if (Menu == null)
                {
                    Menu = new MenuDAO
                    {
                        Code = type.Name,
                        Name = DisplayName,
                        IsDeleted = false,
                    };
                    Menus.Add(Menu);
                }
                else
                {
                    Menu.Name = DisplayName;
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
               .Where(fi => !fi.IsInitOnly && fi.FieldType == typeof(Dictionary<string, IEnumerable<string>>))
               .Select(x => (Dictionary<string, IEnumerable<string>>)x.GetValue(x))
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
               .Where(fi => !fi.IsInitOnly && fi.FieldType == typeof(Dictionary<string, IEnumerable<string>>))
               .Select(x => (Dictionary<string, IEnumerable<string>>)x.GetValue(x))
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
                    IEnumerable<string> pages = pair.Value;
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
        private ActionResult InitEnum()
        {
            InitStoreStatusEnum();
            InitStoreStatusHistoryTypeEnum();
            InitPriceListTypeEnum();
            InitSalesOrderTypeEnum();
            InitEditedPriceStatusEnum();
            InitProblemStatusEnum();
            InitERouteTypeEnum();
            InitNotificationStatusEnum();
            InitSurveyEnum();
            InitKpiEnum();
            InitPermissionEnum();
            InitStoreScoutingStatusEnum();
            InitSystemConfigurationEnum();
            InitWorkflowEnum();
            InitPromotionTypeEnum();
            InitPromotionProductAppliedTypeEnum();
            InitPromotionPolicyEnum();
            InitPromotionDiscountTypeEnum();
            InitRewardStatusEnum();
            InitTransactionTypeEnum();
            InitStoreApprovalStateEnum();
            InitErpApprovalStateEnum();
            InitDirectSalesOrderSourceTypeEnum();
            InitExportTemplateEnum();
            return Ok();
        }

        private void InitStoreStatusHistoryTypeEnum()
        {
            List<StoreStatusHistoryTypeDAO> StoreStatusHistoryTypeDAOs = StoreStatusHistoryTypeEnum.StoreStatusHistoryTypeEnumList.Select(x => new StoreStatusHistoryTypeDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            DataContext.StoreStatusHistoryType.BulkSynchronize(StoreStatusHistoryTypeDAOs);
        }

        private void InitStoreStatusEnum()
        {
            List<StoreStatusDAO> StoreStatusDAOs = StoreStatusEnum.StoreStatusEnumList.Select(x => new StoreStatusDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            DataContext.StoreStatus.BulkSynchronize(StoreStatusDAOs);
            List<StoreStatus> StoreStatuses = StoreStatusDAOs.Select(x => new StoreStatus
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();

            RabbitManager.PublishList(StoreStatuses, RoutingKeyEnum.StoreStatusSync);
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

        private void InitTransactionTypeEnum()
        {
            List<TransactionTypeDAO> TransactionTypeEnumList = TransactionTypeEnum.TransactionTypeEnumList.Select(item => new TransactionTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.TransactionType.BulkSynchronize(TransactionTypeEnumList);
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

            List<KpiProductGroupingTypeDAO> KpiProductGroupingTypeDAOs = KpiProductGroupingTypeEnum.KpiProductGroupingTypeEnumList.Select(item => new KpiProductGroupingTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.KpiProductGroupingType.BulkSynchronize(KpiProductGroupingTypeDAOs);

            List<KpiProductGroupingCriteriaDAO> KpiProductGroupingCriteriaTypeDAOs = KpiProductGroupingCriteriaEnum.KpiProductGroupingCriteriaEnumList.Select(item => new KpiProductGroupingCriteriaDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.KpiProductGroupingCriteria.BulkSynchronize(KpiProductGroupingCriteriaTypeDAOs);
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


            List<WorkflowTypeDAO> WorkflowTypeEnumList = WorkflowTypeEnum.WorkflowTypeEnumList.Select(item => new WorkflowTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.WorkflowType.BulkSynchronize(WorkflowTypeEnumList);

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
        }

        private void InitStoreApprovalStateEnum()
        {
            List<StoreApprovalStateDAO> StoreApprovalStateDAOs = DataContext.StoreApprovalState.ToList();
            foreach (GenericEnum item in StoreApprovalStateEnum.StoreApprovalStateEnumList)
            {
                StoreApprovalStateDAO StoreApprovalStateDAO = StoreApprovalStateDAOs.Where(sc => sc.Id == item.Id).FirstOrDefault();
                if (StoreApprovalStateDAO == null)
                {
                    StoreApprovalStateDAO = new StoreApprovalStateDAO();
                    StoreApprovalStateDAO.Id = item.Id;
                    StoreApprovalStateDAO.Code = item.Code;
                    StoreApprovalStateDAO.Name = item.Name;
                    StoreApprovalStateDAOs.Add(StoreApprovalStateDAO);
                }
            }
            DataContext.StoreApprovalState.BulkSynchronize(StoreApprovalStateDAOs);
        }

        private void InitErpApprovalStateEnum()
        {
            List<ErpApprovalStateDAO> ErpApprovalStateDAOs = DataContext.ErpApprovalState.ToList();
            foreach (GenericEnum item in ErpApprovalStateEnum.ErpApprovalStateEnumList)
            {
                ErpApprovalStateDAO ErpApprovalStateDAO = ErpApprovalStateDAOs.Where(sc => sc.Id == item.Id).FirstOrDefault();
                if (ErpApprovalStateDAO == null)
                {
                    ErpApprovalStateDAO = new ErpApprovalStateDAO();
                    ErpApprovalStateDAO.Id = item.Id;
                    ErpApprovalStateDAO.Code = item.Code;
                    ErpApprovalStateDAO.Name = item.Name;
                    ErpApprovalStateDAOs.Add(ErpApprovalStateDAO);
                }
            }
            DataContext.ErpApprovalState.BulkSynchronize(ErpApprovalStateDAOs);
        }

        private void InitDirectSalesOrderSourceTypeEnum()
        {
            List<DirectSalesOrderSourceTypeDAO> DirectSalesOrderSourceTypeDAOs = DataContext.DirectSalesOrderSourceType.ToList();
            foreach (GenericEnum item in DirectSalesOrderSourceTypeEnum.DirectSalesOrderSourceTypeEnumList)
            {
                DirectSalesOrderSourceTypeDAO DirectSalesOrderSourceTypeDAO = DirectSalesOrderSourceTypeDAOs.Where(sc => sc.Id == item.Id).FirstOrDefault();
                if (DirectSalesOrderSourceTypeDAO == null)
                {
                    DirectSalesOrderSourceTypeDAO = new DirectSalesOrderSourceTypeDAO();
                    DirectSalesOrderSourceTypeDAO.Id = item.Id;
                    DirectSalesOrderSourceTypeDAO.Code = item.Code;
                    DirectSalesOrderSourceTypeDAO.Name = item.Name;
                    DirectSalesOrderSourceTypeDAOs.Add(DirectSalesOrderSourceTypeDAO);
                }
            }
            DataContext.DirectSalesOrderSourceType.BulkSynchronize(DirectSalesOrderSourceTypeDAOs);
        }

        private void InitExportTemplateEnum()
        {
            List<ExportTemplateDAO> ExportTemplateDAOs = DataContext.ExportTemplate.ToList();
            foreach (GenericEnum item in ExportTemplateEnum.ExportTemplateEnumList)
            {
                ExportTemplateDAO ExportTemplateDAO = ExportTemplateDAOs.Where(sc => sc.Id == item.Id).FirstOrDefault();
                if (ExportTemplateDAO == null)
                {
                    ExportTemplateDAO = new ExportTemplateDAO();
                    ExportTemplateDAO.Id = item.Id;
                    ExportTemplateDAO.Code = item.Code;
                    ExportTemplateDAO.Name = item.Name;
                    ExportTemplateDAOs.Add(ExportTemplateDAO);
                }
            }
            DataContext.ExportTemplate.BulkSynchronize(ExportTemplateDAOs);
        }
        #endregion

        [HttpPost, Route("rpc/dms/setup/remove-invalid-brand-in-store")]
        public async Task<IActionResult> RemoveInvalidBrandInStore(IFormFile file)
        {
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["code"];
                if (worksheet == null)
                    return BadRequest("File không đúng biểu mẫu import");
                int StoreCodeColumn = 1; // mã code các cửa hàng cần xóa xếp hạng điểm bán
                int StartRow = 1;
                List<StoreDAO> StoreDAOs = await DataContext.Store.AsNoTracking()
                    .Select(x => new StoreDAO
                    {
                        Id = x.Id,
                        Code = x.Code,
                    }).ToListAsync();
                List<long> StoreIds = new List<long>();
                for (int i = 0; i < worksheet.Dimension.End.Row; i++)
                {
                    string StoreCode = worksheet.Cells[i + StartRow, StoreCodeColumn].Value?.ToString(); // lấy hết các mã cửa hàng
                    if(string.IsNullOrEmpty(StoreCode))
                    {

                    }
                    StoreDAO Store = StoreDAOs.Where(x => x.Code.Trim().Equals(StoreCode.Trim())).FirstOrDefault();
                    if (Store != null)
                        StoreIds.Add(Store.Id);
                    if(Store == null)
                    {

                    }
                }
                List<long> BrandInStoreIds = await DataContext.BrandInStore.AsNoTracking()
                    .Where(x => StoreIds.Contains(x.StoreId))
                    .Select(x => x.Id)
                    .ToListAsync();
                await DataContext.BrandInStoreProductGroupingMapping
                    .Where(x => BrandInStoreIds.Contains(x.BrandInStoreId))
                    .DeleteFromQueryAsync();
                await DataContext.BrandInStore
                    .Where(x => BrandInStoreIds.Contains(x.Id))
                    .DeleteFromQueryAsync();
            }
            return Ok();
        }
    }
}