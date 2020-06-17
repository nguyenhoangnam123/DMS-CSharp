using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards
{
    public class Dashboard_IndirectSalesOrderDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime OrderDate { get; set; }
        public long RequestStateId { get; set; }
        public long SaleEmployeeId { get; set; }
        public decimal Total { get; set; }
        public Dashboard_AppUserDTO SaleEmployee { get; set; }
        public Dashboard_RequestStateDTO RequestState { get; set; }

        public Dashboard_IndirectSalesOrderDTO() { }
        public Dashboard_IndirectSalesOrderDTO(IndirectSalesOrder IndirectSalesOrder)
        {
            this.Id = IndirectSalesOrder.Id;
            this.Code = IndirectSalesOrder.Code;
            this.OrderDate = IndirectSalesOrder.OrderDate;
            this.RequestStateId = IndirectSalesOrder.RequestStateId;
            this.SaleEmployeeId = IndirectSalesOrder.SaleEmployeeId;
            this.Total = IndirectSalesOrder.Total;
            this.SaleEmployee = IndirectSalesOrder.SaleEmployee == null ? null : new Dashboard_AppUserDTO(IndirectSalesOrder.SaleEmployee);
            this.RequestState = IndirectSalesOrder.RequestState == null ? null : new Dashboard_RequestStateDTO(IndirectSalesOrder.RequestState);
        }
    }
}
