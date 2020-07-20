using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Helpers;
using DMS.Repositories;
using DMS.Services.MOrganization;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MKpiItem
{
    public interface IKpiItemService : IServiceScoped
    {
        Task<int> Count(KpiItemFilter KpiItemFilter);
        Task<List<KpiItem>> List(KpiItemFilter KpiItemFilter);
        Task<KpiItem> Get(long Id);
        Task<KpiItem> Create(KpiItem KpiItem);
        Task<KpiItem> Update(KpiItem KpiItem);
        Task<KpiItem> Delete(KpiItem KpiItem);
        Task<List<KpiItem>> BulkDelete(List<KpiItem> KpiItems);
        Task<List<KpiItem>> Import(List<KpiItem> KpiItems);
        Task<int> CountAppUser(AppUserFilter AppUserFilter, IdFilter KpiYearId, IdFilter KpiPeriodId);
        Task<List<AppUser>> ListAppUser(AppUserFilter AppUserFilter, IdFilter KpiYearId, IdFilter KpiPeriodId);

        Task<KpiItemFilter> ToFilter(KpiItemFilter KpiItemFilter);
    }

    public class KpiItemService : BaseService, IKpiItemService
    {
        private IUOW UOW;
        private ILogging Logging;
        private IOrganizationService OrganizationService;
        private ICurrentContext CurrentContext;
        private IKpiItemValidator KpiItemValidator;

        public KpiItemService(
            IUOW UOW,
            ILogging Logging,
            IOrganizationService OrganizationService,
            ICurrentContext CurrentContext,
            IKpiItemValidator KpiItemValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.OrganizationService = OrganizationService;
            this.CurrentContext = CurrentContext;
            this.KpiItemValidator = KpiItemValidator;
        }
        public async Task<int> Count(KpiItemFilter KpiItemFilter)
        {
            try
            {
                int result = await UOW.KpiItemRepository.Count(KpiItemFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<KpiItem>> List(KpiItemFilter KpiItemFilter)
        {
            try
            {
                List<KpiItem> KpiItems = await UOW.KpiItemRepository.List(KpiItemFilter);
                DateTime Now = StaticParams.DateTimeNow;
                foreach (var KpiItem in KpiItems)
                {
                    if (KpiItem.KpiPeriodId < 113)
                    {
                        DateTime periodToDateTime = new DateTime(Convert.ToInt32(KpiItem.KpiYearId), Convert.ToInt32(KpiItem.KpiPeriodId - 100), 1).AddMonths(1).AddSeconds(-1);
                        if (Now > periodToDateTime)
                            KpiItem.ReadOnly = true;
                    }
                    else if (KpiItem.KpiPeriodId < 204)
                    {
                        var firstMonthOfQuarter = (KpiItem.KpiPeriodId - 200) * 3 - 2;
                        DateTime periodToDateTime = new DateTime(Convert.ToInt32(KpiItem.KpiYearId), Convert.ToInt32(firstMonthOfQuarter), 1).AddMonths(3).AddSeconds(-1);
                        if (Now > periodToDateTime)
                            KpiItem.ReadOnly = true;
                    }
                }
                return KpiItems;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<KpiItem> Get(long Id)
        {
            KpiItem KpiItem = await UOW.KpiItemRepository.Get(Id);
            if (KpiItem == null)
                return null;
            return KpiItem;
        }

        public async Task<KpiItem> Create(KpiItem KpiItem)
        {
            if (!await KpiItemValidator.Create(KpiItem))
                return KpiItem;

            try
            {
                await UOW.Begin();
                List<KpiItem> KpiItems = new List<KpiItem>();
                if (KpiItem.EmployeeIds != null && KpiItem.EmployeeIds.Any())
                {
                    foreach (var EmployeeId in KpiItem.EmployeeIds)
                    {
                        var newObj = Utils.Clone(KpiItem);
                        newObj.EmployeeId = EmployeeId;
                        newObj.CreatorId = CurrentContext.UserId;
                        KpiItems.Add(newObj);
                    }
                }
                await UOW.KpiItemRepository.BulkMerge(KpiItems);
                await UOW.Commit();

                await Logging.CreateAuditLog(KpiItem, new { }, nameof(KpiItemService));
                return KpiItem;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<KpiItem> Update(KpiItem KpiItem)
        {
            if (!await KpiItemValidator.Update(KpiItem))
                return KpiItem;
            try
            {
                var oldData = await UOW.KpiItemRepository.Get(KpiItem.Id);

                await UOW.Begin();
                await UOW.KpiItemRepository.Update(KpiItem);
                await UOW.Commit();

                var newData = await UOW.KpiItemRepository.Get(KpiItem.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(KpiItemService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<KpiItem> Delete(KpiItem KpiItem)
        {
            if (!await KpiItemValidator.Delete(KpiItem))
                return KpiItem;

            try
            {
                await UOW.Begin();
                await UOW.KpiItemRepository.Delete(KpiItem);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, KpiItem, nameof(KpiItemService));
                return KpiItem;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<KpiItem>> BulkDelete(List<KpiItem> KpiItems)
        {
            if (!await KpiItemValidator.BulkDelete(KpiItems))
                return KpiItems;

            try
            {
                await UOW.Begin();
                await UOW.KpiItemRepository.BulkDelete(KpiItems);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, KpiItems, nameof(KpiItemService));
                return KpiItems;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<KpiItem>> Import(List<KpiItem> KpiItems)
        {
            if (!await KpiItemValidator.Import(KpiItems))
                return KpiItems;
            try
            {
                await UOW.Begin();
                await UOW.KpiItemRepository.BulkMerge(KpiItems);
                await UOW.Commit();

                await Logging.CreateAuditLog(KpiItems, new { }, nameof(KpiItemService));
                return KpiItems;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<AppUser>> ListAppUser(AppUserFilter AppUserFilter, IdFilter KpiYearId, IdFilter KpiPeriodId)
        {
            try
            {
                KpiItemFilter KpiItemFilter = new KpiItemFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    KpiYearId = KpiYearId,
                    KpiPeriodId = KpiPeriodId,
                    Selects = KpiItemSelect.Id | KpiItemSelect.Employee
                };

                var KpiItems = await UOW.KpiItemRepository.List(KpiItemFilter);
                var AppUserIds = KpiItems.Select(x => x.EmployeeId).ToList();
                AppUserFilter.Id = new IdFilter { NotIn = AppUserIds };
                AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName | AppUserSelect.Phone | AppUserSelect.Email;

                var AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                return AppUsers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                    throw new MessageException(ex.InnerException);
                }
            }

        }

        public async Task<int> CountAppUser(AppUserFilter AppUserFilter, IdFilter KpiYearId, IdFilter KpiPeriodId)
        {
            try
            {
                KpiItemFilter KpiItemFilter = new KpiItemFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    KpiYearId = KpiYearId,
                    KpiPeriodId = KpiPeriodId,
                    Selects = KpiItemSelect.Id | KpiItemSelect.Employee
                };

                var KpiItems = await UOW.KpiItemRepository.List(KpiItemFilter);
                var AppUserIds = KpiItems.Select(x => x.EmployeeId).ToList();

                AppUserFilter.Id = new IdFilter { NotIn = AppUserIds };

                var count = await UOW.AppUserRepository.Count(AppUserFilter);
                return count;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                    throw new MessageException(ex.InnerException);
                }
            }

        }

        public async Task<KpiItemFilter> ToFilter(KpiItemFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<KpiItemFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;

            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.ALL,
                OrderBy = OrganizationOrder.Id,
                OrderType = OrderType.ASC
            });

            foreach (var currentFilter in CurrentContext.Filters)
            {
                KpiItemFilter subFilter = new KpiItemFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);

                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                    {
                        var organizationIds = FilterOrganization(Organizations, FilterPermissionDefinition.IdFilter);
                        IdFilter IdFilter = new IdFilter { In = organizationIds };
                        subFilter.OrganizationId = FilterBuilder.Merge(subFilter.OrganizationId, IdFilter);
                    }

                    if (FilterPermissionDefinition.Name == nameof(subFilter.AppUserId))
                        subFilter.AppUserId = FilterBuilder.Merge(subFilter.AppUserId, FilterPermissionDefinition.IdFilter);


                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                            if (subFilter.CreatorId == null) subFilter.CreatorId = new IdFilter { };
                            subFilter.CreatorId.Equal = CurrentContext.UserId;
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                            if (subFilter.CreatorId == null) subFilter.CreatorId = new IdFilter { };
                            subFilter.CreatorId.NotEqual = CurrentContext.UserId;
                        }
                    }
                }
            }
            return filter;
        }
    }
}
