using Common;
using DMS.Entities;

namespace DMS.Rpc.monitor.monitor_store_images
{
    public class MonitorStoreImage_ImageDTO : DataDTO
    {

        public long Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }


        public MonitorStoreImage_ImageDTO() { }
        public MonitorStoreImage_ImageDTO(Image Image)
        {

            this.Id = Image.Id;

            this.Name = Image.Name;

            this.Url = Image.Url;
            this.ThumbnailUrl = Image.ThumbnailUrl;

            this.Errors = Image.Errors;
        }
    }

    public class MonitorStoreImage_ImageFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Name { get; set; }

        public StringFilter Url { get; set; }
        public StringFilter ThumbnailUrl { get; set; }

        public ImageOrder OrderBy { get; set; }
    }
}