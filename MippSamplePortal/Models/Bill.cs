using System;
using System.Collections.Generic;

namespace MippSamplePortal.Models;

public partial class Bill
{
    public int Id { get; set; }

    public string? ClientId { get; set; }

    public string? Title { get; set; }

    public string? Summary { get; set; }

    public string? BillTo { get; set; }

    public string? CareOf { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? AddressLine3 { get; set; }

    public string? Province { get; set; }

    public string? Country { get; set; }

    public string? BillNumber { get; set; }

    public string? Ponumber { get; set; }

    public string? Wonumber { get; set; }

    public string? InvoiceDate { get; set; }

    public string? PaymentDueOn { get; set; }

    public string? BillItemId { get; set; }

    public string? SubTotal { get; set; }

    public string? TaxAmount { get; set; }

    public string? Total { get; set; }

    public string? Note { get; set; }

    public string? Footer { get; set; }

    public string? Documents { get; set; }

    public string? VendorId { get; set; }

    public string? City { get; set; }

    public string? Zip { get; set; }

    public string? BillDate { get; set; }

    public string? VendorEmail { get; set; }

    public string? ClientEmail { get; set; }
}
