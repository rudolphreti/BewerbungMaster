using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BewerbungMasterApp.Components.Pages
{
    public partial class HomeBase : ComponentBase
    {
        [Inject]
        public IGetJobApplicationsService GetJobApplicationService { get; set; } = default!;

        [Inject]
        public IDeleteJobApplicationService DeleteJobApplicationService { get; set; } = default!;

        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        public ILogger<HomeBase> Logger { get; set; } = default!;

        protected List<JobApplication> jobApplications = [];

        protected override async Task OnInitializedAsync() //why override? 
        {
            jobApplications = await GetJobApplicationService.GetJobApplicationsAsync();
        }
    }
}