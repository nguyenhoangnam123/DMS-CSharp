using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_KpiCriteriaItemDTO : DataDTO
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public Mobile_KpiCriteriaItemDTO() { }
        public Mobile_KpiCriteriaItemDTO(KpiCriteriaItem KpiCriteriaItem)
        {

            this.Id = KpiCriteriaItem.Id;

            this.Code = KpiCriteriaItem.Code;

            this.Name = KpiCriteriaItem.Name;

            this.Errors = KpiCriteriaItem.Errors;
        }
    }
}
