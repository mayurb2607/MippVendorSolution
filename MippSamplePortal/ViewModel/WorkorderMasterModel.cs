namespace MippSamplePortal.ViewModel
{
    public class WorkorderMasterModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToCompany { get; set; }
        public string AssignedToAddress { get; set; }
        public string AssignedToPhone { get; set; }
        public string AssignedToEmailAddress { get; set; }
        public string ExpectedStartDate { get; set; }
        public string ExpectedEndDate { get; set; }
        public string ServiceRequestNumber { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string AdditionalComments { get; set; }
        public string ExpectedNoOfHoursToComplete { get; set; }
        public string WorkPerformedBy { get; set; }
        public string WorkCompletedAndMaterialsUsed { get; set; }
        public string TotalHoursSpent { get; set; }

        public string PropertyName { get; set; }
        public string PropertyAddress { get; set; }
        public string PropertyManager { get; set; }
        public string PropertyManagerPhone { get; set; }
        public string PropertyManagerEmail { get; set; }

        public string TenantName { get; set; }
        public string TenantEmailAddress { get; set; }
        public string TenantPhoneNumber { get; set; }
        public string UnitName { get; set; }
        public string UnitAddress { get; set; }
        public string Note { get; set; }
        public string PreferredTime { get; set; }
        public string EnterCondition { get; set; }
        public string PermissionNote { get; set; }

        public string EntryDate { get; set; }
        public string TimeEntered { get; set; }
        public string TimeDeparted { get; set; }
        public string EntryNote { get; set; }
        public string WorkorderCompiledBy { get; set; }
        public string WorkorderApprovedBy { get; set; }
        public string DateOfApproval { get; set; }
        public string Priority { get; set; }
        public string CostOfLabor { get; set; }
        public string CostOfMaterials { get; set; }
        public string TaxesPaid { get; set; }
        public string Total { get; set; }
        public int ClientId { get; set; }

        public int VendorId { get; set; }

    }
}
