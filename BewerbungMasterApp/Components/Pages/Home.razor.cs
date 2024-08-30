using BewerbungMasterApp.Interfaces;
using BewerbungMasterApp.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BewerbungMasterApp.Components.Pages
{
    public class HomeBase : ComponentBase
    {
        [Inject]
        public IGetJobApplicationsService JobApplicationService { get; set; }

        protected List<JobApplication> jobApplications;

        protected override async Task OnInitializedAsync()
        {
            jobApplications = await JobApplicationService.GetJobApplicationsAsync();
        }
    }
}