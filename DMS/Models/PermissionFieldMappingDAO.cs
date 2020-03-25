namespace DMS.Models
{
    public partial class PermissionFieldMappingDAO
    {
        public long PermissionId { get; set; }
        public long FieldId { get; set; }
        public string Value { get; set; }

        public virtual FieldDAO Field { get; set; }
        public virtual PermissionDAO Permission { get; set; }
    }
}
