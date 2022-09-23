using Microsoft.JSInterop;

namespace ClientUI.Components.Shared
{
    public partial class MainLayout
    {
        protected override Task OnParametersSetAsync()
        {
            return Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JsRuntime.InvokeVoidAsync("Startup");
        
        }

        private async Task LogoutUser()
        {
            await _accessTokenService.RemoveAccessTokenAsync("jwt_token");
            _navigationManager.NavigateTo("/", true);
        }
    }
}