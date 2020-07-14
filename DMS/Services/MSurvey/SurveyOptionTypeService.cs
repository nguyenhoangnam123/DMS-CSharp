using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MSurvey
{
    public interface ISurveyOptionTypeService : IServiceScoped
    {
        Task<int> Count(SurveyOptionTypeFilter SurveyOptionTypeFilter);
        Task<List<SurveyOptionType>> List(SurveyOptionTypeFilter SurveyOptionTypeFilter);
    }

    public class SurveyOptionTypeService : BaseService, ISurveyOptionTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public SurveyOptionTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(SurveyOptionTypeFilter SurveyOptionTypeFilter)
        {
            try
            {
                int result = await UOW.SurveyOptionTypeRepository.Count(SurveyOptionTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SurveyOptionTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyOptionTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<SurveyOptionType>> List(SurveyOptionTypeFilter SurveyOptionTypeFilter)
        {
            try
            {
                List<SurveyOptionType> SurveyOptionTypes = await UOW.SurveyOptionTypeRepository.List(SurveyOptionTypeFilter);
                return SurveyOptionTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SurveyOptionTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyOptionTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
    }
}
