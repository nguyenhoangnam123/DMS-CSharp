using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class SystemConfigurationEnum
    {
        public static GenericEnum STORE_CHECKING_DISTANCE = new GenericEnum { Id = 1, Code = "STORE_CHECKING_DISTANCE", Name = "Khoảng cách cho phép viếng thăm" };
        public static GenericEnum STORE_CHECKING_OFFLINE_CONSTRAINT_DISTANCE = new GenericEnum { Id = 2, Code = "STORE_CHECKING_OFFLINE_CONSTRAINT_DISTANCE", Name = "Viếng thăm ngoại tuyến có ràng buộc khoảng cách hay không?" };
        public static GenericEnum USE_DIRECT_SALES_ORDER = new GenericEnum { Id = 3, Code = "USE_DIRECT_SALES_ORDER", Name = "Sử dụng đơn hàng trực tiếp" };
        public static GenericEnum USE_INDIRECT_SALES_ORDER = new GenericEnum { Id = 4, Code = "USE_INDIRECT_SALES_ORDER", Name = "Sử dụng đơn hàng gián tiếp" };
        public static GenericEnum ALLOW_EDIT_KPI_IN_PERIOD = new GenericEnum { Id = 5, Code = "ALLOW_EDIT_KPI_IN_PERIOD", Name = "Cho phép sửa KPI trong tháng" };
        public static GenericEnum PRIORITY_USE_PRICE_LIST = new GenericEnum { Id = 6, Code = "PRIORITY_USE_PRICE_LIST", Name = "Ưu tiên lấy giá theo bảng giá" };
        public static GenericEnum PRIORITY_USE_PROMOTION = new GenericEnum { Id = 7, Code = "PRIORITY_USE_PROMOTION", Name = "Ưu tiên lấy khuyến mãi theo" };
        public static GenericEnum STORE_CHECKING_MINIMUM_TIME = new GenericEnum { Id = 8, Code = "STORE_CHECKING_MINIMUM_TIME", Name = "Thời gian tối thiểu viếng thăm khách hàng" };
        public static GenericEnum DASH_BOARD_REFRESH_TIME = new GenericEnum { Id = 9, Code = "DASH_BOARD_REFRESH_TIME", Name = "Tần suất tự Reload dữ liệu trên màn hình Dashboard" };
        public static GenericEnum AMPLITUDE_PRICE_IN_DIRECT = new GenericEnum { Id = 10, Code = "AMPLITUDE_PRICE_IN_DIRECT", Name = "Biên độ cho phép sửa giá trên đơn hàng trực tiếp" };
        public static GenericEnum AMPLITUDE_PRICE_IN_INDIRECT = new GenericEnum { Id = 11, Code = "AMPLITUDE_PRICE_IN_INDIRECT", Name = "Biên độ cho phép sửa giá trên đơn hàng gián tiếp" };
        public static GenericEnum USE_STORE_APPROVAL = new GenericEnum { Id = 12, Code = "USE_STORE_APPROVAL", Name = "Sử dụng cửa hàng phê duyệt" };
        public static GenericEnum USE_ERP_APPROVAL = new GenericEnum { Id = 13, Code = "USE_ERP_APPROVAL", Name = "Sử dụng ERP phê duyệt" };

        public static List<GenericEnum> SystemConfigurationEnumList = new List<GenericEnum>
        {
            STORE_CHECKING_DISTANCE,
            STORE_CHECKING_OFFLINE_CONSTRAINT_DISTANCE,
            USE_DIRECT_SALES_ORDER,
            USE_INDIRECT_SALES_ORDER,
            ALLOW_EDIT_KPI_IN_PERIOD,
            PRIORITY_USE_PRICE_LIST,
            PRIORITY_USE_PROMOTION,
            STORE_CHECKING_MINIMUM_TIME,
            DASH_BOARD_REFRESH_TIME,
            AMPLITUDE_PRICE_IN_DIRECT,
            AMPLITUDE_PRICE_IN_INDIRECT,
            USE_STORE_APPROVAL,
            USE_ERP_APPROVAL
        };
    }
}
