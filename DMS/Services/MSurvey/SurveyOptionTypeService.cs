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
    public interface ISurveyOptionTypeService :  IServiceScoped
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyOptionTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyOptionTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
    }
}
