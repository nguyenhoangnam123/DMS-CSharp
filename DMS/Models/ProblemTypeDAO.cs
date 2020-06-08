using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ProblemTypeDAO
    {
        public ProblemTypeDAO()
        {
            Problems = new HashSet<ProblemDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ProblemDAO> Problems { get; set; }
    }
}
