using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Entities
{
    public class Action : DataEntity
    {
        public long Id { get; set; }
        public long MenuId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
