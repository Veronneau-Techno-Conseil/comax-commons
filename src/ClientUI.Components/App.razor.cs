using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.Shared;

namespace ClientUI.Components
{
    public partial class App
    {
        private async Task CheckState()
        {
            OperationResult<string>? state = await _loginViewModel.GetState();
            if (state.Result != AuthSteps.OK)
            {
                _navigationManager.NavigateTo("/login");
            }
        }

        protected override Task OnInitializedAsync() => _job.Start();

        public void Dispose() => _job.Stop();
    }
}