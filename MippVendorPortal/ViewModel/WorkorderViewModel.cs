

using MippVendorPortal.Models;

namespace MippVendorPortal.ViewModel
{
    public class WorkorderViewModel
    {
        public int Id { get; set; }

        public string? OrderNumber { get; set; }

        public string? OrderDate { get; set; }

        public string? AssignedTo { get; set; }

        public string? AssignedToCompany { get; set; }

        public string? AssignedToAddress { get; set; }

        public string? AssignedToPhone { get; set; }

        public string? AssignedToEmailAddress { get; set; }

        public string? ExpectedStartDate { get; set; }

        public string? ExpectedEndDate { get; set; }

        public string? ServiceRequestNumber { get; set; }

        public string? Status { get; set; }

        public string? Description { get; set; }

        public string? AdditionalComments { get; set; }

        public string? ExpectedNoOfHoursToComplete { get; set; }

        public string? WorkPerformedBy { get; set; }

        public string? WorkCompletedAndMaterialsUsed { get; set; }

        public string? TotalHoursSpent { get; set; }

        public string? PropertyName { get; set; }

        public string? PropertyAddress { get; set; }

        public string? PropertyManager { get; set; }

        public string? PropertyManagerPhone { get; set; }

        public string? PropertyManagerEmail { get; set; }

        public string? TenantName { get; set; }

        public string? TenantEmailAddress { get; set; }

        public string? TenantPhoneNumber { get; set; }

        public string? UnitName { get; set; }

        public string? UnitAddress { get; set; }

        public string? Note { get; set; }

        public string? PreferredTime { get; set; }

        public string? EnterCondition { get; set; }

        public string? PermissionNote { get; set; }

        public string? EntryDate { get; set; }

        public string? TimeEntered { get; set; }

        public string? TimeDeparted { get; set; }

        public string? EntryNote { get; set; }

        public string? WorkorderCompiledBy { get; set; }

        public string? WorkorderApprovedBy { get; set; }

        public string? DateOfApproval { get; set; }

        public string? Priority { get; set; }

        public string? CostOfLabor { get; set; }

        public string? CostOfMaterials { get; set; }

        public string? TaxesPaid { get; set; }

        public string? Total { get; set; }

        public int? ClientId { get; set; }

        public int? VendorId { get; set; }

        public List<WorkorderTask> WorkorderTasks { get; set; }
        public List<WorkorderWorkDescription> WorkDescriptions { get; set; }


        public WorkorderViewModel(Workorder workorder)
        {
            this.Id=workorder.Id;
            this.OrderNumber = workorder.OrderNumber;
            this.OrderDate = workorder.OrderDate;
            this.AssignedTo = workorder.AssignedTo;

           this.AssignedToCompany = workorder.AssignedToCompany;

           this.AssignedToAddress = workorder.AssignedToAddress;

           this.AssignedToPhone = workorder.AssignedToPhone;

           this.AssignedToEmailAddress = workorder.AssignedToEmailAddress;

           this.ExpectedStartDate = workorder.ExpectedStartDate;

           this.ExpectedEndDate = workorder.ExpectedEndDate;

           this.ServiceRequestNumber = workorder.ServiceRequestNumber;

                this.Status = workorder.Status;

           this.Description =workorder.Description;

           this.AdditionalComments =workorder.AdditionalComments;

           this.ExpectedNoOfHoursToComplete =workorder.ExpectedNoOfHoursToComplete;

           this.WorkPerformedBy =workorder.WorkPerformedBy;

           this.WorkCompletedAndMaterialsUsed =workorder.WorkCompletedAndMaterialsUsed;

           this.TotalHoursSpent =workorder.TotalHoursSpent;

           this.PropertyName =workorder.PropertyName;

           this.PropertyAddress =workorder.PropertyAddress;

           this.PropertyManager =workorder.PropertyManager;

           this.PropertyManagerPhone =workorder.PropertyManagerPhone;

           this.PropertyManagerEmail =workorder.PropertyManagerEmail;

           this.TenantName =workorder.TenantName;

           this.TenantEmailAddress =workorder.TenantEmailAddress;

           this.TenantPhoneNumber =workorder.TenantPhoneNumber;

                this.UnitName = workorder.UnitName;

           this.UnitAddress =workorder.UnitAddress;

           this.Note =workorder.Note;

           this.PreferredTime =workorder.PreferredTime;

           this.EnterCondition =workorder.EnterCondition;

                this.PermissionNote = workorder.PermissionNote;

           this.EntryDate =workorder.EntryDate;

           this.TimeEntered =workorder.TimeEntered;

           this.TimeDeparted =workorder.TimeDeparted;
            
           this.EntryNote = workorder.EntryNote;

           this.WorkorderCompiledBy = workorder.WorkorderCompiledBy;

           this.WorkorderApprovedBy = workorder.WorkorderApprovedBy;

           this.DateOfApproval = workorder.DateOfApproval;

           this.Priority = workorder.Priority;

           this.CostOfLabor = workorder.CostOfLabor;

           this.CostOfMaterials = workorder.CostOfMaterials;

           this.TaxesPaid = workorder.TaxesPaid;

            this.Total = workorder.Total;

            this.ClientId = workorder.ClientId;

            this.VendorId =workorder.VendorId;

            this.WorkDescriptions = new List<WorkorderWorkDescription>();

        }

    }
    
}
