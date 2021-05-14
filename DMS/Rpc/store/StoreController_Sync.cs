using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Helpers;
using DMS.Models;
using DMS.Repositories;
using DMS.Services.MAppUser;
using DMS.Services.MBrand;
using DMS.Services.MDistrict;
using DMS.Services.MOrganization;
using DMS.Services.MProductGrouping;
using DMS.Services.MProvince;
using DMS.Services.MStatus;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreScouting;
using DMS.Services.MStoreStatus;
using DMS.Services.MStoreType;
using DMS.Services.MStoreUser;
using DMS.Services.MWard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Rpc.store
{
    public partial class StoreController
    {
        [Route(StoreRoute.BulkCreate), HttpPost]
        public async Task<ActionResult<Store_StoreDTO>> BulkCreate([FromBody] List<Store> Stores)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            Stores = await StoreService.Import(Stores);
            bool IsValid = true;
            foreach(var Store in Stores)
            {
                if (!Store.IsValidated) IsValid = false;
            }
            if (!IsValid)
                return BadRequest();
            return Ok();
        }
    }
}

