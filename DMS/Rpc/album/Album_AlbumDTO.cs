using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.album
{
    public class Album_AlbumDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Album_AlbumDTO() {}
        public Album_AlbumDTO(Album Album)
        {
            this.Id = Album.Id;
            this.Name = Album.Name;
            this.CreatedAt = Album.CreatedAt;
            this.UpdatedAt = Album.UpdatedAt;
            this.Errors = Album.Errors;
        }
    }

    public class Album_AlbumFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public AlbumOrder OrderBy { get; set; }
    }
}