﻿using DMS.Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_direct_sales_order_by_item
{
    public class ReportDirectSalesOrderByItem_ProductTypeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public ReportDirectSalesOrderByItem_ProductTypeDTO() { }
        public ReportDirectSalesOrderByItem_ProductTypeDTO(ProductType ProductType)
        {
            this.Id = ProductType.Id;
            this.Code = ProductType.Code;
            this.Name = ProductType.Name;
            this.Errors = ProductType.Errors;
        }
    }

    public class ReportDirectSalesOrderByItem_ProductTypeFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public ProductTypeOrder OrderBy { get; set; }
    }
}
