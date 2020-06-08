namespace DMS.Models
{
    public partial class NotificationDAO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long? OrganizationId { get; set; }

        public virtual OrganizationDAO Organization { get; set; }
    }
}
