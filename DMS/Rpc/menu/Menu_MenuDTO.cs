using Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.menu
{
    public class Menu_MenuDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDeleted { get; set; }
        public List<Menu_FieldDTO> Fields { get; set; }
        public List<Menu_ActionDTO> Actions { get; set; }
        public Menu_MenuDTO() { }
        public Menu_MenuDTO(Menu Menu)
        {
            this.Id = Menu.Id;
            this.Code = Menu.Code;
            this.Name = Menu.Name;
            this.Path = Menu.Path;
            this.IsDeleted = Menu.IsDeleted;
            this.Fields = Menu.Fields?.Select(x => new Menu_FieldDTO(x)).ToList();
            this.Actions = Menu.Actions?.Select(x => new Menu_ActionDTO(x)).ToList();
        }
    }

    public class Menu_MenuFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Path { get; set; }
        public MenuOrder OrderBy { get; set; }
    }
}
