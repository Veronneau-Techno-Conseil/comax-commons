using System.Security.Claims;
using System.Text.Json;
using Blazored.Toast.Services;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClientUI.Components.Shared
{
    public partial class Error
    {
        [Inject] protected IHostingEnvironment HostingEnvironment { get; set; }
        [Inject] protected IToastService ToastService { get; set; }
        [Inject] protected ILogger<Error> Logger { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [CascadingParameter] public Task<AuthenticationState>? AuthenticationState { get; set; }

        public async Task ProcessError(Exception ex)
        {
            ToastService.ShowError(
                HostingEnvironment.IsDevelopment()
                    ? "Oops, something has gone wrong. Please check the logs"
                    : "Oops, something has gone wrong. Please contact system admin");

            var authState = await AuthenticationState;
            var userId = Convert.ToInt64(authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var message = new LogMessage
            {
                LogLevel = LogLevel.Error.ToString(),
                UserId = userId,
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace,
                Source = "Client",
                CreatedDate = DateTime.Now.ToString(),
            };
            Logger.Log(LogLevel.Error, 0, message, null, (logMessage, _) => JsonSerializer.Serialize(logMessage));
        }
    }
}