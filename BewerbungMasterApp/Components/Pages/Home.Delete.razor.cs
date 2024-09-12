using Microsoft.JSInterop;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Components.Pages
{
    public partial class HomeBase
    {
        protected async Task DeleteJobApplication(Guid id)
        {
            var success = await JsonService.DeleteAsync<JobApplication>(id);
            if (success)
            {
                jobApplications.RemoveAll(job => job.Id == id);
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "Failed to delete the job application.");
            }
        }
    }
}