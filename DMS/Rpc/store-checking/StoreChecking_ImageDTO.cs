using Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_ImageDTO : DataDTO
    {

        public long Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public List<StoreChecking_ImageStoreCheckingMappingDTO> ImageStoreCheckingMapping { get; set; }
        public StoreChecking_ImageDTO() { }
        public StoreChecking_ImageDTO(Image Image)
        {

            this.Id = Image.Id;

            this.Name = Image.Name;

            this.Url = Image.Url;
            this.ImageStoreCheckingMapping = Image.ImageStoreCheckingMapping?.Select(x => new StoreChecking_ImageStoreCheckingMappingDTO(x)).ToList();
            this.Errors = Image.Errors;
        }
    }

    public class StoreChecking_ImageFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Name { get; set; }

        public StringFilter Url { get; set; }

        public IdFilter StoreCheckingId { get; set; }
        public IdFilter AlbumId { get; set; }

        public ImageOrder OrderBy { get; set; }
    }
}