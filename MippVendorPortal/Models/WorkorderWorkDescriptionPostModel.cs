namespace MippVendorPortal.Models
{
    public class WorkorderWorkDescriptionPostModel

    {
        public int Id { get; set; }

        public int WorkorderId { get; set; }

        public int? TaskId { get; set; }

        public string WorkPerformedBy { get; set; }

        public string DescriptionOfWork { get; set; }

        public string WorkMaterials { get; set; }

        public decimal HourSpent { get; set; }

        public string AdditionalComment { get; set; }

        public bool IsDelete { get; set; }

        public string CreatedBy { get; set; }

        public string ModifideBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

        public List<IFormFile> files { get; set;}
    }
}
