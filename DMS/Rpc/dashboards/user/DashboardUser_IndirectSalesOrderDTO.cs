using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.user
{
    public class DashboardUser_IndirectSalesOrderDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime OrderDate { get; set; }
        public long RequestStateId { get; set; }
        public long SaleEmployeeId { get; set; }
        public decimal Total { get; set; }
        public DashboardUser_AppUserDTO SaleEmployee { get; set; }
        public DashboardUser_RequestStateDTO RequestState { get; set; }

        public DashboardUser_IndirectSalesOrderDTO() { }
        public DashboardUser_IndirectSalesOrderDTO(IndirectSalesOrder IndirectSalesOrder)
        {
            this.Id = IndirectSalesOrder.Id;
            this.Code = IndirectSalesOrder.Code;
            this.OrderDate = IndirectSalesOrder.OrderDate;
            this.RequestStateId = IndirectSalesOrder.RequestStateId;
            this.SaleEmployeeId = IndirectSalesOrder.SaleEmployeeId;
            this.Total = IndirectSalesOrder.Total;
            this.SaleEmployee = IndirectSalesOrder.SaleEmployee == null ? null : new DashboardUser_AppUserDTO(IndirectSalesOrder.SaleEmployee);
            this.RequestState = IndirectSalesOrder.RequestState == null ? null : new DashboardUser_RequestStateDTO(IndirectSalesOrder.RequestState);
        }
    }
}
