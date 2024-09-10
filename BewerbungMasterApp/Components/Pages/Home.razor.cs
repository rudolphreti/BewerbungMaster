using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BewerbungMasterApp.Components.Pages
{
    public partial class HomeBase : ComponentBase
    {
        [Inject]
        public IGetJobApplicationsService GetJobApplicationService { get; set; } //

        [Inject]
        public IDeleteJobApplicationService DeleteJobApplicationService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        protected List<JobApplication> jobApplications;

        protected override async Task OnInitializedAsync()
        {
            jobApplications = await GetJobApplicationService.GetJobApplicationsAsync();
        }

        protected async Task DeleteJobApplication(Guid id) //create folder PagesPartials, move Home.Delete.razor.cs and other crud partial classes to it
        {
            var success = await DeleteJobApplicationService.DeleteJobApplicationAsync(id);
            if (success)
            {
                jobApplications.RemoveAll(job => job.Id == id);
            }
            else
            {
                // Optionally, add error handling logic here, such as showing a notification to the user
            }
        }
    }
}