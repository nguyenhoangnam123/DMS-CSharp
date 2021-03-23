using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MShowingItem;
using DMS.Services.MCategory;
using DMS.Services.MStatus;
using DMS.Services.MUnitOfMeasure;

namespace DMS.Rpc.showing_item
{
    public partial class ShowingItemController : RpcController
    {
    }
}

