using BewerbungMasterApp.Models;
using BewerbungMasterApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BewerbungMasterApp.Components
{
    public class HomeEditBase : ComponentBase
    {
        [Inject] protected IJsonService JsonService { get; set; }

        [Parameter] public JobApplication Job { get; set; }
        [Parameter] public EventCallback OnUpdate { get; set; }
        [Parameter] public bool IsEditingPosition { get; set; }
        [Parameter] public bool IsEditingCompany { get; set; }
        [Parameter] public EventCallback<string> OnStartEditing { get; set; }
        [Parameter] public EventCallback OnStopEditing { get; set; }

        protected string EditedPosition { get; set; }
        protected string EditedCompany { get; set; }

        protected ElementReference positionInput;
        protected ElementReference companyInput;

        protected override void OnParametersSet()
        {
            EditedPosition = Job.Position;
            EditedCompany = Job.Company;
        }

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

        protected async Task UpdatePosition()
        {
            if (Job.Position != EditedPosition)
            {
                Job.Position = EditedPosition;
                await UpdateJob();
            }
            await OnStopEditing.InvokeAsync();
        }

        protected async Task UpdateCompany()
        {
            if (Job.Company != EditedCompany)
            {
                Job.Company = EditedCompany;
                await UpdateJob();
            }
            await OnStopEditing.InvokeAsync();
        }

        private async Task UpdateJob()
        {
            await JsonService.UpdateAsync(Job);
            await OnUpdate.InvokeAsync();
        }

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

        protected async Task StartEditingPosition() => await OnStartEditing.InvokeAsync("position");
        protected async Task StartEditingCompany() => await OnStartEditing.InvokeAsync("company");
    }
}