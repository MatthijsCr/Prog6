using BeestjeOpEenFeestje.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BeestjeOpEenFeestje
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AnimalDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AnimalDbContext>()
                .AddDefaultTokenProviders();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Add roles to identity
            using (var scope = app.Services.CreateScope())
            {
                List<string> roles = ["Admin", "Employee", "Customer"];

                IServiceProvider serviceProvider = scope.ServiceProvider;
                RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                UserManager<AppUser> userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

                foreach (string role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Add first user
                string adminEmail = "admin@email.com";
                string adminPassword = "Admin123!";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    AppUser adminUser = new AppUser
                    {
                        Email = adminEmail,
                        UserName = "Admin",
                        Address = "Adress 123 Lane",
                        PhoneNumber = "0612345678",
                    };

                    IdentityResult result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }

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
                pattern: "{controller=Reservation}/{action=Date}/{id?}");

            app.Run();
        }
    }
}
