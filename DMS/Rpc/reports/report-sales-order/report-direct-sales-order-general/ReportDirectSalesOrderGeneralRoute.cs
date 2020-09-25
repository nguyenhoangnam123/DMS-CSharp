﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_direct_sales_order_general
{
    public class ReportDirectSalesOrderGeneralRoute : Root
    {
        public const string Parent = Module + "/direct-sales-order-report";
        public const string Master = Module + "/direct-sales-order-report/direct-sales-order-general-report-master";

        private const string Default = Rpc + Module + "/direct-sales-order-general-report";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Total = Default + "/total";
        public const string Export = Default + "/export";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListStore = Default + "/filter-list-store";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.AppUserId), FieldTypeEnum.ID.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, Total, Export, 
                FilterListAppUser, FilterListOrganization, FilterListStore } },

        };
    }
}