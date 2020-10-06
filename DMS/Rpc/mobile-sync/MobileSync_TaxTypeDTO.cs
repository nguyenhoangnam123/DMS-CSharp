using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_TaxTypeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        public long StatusId { get; set; }
        public MobileSync_TaxTypeDTO() { }
        public MobileSync_TaxTypeDTO(TaxTypeDAO TaxTypeDAO)
        {
            this.Id = TaxTypeDAO.Id;
            this.Code = TaxTypeDAO.Code;
            this.Name = TaxTypeDAO.Name;
            this.Percentage = TaxTypeDAO.Percentage;
            this.StatusId = TaxTypeDAO.StatusId;
        }
    }
}
