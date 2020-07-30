using Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.mobile
{
    public class Mobile_AlbumDTO : DataDTO
    {

        public long Id { get; set; }

        public string Name { get; set; }

        public List<Mobile_StoreCheckingImageMappingDTO> StoreCheckingImageMappings { get; set; }
        public Mobile_AlbumDTO() { }
        public Mobile_AlbumDTO(Album Album)
        {

            this.Id = Album.Id;

            this.Name = Album.Name;
            this.StoreCheckingImageMappings = Album.StoreCheckingImageMappings?.Select(x => new Mobile_StoreCheckingImageMappingDTO(x)).ToList();

            this.Errors = Album.Errors;
        }
    }

    public class Mobile_AlbumFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Name { get; set; }

        public AlbumOrder OrderBy { get; set; }
    }
}