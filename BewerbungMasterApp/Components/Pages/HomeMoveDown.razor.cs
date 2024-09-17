using Microsoft.AspNetCore.Components;
using BewerbungMasterApp.Models;
using BewerbungMasterApp.Services;

namespace BewerbungMasterApp.Components
{
    public class HomeMoveDownBase : ComponentBase
    {
        [Inject]
        protected IJsonService JsonService { get; set; } = default!;

        [Parameter]
        public JobApplication Job { get; set; } = default!;

        [Parameter]
        public EventCallback OnMove { get; set; }

        protected async Task MoveJobApplicationToEnd()
        {
            var jobs = await JsonService.GetAllAsync<JobApplication>();
            var jobToMove = jobs.FirstOrDefault(j => j.Id == Job.Id);
            if (jobToMove != null)
            {
                jobs.Remove(jobToMove);
                jobs.Add(jobToMove);

                await JsonService.UpdateAllAsync(jobs);
                await OnMove.InvokeAsync();
            }
        }
    }
}