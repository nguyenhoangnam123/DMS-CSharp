using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_ProblemImageMappingDTO : DataDTO
    {
        public long ProblemId { get; set; }
        public long ImageId { get; set; }
        public Mobile_ImageDTO Image { get; set; }

        public Mobile_ProblemImageMappingDTO() { }
        public Mobile_ProblemImageMappingDTO(ProblemImageMapping ProblemImageMapping)
        {
            this.ProblemId = ProblemImageMapping.ProblemId;
            this.ImageId = ProblemImageMapping.ImageId;
            this.Image = ProblemImageMapping.Image == null ? null : new Mobile_ImageDTO(ProblemImageMapping.Image);
        }
    }
}
