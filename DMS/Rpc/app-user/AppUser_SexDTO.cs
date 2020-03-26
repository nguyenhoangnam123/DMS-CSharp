using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.app_user
{
    public class AppUser_SexDTO : DataDTO
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public AppUser_SexDTO() { }
        public AppUser_SexDTO(Sex Sex)
        {

            this.Id = Sex.Id;

            this.Code = Sex.Code;

            this.Name = Sex.Name;

        }
    }

    public class AppUser_SexFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public SexOrder OrderBy { get; set; }
    }
}
