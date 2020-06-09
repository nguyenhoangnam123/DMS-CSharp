using Common;
using DMS.Entities;

namespace DMS.Rpc.problem
{
    public class Problem_ProblemImageMappingDTO : DataDTO
    {
        public long ProblemId { get; set; }
        public long ImageId { get; set; }
        public Problem_ImageDTO Image { get; set; }

        public Problem_ProblemImageMappingDTO() { }
        public Problem_ProblemImageMappingDTO(ProblemImageMapping ProblemImageMapping)
        {
            this.ProblemId = ProblemImageMapping.ProblemId;
            this.ImageId = ProblemImageMapping.ImageId;
            this.Image = ProblemImageMapping.Image == null ? null : new Problem_ImageDTO(ProblemImageMapping.Image);
            this.Errors = ProblemImageMapping.Errors;
        }
    }

    public class Problem_ProblemImageMappingFilterDTO : FilterDTO
    {

        public IdFilter ProblemId { get; set; }

        public IdFilter ImageId { get; set; }

        public ProblemImageMappingOrder OrderBy { get; set; }
    }
}