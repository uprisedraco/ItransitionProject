using System;
using System.Collections.Generic;
using System.Globalization;
using ItransitionProject;
using ItransitionProject.Models;
using ItransitionProject.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton<LocalizationService>();
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(6000);
    options.Lockout.MaxFailedAccessAttempts = 1;

    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false; ;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;

    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;

}).AddEntityFrameworkStores<ApplicationContext>();

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

builder.Services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
        .AddDataAnnotationsLocalization(options =>
        {
            options.DataAnnotationLocalizerProvider = (type, factory) =>
            {
                var assemblyName = new AssemblyName(typeof(ApplicationResource).GetTypeInfo().Assembly.FullName);
                return factory.Create("ApplicationResource", assemblyName.Name);
            };
        });

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
            new CultureInfo("en"),
            new CultureInfo("ru")
        };

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

await RoleInitializer.InitializeAsync(app.Services.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>());

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

builder.Services.AddControllersWithViews();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

var requestlocalizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(requestlocalizationOptions.Value);

app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
