using Common;
using DMS.Entities;

namespace DMS.Rpc.store_scouting
{
    public class StoreScouting_ImageDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public StoreScouting_ImageDTO() { }
        public StoreScouting_ImageDTO(Image Image)
        {
            this.Id = Image.Id;
            this.Name = Image.Name;
            this.Url = Image.Url;
        }
    }

    public class StoreScouting_ImageFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Url { get; set; }
        public ImageOrder OrderBy { get; set; }
    }
}
