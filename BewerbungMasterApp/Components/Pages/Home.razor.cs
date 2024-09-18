using BewerbungMasterApp.Models;
using BewerbungMasterApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BewerbungMasterApp.Components.Pages
{
    public partial class HomeBase : ComponentBase
    {
        // Inject necessary services
        [Inject]
        public IJsonService JsonService { get; set; } = default!;

        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;

        [Inject]
        public ILogger<HomeBase> Logger { get; set; } = default!;

        // List to store job applications
        protected List<JobApplication> jobApplications = [];

        // Tuple to keep track of which field is being edited
        protected (int Index, string Field) ActiveEditField { get; set; } = (-1, string.Empty);

        // Initialize the component by loading the job applications
        protected override async Task OnInitializedAsync()
        {
            await RefreshList();
        }

        // Refresh the list of job applications
        protected async Task RefreshList()
        {
            jobApplications = await JsonService.GetAllAsync<JobApplication>();
            StateHasChanged();
        }

        // Set the active edit field when a user starts editing
        protected void SetActiveEditField(int index, string field)
        {
            ActiveEditField = (index, field);
            StateHasChanged();
        }

        // Clear the active edit field and refresh the list
        protected async Task ClearActiveEditField()
        {
            if (ActiveEditField != (-1, string.Empty))
            {
                ActiveEditField = (-1, string.Empty);
                await RefreshList();
            }
        }

        // Check if the position field at a specific index is being edited
        protected bool IsEditingPosition(int index)
        {
            return ActiveEditField.Index == index && ActiveEditField.Field == "position";
        }

        // Check if the company field at a specific index is being edited
        protected bool IsEditingCompany(int index)
        {
            return ActiveEditField.Index == index && ActiveEditField.Field == "company";
        }
    }
}