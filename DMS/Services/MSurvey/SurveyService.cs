using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MSurvey
{
    public interface ISurveyService : IServiceScoped
    {
        Task<int> Count(SurveyFilter SurveyFilter);
        Task<List<Survey>> List(SurveyFilter SurveyFilter);
        Task<Survey> Get(long Id);
        Task<Survey> Create(Survey Survey);
        Task<Survey> Update(Survey Survey);
        Task<Survey> Delete(Survey Survey);
        Task<Survey> GetForm(long Id);
        Task<Survey> SaveForm(Survey Survey);
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
            return Survey;
        }

        public async Task<Survey> Create(Survey Survey)
        {
            if (!await SurveyValidator.Create(Survey))
                return Survey;

            try
            {
                Survey.CreatorId = CurrentContext.UserId;
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

        public async Task<Survey> CreateSurveyResult(Survey Survey)
        {
            if (!await SurveyValidator.CreateSurveyResult(Survey))
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
                }
            }
            return filter;
        }

        public async Task<Survey> GetForm(long Id)
        {
            Survey Survey = await UOW.SurveyRepository.Get(Id);
            if (Survey.SurveyQuestions != null)
            {
                foreach (SurveyQuestion SurveyQuestion in Survey.SurveyQuestions)
                {
                    if (SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_MULTIPLE_CHOICE.Id ||
                        SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_SINGLE_CHOICE.Id)
                    {
                        SurveyQuestion.ListResult = new Dictionary<long, bool>();
                        if (SurveyQuestion.SurveyOptions != null)
                        {
                            foreach (SurveyOption SurveyOption in SurveyQuestion.SurveyOptions)
                            {
                                SurveyQuestion.ListResult.Add(SurveyOption.Id, false);
                            }
                        }
                    }
                    if (SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.TABLE_MULTIPLE_CHOICE.Id ||
                        SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.TABLE_SINGLE_CHOICE.Id)
                    {
                        SurveyQuestion.TableResult = new Dictionary<long, Dictionary<long, bool>>();
                        if (SurveyQuestion.SurveyOptions != null)
                        {
                            List<SurveyOption> Columns = SurveyQuestion.SurveyOptions.Where(so => so.SurveyOptionTypeId == SurveyOptionTypeEnum.COLUMN.Id).ToList();
                            List<SurveyOption> Rows = SurveyQuestion.SurveyOptions.Where(so => so.SurveyOptionTypeId == SurveyOptionTypeEnum.ROW.Id).ToList();
                            SurveyQuestion.TableResult = new Dictionary<long, Dictionary<long, bool>>();
                            foreach (SurveyOption Row in Rows)
                            {
                                Dictionary<long, bool> RowResult = new Dictionary<long, bool>();
                                SurveyQuestion.TableResult.Add(Row.Id, RowResult);
                                foreach (SurveyOption Column in Columns)
                                {
                                    RowResult.Add(Column.Id, false);
                                }
                            }
                        }
                    }
                }
            }
            return Survey;
        }

        public async Task<Survey> SaveForm(Survey Survey)
        {
            SurveyResult SurveyResult = new SurveyResult();
            SurveyResult.SurveyId = Survey.Id;
            SurveyResult.AppUserId = CurrentContext.UserId;
            SurveyResult.StoreId = Survey.StoreId;
            SurveyResult.Time = Survey.AnswerAt;
            SurveyResult.SurveyResultSingles = new List<SurveyResultSingle>();
            SurveyResult.SurveyResultCells = new List<SurveyResultCell>();
            if (Survey.SurveyQuestions != null)
            {
                foreach (SurveyQuestion SurveyQuestion in Survey.SurveyQuestions)
                {
                    if (SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_MULTIPLE_CHOICE.Id ||
                        SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.QUESTION_SINGLE_CHOICE.Id)
                    {
                        foreach (SurveyOption SurveyOption in SurveyQuestion.SurveyOptions)
                        {
                            if (SurveyQuestion.ListResult.ContainsKey(SurveyOption.Id) && SurveyQuestion.ListResult[SurveyOption.Id])
                            {
                                SurveyResultSingle SurveyResultSingle = new SurveyResultSingle
                                {
                                    SurveyOptionId = SurveyOption.Id,
                                    SurveyQuestionId = SurveyQuestion.Id,
                                };
                                SurveyResult.SurveyResultSingles.Add(SurveyResultSingle);
                            }
                        }
                    }
                    if (SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.TABLE_MULTIPLE_CHOICE.Id ||
                        SurveyQuestion.SurveyQuestionTypeId == SurveyQuestionTypeEnum.TABLE_SINGLE_CHOICE.Id)
                    {
                        if (SurveyQuestion.SurveyOptions != null)
                        {
                            List<SurveyOption> Columns = SurveyQuestion.SurveyOptions.Where(so => so.SurveyOptionTypeId == SurveyOptionTypeEnum.COLUMN.Id).ToList();
                            List<SurveyOption> Rows = SurveyQuestion.SurveyOptions.Where(so => so.SurveyOptionTypeId == SurveyOptionTypeEnum.ROW.Id).ToList();
                            foreach (SurveyOption Row in Rows)
                            {
                                if (SurveyQuestion.TableResult.ContainsKey(Row.Id))
                                {
                                    Dictionary<long, bool> ColumnResult = SurveyQuestion.TableResult[Row.Id];
                                    foreach (SurveyOption Column in Columns)
                                    {
                                        if (ColumnResult.ContainsKey(Column.Id) && ColumnResult[Column.Id])
                                        {
                                            SurveyResultCell SurveyResultCell = new SurveyResultCell
                                            {
                                                SurveyQuestionId = SurveyQuestion.Id,
                                                ColumnOptionId = Column.Id,
                                                RowOptionId = Row.Id,
                                            };
                                            SurveyResult.SurveyResultCells.Add(SurveyResultCell);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            await UOW.SurveyResultRepository.Create(SurveyResult);
            return await GetForm(Survey.Id);
        }
    }
}
