using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MippPortalWebAPI.Models;

public partial class MippTestContext : DbContext
{
    public MippTestContext()
    {
    }

    public MippTestContext(DbContextOptions<MippTestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<BillItem> BillItems { get; set; }

    public virtual DbSet<BillItemTax> BillItemTaxes { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientStatus> ClientStatuses { get; set; }

    public virtual DbSet<Tax> Taxes { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    public virtual DbSet<VendorInvite> VendorInvites { get; set; }

    public virtual DbSet<VendorList> VendorLists { get; set; }

    public virtual DbSet<Workorder> Workorders { get; set; }

    public virtual DbSet<WorkorderComment> WorkorderComments { get; set; }

    public virtual DbSet<WorkorderWorkDescription> WorkorderWorkDescriptions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=L028;Database=MippTest;Trusted_Connection=True; User Id=ANAR\\Mayur.b;Password=Speak2m; MultipleActiveResultSets=true;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bill__3214EC074C3FBED5");

            entity.ToTable("Bill");

            entity.Property(e => e.AddressLine1).HasMaxLength(50);
            entity.Property(e => e.AddressLine2).HasMaxLength(50);
            entity.Property(e => e.AddressLine3).HasMaxLength(50);
            entity.Property(e => e.BillDate).HasMaxLength(50);
            entity.Property(e => e.BillItemId)
                .HasMaxLength(50)
                .HasColumnName("BillItemID");
            entity.Property(e => e.BillNumber).HasMaxLength(50);
            entity.Property(e => e.BillTo).HasMaxLength(50);
            entity.Property(e => e.CareOf).HasMaxLength(50);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.ClientEmail).HasMaxLength(50);
            entity.Property(e => e.ClientId)
                .HasMaxLength(50)
                .HasColumnName("ClientID");
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.InvoiceDate).HasMaxLength(50);
            entity.Property(e => e.PaymentDueOn).HasMaxLength(50);
            entity.Property(e => e.Ponumber)
                .HasMaxLength(50)
                .HasColumnName("PONumber");
            entity.Property(e => e.Province).HasMaxLength(50);
            entity.Property(e => e.SubTotal).HasMaxLength(50);
            entity.Property(e => e.Summary).HasMaxLength(50);
            entity.Property(e => e.TaxAmount).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.Total).HasMaxLength(50);
            entity.Property(e => e.VendorEmail).HasMaxLength(50);
            entity.Property(e => e.VendorId).HasMaxLength(50);
            entity.Property(e => e.Wonumber)
                .HasMaxLength(50)
                .HasColumnName("WONumber");
            entity.Property(e => e.Zip).HasMaxLength(50);
        });

        modelBuilder.Entity<BillItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BillItem__3214EC07E71DE363");

            entity.ToTable("BillItem");

            entity.Property(e => e.BillId)
                .HasMaxLength(50)
                .HasColumnName("BillID");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Price).HasMaxLength(50);
            entity.Property(e => e.Quantity).HasMaxLength(50);
            entity.Property(e => e.Subtotal).HasMaxLength(50);
            entity.Property(e => e.Tax).HasMaxLength(50);
            entity.Property(e => e.Total).HasMaxLength(50);
            entity.Property(e => e.Unit).HasMaxLength(50);
        });

        modelBuilder.Entity<BillItemTax>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BillItem__3214EC072D7A298D");

            entity.ToTable("BillItemTax");

            entity.Property(e => e.BillItemId).HasColumnName("BillItemID");
            entity.Property(e => e.TaxId).HasColumnName("TaxID");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Client__3214EC07B3B3AC29");

            entity.ToTable("Client");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.ClientName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ClientStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClientSt__3214EC0739DC557B");

            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<Tax>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tax__3214EC07B4DBDBA8");

            entity.ToTable("Tax");

            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.TaxRate).HasMaxLength(50);
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vendor__3214EC07700DBF16");

            entity.ToTable("Vendor");

            entity.Property(e => e.BusinessName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VendorInvite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VendorIn__3214EC07EF74998D");

            entity.ToTable("VendorInvite");

            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.InviteSentDate)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.JoinedDate)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VendorEmail)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VendorId).HasColumnName("VendorID");
        });

        modelBuilder.Entity<VendorList>(entity =>
        {
            entity.ToTable("VendorList");

            entity.Property(e => e.BusinessName).HasMaxLength(50);
            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.VendorEmail).HasMaxLength(50);
            entity.Property(e => e.VendorName).HasMaxLength(50);
            entity.Property(e => e.VendorPhone).HasMaxLength(50);
        });

        modelBuilder.Entity<Workorder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Workorde__3214EC07619A69F2");

            entity.ToTable("Workorder");

            entity.Property(e => e.AssignedTo).HasMaxLength(50);
            entity.Property(e => e.AssignedToAddress).HasMaxLength(50);
            entity.Property(e => e.AssignedToCompany).HasMaxLength(50);
            entity.Property(e => e.AssignedToEmailAddress).HasMaxLength(50);
            entity.Property(e => e.AssignedToPhone).HasMaxLength(50);
            entity.Property(e => e.CostOfLabor).HasMaxLength(50);
            entity.Property(e => e.CostOfMaterials).HasMaxLength(50);
            entity.Property(e => e.DateOfApproval).HasMaxLength(50);
            entity.Property(e => e.EnterCondition).HasMaxLength(50);
            entity.Property(e => e.EntryDate).HasMaxLength(50);
            entity.Property(e => e.EntryNote).HasMaxLength(50);
            entity.Property(e => e.ExpectedEndDate).HasMaxLength(50);
            entity.Property(e => e.ExpectedNoOfHoursToComplete).HasMaxLength(50);
            entity.Property(e => e.ExpectedStartDate).HasMaxLength(50);
            entity.Property(e => e.OrderDate).HasMaxLength(50);
            entity.Property(e => e.OrderNumber).HasMaxLength(50);
            entity.Property(e => e.PermissionNote).HasMaxLength(50);
            entity.Property(e => e.PreferredTime).HasMaxLength(50);
            entity.Property(e => e.Priority).HasMaxLength(50);
            entity.Property(e => e.PropertyAddress).HasMaxLength(50);
            entity.Property(e => e.PropertyManager).HasMaxLength(50);
            entity.Property(e => e.PropertyManagerEmail).HasMaxLength(50);
            entity.Property(e => e.PropertyManagerPhone).HasMaxLength(50);
            entity.Property(e => e.PropertyName).HasMaxLength(50);
            entity.Property(e => e.ServiceRequestNumber).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TaxesPaid).HasMaxLength(50);
            entity.Property(e => e.TenantEmailAddress).HasMaxLength(50);
            entity.Property(e => e.TenantName).HasMaxLength(50);
            entity.Property(e => e.TenantPhoneNumber).HasMaxLength(50);
            entity.Property(e => e.TimeDeparted).HasMaxLength(50);
            entity.Property(e => e.TimeEntered).HasMaxLength(50);
            entity.Property(e => e.Total).HasMaxLength(50);
            entity.Property(e => e.TotalHoursSpent).HasMaxLength(50);
            entity.Property(e => e.UnitAddress).HasMaxLength(50);
            entity.Property(e => e.UnitName).HasMaxLength(50);
            entity.Property(e => e.WorkCompletedAndMaterialsUsed).HasMaxLength(50);
            entity.Property(e => e.WorkPerformedBy).HasMaxLength(50);
            entity.Property(e => e.WorkorderApprovedBy).HasMaxLength(50);
            entity.Property(e => e.WorkorderCompiledBy).HasMaxLength(50);
        });

        modelBuilder.Entity<WorkorderComment>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.WorkorderId).HasColumnName("WorkorderID");
        });

        modelBuilder.Entity<WorkorderWorkDescription>(entity =>
        {
            entity.ToTable("WorkorderWorkDescription");

            entity.Property(e => e.DescriptionOfWorkCompletedMaterialsUsed).HasColumnName("DescriptionOfWorkCompleted&MaterialsUsed");
            entity.Property(e => e.HoursSpent).HasMaxLength(50);
            entity.Property(e => e.WorkorderId).HasColumnName("WorkorderID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
