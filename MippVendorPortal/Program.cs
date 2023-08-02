using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MippVendorPortal.Helpers;

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
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddTransient<MippVendorTestContext>();
//builder.Services.AddTransient<MippPortalWebAPI.Models.MippTestContext>();
//builder.Services.AddTransient<MailHelper>();

builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Homes/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Homes}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

app.Run();
