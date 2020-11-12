using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.store_scouting
{
    public class StoreScouting_ExportDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<StoreScouting_StoreScoutingDTO> StoreScoutings { get; set; }
    }
}
