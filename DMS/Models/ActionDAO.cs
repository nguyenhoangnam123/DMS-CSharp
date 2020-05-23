using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ActionDAO
    {
        public ActionDAO()
        {
            ActionPageMappings = new HashSet<ActionPageMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long MenuId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual MenuDAO Menu { get; set; }
        public virtual ICollection<ActionPageMappingDAO> ActionPageMappings { get; set; }
    }
}
