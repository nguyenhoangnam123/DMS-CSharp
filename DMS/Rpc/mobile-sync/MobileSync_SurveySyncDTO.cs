using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_SurveySyncDTO
    {
        public List<MobileSync_SurveyDTO> Created { get; set; }
        public List<MobileSync_SurveyDTO> Updated { get; set; }
        public List<MobileSync_SurveyDTO> Deleted { get; set; }
    }

    public class MobileSync_SurveyDTO
    {

    }
}
