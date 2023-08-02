using System;
using System.Collections.Generic;

namespace MippVendorPortal.Models;


public class Bill
{
    public int Id { get; set; }

    public int? ClientId { get; set; }

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

    public int? WorderId { get; set; }

    public DateTime? InvoiceDate { get; set; }

    public DateTime? PaymentDueOn { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal? Total { get; set; }

    public string? Note { get; set; }

    public string? Footer { get; set; }

    public string? Documents { get; set; }

    public string? VendorId { get; set; }

    public string? City { get; set; }

    public string? Zip { get; set; }

    public DateTime? BillDate { get; set; }

    public string? VendorEmail { get; set; }

    public string? ClientEmail { get; set; }
}
