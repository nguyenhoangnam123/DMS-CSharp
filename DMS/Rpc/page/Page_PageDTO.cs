using Common;
using DMS.Entities;

namespace DMS.Rpc.page
{
    public class Page_PageDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public long MenuId { get; set; }
        public bool IsDeleted { get; set; }
        public Page_MenuDTO Menu { get; set; }
        public Page_PageDTO() { }
        public Page_PageDTO(Page Page)
        {
            this.Id = Page.Id;
            this.Name = Page.Name;
            this.Path = Page.Path;
            this.MenuId = Page.MenuId;
            this.IsDeleted = Page.IsDeleted;
            this.Menu = Page.Menu == null ? null : new Page_MenuDTO(Page.Menu);
        }
    }

    public class Page_PageFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Path { get; set; }
        public IdFilter MenuId { get; set; }
        public PageOrder OrderBy { get; set; }
    }
}
