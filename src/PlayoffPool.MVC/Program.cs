using AutoMapper;
using AmerFamilyPlayoffs.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlayoffPool.MVC.Models;
using NLog.Extensions.Logging;
using NLog;
using System;
using NLog.Web;
using PlayoffPool.MVC.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

#if DEBUG
var connectionString = builder.Configuration.GetConnectionString("DatabaseContext");
#else
var connectionString = builder.Configuration.GetConnectionString("PlayoffPoolContext");
#endif
builder.Services.AddDbContext<AmerFamilyPlayoffContext>(
    o => o.UseMySql(connectionString, ServerVersion.Create(5,0,0, Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql)), ServiceLifetime.Transient);

builder.Services.AddIdentity<User, IdentityRole>(
    options =>
    {
        options.SignIn.RequireConfirmedAccount = true;

        options.Password.RequiredLength = 7;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AmerFamilyPlayoffContext>().AddRoles<IdentityRole>();

builder.Services.AddTransient<IDataManager, ApplicationDataManager>();

//var config = new MapperConfiguration(cfg => cfg.CreateMap<User, RegistrationUserViewModel>());

builder.Services.AddAutoMapper(typeof(Program));

var config = new ConfigurationBuilder()
   .SetBasePath(System.IO.Directory.GetCurrentDirectory())
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .Build();

NLog.LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));

builder.Logging.ClearProviders();
builder.Host.UseNLog();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
