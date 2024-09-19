using BewerbungMasterApp.Models;
using BewerbungMasterApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BewerbungMasterApp.Components
{
    public class HomeMessageBase : ComponentBase
    {
        [Inject]
        protected IJsonService JsonService { get; set; } = default!;

        [Inject]
        protected IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter]
        public required JobApplication Job { get; set; }

        protected async Task CopyPersonalizedMessage()
        {
            var user = await JsonService.GetUserDataAsync();
            var message = $@"Sehr geehrte Damen und Herren,

als Antwort auf die Anzeige {Job.URL} für die Stelle {Job.Position} in Ihrem Unternehmen {Job.Company} sende ich Ihnen meine Bewerbungsunterlagen im Anhang.

Mit freundlichen Grüßen
{user.FirstName} {user.LastName}";

            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", message);
        }
    }
}