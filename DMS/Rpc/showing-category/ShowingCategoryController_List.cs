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
using DMS.Services.MShowingCategory;
using DMS.Services.MImage;
using DMS.Services.MCategory;
using DMS.Services.MStatus;

namespace DMS.Rpc.showing_category
{
    public partial class ShowingCategoryController : RpcController
    {
    }
}

