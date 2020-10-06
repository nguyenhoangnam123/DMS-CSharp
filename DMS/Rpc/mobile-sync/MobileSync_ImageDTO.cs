using DMS.Entities;
using DMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_ImageDTO
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }


        public MobileSync_ImageDTO() { }
        public MobileSync_ImageDTO(ImageDAO ImageDAO)
        {
            this.Id = ImageDAO.Id;
            this.Name = ImageDAO.Name;
            this.Url = ImageDAO.Url;
        }
    }
}
