using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ProblemStatusDAO
    {
        public ProblemStatusDAO()
        {
            Problems = new HashSet<ProblemDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ProblemDAO> Problems { get; set; }
    }
}
