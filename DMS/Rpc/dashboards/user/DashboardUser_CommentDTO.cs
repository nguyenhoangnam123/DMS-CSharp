﻿using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.user
{
    public class DashboardUser_CommentDTO : DataDTO
    {
        public string Avatar { get; set; }
        public string AppUserName { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DashboardUser_CommentFilterDTO : FilterDTO
    {
        public long? AppUserId { get; set; }
    }
}
