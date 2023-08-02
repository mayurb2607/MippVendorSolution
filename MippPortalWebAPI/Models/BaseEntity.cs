namespace MippPortalWebAPI.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set;}
        public DateTime ModifiedOn { get; set; }

    }
}
