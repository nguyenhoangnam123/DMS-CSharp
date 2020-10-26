using DMS.Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.mobile
{
    public class Mobile_AlbumDTO : DataDTO
    {

        public long Id { get; set; }

        public string Name { get; set; }
        public long StatusId { get; set; }
        public Mobile_StatusDTO Status { get; set; }
        public List<Mobile_AlbumImageMappingDTO> AlbumImageMappings { get; set; }
        public Mobile_AlbumDTO() { }
        public Mobile_AlbumDTO(Album Album)
        {

            this.Id = Album.Id;

            this.Name = Album.Name;
            this.StatusId = Album.StatusId;
            this.Status = Album.Status == null ? null : new Mobile_StatusDTO(Album.Status);
            this.AlbumImageMappings = Album.AlbumImageMappings?.Select(x => new Mobile_AlbumImageMappingDTO(x)).ToList();

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