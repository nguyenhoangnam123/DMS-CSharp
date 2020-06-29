using Common;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_SurveyOptionDTO : DataDTO
    {
        public long Id { get; set; }
        public long SurveyId { get; set; }
        public string Content { get; set; }
        public long SurveyOptionTypeId { get; set; }
        public Mobile_SurveyOptionTypeDTO SurveyOptionType { get; set; }

        public Mobile_SurveyOptionDTO() { }
        public Mobile_SurveyOptionDTO(SurveyOption SurveyOption)
        {
            this.Id = SurveyOption.Id;
            this.Content = SurveyOption.Content;
            this.SurveyOptionTypeId = SurveyOption.SurveyOptionTypeId;
            this.SurveyOptionType = SurveyOption.SurveyOptionType == null ? null : new Mobile_SurveyOptionTypeDTO(SurveyOption.SurveyOptionType);
            this.Errors = SurveyOption.Errors;
        }
    }

    public class Mobile_SurveyOptionFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public IdFilter SurveyId { get; set; }

        public StringFilter Content { get; set; }

        public IdFilter SurveyOptionTypeId { get; set; }

        public SurveyOptionOrder OrderBy { get; set; }
    }
}