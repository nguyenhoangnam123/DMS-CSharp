using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Entities
{
    public class SystemLog
    {
        public long Id { get; set; }
        public long? AppUserId { get; set; }
        public string AppUser { get; set; }
        public string Exception { get; set; }
        public string ModuleName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
    }
}
