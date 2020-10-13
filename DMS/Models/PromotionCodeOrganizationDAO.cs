﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class PromotionCodeOrganizationDAO
    {
        public long PromotionCodeId { get; set; }
        public long OrganizationId { get; set; }

        public virtual OrganizationDAO Organization { get; set; }
        public virtual PromotionCodeDAO PromotionCode { get; set; }
    }
}
