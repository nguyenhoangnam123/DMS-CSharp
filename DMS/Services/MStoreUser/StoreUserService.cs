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
using DMS.Enums;

namespace DMS.Services.MStoreUser
{
    public interface IStoreUserService :  IServiceScoped
    {
        Task<int> Count(StoreUserFilter StoreUserFilter);
        Task<List<StoreUser>> List(StoreUserFilter StoreUserFilter);
        Task<StoreUser> Get(long Id);
        Task<StoreUser> Create(StoreUser StoreUser);
        Task<StoreUser> Update(StoreUser StoreUser);
        Task<StoreUser> Delete(StoreUser StoreUser);
        Task<List<StoreUser>> BulkDelete(List<StoreUser> StoreUsers);
        Task<List<StoreUser>> Import(List<StoreUser> StoreUsers);
        Task<StoreUserFilter> ToFilter(StoreUserFilter StoreUserFilter);
    }

    public class StoreUserService : BaseService, IStoreUserService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IStoreUserValidator StoreUserValidator;

        public StoreUserService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IStoreUserValidator StoreUserValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.StoreUserValidator = StoreUserValidator;
        }
        public async Task<int> Count(StoreUserFilter StoreUserFilter)
        {
            try
            {
                int result = await UOW.StoreUserRepository.Count(StoreUserFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(StoreUserService));
            }
            return 0;
        }

        public async Task<List<StoreUser>> List(StoreUserFilter StoreUserFilter)
        {
            try
            {
                List<StoreUser> StoreUsers = await UOW.StoreUserRepository.List(StoreUserFilter);
                return StoreUsers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(StoreUserService));
            }
            return null;
        }
        
        public async Task<StoreUser> Get(long Id)
        {
            StoreUser StoreUser = await UOW.StoreUserRepository.Get(Id);
            if (StoreUser == null)
                return null;
            return StoreUser;
        }
        public async Task<StoreUser> Create(StoreUser StoreUser)
        {
            if (!await StoreUserValidator.Create(StoreUser))
                return StoreUser;

            try
            {
                await UOW.StoreUserRepository.Create(StoreUser);
                StoreUser = await UOW.StoreUserRepository.Get(StoreUser.Id);
                await Logging.CreateAuditLog(StoreUser, new { }, nameof(StoreUserService));
                return StoreUser;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(StoreUserService));
            }
            return null;
        }

        public async Task<StoreUser> Update(StoreUser StoreUser)
        {
            if (!await StoreUserValidator.Update(StoreUser))
                return StoreUser;
            try
            {
                var oldData = await UOW.StoreUserRepository.Get(StoreUser.Id);

                await UOW.StoreUserRepository.Update(StoreUser);

                StoreUser = await UOW.StoreUserRepository.Get(StoreUser.Id);
                await Logging.CreateAuditLog(StoreUser, oldData, nameof(StoreUserService));
                return StoreUser;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(StoreUserService));
            }
            return null;
        }

        public async Task<StoreUser> Delete(StoreUser StoreUser)
        {
            if (!await StoreUserValidator.Delete(StoreUser))
                return StoreUser;

            try
            {
                await UOW.StoreUserRepository.Delete(StoreUser);
                await Logging.CreateAuditLog(new { }, StoreUser, nameof(StoreUserService));
                return StoreUser;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(StoreUserService));
            }
            return null;
        }

        public async Task<List<StoreUser>> BulkDelete(List<StoreUser> StoreUsers)
        {
            if (!await StoreUserValidator.BulkDelete(StoreUsers))
                return StoreUsers;

            try
            {
                await UOW.StoreUserRepository.BulkDelete(StoreUsers);
                await Logging.CreateAuditLog(new { }, StoreUsers, nameof(StoreUserService));
                return StoreUsers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(StoreUserService));
            }
            return null;

        }
        
        public async Task<List<StoreUser>> Import(List<StoreUser> StoreUsers)
        {
            if (!await StoreUserValidator.Import(StoreUsers))
                return StoreUsers;
            try
            {
                await UOW.StoreUserRepository.BulkMerge(StoreUsers);

                await Logging.CreateAuditLog(StoreUsers, new { }, nameof(StoreUserService));
                return StoreUsers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(StoreUserService));
            }
            return null;
        }     
        
        public async Task<StoreUserFilter> ToFilter(StoreUserFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<StoreUserFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                StoreUserFilter subFilter = new StoreUserFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StoreId))
                        subFilter.StoreId = FilterBuilder.Merge(subFilter.StoreId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Username))
                        subFilter.Username = FilterBuilder.Merge(subFilter.Username, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DisplayName))
                        subFilter.DisplayName = FilterBuilder.Merge(subFilter.DisplayName, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Password))
                        subFilter.Password = FilterBuilder.Merge(subFilter.Password, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OtpCode))
                        subFilter.OtpCode = FilterBuilder.Merge(subFilter.OtpCode, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OtpExpired))
                        subFilter.OtpExpired = FilterBuilder.Merge(subFilter.OtpExpired, FilterPermissionDefinition.DateFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.RowId))
                        subFilter.RowId = FilterBuilder.Merge(subFilter.RowId, FilterPermissionDefinition.GuidFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }
    }
}
