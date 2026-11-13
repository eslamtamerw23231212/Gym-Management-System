using Gym_Management_System.DbContexts;
using Gym_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GymManangement.DAL.Data.DataSeeding
{
    public class GymDataSeeding
    {
        public static async Task SeedAsync(
            GymDbContext dbContext,
            string seedFilesPath,
            CancellationToken cancellationToken = default)
        {
            try
            {
                Console.WriteLine("========== Gym Data Seeding Started ==========");

                var existingPlansCount = await dbContext.Plans
                    .CountAsync(cancellationToken);

                Console.WriteLine($"Existing Plans Count: {existingPlansCount}");

                if (existingPlansCount > 0)
                {
                    Console.WriteLine("Plans already exist. Seeding skipped.");
                    return;
                }

                var filePath = Path.Combine(seedFilesPath, "plans.json");

                Console.WriteLine($"JSON File Path: {filePath}");

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("ERROR: plans.json file was not found.");
                    throw new FileNotFoundException(
                        "Seeding file not found.",
                        filePath);
                }

                var jsonData = await File.ReadAllTextAsync(
                    filePath,
                    cancellationToken);

                Console.WriteLine("JSON file loaded successfully.");

                var plans = JsonSerializer.Deserialize<List<Plan>>(
                    jsonData,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (plans == null || plans.Count == 0)
                {
                    Console.WriteLine("ERROR: No plans found in JSON file.");

                    throw new InvalidOperationException(
                        "No plans found in JSON file.");
                }

                Console.WriteLine($"Plans Read From JSON: {plans.Count}");

                await dbContext.Plans.AddRangeAsync(
                    plans,
                    cancellationToken);

                var savedChanges = await dbContext.SaveChangesAsync(
                    cancellationToken);

                Console.WriteLine($"Saved Changes: {savedChanges}");

                var totalPlansCount = await dbContext.Plans
                    .CountAsync(cancellationToken);

                Console.WriteLine($"Total Plans After Seeding: {totalPlansCount}");
                Console.WriteLine("========== Gym Data Seeding Completed ==========");
            }
            catch (Exception ex)
            {
                Console.WriteLine("========== Gym Data Seeding Failed ==========");
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}