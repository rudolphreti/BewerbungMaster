using BewerbungMasterApp.Models;
using BewerbungMasterApp.Services;
using Microsoft.AspNetCore.Components;

namespace BewerbungMasterApp.Components.Pages
{
    public partial class Home //TODO: Move to code brackets into the Home.razor; files don't must be named with 'Home' prefix, but they can be ordered into the folders
    {
        [Inject]
        private IJsonService JsonService { get; set; } = default!;

        [Inject]
        private JobEditService JobEditService { get; set; } = default!;

        private List<JobApplication> jobApplications = [];
        private List<JobAppContent> jobAppContents = [];

        protected override async Task OnInitializedAsync()
        {
            await RefreshList();
            await LoadJobAppContents();
            Console.WriteLine($"Home component initialized with {jobApplications.Count} applications");
        }

        private async Task RefreshList()
        {
            jobApplications = await JsonService.GetAllAsync<JobApplication>();
            Console.WriteLine($"RefreshList: Loaded {jobApplications.Count} applications");
            StateHasChanged();
        }

        private async Task LoadJobAppContents()
        {
            jobAppContents = await JsonService.GetAllJobAppContentsAsync();
        }
    }
}