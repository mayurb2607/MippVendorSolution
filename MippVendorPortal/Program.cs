using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MippPortalWebAPI.Models;
using MippVendorPortal.Areas.Identity.Data;
using MippVendorPortal.Data;
using MippVendorPortal.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MippVendorPortalContextConnection") ?? throw new InvalidOperationException("Connection string 'MippVendorPortalContextConnection' not found.");

builder.Services.AddDbContext<MippVendorPortalContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<MippVendorPortalUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MippVendorPortalContext>();

//builder.Services.AddIdentity<MippVendorPortalUser, IdentityUser>().AddDefaultUI()
//    .AddEntityFrameworkStores<MippVendorPortalContext>().AddDefaultTokenProviders();
// Add services to the container.

//builder.Services.AddIdentity<UserManager<IdentityUser>, IdentityRole>();

builder.Services.AddTransient<MippVendorTestContext>();
builder.Services.AddTransient<MippTestContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Homes}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

app.Run();
