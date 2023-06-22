using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MippPortalWebAPI.Helpers;
using MippPortalWebAPI.Controllers;
using MippPortalWebAPI.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MippPortalWebAPIContextConnection") ?? throw new InvalidOperationException("Connection string 'MippPortalWebAPIContextConnection' not found.");

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// Add services to the container.
builder.Services.AddTransient<MippTestContext>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<MailHelper>();
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin",
options => options
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);
});

var app = builder.Build();
app.UseCors("AllowOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapControllers();


app.Run();
