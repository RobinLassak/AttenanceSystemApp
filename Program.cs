using AttenanceSystemApp;
using AttenanceSystemApp.Models;
using AttenanceSystemApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Pridani databaze
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AttenanceDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AttenanceDbConnection"));
});
//Prihlasovani - Autentizace
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AttenanceDbContext>().AddDefaultTokenProviders();
//Pridani servisek
builder.Services.AddHttpClient<PublicHolidayService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<AttenanceRecordService>();
builder.Services.AddScoped<CalendaryDayService>();
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
