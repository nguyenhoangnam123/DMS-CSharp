using Common;
using DMS.Entities;

namespace DMS.Rpc.store
{
    public class Store_ImageDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Store_ImageDTO() { }
        public Store_ImageDTO(Image Image)
        {
            this.Id = Image.Id;
            this.Name = Image.Name;
            this.Url = Image.Url;
        }
    }

    public class Store_ImageFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Url { get; set; }
        public ImageOrder OrderBy { get; set; }
    }
}
