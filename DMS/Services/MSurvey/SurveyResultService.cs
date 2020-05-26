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

namespace DMS.Services.MSurveyResult
{
    public interface ISurveyResultService : IServiceScoped
    {
        Task<int> Count(SurveyResultFilter SurveyResultFilter);
        Task<List<SurveyResult>> List(SurveyResultFilter SurveyResultFilter);
        Task<SurveyResult> Get(long Id);
        Task<SurveyResult> Create(SurveyResult SurveyResult);
        Task<SurveyResult> Delete(SurveyResult SurveyResult);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyResultService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<SurveyResult> Get(long Id)
        {
            SurveyResult SurveyResult = await UOW.SurveyResultRepository.Get(Id);
            if (SurveyResult == null)
                return null;
            return SurveyResult;
        }

        public async Task<SurveyResult> Create(SurveyResult SurveyResult)
        {
            if (!await SurveyResultValidator.Create(SurveyResult))
                return SurveyResult;

            try
            {
                await UOW.Begin();
                await UOW.SurveyResultRepository.Create(SurveyResult);
                await UOW.Commit();

                await Logging.CreateAuditLog(SurveyResult, new { }, nameof(SurveyResultService));
                return await UOW.SurveyResultRepository.Get(SurveyResult.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyResultService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }


        public async Task<SurveyResult> Delete(SurveyResult SurveyResult)
        {
            if (!await SurveyResultValidator.Delete(SurveyResult))
                return SurveyResult;

            try
            {
                await UOW.Begin();
                await UOW.SurveyResultRepository.Delete(SurveyResult);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, SurveyResult, nameof(SurveyResultService));
                return SurveyResult;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(SurveyResultService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
    }
}
