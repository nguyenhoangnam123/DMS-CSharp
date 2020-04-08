using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Rpc
{
    [Authorize]
    [Authorize(Policy = "Permission")]

    public class RpcController : ControllerBase
    {
    }
}
