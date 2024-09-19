using BewerbungMasterApp.Models;
using BewerbungMasterApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BewerbungMasterApp.Components
{
    public class HomeEditBase : ComponentBase
    {
        // Inject the JSON service for data operations
        [Inject] protected IJsonService JsonService { get; set; } = default!;

        // Parameters for component configuration and data binding
        [Parameter] public JobApplication Job { get; set; } = default!;
        [Parameter] public EventCallback OnUpdate { get; set; } = default!;
        [Parameter] public bool IsEditingPosition { get; set; }
        [Parameter] public bool IsEditingCompany { get; set; }
        [Parameter] public EventCallback<string> OnStartEditing { get; set; }
        [Parameter] public EventCallback OnStopEditing { get; set; } = default!;

        // Properties to hold edited values
        protected string EditedPosition { get; set; } = string.Empty;
        protected string EditedCompany { get; set; } = string.Empty; 

        // References to input elements for focus management
        protected ElementReference positionInput;
        protected ElementReference companyInput;

        // Update local edit fields when parameters change
        protected override void OnParametersSet()
        {
            EditedPosition = Job.Position;
            EditedCompany = Job.Company;
        }

        // Set focus to the active input field after rendering
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (IsEditingPosition)
            {
                await positionInput.FocusAsync();
            }
            else if (IsEditingCompany)
            {
                await companyInput.FocusAsync();
            }
        }

        // Update the position and save changes
        protected async Task UpdatePosition()
        {
            if (Job.Position != EditedPosition)
            {
                Job.Position = EditedPosition;
                await UpdateJob();
            }
            await OnStopEditing.InvokeAsync();
        }

        // Update the company and save changes
        protected async Task UpdateCompany()
        {
            if (Job.Company != EditedCompany)
            {
                Job.Company = EditedCompany;
                await UpdateJob();
            }
            await OnStopEditing.InvokeAsync();
        }

        // Save the updated job to the JSON service
        private async Task UpdateJob()
        {
            await JsonService.UpdateAsync(Job);
            await OnUpdate.InvokeAsync();
        }

        // Handle keyboard events (Enter to save, Escape to cancel)
        protected async Task HandleKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                if (IsEditingPosition)
                    await UpdatePosition();
                else if (IsEditingCompany)
                    await UpdateCompany();
            }
            else if (e.Key == "Escape")
            {
                await OnStopEditing.InvokeAsync();
            }
        }

        // Start editing the position field
        protected async Task StartEditingPosition() => await OnStartEditing.InvokeAsync("position");

        // Start editing the company field
        protected async Task StartEditingCompany() => await OnStartEditing.InvokeAsync("company");
    }
}