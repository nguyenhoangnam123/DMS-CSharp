using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Common;
using DMS.Enums;
using DMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DMS.Rpc
{
    public class SetupController : ControllerBase
    {
        private DataContext DataContext;
        public SetupController(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        [HttpGet, Route("rpc/dms/setup/init")]
        public ActionResult Init()
        {
            InitEnum();
            InitRoute();
            InitAdmin();
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
            DataContext.BulkMerge(ActionPageMappingDAOs);
        }

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

        [HttpGet, Route("rpc/dms/setup/init-enum")]
        public ActionResult InitEnum()
        {
            InitDirectPriceListTypeEnum();
            InitIndirectPriceListTypeEnum();
            InitEditedPriceStatusEnum();
            InitKpiCriteriaItemEnum();
            InitGeneralCriteriaEnum();
            InitProblemTypeEnum();
            InitProblemStatusEnum();
            InitResellerStatusEnum();
            InitRequestStateEnum();
            InitStatusEnum();
            InitSurveyQuestionTypeEnum();
            InitSurveyOptionTypeEnum();
            InitTotalItemSpecificCriteriaEnum();
            InitUsedVariationEnum();
            InitWorkflowStateEnum();
            InitWorkflowTypeEnum();
            DataContext.SaveChanges();
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

            List<PermissionActionMappingDAO> permissionPageMappings = permissions
                .SelectMany(p => p.PermissionActionMappings).ToList();
            DataContext.PermissionContent.Where(pf => pf.Permission.RoleId == Admin.Id).DeleteFromQuery();
            DataContext.PermissionActionMapping.Where(pf => pf.Permission.RoleId == Admin.Id).DeleteFromQuery();
            DataContext.PermissionActionMapping.BulkMerge(permissionPageMappings);
            return Ok();
        }

        private void InitStatusEnum()
        {
            List<StatusDAO> statuses = DataContext.Status.ToList();
            foreach (var item in StatusEnum.StatusEnumList)
            {
                if (!statuses.Any(pt => pt.Id == item.Id))
                {
                    DataContext.Status.Add(new StatusDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }

        private void InitKpiCriteriaItemEnum()
        {
            List<KpiCriteriaItemDAO> statuses = DataContext.KpiCriteriaItem.ToList();
            foreach (var item in KpiCriteriaItemEnum.KpiCriteriaItemEnumList)
            {
                if (!statuses.Any(pt => pt.Id == item.Id))
                {
                    DataContext.KpiCriteriaItem.Add(new KpiCriteriaItemDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }
        private void InitGeneralCriteriaEnum()
        {
            List<GeneralCriteriaDAO> statuses = DataContext.GeneralCriteria.ToList();
            foreach (var item in GeneralCriteriaEnum.KpiCriteriaEnumList)
            {
                if (!statuses.Any(pt => pt.Id == item.Id))
                {
                    DataContext.GeneralCriteria.Add(new GeneralCriteriaDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }

        private void InitTotalItemSpecificCriteriaEnum()
        {
            List<KpiCriteriaTotalDAO> statuses = DataContext.KpiCriteriaTotal.ToList();
            foreach (var item in KpiCriteriaTotalEnum.KpiCriteriaTotalEnumList)
            {
                if (!statuses.Any(pt => pt.Id == item.Id))
                {
                    DataContext.KpiCriteriaTotal.Add(new KpiCriteriaTotalDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }
        private void InitUsedVariationEnum()
        {
            List<UsedVariationDAO> statuses = DataContext.UsedVariation.ToList();
            foreach (var item in UsedVariationEnum.UsedVariationEnumList)
            {
                if (!statuses.Any(pt => pt.Id == item.Id))
                {
                    DataContext.UsedVariation.Add(new UsedVariationDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }
        private void InitEditedPriceStatusEnum()
        {
            List<EditedPriceStatusDAO> statuses = DataContext.EditedPriceStatus.ToList();
            foreach (var item in EditedPriceStatusEnum.EditedPriceStatusEnumList)
            {
                if (!statuses.Any(pt => pt.Id == item.Id))
                {
                    DataContext.EditedPriceStatus.Add(new EditedPriceStatusDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }
        private void InitResellerStatusEnum()
        {
            List<ResellerStatusDAO> resellerStatuses = DataContext.ResellerStatus.ToList();
            foreach (var item in ResellerStatusEnum.ResellerStatusEnumList)
            {
                if (!resellerStatuses.Any(pt => pt.Id == item.Id))
                {
                    DataContext.ResellerStatus.Add(new ResellerStatusDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }

        private void InitRequestStateEnum()
        {
            List<RequestStateDAO> statuses = DataContext.RequestState.ToList();
            foreach (var item in RequestStateEnum.RequestStateEnumList)
            {
                if (!statuses.Any(pt => pt.Id == item.Id))
                {
                    DataContext.RequestState.Add(new RequestStateDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }

        private void InitWorkflowStateEnum()
        {
            List<WorkflowStateDAO> list = DataContext.WorkflowState.ToList();
            foreach (var item in WorkflowStateEnum.WorkflowStateEnumList)
            {
                if (!list.Any(pt => pt.Id == item.Id))
                {
                    DataContext.WorkflowState.Add(new WorkflowStateDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }

        private void InitWorkflowTypeEnum()
        {
            List<WorkflowTypeDAO> list = DataContext.WorkflowType.ToList();
            foreach (var item in WorkflowTypeEnum.WorkflowTypeEnumList)
            {
                if (!list.Any(pt => pt.Id == item.Id))
                {
                    DataContext.WorkflowType.Add(new WorkflowTypeDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }

        private void InitIndirectPriceListTypeEnum()
        {
            List<IndirectPriceListTypeDAO> list = DataContext.IndirectPriceListType.ToList();
            foreach (var item in IndirectPriceListTypeEnum.IndirectPriceListTypeEnumList)
            {
                if (!list.Any(pt => pt.Id == item.Id))
                {
                    DataContext.IndirectPriceListType.Add(new IndirectPriceListTypeDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }

        private void InitDirectPriceListTypeEnum()
        {
            List<DirectPriceListTypeDAO> list = DataContext.DirectPriceListType.ToList();
            foreach (var item in DirectPriceListTypeEnum.DirectPriceListTypeEnumList)
            {
                if (!list.Any(pt => pt.Id == item.Id))
                {
                    DataContext.DirectPriceListType.Add(new DirectPriceListTypeDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }

        private void InitSurveyQuestionTypeEnum()
        {
            List<SurveyQuestionTypeDAO> list = DataContext.SurveyQuestionType.ToList();
            foreach (var item in SurveyQuestionTypeEnum.SurveyQuestionTypeEnumList)
            {
                if (!list.Any(pt => pt.Id == item.Id))
                {
                    DataContext.SurveyQuestionType.Add(new SurveyQuestionTypeDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }

        private void InitSurveyOptionTypeEnum()
        {
            List<SurveyOptionTypeDAO> list = DataContext.SurveyOptionType.ToList();
            foreach (var item in SurveyOptionTypeEnum.SurveyOptionTypeEnumList)
            {
                if (!list.Any(pt => pt.Id == item.Id))
                {
                    DataContext.SurveyOptionType.Add(new SurveyOptionTypeDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }

        private void InitProblemTypeEnum()
        {
            List<ProblemTypeDAO> list = DataContext.ProblemType.ToList();
            foreach (var item in ProblemTypeEnum.ProblemTypeEnumList)
            {
                if (!list.Any(pt => pt.Id == item.Id))
                {
                    DataContext.ProblemType.Add(new ProblemTypeDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }

        private void InitProblemStatusEnum()
        {
            List<ProblemStatusDAO> list = DataContext.ProblemStatus.ToList();
            foreach (var item in ProblemStatusEnum.ProblemStatusEnumList)
            {
                if (!list.Any(pt => pt.Id == item.Id))
                {
                    DataContext.ProblemStatus.Add(new ProblemStatusDAO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                    });
                }
            }
        }
    }
}