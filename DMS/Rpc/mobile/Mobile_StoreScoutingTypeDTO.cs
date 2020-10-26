using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_StoreScoutingTypeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public Mobile_StoreScoutingTypeDTO() { }
        public Mobile_StoreScoutingTypeDTO(StoreScoutingType StoreScoutingType)
        {
            this.Id = StoreScoutingType.Id;
            this.Code = StoreScoutingType.Code;
            this.Name = StoreScoutingType.Name;
        }
    }
}
