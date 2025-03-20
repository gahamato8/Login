using Microsoft.AspNetCore.Identity;
using Web_KhachSanResort.Data;
using Web_KhachSanResort.Models;

namespace Web_KhachSanResort.Services
{
    public class SeedService
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();
            try
            {
                //Đảm bảo cơ sở dữ liệu được tạo
                logger.LogInformation("Đảm bảo cơ sở dữ liệu được tạo.");
                await context.Database.EnsureCreatedAsync();

                //Tạo role Admin
                logger.LogInformation("Tạo role Admin");
                await AddRoleAsync(roleManager, "Admin");
                await AddRoleAsync(roleManager, "User");

                //Tạo admin User
                logger.LogInformation("Tạo admin User");
                var adminEmail = "admin@gmail.hub.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var admin = new Users
                    {
                        FullName = "Admin",
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        // PhoneNumberConfirmed = true,
                        NormalizedUserName = adminEmail.ToUpper(),
                        NormalizedEmail = adminEmail.ToUpper(),
                        SecurityStamp = Guid.NewGuid().ToString(),
                        // ConcurrencyStamp = Guid.NewGuid().ToString(),
                        // TwoFactorEnabled = false,
                        // LockoutEnabled = false,
                        // AccessFailedCount = 0,
                        
                    };
                    var result = await userManager.CreateAsync(admin, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                        logger.LogInformation("Tạo admin User thành công.");
                    }
                    else
                    {
                        logger.LogError("Tạo admin User thất bại: {0}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi tạo dữ liệu mẫu.");
            }
        }

        private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    throw new Exception($"Lỗi tạo role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
                
        }
    }
}
