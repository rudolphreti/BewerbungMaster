using System.Text.Json;
using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BewerbungMasterApp.Services
{
    public class GetJobApplicationsService(IWebHostEnvironment environment) : IGetJobApplicationsService
    {
        private readonly IWebHostEnvironment _environment = environment;

        public async Task<List<JobApplication>> GetJobApplicationsAsync()
        {
            try
            {
                // Use the environment to get the content root path (wwwroot)
                var jsonFilePath = Path.Combine(_environment.WebRootPath, "data.json");

                if (!File.Exists(jsonFilePath))
                {
                    Console.WriteLine($"File not found: {jsonFilePath}");
                    return [];
                }

                var jsonData = await File.ReadAllTextAsync(jsonFilePath);
                return JsonSerializer.Deserialize<List<JobApplication>>(jsonData);
            }
            catch (Exception ex)
            {
                // Handle other exceptions (e.g., JSON parsing errors)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return []; // Return an empty list on error
            }
        }
    }
}
