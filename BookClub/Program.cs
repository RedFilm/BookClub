using BookClub.Data;
using BookClub.Data.FileManager;
using BookClub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using static System.Formats.Asn1.AsnWriter;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.

builder.Services.AddControllersWithViews(opt => opt.EnableEndpointRouting = false);
builder.Services.AddTransient<IFileManager, FileManager>();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connection));

builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
     opt.Password.RequireNonAlphanumeric = false;
     opt.Password.RequiredUniqueChars = 0;
     opt.Password.RequiredLength = 3;
     opt.Password.RequireDigit = false;
     opt.Password.RequireLowercase = false;
     opt.Password.RequireUppercase = false;
     opt.User.AllowedUserNameCharacters = null;
     opt.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = "/Auth/Login";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAuthentication();
app.UseMvcWithDefaultRoute();
app.UseStaticFiles();
app.UseRouting();

try
{
    var scope = app.Services.CreateScope();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();


    var adminRole = new IdentityRole("Administrator");
    if (!context.Roles.Any())
    {
        roleMgr.CreateAsync(adminRole).GetAwaiter().GetResult();
    }

    if (!context.Users.Any(u => u.UserName == "Admin"))
    {
        var adminUser = new AppUser
        {
            UserName = "Admin",
            Email = "Admin@test.com"
        };
        var result = userMgr.CreateAsync(adminUser, "password").GetAwaiter().GetResult();
  
        userMgr.AddToRoleAsync(adminUser, adminRole.Name).GetAwaiter().GetResult();
    }
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

app.Run();