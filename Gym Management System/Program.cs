using Gym_Management_System.DbContexts;
using Gym_Management_System.Migrations;
using GymManagement.BLL;
using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.Interfaces.Attachement;
using GymManangement.DAL.Data.DataSeeding;
using GymManangement.DAL.Repositories.Interfaces;
using GymManangement.DAL.Repositories.Interfaces.Classes;
using Microsoft.EntityFrameworkCore;

namespace Gym_Management_System
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IMemberService, MemberSerivce>();
            builder.Services.AddScoped<ISessionService, SessionService>();

            builder.Services.AddScoped<IUnitofWork, UnitofWork>();

            builder.Services.AddScoped<SessionRepository>();
            builder.Services.AddScoped<ISessionRepository, SessionRepository>();
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<ITrainerService, TrainerService>();
            builder.Services.AddScoped<IAttachementService, AttachemntService>();

            builder.Services.AddScoped(
                typeof(IGenericRepository<>),
                typeof(GenericRepository<>));

            builder.Services.AddDbContext<GymDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddAutoMapper(m => m.AddProfile(new MappingProfile()));

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider
                    .GetRequiredService<GymDbContext>();

                var seedFilesPath = Path.Combine(
                    app.Environment.ContentRootPath,
                    "wwwroot",
                    "Files");

                var filePath = Path.Combine(seedFilesPath, "plans.json");

                Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
                Console.WriteLine($"ContentRoot: {app.Environment.ContentRootPath}");
                Console.WriteLine($"File path: {filePath}");
                Console.WriteLine($"File exists: {File.Exists(filePath)}");
                Console.WriteLine($"Database: {dbContext.Database.GetDbConnection().DataSource}");
                Console.WriteLine($"Database name: {dbContext.Database.GetDbConnection().Database}");

                await GymDataSeeding.SeedAsync(dbContext, seedFilesPath);

                Console.WriteLine($"Plans count: {await dbContext.Plans.CountAsync()}");
            }



            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}