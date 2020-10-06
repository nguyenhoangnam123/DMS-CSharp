using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_ProblemImageMappingDTO : DataDTO
    {
        public long ProblemId { get; set; }
        public long ImageId { get; set; }
        public MobileSync_ImageDTO Image { get; set; }

        public MobileSync_ProblemImageMappingDTO() { }
        public MobileSync_ProblemImageMappingDTO(ProblemImageMappingDAO ProblemImageMappingDAO)
        {
            this.ProblemId = ProblemImageMappingDAO.ProblemId;
            this.ImageId = ProblemImageMappingDAO.ImageId;
            this.Image = ProblemImageMappingDAO.Image == null ? null : new MobileSync_ImageDTO(ProblemImageMappingDAO.Image);
        }
    }

    public class MobileSync_ProblemImageMappingFilterDTO : FilterDTO
    {

        public IdFilter ProblemId { get; set; }

        public IdFilter ImageId { get; set; }

        public ProblemImageMappingOrder OrderBy { get; set; }
    }
}