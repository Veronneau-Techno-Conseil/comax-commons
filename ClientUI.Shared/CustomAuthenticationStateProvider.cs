using System;
using System.Security.Claims;
using System.Threading.Tasks;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.Services;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Claim = System.Security.Claims.Claim;

namespace CommunAxiom.Commons.ClientUI.Shared
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionViewModel _loginViewModel;
        private readonly IAccessTokenService _accessTokenService;

        public CustomAuthenticationStateProvider(ISessionViewModel loginViewModel, 
            IAccessTokenService accessTokenService)
        {
            _loginViewModel = loginViewModel;
            _accessTokenService = accessTokenService;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var state = await _loginViewModel.GetState();
            if (state.Result != AuthSteps.OK)
            {
                await _accessTokenService.RemoveAccessTokenAsync("jwt_token");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
                User currentUser = await GetUserByJWTAsync();

            if (currentUser != null && currentUser.Email != null)
            {
                //create claimsPrincipal
                var claimsPrincipal = GetClaimsPrinciple(currentUser);
                return new AuthenticationState(claimsPrincipal);
            }
            else
            {
                await _accessTokenService.RemoveAccessTokenAsync("jwt_token");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

        }

        public async Task MarkUserAsAuthenticated()
        {
            var user = await GetUserByJWTAsync();
            var claimsPrincipal = GetClaimsPrinciple(user);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _accessTokenService.RemoveAccessTokenAsync("jwt_token");

            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task<User?> GetUserByJWTAsync()
        {
            
            return await _loginViewModel.GetUserByJWTAsync();
            
        }



        private ClaimsPrincipal GetClaimsPrinciple(User currentUser)
        {
            //create a claims
            var claimEmailAddress = new Claim(ClaimTypes.Name, currentUser.Email);
            var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, Convert.ToString(currentUser.Id));
            
            //create claimsIdentity
            var claimsIdentity = new ClaimsIdentity(new[] { claimEmailAddress, claimNameIdentifier }, "serverAuth");
            //create claimsPrincipal
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return claimsPrincipal;
        }


    }
}