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

namespace DMS.Services.MGeneralCriteria
{
    public interface IGeneralCriteriaService :  IServiceScoped
    {
        Task<int> Count(GeneralCriteriaFilter GeneralCriteriaFilter);
        Task<List<GeneralCriteria>> List(GeneralCriteriaFilter GeneralCriteriaFilter);
        Task<GeneralCriteria> Get(long Id);
        Task<GeneralCriteria> Create(GeneralCriteria GeneralCriteria);
        Task<GeneralCriteria> Update(GeneralCriteria GeneralCriteria);
        Task<GeneralCriteria> Delete(GeneralCriteria GeneralCriteria);
        Task<List<GeneralCriteria>> BulkDelete(List<GeneralCriteria> GeneralCriterias);
        Task<List<GeneralCriteria>> Import(List<GeneralCriteria> GeneralCriterias);
        GeneralCriteriaFilter ToFilter(GeneralCriteriaFilter GeneralCriteriaFilter);
    }

    public class GeneralCriteriaService : BaseService, IGeneralCriteriaService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IGeneralCriteriaValidator GeneralCriteriaValidator;

        public GeneralCriteriaService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IGeneralCriteriaValidator GeneralCriteriaValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.GeneralCriteriaValidator = GeneralCriteriaValidator;
        }
        public async Task<int> Count(GeneralCriteriaFilter GeneralCriteriaFilter)
        {
            try
            {
                int result = await UOW.GeneralCriteriaRepository.Count(GeneralCriteriaFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<GeneralCriteria>> List(GeneralCriteriaFilter GeneralCriteriaFilter)
        {
            try
            {
                List<GeneralCriteria> GeneralCriterias = await UOW.GeneralCriteriaRepository.List(GeneralCriteriaFilter);
                return GeneralCriterias;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<GeneralCriteria> Get(long Id)
        {
            GeneralCriteria GeneralCriteria = await UOW.GeneralCriteriaRepository.Get(Id);
            if (GeneralCriteria == null)
                return null;
            return GeneralCriteria;
        }
       
        public async Task<GeneralCriteria> Create(GeneralCriteria GeneralCriteria)
        {
            if (!await GeneralCriteriaValidator.Create(GeneralCriteria))
                return GeneralCriteria;

            try
            {
                await UOW.Begin();
                await UOW.GeneralCriteriaRepository.Create(GeneralCriteria);
                await UOW.Commit();

                await Logging.CreateAuditLog(GeneralCriteria, new { }, nameof(GeneralCriteriaService));
                return await UOW.GeneralCriteriaRepository.Get(GeneralCriteria.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<GeneralCriteria> Update(GeneralCriteria GeneralCriteria)
        {
            if (!await GeneralCriteriaValidator.Update(GeneralCriteria))
                return GeneralCriteria;
            try
            {
                var oldData = await UOW.GeneralCriteriaRepository.Get(GeneralCriteria.Id);

                await UOW.Begin();
                await UOW.GeneralCriteriaRepository.Update(GeneralCriteria);
                await UOW.Commit();

                var newData = await UOW.GeneralCriteriaRepository.Get(GeneralCriteria.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(GeneralCriteriaService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<GeneralCriteria> Delete(GeneralCriteria GeneralCriteria)
        {
            if (!await GeneralCriteriaValidator.Delete(GeneralCriteria))
                return GeneralCriteria;

            try
            {
                await UOW.Begin();
                await UOW.GeneralCriteriaRepository.Delete(GeneralCriteria);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, GeneralCriteria, nameof(GeneralCriteriaService));
                return GeneralCriteria;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<GeneralCriteria>> BulkDelete(List<GeneralCriteria> GeneralCriterias)
        {
            if (!await GeneralCriteriaValidator.BulkDelete(GeneralCriterias))
                return GeneralCriterias;

            try
            {
                await UOW.Begin();
                await UOW.GeneralCriteriaRepository.BulkDelete(GeneralCriterias);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, GeneralCriterias, nameof(GeneralCriteriaService));
                return GeneralCriterias;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<GeneralCriteria>> Import(List<GeneralCriteria> GeneralCriterias)
        {
            if (!await GeneralCriteriaValidator.Import(GeneralCriterias))
                return GeneralCriterias;
            try
            {
                await UOW.Begin();
                await UOW.GeneralCriteriaRepository.BulkMerge(GeneralCriterias);
                await UOW.Commit();

                await Logging.CreateAuditLog(GeneralCriterias, new { }, nameof(GeneralCriteriaService));
                return GeneralCriterias;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(GeneralCriteriaService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public GeneralCriteriaFilter ToFilter(GeneralCriteriaFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<GeneralCriteriaFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                GeneralCriteriaFilter subFilter = new GeneralCriteriaFilter();
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
