using Common;
using DMS.Entities;

namespace DMS.Rpc.role
{
    public class Role_PageDTO : DataDTO
    {

        public long Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public long MenuId { get; set; }

        public bool IsDeleted { get; set; }


        public Role_PageDTO() { }
        public Role_PageDTO(Page Page)
        {

            this.Id = Page.Id;

            this.Name = Page.Name;

            this.Path = Page.Path;

            this.MenuId = Page.MenuId;

            this.IsDeleted = Page.IsDeleted;

        }
    }

    public class Role_PageFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Name { get; set; }

        public StringFilter Path { get; set; }

        public IdFilter MenuId { get; set; }

        public PageOrder OrderBy { get; set; }
    }
}
