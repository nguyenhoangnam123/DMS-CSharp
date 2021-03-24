using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.mobile.general_mobile
{
    public class GeneralMobile_SystemConfigurationDTO : DataDTO
    {
        public long STORE_CHECKING_DISTANCE { get; set; }
        public bool STORE_CHECKING_OFFLINE_CONSTRAINT_DISTANCE { get; set; }
        public bool USE_DIRECT_SALES_ORDER { get; set; }
        public bool USE_INDIRECT_SALES_ORDER { get; set; }
        public bool ALLOW_EDIT_KPI_IN_PERIOD { get; set; }
        public long PRIORITY_USE_PRICE_LIST { get; set; }
        public long PRIORITY_USE_PROMOTION { get; set; }
        public long STORE_CHECKING_MINIMUM_TIME { get; set; }
        public long DASH_BOARD_REFRESH_TIME { get; set; }
        public decimal AMPLITUDE_PRICE_IN_DIRECT { get; set; }
        public decimal AMPLITUDE_PRICE_IN_INDIRECT { get; set; }
        public string YOUTUBE_ID { get; set; }

        public GeneralMobile_SystemConfigurationDTO() { }
        public GeneralMobile_SystemConfigurationDTO(SystemConfiguration SystemConfiguration)
        {
            this.STORE_CHECKING_DISTANCE = SystemConfiguration.STORE_CHECKING_DISTANCE;
            this.STORE_CHECKING_OFFLINE_CONSTRAINT_DISTANCE = SystemConfiguration.STORE_CHECKING_OFFLINE_CONSTRAINT_DISTANCE;
            this.USE_DIRECT_SALES_ORDER = SystemConfiguration.USE_DIRECT_SALES_ORDER;
            this.USE_INDIRECT_SALES_ORDER = SystemConfiguration.USE_INDIRECT_SALES_ORDER;
            this.ALLOW_EDIT_KPI_IN_PERIOD = SystemConfiguration.ALLOW_EDIT_KPI_IN_PERIOD;
            this.PRIORITY_USE_PRICE_LIST = SystemConfiguration.PRIORITY_USE_PRICE_LIST;
            this.PRIORITY_USE_PROMOTION = SystemConfiguration.PRIORITY_USE_PROMOTION;
            this.STORE_CHECKING_MINIMUM_TIME = SystemConfiguration.STORE_CHECKING_MINIMUM_TIME;
            this.DASH_BOARD_REFRESH_TIME = SystemConfiguration.DASH_BOARD_REFRESH_TIME;
            this.AMPLITUDE_PRICE_IN_DIRECT = SystemConfiguration.AMPLITUDE_PRICE_IN_DIRECT;
            this.AMPLITUDE_PRICE_IN_INDIRECT = SystemConfiguration.AMPLITUDE_PRICE_IN_INDIRECT;
            this.YOUTUBE_ID = SystemConfiguration.YOUTUBE_ID;
        }
    }
}
