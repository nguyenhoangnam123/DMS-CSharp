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

namespace DMS.Services.MRequestState
{
    public interface IRequestStateService :  IServiceScoped
    {
        Task<int> Count(RequestStateFilter RequestStateFilter);
        Task<List<RequestState>> List(RequestStateFilter RequestStateFilter);
        Task<RequestState> Get(long Id);
        Task<RequestState> Create(RequestState RequestState);
        Task<RequestState> Update(RequestState RequestState);
        Task<RequestState> Delete(RequestState RequestState);
        Task<List<RequestState>> BulkDelete(List<RequestState> RequestStates);
        Task<List<RequestState>> Import(List<RequestState> RequestStates);
        RequestStateFilter ToFilter(RequestStateFilter RequestStateFilter);
    }

    public class RequestStateService : BaseService, IRequestStateService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IRequestStateValidator RequestStateValidator;

        public RequestStateService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IRequestStateValidator RequestStateValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.RequestStateValidator = RequestStateValidator;
        }
        public async Task<int> Count(RequestStateFilter RequestStateFilter)
        {
            try
            {
                int result = await UOW.RequestStateRepository.Count(RequestStateFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(RequestStateService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<RequestState>> List(RequestStateFilter RequestStateFilter)
        {
            try
            {
                List<RequestState> RequestStates = await UOW.RequestStateRepository.List(RequestStateFilter);
                return RequestStates;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(RequestStateService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<RequestState> Get(long Id)
        {
            RequestState RequestState = await UOW.RequestStateRepository.Get(Id);
            if (RequestState == null)
                return null;
            return RequestState;
        }
       
        public async Task<RequestState> Create(RequestState RequestState)
        {
            if (!await RequestStateValidator.Create(RequestState))
                return RequestState;

            try
            {
                await UOW.Begin();
                await UOW.RequestStateRepository.Create(RequestState);
                await UOW.Commit();

                await Logging.CreateAuditLog(RequestState, new { }, nameof(RequestStateService));
                return await UOW.RequestStateRepository.Get(RequestState.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(RequestStateService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<RequestState> Update(RequestState RequestState)
        {
            if (!await RequestStateValidator.Update(RequestState))
                return RequestState;
            try
            {
                var oldData = await UOW.RequestStateRepository.Get(RequestState.Id);

                await UOW.Begin();
                await UOW.RequestStateRepository.Update(RequestState);
                await UOW.Commit();

                var newData = await UOW.RequestStateRepository.Get(RequestState.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(RequestStateService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(RequestStateService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<RequestState> Delete(RequestState RequestState)
        {
            if (!await RequestStateValidator.Delete(RequestState))
                return RequestState;

            try
            {
                await UOW.Begin();
                await UOW.RequestStateRepository.Delete(RequestState);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, RequestState, nameof(RequestStateService));
                return RequestState;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(RequestStateService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<RequestState>> BulkDelete(List<RequestState> RequestStates)
        {
            if (!await RequestStateValidator.BulkDelete(RequestStates))
                return RequestStates;

            try
            {
                await UOW.Begin();
                await UOW.RequestStateRepository.BulkDelete(RequestStates);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, RequestStates, nameof(RequestStateService));
                return RequestStates;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(RequestStateService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<RequestState>> Import(List<RequestState> RequestStates)
        {
            if (!await RequestStateValidator.Import(RequestStates))
                return RequestStates;
            try
            {
                await UOW.Begin();
                await UOW.RequestStateRepository.BulkMerge(RequestStates);
                await UOW.Commit();

                await Logging.CreateAuditLog(RequestStates, new { }, nameof(RequestStateService));
                return RequestStates;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(RequestStateService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public RequestStateFilter ToFilter(RequestStateFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<RequestStateFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                RequestStateFilter subFilter = new RequestStateFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = Map(subFilter.Code, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = Map(subFilter.Name, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
