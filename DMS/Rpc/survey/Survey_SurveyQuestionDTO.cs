using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.survey
{
    public class Survey_SurveyQuestionDTO : DataDTO
    {
        public long Id { get; set; }
        public long SurveyId { get; set; }
        public string Content { get; set; }
        public long SurveyQuestionTypeId { get; set; }
        public bool IsMandatory { get; set; }
        public Survey_SurveyQuestionTypeDTO SurveyQuestionType { get; set; }   
        public List<Survey_SurveyOptionDTO> SurveyOptions { get; set; }   
        
        public Survey_SurveyQuestionDTO() {}
        public Survey_SurveyQuestionDTO(SurveyQuestion SurveyQuestion)
        {
            this.Id = SurveyQuestion.Id;
            this.SurveyId = SurveyQuestion.SurveyId;
            this.Content = SurveyQuestion.Content;
            this.SurveyQuestionTypeId = SurveyQuestion.SurveyQuestionTypeId;
            this.IsMandatory = SurveyQuestion.IsMandatory;
            this.SurveyQuestionType = SurveyQuestion.SurveyQuestionType == null ? null : new Survey_SurveyQuestionTypeDTO(SurveyQuestion.SurveyQuestionType);
            this.SurveyOptions = SurveyQuestion.SurveyOptions?.Select(so => new Survey_SurveyOptionDTO(so)).ToList();
            this.Errors = SurveyQuestion.Errors;
        }
    }

    public class Survey_SurveyQuestionFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter SurveyId { get; set; }
        
        public StringFilter Content { get; set; }
        
        public IdFilter SurveyQuestionTypeId { get; set; }
        
        public SurveyQuestionOrder OrderBy { get; set; }
    }
}