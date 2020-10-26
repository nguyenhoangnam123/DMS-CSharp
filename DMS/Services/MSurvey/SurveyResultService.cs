using DMS.Common;
using DMS.Entities;
using DMS.Repositories;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MSurveyResult
{
    public interface ISurveyResultService : IServiceScoped
    {
        Task<int> Count(SurveyResultFilter SurveyResultFilter);
        Task<List<SurveyResult>> List(SurveyResultFilter SurveyResultFilter);
        Task<SurveyResult> Get(long Id);
    }

    public class SurveyResultService : BaseService, ISurveyResultService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ISurveyResultValidator SurveyResultValidator;

        public SurveyResultService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ISurveyResultValidator SurveyResultValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.SurveyResultValidator = SurveyResultValidator;
        }
        public async Task<int> Count(SurveyResultFilter SurveyResultFilter)
        {
            try
            {
                int result = await UOW.SurveyResultRepository.Count(SurveyResultFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyResultService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<SurveyResult>> List(SurveyResultFilter SurveyResultFilter)
        {
            try
            {
                List<SurveyResult> SurveyResults = await UOW.SurveyResultRepository.List(SurveyResultFilter);
                return SurveyResults;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SurveyResultService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyResultService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        public async Task<SurveyResult> Get(long Id)
        {
            SurveyResult SurveyResult = await UOW.SurveyResultRepository.Get(Id);
            if (SurveyResult == null)
                return null;
            return SurveyResult;
        }
    }
}
