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
        protected (int Index, string Field) ActiveEditField { get; set; } = (-1, string.Empty);

        protected override async Task OnInitializedAsync()
        {
            await RefreshList();
        }

        protected async Task RefreshList()
        {
            jobApplications = await JsonService.GetAllAsync<JobApplication>();
            StateHasChanged();
        }

        protected void SetActiveEditField(int index, string field)
        {
            ActiveEditField = (index, field);
            StateHasChanged();
        }

        protected async Task ClearActiveEditField()
        {
            if (ActiveEditField != (-1, string.Empty))
            {
                ActiveEditField = (-1, string.Empty);
                await RefreshList();
            }
        }

        protected bool IsEditingPosition(int index)
        {
            return ActiveEditField.Index == index && ActiveEditField.Field == "position";
        }

        protected bool IsEditingCompany(int index)
        {
            return ActiveEditField.Index == index && ActiveEditField.Field == "company";
        }
    }
}