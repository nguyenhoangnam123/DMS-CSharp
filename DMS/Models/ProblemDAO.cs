using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ProblemDAO
    {
        public ProblemDAO()
        {
            ProblemImageMappings = new HashSet<ProblemImageMappingDAO>();
        }

        public long Id { get; set; }
        public long? StoreCheckingId { get; set; }
        public long StoreId { get; set; }
        public long ProblemTypeId { get; set; }
        public DateTime NoteAt { get; set; }
        public string Content { get; set; }

        public virtual ProblemTypeDAO ProblemType { get; set; }
        public virtual StoreDAO Store { get; set; }
        public virtual StoreCheckingDAO StoreChecking { get; set; }
        public virtual ICollection<ProblemImageMappingDAO> ProblemImageMappings { get; set; }
    }
}
