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
using DMS.Enums;

namespace DMS.Services.MCombo
{
    public interface IComboService :  IServiceScoped
    {
        Task<int> Count(ComboFilter ComboFilter);
        Task<List<Combo>> List(ComboFilter ComboFilter);
        Task<Combo> Get(long Id);
        Task<Combo> Create(Combo Combo);
        Task<Combo> Update(Combo Combo);
        Task<Combo> Delete(Combo Combo);
        Task<List<Combo>> BulkDelete(List<Combo> Combos);
        Task<List<Combo>> Import(List<Combo> Combos);
        Task<ComboFilter> ToFilter(ComboFilter ComboFilter);
    }

    public class ComboService : BaseService, IComboService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IComboValidator ComboValidator;

        public ComboService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IComboValidator ComboValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ComboValidator = ComboValidator;
        }
        public async Task<int> Count(ComboFilter ComboFilter)
        {
            try
            {
                int result = await UOW.ComboRepository.Count(ComboFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ComboService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ComboService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Combo>> List(ComboFilter ComboFilter)
        {
            try
            {
                List<Combo> Combos = await UOW.ComboRepository.List(ComboFilter);
                return Combos;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ComboService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ComboService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<Combo> Get(long Id)
        {
            Combo Combo = await UOW.ComboRepository.Get(Id);
            if (Combo == null)
                return null;
            return Combo;
        }
       
        public async Task<Combo> Create(Combo Combo)
        {
            if (!await ComboValidator.Create(Combo))
                return Combo;

            try
            {
                await UOW.Begin();
                await UOW.ComboRepository.Create(Combo);
                await UOW.Commit();
                Combo = await UOW.ComboRepository.Get(Combo.Id);
                await Logging.CreateAuditLog(Combo, new { }, nameof(ComboService));
                return Combo;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ComboService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ComboService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Combo> Update(Combo Combo)
        {
            if (!await ComboValidator.Update(Combo))
                return Combo;
            try
            {
                var oldData = await UOW.ComboRepository.Get(Combo.Id);

                await UOW.Begin();
                await UOW.ComboRepository.Update(Combo);
                await UOW.Commit();

                Combo = await UOW.ComboRepository.Get(Combo.Id);
                await Logging.CreateAuditLog(Combo, oldData, nameof(ComboService));
                return Combo;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ComboService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ComboService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Combo> Delete(Combo Combo)
        {
            if (!await ComboValidator.Delete(Combo))
                return Combo;

            try
            {
                await UOW.Begin();
                await UOW.ComboRepository.Delete(Combo);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Combo, nameof(ComboService));
                return Combo;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ComboService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ComboService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Combo>> BulkDelete(List<Combo> Combos)
        {
            if (!await ComboValidator.BulkDelete(Combos))
                return Combos;

            try
            {
                await UOW.Begin();
                await UOW.ComboRepository.BulkDelete(Combos);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Combos, nameof(ComboService));
                return Combos;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ComboService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ComboService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        
        public async Task<List<Combo>> Import(List<Combo> Combos)
        {
            if (!await ComboValidator.Import(Combos))
                return Combos;
            try
            {
                await UOW.Begin();
                await UOW.ComboRepository.BulkMerge(Combos);
                await UOW.Commit();

                await Logging.CreateAuditLog(Combos, new { }, nameof(ComboService));
                return Combos;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ComboService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ComboService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }     
        
        public async Task<ComboFilter> ToFilter(ComboFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ComboFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ComboFilter subFilter = new ComboFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.PromotionComboId))
                        subFilter.PromotionComboId = FilterBuilder.Merge(subFilter.PromotionComboId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        
                        
                        
                        
                        
                        
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PromotionDiscountTypeId))
                        subFilter.PromotionDiscountTypeId = FilterBuilder.Merge(subFilter.PromotionDiscountTypeId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.DiscountPercentage))
                        
                        
                        subFilter.DiscountPercentage = FilterBuilder.Merge(subFilter.DiscountPercentage, FilterPermissionDefinition.DecimalFilter);
                        
                        
                        
                        
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DiscountValue))
                        
                        
                        subFilter.DiscountValue = FilterBuilder.Merge(subFilter.DiscountValue, FilterPermissionDefinition.DecimalFilter);
                        
                        
                        
                        
                        
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
