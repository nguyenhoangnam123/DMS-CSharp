using System;
using System.Collections.Generic;

namespace DMS.ABE.Models
{
    public partial class IdGeneratorDAO
    {
        public long Id { get; set; }
        public long IdGenerateTypeId { get; set; }
        public bool Used { get; set; }
        public long Counter { get; set; }
    }
}
