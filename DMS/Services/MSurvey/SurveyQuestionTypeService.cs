using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MSurvey
{
    public interface ISurveyQuestionTypeService : IServiceScoped
    {
        Task<int> Count(SurveyQuestionTypeFilter SurveyQuestionTypeFilter);
        Task<List<SurveyQuestionType>> List(SurveyQuestionTypeFilter SurveyQuestionTypeFilter);
    }

    public class SurveyQuestionTypeService : BaseService, ISurveyQuestionTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public SurveyQuestionTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(SurveyQuestionTypeFilter SurveyQuestionTypeFilter)
        {
            try
            {
                int result = await UOW.SurveyQuestionTypeRepository.Count(SurveyQuestionTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SurveyQuestionTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyQuestionTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<SurveyQuestionType>> List(SurveyQuestionTypeFilter SurveyQuestionTypeFilter)
        {
            try
            {
                List<SurveyQuestionType> SurveyQuestionTypes = await UOW.SurveyQuestionTypeRepository.List(SurveyQuestionTypeFilter);
                return SurveyQuestionTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SurveyQuestionTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyQuestionTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
    }
}
