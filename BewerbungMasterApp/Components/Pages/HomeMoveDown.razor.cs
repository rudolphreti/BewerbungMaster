using Microsoft.JSInterop;
using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Components.Pages
{
    public partial class HomeBase
    {
        protected async Task MoveJobApplicationToEnd(Guid id)
        {
            var jobToMove = jobApplications.FirstOrDefault(j => j.Id == id);
            if (jobToMove != null)
            {
                jobApplications.Remove(jobToMove);
                jobApplications.Add(jobToMove);

                // Update the JSON file
                await JsonService.UpdateAllAsync(jobApplications);

                // Trigger a re-render of the component
                StateHasChanged();
            }
        }
    }
}