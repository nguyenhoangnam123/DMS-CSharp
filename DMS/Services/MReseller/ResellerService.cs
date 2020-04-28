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

namespace DMS.Services.MReseller
{
    public interface IResellerService :  IServiceScoped
    {
        Task<int> Count(ResellerFilter ResellerFilter);
        Task<List<Reseller>> List(ResellerFilter ResellerFilter);
        Task<Reseller> Get(long Id);
        Task<Reseller> Create(Reseller Reseller);
        Task<Reseller> Update(Reseller Reseller);
        Task<Reseller> Delete(Reseller Reseller);
        Task<List<Reseller>> BulkDelete(List<Reseller> Resellers);
        Task<List<Reseller>> Import(List<Reseller> Resellers);
        ResellerFilter ToFilter(ResellerFilter ResellerFilter);
    }

    public class ResellerService : BaseService, IResellerService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IResellerValidator ResellerValidator;

        public ResellerService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IResellerValidator ResellerValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ResellerValidator = ResellerValidator;
        }
        public async Task<int> Count(ResellerFilter ResellerFilter)
        {
            try
            {
                int result = await UOW.ResellerRepository.Count(ResellerFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Reseller>> List(ResellerFilter ResellerFilter)
        {
            try
            {
                List<Reseller> Resellers = await UOW.ResellerRepository.List(ResellerFilter);
                return Resellers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Reseller> Get(long Id)
        {
            Reseller Reseller = await UOW.ResellerRepository.Get(Id);
            if (Reseller == null)
                return null;
            return Reseller;
        }
       
        public async Task<Reseller> Create(Reseller Reseller)
        {
            if (!await ResellerValidator.Create(Reseller))
                return Reseller;

            try
            {
                Reseller.Id = 0;
                await UOW.Begin();
                await UOW.ResellerRepository.Create(Reseller);
                await UOW.Commit();

                await Logging.CreateAuditLog(Reseller, new { }, nameof(ResellerService));
                return await UOW.ResellerRepository.Get(Reseller.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Reseller> Update(Reseller Reseller)
        {
            if (!await ResellerValidator.Update(Reseller))
                return Reseller;
            try
            {
                var oldData = await UOW.ResellerRepository.Get(Reseller.Id);

                await UOW.Begin();
                await UOW.ResellerRepository.Update(Reseller);
                await UOW.Commit();

                var newData = await UOW.ResellerRepository.Get(Reseller.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ResellerService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Reseller> Delete(Reseller Reseller)
        {
            if (!await ResellerValidator.Delete(Reseller))
                return Reseller;

            try
            {
                await UOW.Begin();
                await UOW.ResellerRepository.Delete(Reseller);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Reseller, nameof(ResellerService));
                return Reseller;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Reseller>> BulkDelete(List<Reseller> Resellers)
        {
            if (!await ResellerValidator.BulkDelete(Resellers))
                return Resellers;

            try
            {
                await UOW.Begin();
                await UOW.ResellerRepository.BulkDelete(Resellers);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Resellers, nameof(ResellerService));
                return Resellers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<Reseller>> Import(List<Reseller> Resellers)
        {
            if (!await ResellerValidator.Import(Resellers))
                return Resellers;
            try
            {
                Resellers.ForEach(r => r.Id = 0);
                await UOW.Begin();
                await UOW.ResellerRepository.BulkMerge(Resellers);
                await UOW.Commit();

                await Logging.CreateAuditLog(Resellers, new { }, nameof(ResellerService));
                return Resellers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public ResellerFilter ToFilter(ResellerFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ResellerFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ResellerFilter subFilter = new ResellerFilter();
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
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Phone))
                        subFilter.Phone = Map(subFilter.Phone, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Email))
                        subFilter.Email = Map(subFilter.Email, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Address))
                        subFilter.Address = Map(subFilter.Address, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TaxCode))
                        subFilter.TaxCode = Map(subFilter.TaxCode, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CompanyName))
                        subFilter.CompanyName = Map(subFilter.CompanyName, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DeputyName))
                        subFilter.DeputyName = Map(subFilter.DeputyName, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Description))
                        subFilter.Description = Map(subFilter.Description, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = Map(subFilter.OrganizationId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = Map(subFilter.StatusId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ResellerTypeId))
                        subFilter.ResellerTypeId = Map(subFilter.ResellerTypeId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ResellerStatusId))
                        subFilter.ResellerStatusId = Map(subFilter.ResellerStatusId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StaffId))
                        subFilter.StaffId = Map(subFilter.StaffId, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
