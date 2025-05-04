using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.EntityFrameworkCore; 
using NotinLite.Data;
using Microsoft.AspNetCore.DataProtection;





var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });
var dataProtectionBuilder = builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(@"C:\cryptokeyDoNotDelete")).UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
        {
            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
        });
dataProtectionBuilder.SetApplicationName("NotinLiteApp");
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization();
var app = builder.Build();

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

app.Run();
