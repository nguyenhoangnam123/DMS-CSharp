using Common;
using DMS.Entities;

namespace DMS.Rpc.field
{
    public class Field_MenuDTO : DataDTO
    {

        public long Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public bool IsDeleted { get; set; }


        public Field_MenuDTO() { }
        public Field_MenuDTO(Menu Menu)
        {

            this.Id = Menu.Id;

            this.Name = Menu.Name;

            this.Path = Menu.Path;

            this.IsDeleted = Menu.IsDeleted;

        }
    }

    public class Field_MenuFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Name { get; set; }

        public StringFilter Path { get; set; }

        public MenuOrder OrderBy { get; set; }
    }
}