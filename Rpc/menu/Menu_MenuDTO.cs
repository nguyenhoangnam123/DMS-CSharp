using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.menu
{
    public class Menu_MenuDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDeleted { get; set; }
        public List<Menu_FieldDTO> Fields { get; set; }
        public List<Menu_PageDTO> Pages { get; set; }
        public Menu_MenuDTO() {}
        public Menu_MenuDTO(Menu Menu)
        {
            this.Id = Menu.Id;
            this.Name = Menu.Name;
            this.Path = Menu.Path;
            this.IsDeleted = Menu.IsDeleted;
            this.Fields = Menu.Fields?.Select(x => new Menu_FieldDTO(x)).ToList();
            this.Pages = Menu.Pages?.Select(x => new Menu_PageDTO(x)).ToList();
        }
    }

    public class Menu_MenuFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Path { get; set; }
        public MenuOrder OrderBy { get; set; }
    }
}
