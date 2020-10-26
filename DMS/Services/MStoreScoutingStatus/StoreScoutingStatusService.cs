using DMS.Common;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;

namespace DMS.Services.MStoreScoutingStatus
{
    public interface IStoreScoutingStatusService :  IServiceScoped
    {
        Task<int> Count(StoreScoutingStatusFilter StoreScoutingStatusFilter);
        Task<List<StoreScoutingStatus>> List(StoreScoutingStatusFilter StoreScoutingStatusFilter);
        Task<StoreScoutingStatus> Get(long Id);
        Task<StoreScoutingStatus> Create(StoreScoutingStatus StoreScoutingStatus);
        Task<StoreScoutingStatus> Update(StoreScoutingStatus StoreScoutingStatus);
        Task<StoreScoutingStatus> Delete(StoreScoutingStatus StoreScoutingStatus);
        Task<List<StoreScoutingStatus>> BulkDelete(List<StoreScoutingStatus> StoreScoutingStatuses);
        Task<List<StoreScoutingStatus>> Import(List<StoreScoutingStatus> StoreScoutingStatuses);
        StoreScoutingStatusFilter ToFilter(StoreScoutingStatusFilter StoreScoutingStatusFilter);
    }

    public class StoreScoutingStatusService : BaseService, IStoreScoutingStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IStoreScoutingStatusValidator StoreScoutingStatusValidator;

        public StoreScoutingStatusService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IStoreScoutingStatusValidator StoreScoutingStatusValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.StoreScoutingStatusValidator = StoreScoutingStatusValidator;
        }
        public async Task<int> Count(StoreScoutingStatusFilter StoreScoutingStatusFilter)
        {
            try
            {
                int result = await UOW.StoreScoutingStatusRepository.Count(StoreScoutingStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreScoutingStatusService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingStatusService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<StoreScoutingStatus>> List(StoreScoutingStatusFilter StoreScoutingStatusFilter)
        {
            try
            {
                List<StoreScoutingStatus> StoreScoutingStatuss = await UOW.StoreScoutingStatusRepository.List(StoreScoutingStatusFilter);
                return StoreScoutingStatuss;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreScoutingStatusService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingStatusService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        public async Task<StoreScoutingStatus> Get(long Id)
        {
            StoreScoutingStatus StoreScoutingStatus = await UOW.StoreScoutingStatusRepository.Get(Id);
            if (StoreScoutingStatus == null)
                return null;
            return StoreScoutingStatus;
        }
       
        public async Task<StoreScoutingStatus> Create(StoreScoutingStatus StoreScoutingStatus)
        {
            if (!await StoreScoutingStatusValidator.Create(StoreScoutingStatus))
                return StoreScoutingStatus;

            try
            {
                await UOW.Begin();
                await UOW.StoreScoutingStatusRepository.Create(StoreScoutingStatus);
                await UOW.Commit();

                await Logging.CreateAuditLog(StoreScoutingStatus, new { }, nameof(StoreScoutingStatusService));
                return await UOW.StoreScoutingStatusRepository.Get(StoreScoutingStatus.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreScoutingStatusService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingStatusService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<StoreScoutingStatus> Update(StoreScoutingStatus StoreScoutingStatus)
        {
            if (!await StoreScoutingStatusValidator.Update(StoreScoutingStatus))
                return StoreScoutingStatus;
            try
            {
                var oldData = await UOW.StoreScoutingStatusRepository.Get(StoreScoutingStatus.Id);

                await UOW.Begin();
                await UOW.StoreScoutingStatusRepository.Update(StoreScoutingStatus);
                await UOW.Commit();

                var newData = await UOW.StoreScoutingStatusRepository.Get(StoreScoutingStatus.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(StoreScoutingStatusService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreScoutingStatusService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingStatusService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<StoreScoutingStatus> Delete(StoreScoutingStatus StoreScoutingStatus)
        {
            if (!await StoreScoutingStatusValidator.Delete(StoreScoutingStatus))
                return StoreScoutingStatus;

            try
            {
                await UOW.Begin();
                await UOW.StoreScoutingStatusRepository.Delete(StoreScoutingStatus);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, StoreScoutingStatus, nameof(StoreScoutingStatusService));
                return StoreScoutingStatus;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreScoutingStatusService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingStatusService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<StoreScoutingStatus>> BulkDelete(List<StoreScoutingStatus> StoreScoutingStatuses)
        {
            if (!await StoreScoutingStatusValidator.BulkDelete(StoreScoutingStatuses))
                return StoreScoutingStatuses;

            try
            {
                await UOW.Begin();
                await UOW.StoreScoutingStatusRepository.BulkDelete(StoreScoutingStatuses);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, StoreScoutingStatuses, nameof(StoreScoutingStatusService));
                return StoreScoutingStatuses;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreScoutingStatusService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingStatusService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        
        public async Task<List<StoreScoutingStatus>> Import(List<StoreScoutingStatus> StoreScoutingStatuses)
        {
            if (!await StoreScoutingStatusValidator.Import(StoreScoutingStatuses))
                return StoreScoutingStatuses;
            try
            {
                await UOW.Begin();
                await UOW.StoreScoutingStatusRepository.BulkMerge(StoreScoutingStatuses);
                await UOW.Commit();

                await Logging.CreateAuditLog(StoreScoutingStatuses, new { }, nameof(StoreScoutingStatusService));
                return StoreScoutingStatuses;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreScoutingStatusService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingStatusService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }     
        
        public StoreScoutingStatusFilter ToFilter(StoreScoutingStatusFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<StoreScoutingStatusFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                StoreScoutingStatusFilter subFilter = new StoreScoutingStatusFilter();
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
