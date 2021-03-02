using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.mobile.permission_mobile
{
    public class PermissionMobile_KpiPeriodDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public PermissionMobile_KpiPeriodDTO() { }
        public PermissionMobile_KpiPeriodDTO(KpiPeriod KpiPeriod)
        {

            this.Id = KpiPeriod.Id;

            this.Code = KpiPeriod.Code;

            this.Name = KpiPeriod.Name;

            this.Errors = KpiPeriod.Errors;
        }
    }

    public class PermissionMobile_KpiPeriodlFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public KpiPeriodOrder OrderBy { get; set; }
    }
}