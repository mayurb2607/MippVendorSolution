using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MippSamplePortal.Areas.Identity.Data;
using MippSamplePortal.Data;
using MippSamplePortal.Models;
using MippSamplePortal.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MippSamplePortalContextConnection") ?? throw new InvalidOperationException("Connection string 'MippSamplePortalContextConnection' not found.");

builder.Services.AddDbContext<MippSamplePortalContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<MippSamplePortalUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MippSamplePortalContext>();


builder.Services.AddTransient<EmailService>();
builder.Services.AddScoped<MippTestContext>();
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin",
options => options
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);
});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseCors("AllowOrigin");

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
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

app.Run();
