using Microsoft.AspNetCore.Components;
using BewerbungMasterApp.Models;
using BewerbungMasterApp.Services;

namespace BewerbungMasterApp.Components
{
    public class HomeDeleteBase : ComponentBase //TODO: Move to code brackets into the HomeDelete.razor
    {
        [Inject]
        protected IJsonService JsonService { get; set; } = default!;

        [Parameter]
        public JobApplication Job { get; set; } = default!;

        [Parameter]
        public EventCallback OnDelete { get; set; }

        protected async Task DeleteJobApplication()
        {
            var success = await JsonService.DeleteAsync<JobApplication>(Job.Id);
            if (success)
            {
                await OnDelete.InvokeAsync();
            }
        }
    }
}