namespace MippVendorPortal.ViewModel
{
    public class WorkorderViewModel
    {
        public string AssignedTo {get;set;}
        public string AssignedToCompany { get; set; }
        public string AssignedToAddress { get; set; }
        public string AssignedToPhone  { get; set;}
        public string AssignedToEmailAddress { get; set; }
        public string Status { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string ExpectedStartDate { get; set; }
        public string ExpectedEndDate { get; set; }
        public string ServiceRequestNumber { get; set; }
        public string TenantName { get; set; }
        public string TenantEmailAddress { get; set; }
        public string TenantPhoneNumber { get; set; }
        public string UnitName { get; set; }
        public string UnitAddress { get; set; }
        public string Note { get; set; }
        public string PreferredTime { get; set; }
        public string EnterCondition { get; set; }
        public string PermissionNote { get; set; }
        public string AdditionalComments { get; set; }
        public string WorkPerformedBy { get; set; }
        public string ExpectedNoOfHoursToComplete { get; set; }
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string VendorId { get; set; }
        public string Description { get; set; }
    }
}
