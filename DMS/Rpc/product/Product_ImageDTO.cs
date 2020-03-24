using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.product
{
    public class Product_ImageDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public string Url { get; set; }
        

        public Product_ImageDTO() {}
        public Product_ImageDTO(Image Image)
        {
            
            this.Id = Image.Id;
            
            this.Name = Image.Name;
            
            this.Url = Image.Url;
            
        }
    }

    public class Product_ImageFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter Url { get; set; }
        
        public ImageOrder OrderBy { get; set; }
    }
}