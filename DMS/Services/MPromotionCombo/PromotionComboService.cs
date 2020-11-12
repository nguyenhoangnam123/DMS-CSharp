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

namespace DMS.Services.MPromotionCombo
{
    public interface IPromotionComboService :  IServiceScoped
    {
        Task<int> Count(PromotionComboFilter PromotionComboFilter);
        Task<List<PromotionCombo>> List(PromotionComboFilter PromotionComboFilter);
        Task<PromotionCombo> Get(long Id);
        Task<PromotionCombo> Create(PromotionCombo PromotionCombo);
        Task<PromotionCombo> Update(PromotionCombo PromotionCombo);
        Task<PromotionCombo> Delete(PromotionCombo PromotionCombo);
        Task<List<PromotionCombo>> BulkDelete(List<PromotionCombo> PromotionCombos);
        Task<List<PromotionCombo>> Import(List<PromotionCombo> PromotionCombos);
        Task<PromotionComboFilter> ToFilter(PromotionComboFilter PromotionComboFilter);
    }

    public class PromotionComboService : BaseService, IPromotionComboService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPromotionComboValidator PromotionComboValidator;

        public PromotionComboService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPromotionComboValidator PromotionComboValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PromotionComboValidator = PromotionComboValidator;
        }
        public async Task<int> Count(PromotionComboFilter PromotionComboFilter)
        {
            try
            {
                int result = await UOW.PromotionComboRepository.Count(PromotionComboFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionComboService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionComboService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PromotionCombo>> List(PromotionComboFilter PromotionComboFilter)
        {
            try
            {
                List<PromotionCombo> PromotionCombos = await UOW.PromotionComboRepository.List(PromotionComboFilter);
                return PromotionCombos;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionComboService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionComboService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<PromotionCombo> Get(long Id)
        {
            PromotionCombo PromotionCombo = await UOW.PromotionComboRepository.Get(Id);
            if (PromotionCombo == null)
                return null;
            return PromotionCombo;
        }
       
        public async Task<PromotionCombo> Create(PromotionCombo PromotionCombo)
        {
            if (!await PromotionComboValidator.Create(PromotionCombo))
                return PromotionCombo;

            try
            {
                await UOW.Begin();
                await UOW.PromotionComboRepository.Create(PromotionCombo);
                await UOW.Commit();
                PromotionCombo = await UOW.PromotionComboRepository.Get(PromotionCombo.Id);
                await Logging.CreateAuditLog(PromotionCombo, new { }, nameof(PromotionComboService));
                return PromotionCombo;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionComboService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionComboService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PromotionCombo> Update(PromotionCombo PromotionCombo)
        {
            if (!await PromotionComboValidator.Update(PromotionCombo))
                return PromotionCombo;
            try
            {
                var oldData = await UOW.PromotionComboRepository.Get(PromotionCombo.Id);

                await UOW.Begin();
                await UOW.PromotionComboRepository.Update(PromotionCombo);
                await UOW.Commit();

                PromotionCombo = await UOW.PromotionComboRepository.Get(PromotionCombo.Id);
                await Logging.CreateAuditLog(PromotionCombo, oldData, nameof(PromotionComboService));
                return PromotionCombo;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionComboService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionComboService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PromotionCombo> Delete(PromotionCombo PromotionCombo)
        {
            if (!await PromotionComboValidator.Delete(PromotionCombo))
                return PromotionCombo;

            try
            {
                await UOW.Begin();
                await UOW.PromotionComboRepository.Delete(PromotionCombo);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PromotionCombo, nameof(PromotionComboService));
                return PromotionCombo;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionComboService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionComboService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PromotionCombo>> BulkDelete(List<PromotionCombo> PromotionCombos)
        {
            if (!await PromotionComboValidator.BulkDelete(PromotionCombos))
                return PromotionCombos;

            try
            {
                await UOW.Begin();
                await UOW.PromotionComboRepository.BulkDelete(PromotionCombos);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PromotionCombos, nameof(PromotionComboService));
                return PromotionCombos;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionComboService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionComboService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        
        public async Task<List<PromotionCombo>> Import(List<PromotionCombo> PromotionCombos)
        {
            if (!await PromotionComboValidator.Import(PromotionCombos))
                return PromotionCombos;
            try
            {
                await UOW.Begin();
                await UOW.PromotionComboRepository.BulkMerge(PromotionCombos);
                await UOW.Commit();

                await Logging.CreateAuditLog(PromotionCombos, new { }, nameof(PromotionComboService));
                return PromotionCombos;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionComboService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionComboService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }     
        
        public async Task<PromotionComboFilter> ToFilter(PromotionComboFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PromotionComboFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PromotionComboFilter subFilter = new PromotionComboFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.Note))
                        
                        
                        
                        
                        
                        
                        subFilter.Note = FilterBuilder.Merge(subFilter.Note, FilterPermissionDefinition.StringFilter);
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PromotionPolicyId))
                        subFilter.PromotionPolicyId = FilterBuilder.Merge(subFilter.PromotionPolicyId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.PromotionId))
                        subFilter.PromotionId = FilterBuilder.Merge(subFilter.PromotionId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
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
