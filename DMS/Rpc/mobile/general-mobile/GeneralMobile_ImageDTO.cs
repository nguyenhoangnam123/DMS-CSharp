using DMS.Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.mobile.general_mobile
{
    public class GeneralMobile_ImageDTO : DataDTO
    {

        public long Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }

        public List<GeneralMobile_StoreCheckingImageMappingDTO> ImageStoreCheckingMapping { get; set; }
        public GeneralMobile_ImageDTO() { }
        public GeneralMobile_ImageDTO(Image Image)
        {

            this.Id = Image.Id;

            this.Name = Image.Name;

            this.Url = Image.Url;
            this.ThumbnailUrl = Image.ThumbnailUrl;
            this.ImageStoreCheckingMapping = Image.ImageStoreCheckingMapping?.Select(x => new GeneralMobile_StoreCheckingImageMappingDTO(x)).ToList();
            this.Errors = Image.Errors;
        }
    }

    public class GeneralMobile_ImageFilterDTO : FilterDTO
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