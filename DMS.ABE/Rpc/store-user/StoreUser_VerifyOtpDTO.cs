using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Rpc.store_user
{
    public class StoreUser_VerifyOtpDTO
    {
        public string Username { get; set; }
        public string OtpCode { get; set; }
    }
}
