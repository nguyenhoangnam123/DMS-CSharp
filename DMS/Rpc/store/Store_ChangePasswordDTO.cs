using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.store
{
    public class Store_ChangePasswordDTO
    {
        public long Id { get; set; }
        public string NewPassword { get; set; }
    }
}
