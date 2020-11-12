using DMS.Common;
using DMS.Entities;
using DMS.Repositories;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MReseller
{
    public interface IResellerService : IServiceScoped
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ResellerService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerService));
                    throw new MessageException(ex.InnerException);
                };
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ResellerService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerService));
                    throw new MessageException(ex.InnerException);
                };
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ResellerService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerService));
                    throw new MessageException(ex.InnerException);
                };
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ResellerService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerService));
                    throw new MessageException(ex.InnerException);
                };
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ResellerService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerService));
                    throw new MessageException(ex.InnerException);
                };
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ResellerService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerService));
                    throw new MessageException(ex.InnerException);
                };
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ResellerService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerService));
                    throw new MessageException(ex.InnerException);
                };
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
                }
            }
            return filter;
        }
    }
}
