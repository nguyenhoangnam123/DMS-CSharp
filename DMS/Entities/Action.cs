﻿using Common;

namespace DMS.Entities
{
    public class Action : DataEntity
    {
        public long Id { get; set; }
        public long MenuId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
