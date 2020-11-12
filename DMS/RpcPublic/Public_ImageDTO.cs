using DMS.Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.RpcPublic
{
    public class Public_ImageDTO : DataDTO
    {

        public long Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }

        public Public_ImageDTO() { }
        public Public_ImageDTO(Image Image)
        {
            this.Id = Image.Id;
            this.Name = Image.Name;
            this.Url = Image.Url;
            this.ThumbnailUrl = Image.ThumbnailUrl;
            this.Errors = Image.Errors;
        }
    }

    public class Public_ImageFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Name { get; set; }

        public StringFilter Url { get; set; }
        public StringFilter ThumbnailUrl { get; set; }

        public IdFilter StoreCheckingId { get; set; }
        public IdFilter AlbumId { get; set; }

        public ImageOrder OrderBy { get; set; }
    }
}