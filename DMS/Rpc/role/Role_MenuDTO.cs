using Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.role
{
    public class Role_MenuDTO : DataDTO
    {

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public string Path { get; set; }

        public bool IsDeleted { get; set; }
        public List<Role_FieldDTO> Fields { get; set; }
        public List<Role_PageDTO> Pages { get; set; }

        public Role_MenuDTO() { }
        public Role_MenuDTO(Menu Menu)
        {

            this.Id = Menu.Id;
            this.Code = Menu.Code;
            this.Name = Menu.Name;

            this.Path = Menu.Path;

            this.IsDeleted = Menu.IsDeleted;

            this.Fields = Menu.Fields?.Select(x => new Role_FieldDTO(x)).ToList();
            this.Pages = Menu.Pages?.Select(x => new Role_PageDTO(x)).ToList();
        }
    }

    public class Role_MenuFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }

        public StringFilter Path { get; set; }

        public MenuOrder OrderBy { get; set; }
    }
}