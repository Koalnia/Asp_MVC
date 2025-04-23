using Asp_MVC.Data;
using Asp_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Asp_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

            builder.Services.AddIdentity<User, IdentityRole<long>>(options =>
            {
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddRoleManager<RoleManager<IdentityRole<long>>>()
            .AddDefaultTokenProviders();

            builder.Services.AddControllersWithViews();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = false;
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Home/Error";
                options.ExpireTimeSpan = TimeSpan.FromDays(14);
                options.SlidingExpiration = true;
            });

            builder.Services.AddScoped<IRoleStore<IdentityRole<long>>, RoleStore<IdentityRole<long>, AppDbContext, long>>();
            builder.Services.AddScoped<IUserStore<User>, UserStore<User, IdentityRole<long>, AppDbContext, long>>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            });

          
            builder.Services.AddAntiforgery(options =>
             {
                 options.Cookie.Name = "AntiForgery";
                 options.HeaderName = "X-CSRF-TOKEN";
                 options.SuppressXFrameOptionsHeader = false;
             });

            builder.Services.AddHostedService<DataSeeder>();

            var app = builder.Build();

            
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureDeleted(); 
                dbContext.Database.EnsureCreated(); 
            }
            

            app.UseExceptionHandler("/Home/Error");
            app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
            app.UseHsts();
            

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();   
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
