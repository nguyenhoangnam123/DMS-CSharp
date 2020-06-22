using Common;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;
using DMS.Helpers;

namespace DMS.Services.MKpiGeneral
{
    public interface IKpiGeneralService :  IServiceScoped
    {
        Task<int> Count(KpiGeneralFilter KpiGeneralFilter);
        Task<List<KpiGeneral>> List(KpiGeneralFilter KpiGeneralFilter);
        Task<KpiGeneral> Get(long Id);
        Task<KpiGeneral> Create(KpiGeneral KpiGeneral);
        Task<KpiGeneral> Update(KpiGeneral KpiGeneral);
        Task<KpiGeneral> Delete(KpiGeneral KpiGeneral);
        Task<List<KpiGeneral>> BulkDelete(List<KpiGeneral> KpiGenerals);
        Task<List<KpiGeneral>> Import(List<KpiGeneral> KpiGenerals);
        Task<List<AppUser>> ListAppUser(AppUserFilter appUserFilter, IdFilter KpiYearId);
        Task<int> CountAppUser(AppUserFilter appUserFilter, IdFilter KpiYearId);
        KpiGeneralFilter ToFilter(KpiGeneralFilter KpiGeneralFilter);
    }

    public class KpiGeneralService : BaseService, IKpiGeneralService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IKpiGeneralValidator KpiGeneralValidator;

        public KpiGeneralService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IKpiGeneralValidator KpiGeneralValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.KpiGeneralValidator = KpiGeneralValidator;
        }
        public async Task<int> Count(KpiGeneralFilter KpiGeneralFilter)
        {
            try
            {
                int result = await UOW.KpiGeneralRepository.Count(KpiGeneralFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiGeneralService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<KpiGeneral>> List(KpiGeneralFilter KpiGeneralFilter)
        {
            try
            {
                List<KpiGeneral> KpiGenerals = await UOW.KpiGeneralRepository.List(KpiGeneralFilter);
                return KpiGenerals;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiGeneralService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<KpiGeneral> Get(long Id)
        {
            KpiGeneral KpiGeneral = await UOW.KpiGeneralRepository.Get(Id);
            if (KpiGeneral == null)
                return null;
            return KpiGeneral;
        }

        public async Task<KpiGeneral> Create(KpiGeneral KpiGeneral)
        {
            if (!await KpiGeneralValidator.Create(KpiGeneral))
                return KpiGeneral;

            try
            {
                await UOW.Begin();
                List<KpiGeneral> KpiGenerals = new List<KpiGeneral>();
                if (KpiGeneral.EmployeeIds != null && KpiGeneral.EmployeeIds.Any())
                {
                    foreach (var EmployeeId in KpiGeneral.EmployeeIds)
                    {
                        var newObj = Utils.Clone(KpiGeneral);
                        newObj.EmployeeId = EmployeeId;
                        newObj.CreatorId = CurrentContext.UserId;
                        KpiGenerals.Add(newObj);
                    }
                }
                await UOW.KpiGeneralRepository.BulkMerge(KpiGenerals);
                await UOW.Commit();

                await Logging.CreateAuditLog(KpiGeneral, new { }, nameof(KpiGeneralService));
                return KpiGeneral;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiGeneralService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<KpiGeneral> Update(KpiGeneral KpiGeneral)
        {
            if (!await KpiGeneralValidator.Update(KpiGeneral))
                return KpiGeneral;
            try
            {
                var oldData = await UOW.KpiGeneralRepository.Get(KpiGeneral.Id);

                await UOW.Begin();
                await UOW.KpiGeneralRepository.Update(KpiGeneral);
                await UOW.Commit();

                var newData = await UOW.KpiGeneralRepository.Get(KpiGeneral.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(KpiGeneralService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiGeneralService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<KpiGeneral> Delete(KpiGeneral KpiGeneral)
        {
            if (!await KpiGeneralValidator.Delete(KpiGeneral))
                return KpiGeneral;

            try
            {
                await UOW.Begin();
                await UOW.KpiGeneralRepository.Delete(KpiGeneral);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, KpiGeneral, nameof(KpiGeneralService));
                return KpiGeneral;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiGeneralService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<KpiGeneral>> BulkDelete(List<KpiGeneral> KpiGenerals)
        {
            if (!await KpiGeneralValidator.BulkDelete(KpiGenerals))
                return KpiGenerals;

            try
            {
                await UOW.Begin();
                await UOW.KpiGeneralRepository.BulkDelete(KpiGenerals);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, KpiGenerals, nameof(KpiGeneralService));
                return KpiGenerals;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiGeneralService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<KpiGeneral>> Import(List<KpiGeneral> KpiGenerals)
        {
            if (!await KpiGeneralValidator.Import(KpiGenerals))
                return KpiGenerals;
            try
            {
                await UOW.Begin();
                await UOW.KpiGeneralRepository.BulkMerge(KpiGenerals);
                await UOW.Commit();

                await Logging.CreateAuditLog(KpiGenerals, new { }, nameof(KpiGeneralService));
                return KpiGenerals;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiGeneralService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<List<AppUser>> ListAppUser(AppUserFilter AppUserFilter, IdFilter KpiYearId)
        {
            try
            {
                KpiGeneralFilter KpiGeneralFilter = new KpiGeneralFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    KpiYearId = KpiYearId,
                    Selects = KpiGeneralSelect.Id | KpiGeneralSelect.Employee
                };

                var KpiGenerals = await UOW.KpiGeneralRepository.List(KpiGeneralFilter);
                var AppUserIds = KpiGenerals.Select(x => x.EmployeeId).ToList();
                AppUserFilter = new AppUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { NotIn = AppUserIds },
                    Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName | AppUserSelect.Phone | AppUserSelect.Email,
                    DisplayName = AppUserFilter.DisplayName,
                    Username = AppUserFilter.Username,
                    Phone = AppUserFilter.Phone,
                    Email = AppUserFilter.Email,
                };

                var AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                return AppUsers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiGeneralService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }

        }

        public async Task<int> CountAppUser(AppUserFilter AppUserFilter, IdFilter KpiYearId)
        {
            try
            {
                KpiGeneralFilter KpiGeneralFilter = new KpiGeneralFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    KpiYearId = KpiYearId,
                    Selects = KpiGeneralSelect.Id | KpiGeneralSelect.Employee
                };

                var KpiGenerals = await UOW.KpiGeneralRepository.List(KpiGeneralFilter);
                var AppUserIds = KpiGenerals.Select(x => x.EmployeeId).ToList();
                AppUserFilter = new AppUserFilter
                {
                    Id = new IdFilter { NotIn = AppUserIds },
                    DisplayName = AppUserFilter.DisplayName,
                    Username = AppUserFilter.Username,
                    Phone = AppUserFilter.Phone,
                    Email = AppUserFilter.Email,
                };

                var count = await UOW.AppUserRepository.Count(AppUserFilter);
                return count;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiGeneralService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }

        }
        public KpiGeneralFilter ToFilter(KpiGeneralFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<KpiGeneralFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                KpiGeneralFilter subFilter = new KpiGeneralFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                }
            }
            return filter;
        }
    }
}