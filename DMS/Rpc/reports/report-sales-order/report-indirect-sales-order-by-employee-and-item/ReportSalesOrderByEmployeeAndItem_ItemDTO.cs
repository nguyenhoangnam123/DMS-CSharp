﻿using DMS.Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_indirect_sales_order_by_employee_and_item
{
    public class ReportSalesOrderByEmployeeAndItem_ItemDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string UnitOfMeasureName { get; set; }
        public long SaleStock { get; set; }
        public long PromotionStock { get; set; }
        public decimal SalePriceAverage { get; set; }
        public decimal Discount { get; set; }
        public decimal Revenue { get; set; }
        public long BuyerStoreCounter => StoreIds?.Count() ?? 0;
        public long SalesOrderCounter => IndirectSalesOrderIds?.Count() ?? 0;
        internal HashSet<long> StoreIds { get; set; }
        internal HashSet<long> IndirectSalesOrderIds { get; set; }
        public ReportSalesOrderByEmployeeAndItem_ItemDTO() { }
        public ReportSalesOrderByEmployeeAndItem_ItemDTO(Item Item)
        {
            this.Id = Item.Id;
            this.Code = Item.Code;
            this.Name = Item.Name;
            this.SaleStock = Item.SaleStock;
            this.Errors = Item.Errors;
        }
    }

    public class ReportSalesOrderByEmployeeAndItem_ItemFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter ProductTypeId { get; set; }
        public IdFilter ProductGroupingId { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public ItemOrder OrderBy { get; set; }
    }
}
