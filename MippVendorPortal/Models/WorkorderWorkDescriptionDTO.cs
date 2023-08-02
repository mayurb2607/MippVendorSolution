using MippVendorPortal.ViewModel;

namespace MippVendorPortal.Models
{
    public class WorkorderWorkDescriptionDTO
    {
        public WorkorderWorkDescriptionDTO(WorkorderWorkDescription workDescription, List<BlobFile> fileUrls)
        {
             Id= workDescription.Id;
             WorkorderId = workDescription.WorkorderId;
             TaskId = workDescription.TaskId;
             IsDelete= workDescription.IsDelete;
             WorkPerformedBy = workDescription.WorkPerformedBy;
             DescriptionOfWork= workDescription.WorkPerformedBy;
             WorkMaterials= workDescription.WorkMaterials;
             HourSpent= workDescription.HourSpent;
             CreatedBy = workDescription.CreatedBy;
             CreatedAt= workDescription.CreatedAt;
             ModifideBy = workDescription.ModifideBy;
             ModifiedAt= workDescription.ModifiedAt;
             
             Files = fileUrls;
        }

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

        public List<BlobFile> Files { get; set; }
        
    }

    
}

