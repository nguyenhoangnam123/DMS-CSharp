using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.app_user
{
    public class AppUser_PositionDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public AppUser_PositionDTO() { }
        public AppUser_PositionDTO(Position Position)
        {
            this.Id = Position.Id;
            this.Code = Position.Code;
            this.Name = Position.Name;
            this.StatusId = Position.StatusId;
        }
    }

    public class AppUser_PositionFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public PositionOrder OrderBy { get; set; }
    }
}
