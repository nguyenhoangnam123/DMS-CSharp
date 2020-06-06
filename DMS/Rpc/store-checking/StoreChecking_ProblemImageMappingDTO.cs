using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_ProblemImageMappingDTO : DataDTO
    {
        public long ProblemId { get; set; }
        public long ImageId { get; set; }
        public StoreChecking_ImageDTO Image { get; set; }

        public StoreChecking_ProblemImageMappingDTO() { }
        public StoreChecking_ProblemImageMappingDTO(ProblemImageMapping ProblemImageMapping)
        {
            this.ProblemId = ProblemImageMapping.ProblemId;
            this.ImageId = ProblemImageMapping.ImageId;
            this.Image = ProblemImageMapping.Image == null ? null : new StoreChecking_ImageDTO(ProblemImageMapping.Image);
        }
    }
}
