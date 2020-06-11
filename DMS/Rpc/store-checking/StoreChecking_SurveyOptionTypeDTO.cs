using Common;
using DMS.Entities;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_SurveyOptionTypeDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public StoreChecking_SurveyOptionTypeDTO() { }
        public StoreChecking_SurveyOptionTypeDTO(SurveyOptionType SurveyOptionType)
        {

            this.Id = SurveyOptionType.Id;

            this.Code = SurveyOptionType.Code;

            this.Name = SurveyOptionType.Name;

            this.Errors = SurveyOptionType.Errors;
        }
    }

    public class Survey_SurveyOptionTypeFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public SurveyOptionTypeOrder OrderBy { get; set; }
    }
}