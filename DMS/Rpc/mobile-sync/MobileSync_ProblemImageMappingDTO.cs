using Common;
using DMS.Entities;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_ProblemImageMappingDTO : DataDTO
    {
        public long ProblemId { get; set; }
        public long ImageId { get; set; }
        public MobileSync_ImageDTO Image { get; set; }

        public MobileSync_ProblemImageMappingDTO() { }
        public MobileSync_ProblemImageMappingDTO(ProblemImageMapping ProblemImageMapping)
        {
            this.ProblemId = ProblemImageMapping.ProblemId;
            this.ImageId = ProblemImageMapping.ImageId;
            this.Image = ProblemImageMapping.Image == null ? null : new MobileSync_ImageDTO(ProblemImageMapping.Image);
            this.Errors = ProblemImageMapping.Errors;
        }
    }

    public class MobileSync_ProblemImageMappingFilterDTO : FilterDTO
    {

        public IdFilter ProblemId { get; set; }

        public IdFilter ImageId { get; set; }

        public ProblemImageMappingOrder OrderBy { get; set; }
    }
}