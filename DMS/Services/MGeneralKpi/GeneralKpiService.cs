using Common;
using DMS.Entities;
using DMS.Helpers;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MGeneralKpi
{
    public interface IGeneralKpiService : IServiceScoped
    {
        Task<int> Count(GeneralKpiFilter GeneralKpiFilter);
        Task<List<GeneralKpi>> List(GeneralKpiFilter GeneralKpiFilter);
        Task<GeneralKpi> Get(long Id);
        Task<GeneralKpi> Create(GeneralKpi GeneralKpi);
        Task<GeneralKpi> Update(GeneralKpi GeneralKpi);
        Task<GeneralKpi> Delete(GeneralKpi GeneralKpi);
        Task<List<GeneralKpi>> BulkDelete(List<GeneralKpi> GeneralKpis);
        Task<List<GeneralKpi>> Import(List<GeneralKpi> GeneralKpis);
        Task<List<AppUser>> ListAppUser(AppUserFilter AppUserFilter, IdFilter KpiPeriodId);
        Task<int> CountAppUser(AppUserFilter AppUserFilter, IdFilter KpiPeriodId);
        GeneralKpiFilter ToFilter(GeneralKpiFilter GeneralKpiFilter);
    }

    public class GeneralKpiService : BaseService, IGeneralKpiService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IGeneralKpiValidator GeneralKpiValidator;

        public GeneralKpiService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IGeneralKpiValidator GeneralKpiValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.GeneralKpiValidator = GeneralKpiValidator;
        }
        public async Task<int> Count(GeneralKpiFilter GeneralKpiFilter)
        {
            try
            {
                int result = await UOW.GeneralKpiRepository.Count(GeneralKpiFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<GeneralKpi>> List(GeneralKpiFilter GeneralKpiFilter)
        {
            try
            {
                List<GeneralKpi> GeneralKpis = await UOW.GeneralKpiRepository.List(GeneralKpiFilter);
                return GeneralKpis;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<GeneralKpi> Get(long Id)
        {
            GeneralKpi GeneralKpi = await UOW.GeneralKpiRepository.Get(Id);
            if (GeneralKpi == null)
                return null;
            return GeneralKpi;
        }

        public async Task<GeneralKpi> Create(GeneralKpi GeneralKpi)
        {
            if (!await GeneralKpiValidator.Create(GeneralKpi))
                return GeneralKpi;

            try
            {
                await UOW.Begin();
                List<GeneralKpi> GeneralKpis = new List<GeneralKpi>();
                if (GeneralKpi.EmployeeIds != null && GeneralKpi.EmployeeIds.Any())
                {
                    foreach (var EmployeeId in GeneralKpi.EmployeeIds)
                    {
                        var newObj = Utils.Clone(GeneralKpi);
                        newObj.EmployeeId = EmployeeId;
                        newObj.CreatorId = CurrentContext.UserId;
                        GeneralKpis.Add(newObj);
                    }
                }
                await UOW.GeneralKpiRepository.BulkMerge(GeneralKpis);
                await UOW.Commit();

                await Logging.CreateAuditLog(GeneralKpis, new { }, nameof(GeneralKpiService));
                return GeneralKpi;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<GeneralKpi> Update(GeneralKpi GeneralKpi)
        {
            if (!await GeneralKpiValidator.Update(GeneralKpi))
                return GeneralKpi;
            try
            {
                var oldData = await UOW.GeneralKpiRepository.Get(GeneralKpi.Id);

                await UOW.Begin();
                await UOW.GeneralKpiRepository.Update(GeneralKpi);
                await UOW.Commit();

                var newData = await UOW.GeneralKpiRepository.Get(GeneralKpi.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(GeneralKpiService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<GeneralKpi> Delete(GeneralKpi GeneralKpi)
        {
            if (!await GeneralKpiValidator.Delete(GeneralKpi))
                return GeneralKpi;

            try
            {
                await UOW.Begin();
                await UOW.GeneralKpiRepository.Delete(GeneralKpi);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, GeneralKpi, nameof(GeneralKpiService));
                return GeneralKpi;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<GeneralKpi>> BulkDelete(List<GeneralKpi> GeneralKpis)
        {
            if (!await GeneralKpiValidator.BulkDelete(GeneralKpis))
                return GeneralKpis;

            try
            {
                await UOW.Begin();
                await UOW.GeneralKpiRepository.BulkDelete(GeneralKpis);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, GeneralKpis, nameof(GeneralKpiService));
                return GeneralKpis;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<GeneralKpi>> Import(List<GeneralKpi> GeneralKpis)
        {
            if (!await GeneralKpiValidator.Import(GeneralKpis))
                return GeneralKpis;
            try
            {
                await UOW.Begin();
                await UOW.GeneralKpiRepository.BulkMerge(GeneralKpis);
                await UOW.Commit();

                await Logging.CreateAuditLog(GeneralKpis, new { }, nameof(GeneralKpiService));
                return GeneralKpis;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<AppUser>> ListAppUser(AppUserFilter AppUserFilter, IdFilter KpiPeriodId)
        {
            try
            {
                GeneralKpiFilter GeneralKpiFilter = new GeneralKpiFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    KpiPeriodId = KpiPeriodId,
                    Selects = GeneralKpiSelect.Id | GeneralKpiSelect.Employee
                };

                var GeneralKpis = await UOW.GeneralKpiRepository.List(GeneralKpiFilter);
                var AppUserIds = GeneralKpis.Select(x => x.EmployeeId).ToList();
                AppUserFilter = new AppUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { NotIn = AppUserIds },
                    Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName | AppUserSelect.Phone | AppUserSelect.Email
                };

                var AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                return AppUsers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
            
        }

        public async Task<int> CountAppUser(AppUserFilter AppUserFilter, IdFilter KpiPeriodId)
        {
            try
            {
                GeneralKpiFilter GeneralKpiFilter = new GeneralKpiFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    KpiPeriodId = KpiPeriodId,
                    Selects = GeneralKpiSelect.Id | GeneralKpiSelect.Employee
                };

                var GeneralKpis = await UOW.GeneralKpiRepository.List(GeneralKpiFilter);
                var AppUserIds = GeneralKpis.Select(x => x.EmployeeId).ToList();
                AppUserFilter = new AppUserFilter
                {
                    Id = new IdFilter { NotIn = AppUserIds },
                };

                var count = await UOW.AppUserRepository.Count(AppUserFilter);
                return count;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralKpiService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }

        }

        public GeneralKpiFilter ToFilter(GeneralKpiFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<GeneralKpiFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                GeneralKpiFilter subFilter = new GeneralKpiFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterPermissionDefinition.IdFilter;

                    if (FilterPermissionDefinition.Name == nameof(subFilter.EmployeeId))
                        subFilter.EmployeeId = FilterPermissionDefinition.IdFilter;

                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = FilterPermissionDefinition.IdFilter;
                }
            }
            return filter;
        }
    }
}
