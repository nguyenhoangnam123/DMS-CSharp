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

namespace DMS.Services.MSurvey
{
    public interface ISurveyService :  IServiceScoped
    {
        Task<int> Count(SurveyFilter SurveyFilter);
        Task<List<Survey>> List(SurveyFilter SurveyFilter);
        Task<Survey> Get(long Id);
        Task<Survey> Create(Survey Survey);
        Task<Survey> Update(Survey Survey);
        Task<Survey> Delete(Survey Survey);
        SurveyFilter ToFilter(SurveyFilter SurveyFilter);
    }

    public class SurveyService : BaseService, ISurveyService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ISurveyValidator SurveyValidator;

        public SurveyService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ISurveyValidator SurveyValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.SurveyValidator = SurveyValidator;
        }
        public async Task<int> Count(SurveyFilter SurveyFilter)
        {
            try
            {
                int result = await UOW.SurveyRepository.Count(SurveyFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Survey>> List(SurveyFilter SurveyFilter)
        {
            try
            {
                List<Survey> Surveys = await UOW.SurveyRepository.List(SurveyFilter);
                return Surveys;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Survey> Get(long Id)
        {
            Survey Survey = await UOW.SurveyRepository.Get(Id);
            if (Survey == null)
                return null;
            return Survey;
        }
       
        public async Task<Survey> Create(Survey Survey)
        {
            if (!await SurveyValidator.Create(Survey))
                return Survey;

            try
            {
                await UOW.Begin();
                await UOW.SurveyRepository.Create(Survey);
                await UOW.Commit();

                await Logging.CreateAuditLog(Survey, new { }, nameof(SurveyService));
                return await UOW.SurveyRepository.Get(Survey.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Survey> Update(Survey Survey)
        {
            if (!await SurveyValidator.Update(Survey))
                return Survey;
            try
            {
                var oldData = await UOW.SurveyRepository.Get(Survey.Id);

                await UOW.Begin();
                await UOW.SurveyRepository.Update(Survey);
                await UOW.Commit();

                var newData = await UOW.SurveyRepository.Get(Survey.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(SurveyService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Survey> Delete(Survey Survey)
        {
            if (!await SurveyValidator.Delete(Survey))
                return Survey;

            try
            {
                await UOW.Begin();
                await UOW.SurveyRepository.Delete(Survey);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Survey, nameof(SurveyService));
                return Survey;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public SurveyFilter ToFilter(SurveyFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<SurveyFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                SurveyFilter subFilter = new SurveyFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Title))
                        subFilter.Title = Map(subFilter.Title, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Description))
                        subFilter.Description = Map(subFilter.Description, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StartAt))
                        subFilter.StartAt = Map(subFilter.StartAt, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.EndAt))
                        subFilter.EndAt = Map(subFilter.EndAt, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = Map(subFilter.StatusId, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}