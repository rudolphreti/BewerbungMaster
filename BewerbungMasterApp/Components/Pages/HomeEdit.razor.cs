using BewerbungMasterApp.Models;
using BewerbungMasterApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace BewerbungMasterApp.Components
{
    public class HomeEditBase : ComponentBase //TODO: refactor -> to many responsibilities
    {
        [Inject] protected IJsonService JsonService { get; set; } = default!;
        [Inject] protected ILogger<HomeEditBase> Logger { get; set; } = default!;


        [Parameter] public JobApplication Job { get; set; } = default!;
        [Parameter] public EventCallback OnUpdate { get; set; } = default!;
        [Parameter] public bool IsEditingPosition { get; set; }
        [Parameter] public bool IsEditingCompany { get; set; }
        [Parameter] public EventCallback<string> OnStartEditing { get; set; }
        [Parameter] public EventCallback OnStopEditing { get; set; } = default!;

        [Parameter] public List<JobAppContent> JobAppContents { get; set; } = new();

        private string _editedType = string.Empty;
        protected string EditedType
        {
            get => _editedType;
            set
            {
                if (_editedType != value)
                {
                    _editedType = value;
                    UpdateTypeAsync().ConfigureAwait(false);
                }
            }
        }

        protected string EditedPosition { get; set; } = string.Empty;
        protected string EditedCompany { get; set; } = string.Empty;

        protected ElementReference positionInput;
        protected ElementReference companyInput;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            EditedPosition = Job.Position;
            EditedCompany = Job.Company;
            _editedType = Job.Type;

            Logger.LogInformation($"JobAppContents count: {JobAppContents.Count}");
            foreach (var content in JobAppContents)
            {
                Logger.LogInformation($"JobAppContent: {content.Name}");
            }
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

        private async Task UpdateTypeAsync()
        {
            if (Job.Type != _editedType)
            {
                Job.Type = _editedType;
                await UpdateJob();
            }
        }

        private async Task UpdateJob()
        {
            await JsonService.UpdateAsync(Job);
            await OnUpdate.InvokeAsync();
        }

        protected async Task HandleKeyDown(KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case "Enter":
                    if (IsEditingPosition)
                        await UpdatePosition();
                    else if (IsEditingCompany)
                        await UpdateCompany();
                    break;
                case "Escape":
                    await OnStopEditing.InvokeAsync();
                    break;
            }
        }

        protected async Task StartEditingPosition() => await OnStartEditing.InvokeAsync("position");
        protected async Task StartEditingCompany() => await OnStartEditing.InvokeAsync("company");
    }
}