using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile
{
    public class Mobile_ColorDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Mobile_ColorDTO() { }
        public Mobile_ColorDTO(Color Color)
        {
            this.Id = Color.Id;
            this.Code = Color.Code;
            this.Name = Color.Name;
            this.Errors = Color.Errors;
        }
    }

    public class Mobile_ColorFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public ColorOrder OrderBy { get; set; }
    }
}
