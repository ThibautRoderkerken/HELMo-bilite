using HELMo_bilite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HELMo_bilite.Data;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Register", "Account/Register");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "Account/Login");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Logout", "Account/Logout");
        options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/Index", "Account/Manage/Index");
    });

builder.Services.AddDbContext<HELMoBiliteDbContext>(options =>
    options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=HELMoBilite;Trusted_Connection=True;"));

builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<HELMoBiliteDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "registerClient",
    pattern: "Clients/Register",
    defaults: new { controller = "Clients", action = "Register" });

app.MapControllerRoute(
    name: "registerDriver",
    pattern: "Account/RegisterDriver",
    defaults: new { controller = "Account", action = "RegisterDriver" });

app.MapControllerRoute(
    name: "registerDispatcher",
    pattern: "Account/RegisterDispatcher",
    defaults: new { controller = "Account", action = "RegisterDispatcher" });

app.MapRazorPages();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<HELMoBiliteDbContext>();
    await DataInitializer.Initialize(userManager, roleManager, dbContext);
}



app.Run();
