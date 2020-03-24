using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.menu
{
    public class Menu_PageDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public long MenuId { get; set; }
        public bool IsDeleted { get; set; }
        
        public Menu_PageDTO() {}
        public Menu_PageDTO(Page Page)
        {
            this.Id = Page.Id;
            this.Name = Page.Name;
            this.Path = Page.Path;
            this.MenuId = Page.MenuId;
            this.IsDeleted = Page.IsDeleted;
        }
    }

    public class Menu_PageFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter Path { get; set; }
        
        public IdFilter MenuId { get; set; }
        
        public PageOrder OrderBy { get; set; }
    }
}