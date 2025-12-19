using EgitimPortali.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EgitimPortali.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<EgitimPortali.Data.ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true; 
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false; 
    options.Password.RequiredLength = 6; 
});

builder.Services.AddSession();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthentication(); 
app.UseAuthorization();  

app.MapStaticAssets();
app.MapHub<EgitimPortali.Hubs.SatisHub>("/satisHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); 

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Ogrenci", "Egitmen" }; 
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}
app.Run();