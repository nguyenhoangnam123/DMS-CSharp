using Common;
using DMS.Entities;

namespace DMS.Rpc.problem
{
    public class MonitorStoreProblem_ImageDTO : DataDTO
    {

        public long Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }


        public MonitorStoreProblem_ImageDTO() { }
        public MonitorStoreProblem_ImageDTO(Image Image)
        {

            this.Id = Image.Id;

            this.Name = Image.Name;

            this.Url = Image.Url;

            this.Errors = Image.Errors;
        }
    }

    public class Problem_ImageFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Name { get; set; }

        public StringFilter Url { get; set; }

        public ImageOrder OrderBy { get; set; }
    }
}