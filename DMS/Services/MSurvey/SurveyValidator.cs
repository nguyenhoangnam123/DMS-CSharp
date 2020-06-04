using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MSurvey
{
    public interface ISurveyValidator : IServiceScoped
    {
        Task<bool> Create(Survey Survey);
        Task<bool> Update(Survey Survey);
        Task<bool> CreateSurveyResult(Survey Survey);
        Task<bool> Delete(Survey Survey);
        Task<bool> SaveResult(Survey Survey);
    }

    public class SurveyValidator : ISurveyValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            EndDateWrong,
            TitleEmpty,
            StartDateWrong,
            QuestionIsMandatory
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public SurveyValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Survey Survey)
        {
            SurveyFilter SurveyFilter = new SurveyFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Survey.Id },
                Selects = SurveySelect.Id
            };

            int count = await UOW.SurveyRepository.Count(SurveyFilter);
            if (count == 0)
                Survey.AddError(nameof(SurveyValidator), nameof(Survey.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        private async Task<bool> ValidateSurveyQuestions(Survey Survey)
        {
            if (Survey.SurveyQuestions == null) Survey.SurveyQuestions = new List<SurveyQuestion>();
            else
            {
                foreach (var SurveyQuestion in Survey.SurveyQuestions)
                {
                    if (SurveyQuestion.SurveyOptions == null) SurveyQuestion.SurveyOptions = new List<SurveyOption>();
                }
            }
            return Survey.IsValidated;
        }

        private async Task<bool> ValidateTitle(Survey Survey)
        {
            if (string.IsNullOrWhiteSpace(Survey.Title))
            {
                Survey.AddError(nameof(SurveyValidator), nameof(Survey.Title), ErrorCode.TitleEmpty);
            }
            return Survey.IsValidated;
        }

        private async Task<bool> ValidateStartDate(Survey Survey)
        {
            if(Survey.StartAt == default(DateTime))
            {
                Survey.AddError(nameof(SurveyValidator), nameof(Survey.StartAt), ErrorCode.StartDateWrong);
            }
            return Survey.IsValidated;
        }

        private async Task<bool> ValidateMandatoryQuestion(Survey Survey)
        {
            var MandatoryQuestions = Survey.SurveyQuestions.Where(x => x.IsMandatory).ToList();
            foreach (var MandatoryQuestion in MandatoryQuestions)
            {
                if(MandatoryQuestion.SurveyQuestionTypeId == Enums.SurveyQuestionTypeEnum.QUESTION_SINGLE_CHOICE.Id
                || MandatoryQuestion.SurveyQuestionTypeId == Enums.SurveyQuestionTypeEnum.QUESTION_MULTIPLE_CHOICE.Id)
                {
                    if (MandatoryQuestion.SurveyOptions.All(x => x.Result == false))
                        MandatoryQuestion.AddError(nameof(SurveyValidator), nameof(MandatoryQuestion.IsMandatory), ErrorCode.QuestionIsMandatory);
                }
                else if(MandatoryQuestion.SurveyQuestionTypeId == Enums.SurveyQuestionTypeEnum.TABLE_MULTIPLE_CHOICE.Id
                || MandatoryQuestion.SurveyQuestionTypeId == Enums.SurveyQuestionTypeEnum.TABLE_SINGLE_CHOICE.Id)
                {

                }
            }
            return Survey.IsValidated;
        }

        public async Task<bool> Create(Survey Survey)
        {
            await ValidateSurveyQuestions(Survey);
            await ValidateTitle(Survey);
            await ValidateStartDate(Survey);

            return Survey.IsValidated;
        }

        public async Task<bool> Update(Survey Survey)
        {
            if (await ValidateId(Survey))
            {
                await ValidateSurveyQuestions(Survey);
                await ValidateTitle(Survey);
                await ValidateStartDate(Survey);
            }
            return Survey.IsValidated;
        }

        public async Task<bool> CreateSurveyResult(Survey Survey)
        {
            if (await ValidateId(Survey))
            {
                await ValidateMandatoryQuestion(Survey);
            }
            return Survey.IsValidated;
        }

        public async Task<bool> Delete(Survey Survey)
        {
            if (await ValidateId(Survey))
            {
            }
            return Survey.IsValidated;
        }

        public Task<bool> SaveResult(Survey Survey)
        {
            throw new NotImplementedException();
        }
    }
}
