using Microsoft.AspNetCore.Components;
using BewerbungMasterApp.Models;
using BewerbungMasterApp.Services;

namespace BewerbungMasterApp.Components
{
    public class HomeMoveUpDownBase : ComponentBase
    {
        [Inject]
        protected IJsonService JsonService { get; set; } = default!;

        [Parameter]
        public JobApplication Job { get; set; } = default!;

        [Parameter]
        public EventCallback OnMove { get; set; }

        protected async Task MoveJobApplicationToTop()
        {
            var jobs = await JsonService.GetAllAsync<JobApplication>();
            var jobToMove = jobs.FirstOrDefault(j => j.Id == Job.Id);
            if (jobToMove != null)
            {
                jobs.Remove(jobToMove);
                jobs.Insert(0, jobToMove);

                await JsonService.UpdateAllAsync(jobs);
                await OnMove.InvokeAsync();
            }
        }

        protected async Task MoveJobApplicationToBottom()
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