using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_AlbumDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        

        public StoreChecking_AlbumDTO() {}
        public StoreChecking_AlbumDTO(Album Album)
        {
            
            this.Id = Album.Id;
            
            this.Name = Album.Name;
            
            this.Errors = Album.Errors;
        }
    }

    public class StoreChecking_AlbumFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public AlbumOrder OrderBy { get; set; }
    }
}