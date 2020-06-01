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

namespace DMS.Services.MTotalItemSpecificCriteria
{
    public interface ITotalItemSpecificCriteriaService :  IServiceScoped
    {
        Task<int> Count(TotalItemSpecificCriteriaFilter TotalItemSpecificCriteriaFilter);
        Task<List<TotalItemSpecificCriteria>> List(TotalItemSpecificCriteriaFilter TotalItemSpecificCriteriaFilter);
        Task<TotalItemSpecificCriteria> Get(long Id);
        Task<TotalItemSpecificCriteria> Create(TotalItemSpecificCriteria TotalItemSpecificCriteria);
        Task<TotalItemSpecificCriteria> Update(TotalItemSpecificCriteria TotalItemSpecificCriteria);
        Task<TotalItemSpecificCriteria> Delete(TotalItemSpecificCriteria TotalItemSpecificCriteria);
        Task<List<TotalItemSpecificCriteria>> BulkDelete(List<TotalItemSpecificCriteria> TotalItemSpecificCriterias);
        Task<List<TotalItemSpecificCriteria>> Import(List<TotalItemSpecificCriteria> TotalItemSpecificCriterias);
        TotalItemSpecificCriteriaFilter ToFilter(TotalItemSpecificCriteriaFilter TotalItemSpecificCriteriaFilter);
    }

    public class TotalItemSpecificCriteriaService : BaseService, ITotalItemSpecificCriteriaService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ITotalItemSpecificCriteriaValidator TotalItemSpecificCriteriaValidator;

        public TotalItemSpecificCriteriaService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ITotalItemSpecificCriteriaValidator TotalItemSpecificCriteriaValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.TotalItemSpecificCriteriaValidator = TotalItemSpecificCriteriaValidator;
        }
        public async Task<int> Count(TotalItemSpecificCriteriaFilter TotalItemSpecificCriteriaFilter)
        {
            try
            {
                int result = await UOW.TotalItemSpecificCriteriaRepository.Count(TotalItemSpecificCriteriaFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(TotalItemSpecificCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<TotalItemSpecificCriteria>> List(TotalItemSpecificCriteriaFilter TotalItemSpecificCriteriaFilter)
        {
            try
            {
                List<TotalItemSpecificCriteria> TotalItemSpecificCriterias = await UOW.TotalItemSpecificCriteriaRepository.List(TotalItemSpecificCriteriaFilter);
                return TotalItemSpecificCriterias;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(TotalItemSpecificCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<TotalItemSpecificCriteria> Get(long Id)
        {
            TotalItemSpecificCriteria TotalItemSpecificCriteria = await UOW.TotalItemSpecificCriteriaRepository.Get(Id);
            if (TotalItemSpecificCriteria == null)
                return null;
            return TotalItemSpecificCriteria;
        }
       
        public async Task<TotalItemSpecificCriteria> Create(TotalItemSpecificCriteria TotalItemSpecificCriteria)
        {
            if (!await TotalItemSpecificCriteriaValidator.Create(TotalItemSpecificCriteria))
                return TotalItemSpecificCriteria;

            try
            {
                await UOW.Begin();
                await UOW.TotalItemSpecificCriteriaRepository.Create(TotalItemSpecificCriteria);
                await UOW.Commit();

                await Logging.CreateAuditLog(TotalItemSpecificCriteria, new { }, nameof(TotalItemSpecificCriteriaService));
                return await UOW.TotalItemSpecificCriteriaRepository.Get(TotalItemSpecificCriteria.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(TotalItemSpecificCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<TotalItemSpecificCriteria> Update(TotalItemSpecificCriteria TotalItemSpecificCriteria)
        {
            if (!await TotalItemSpecificCriteriaValidator.Update(TotalItemSpecificCriteria))
                return TotalItemSpecificCriteria;
            try
            {
                var oldData = await UOW.TotalItemSpecificCriteriaRepository.Get(TotalItemSpecificCriteria.Id);

                await UOW.Begin();
                await UOW.TotalItemSpecificCriteriaRepository.Update(TotalItemSpecificCriteria);
                await UOW.Commit();

                var newData = await UOW.TotalItemSpecificCriteriaRepository.Get(TotalItemSpecificCriteria.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(TotalItemSpecificCriteriaService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(TotalItemSpecificCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<TotalItemSpecificCriteria> Delete(TotalItemSpecificCriteria TotalItemSpecificCriteria)
        {
            if (!await TotalItemSpecificCriteriaValidator.Delete(TotalItemSpecificCriteria))
                return TotalItemSpecificCriteria;

            try
            {
                await UOW.Begin();
                await UOW.TotalItemSpecificCriteriaRepository.Delete(TotalItemSpecificCriteria);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, TotalItemSpecificCriteria, nameof(TotalItemSpecificCriteriaService));
                return TotalItemSpecificCriteria;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(TotalItemSpecificCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<TotalItemSpecificCriteria>> BulkDelete(List<TotalItemSpecificCriteria> TotalItemSpecificCriterias)
        {
            if (!await TotalItemSpecificCriteriaValidator.BulkDelete(TotalItemSpecificCriterias))
                return TotalItemSpecificCriterias;

            try
            {
                await UOW.Begin();
                await UOW.TotalItemSpecificCriteriaRepository.BulkDelete(TotalItemSpecificCriterias);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, TotalItemSpecificCriterias, nameof(TotalItemSpecificCriteriaService));
                return TotalItemSpecificCriterias;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(TotalItemSpecificCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<TotalItemSpecificCriteria>> Import(List<TotalItemSpecificCriteria> TotalItemSpecificCriterias)
        {
            if (!await TotalItemSpecificCriteriaValidator.Import(TotalItemSpecificCriterias))
                return TotalItemSpecificCriterias;
            try
            {
                await UOW.Begin();
                await UOW.TotalItemSpecificCriteriaRepository.BulkMerge(TotalItemSpecificCriterias);
                await UOW.Commit();

                await Logging.CreateAuditLog(TotalItemSpecificCriterias, new { }, nameof(TotalItemSpecificCriteriaService));
                return TotalItemSpecificCriterias;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(TotalItemSpecificCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public TotalItemSpecificCriteriaFilter ToFilter(TotalItemSpecificCriteriaFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<TotalItemSpecificCriteriaFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                TotalItemSpecificCriteriaFilter subFilter = new TotalItemSpecificCriteriaFilter();
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
