using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile.permission_mobile
{
    public class PermissionMobile_TopRevenueBySalesEmployeeDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public string SaleEmployeeName { get; set; }
        public decimal Revenue { get; set; }
    }

    public class PermissionMobile_FilterDTO : FilterDTO
    {
        public IdFilter Time { get; set; }
        public IdFilter EmployeeId { get; set; }
    }
}
