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

namespace DMS.Services.MStoreScouting
{
    public interface IStoreScoutingService :  IServiceScoped
    {
        Task<int> Count(StoreScoutingFilter StoreScoutingFilter);
        Task<List<StoreScouting>> List(StoreScoutingFilter StoreScoutingFilter);
        Task<StoreScouting> Get(long Id);
        Task<StoreScouting> Create(StoreScouting StoreScouting);
        Task<StoreScouting> Update(StoreScouting StoreScouting);
        Task<StoreScouting> Reject(StoreScouting StoreScouting);
        StoreScoutingFilter ToFilter(StoreScoutingFilter StoreScoutingFilter);
    }

    public class StoreScoutingService : BaseService, IStoreScoutingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IStoreScoutingValidator StoreScoutingValidator;

        public StoreScoutingService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IStoreScoutingValidator StoreScoutingValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.StoreScoutingValidator = StoreScoutingValidator;
        }
        public async Task<int> Count(StoreScoutingFilter StoreScoutingFilter)
        {
            try
            {
                int result = await UOW.StoreScoutingRepository.Count(StoreScoutingFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<StoreScouting>> List(StoreScoutingFilter StoreScoutingFilter)
        {
            try
            {
                List<StoreScouting> StoreScoutings = await UOW.StoreScoutingRepository.List(StoreScoutingFilter);
                return StoreScoutings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<StoreScouting> Get(long Id)
        {
            StoreScouting StoreScouting = await UOW.StoreScoutingRepository.Get(Id);
            if (StoreScouting == null)
                return null;
            return StoreScouting;
        }
       
        public async Task<StoreScouting> Create(StoreScouting StoreScouting)
        {
            if (!await StoreScoutingValidator.Create(StoreScouting))
                return StoreScouting;

            try
            {
                var User = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                StoreScouting.CreatorId = User.Id;
                StoreScouting.OrganizationId = User.OrganizationId;
                StoreScouting.Code = Guid.NewGuid().ToString();
                StoreScouting.StoreScoutingStatusId = Enums.StoreScoutingStatusEnum.NOTOPEN.Id;
                await UOW.Begin();
                await UOW.StoreScoutingRepository.Create(StoreScouting);
                StoreScouting.Code = StoreScouting.Id.ToString();
                await UOW.StoreScoutingRepository.Update(StoreScouting);
                await UOW.Commit();

                await Logging.CreateAuditLog(StoreScouting, new { }, nameof(StoreScoutingService));
                return await UOW.StoreScoutingRepository.Get(StoreScouting.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<StoreScouting> Update(StoreScouting StoreScouting)
        {
            if (!await StoreScoutingValidator.Update(StoreScouting))
                return StoreScouting;
            try
            {
                var oldData = await UOW.StoreScoutingRepository.Get(StoreScouting.Id);

                await UOW.Begin();
                await UOW.StoreScoutingRepository.Update(StoreScouting);
                await UOW.Commit();

                var newData = await UOW.StoreScoutingRepository.Get(StoreScouting.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(StoreScoutingService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<StoreScouting> Reject(StoreScouting StoreScouting)
        {
            if (!await StoreScoutingValidator.Reject(StoreScouting))
                return StoreScouting;

            try
            {
                var oldData = await UOW.StoreScoutingRepository.Get(StoreScouting.Id);
                oldData.StoreScoutingStatusId = Enums.StoreScoutingStatusEnum.REJECTED.Id;
                await UOW.Begin();
                await UOW.StoreScoutingRepository.Update(oldData);
                await UOW.Commit();
                await Logging.CreateAuditLog(StoreScouting, oldData, nameof(StoreScoutingService));
                return StoreScouting;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public StoreScoutingFilter ToFilter(StoreScoutingFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<StoreScoutingFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                StoreScoutingFilter subFilter = new StoreScoutingFilter();
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
