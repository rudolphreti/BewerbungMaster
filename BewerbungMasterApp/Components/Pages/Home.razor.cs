using BewerbungMasterApp.Models;
using BewerbungMasterApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BewerbungMasterApp.Components.Pages
{
    public partial class HomeBase : ComponentBase
    {
        [Inject]
        public IJsonService JsonService { get; set; } = default!;

        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;

        [Inject]
        public ILogger<HomeBase> Logger { get; set; } = default!;

        protected List<JobApplication> jobApplications = [];

        protected override async Task OnInitializedAsync()
        {
            jobApplications = await JsonService.GetAllAsync<JobApplication>();
        }

        protected async Task MoveJobApplicationToEnd(Guid id)
        {
            var jobToMove = jobApplications.FirstOrDefault(j => j.Id == id);
            if (jobToMove != null)
            {
                jobApplications.Remove(jobToMove);
                jobApplications.Add(jobToMove);

                // Update the JSON file
                await JsonService.UpdateAllAsync(jobApplications);

                // Trigger a re-render of the component
                StateHasChanged();
            }
        }
    }
}