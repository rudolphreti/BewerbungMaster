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
    }
}