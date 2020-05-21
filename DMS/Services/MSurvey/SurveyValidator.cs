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
            DescriptionEmpty,
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

        public async Task<bool> Create(Survey Survey)
        {
            if (Survey.SurveyQuestions == null)
                Survey.SurveyQuestions = new List<SurveyQuestion>();
            if (Survey.StartAt > Survey.EndAt)
                Survey.AddError(nameof(SurveyValidator), nameof(Survey.EndAt), ErrorCode.EndDateWrong);
            if (string.IsNullOrWhiteSpace(Survey.Title))
            {
                Survey.AddError(nameof(SurveyValidator), nameof(Survey.Title), ErrorCode.TitleEmpty);
            }
            if (string.IsNullOrWhiteSpace(Survey.Description))
            {
                Survey.AddError(nameof(SurveyValidator), nameof(Survey.Description), ErrorCode.DescriptionEmpty);
            }
            foreach(SurveyQuestion SurveyQuestion in Survey.SurveyQuestions)
            {
                if (string.IsNullOrWhiteSpace( SurveyQuestion.Content))
                    SurveyQuestion.AddError(nameof(SurveyValidator), nameof(Survey.Description), ErrorCode.DescriptionEmpty);

            }    
            return Survey.IsValidated;
        }

        public async Task<bool> Update(Survey Survey)
        {
            if (await ValidateId(Survey))
            {
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
