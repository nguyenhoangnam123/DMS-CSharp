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
    }

    public class SurveyValidator : ISurveyValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
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
    }
}
