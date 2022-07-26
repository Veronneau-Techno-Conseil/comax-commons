using Blazored.LocalStorage;
using Blazored.Toast;
using CommunAxiom.Commons.ClientUI.Shared.JsonLocalizer;
using CommunAxiom.Commons.ClientUI.Shared.Logging;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.Services;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection SetBlazorApp(this IServiceCollection services,
            ApplicationSettings applicationSettings)
        {
            // blazored services
            services.AddBlazoredToast();
            services.AddBlazoredLocalStorage();

            // authetication & authorization
            services.AddOptions();
            services.AddAuthorizationCore();
            services.AddLocalization();

            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            services.AddScoped<IAccessTokenService, WebAppAccessTokenService>();

            // configuring http clients
            services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(applicationSettings.BaseAddress) });

            // transactional named http clients
            var clientConfigurator = void (HttpClient client) => client.BaseAddress = new Uri(applicationSettings.BaseAddress);

            //services.AddHttpClient<IProfileViewModel, ProfileViewModel>("ProfileViewModelClient", clientConfigurator);
            //services.AddHttpClient<IContactsViewModel, ContactsViewModel>("ContactsViewModelClient", clientConfigurator);
            //services.AddHttpClient<ISettingsViewModel, SettingsViewModel>("SettingsViewModelClient", clientConfigurator);
            //services.AddHttpClient<IAssignRolesViewModel, AssignRolesViewModel>("AssignRolesViewModel", clientConfigurator);

            //// authentication http clients
            services.AddHttpClient<ISessionViewModel, SessionViewModel>("LoginViewModelClient", clientConfigurator);
            services.AddHttpClient<IPortfolioViewModel, PortfolioViewModel>("CreatePortfolioViewMode", clientConfigurator);
            //services.AddHttpClient<IRegisterViewModel, RegisterViewModel>("RegisterViewModelClient", clientConfigurator);

            // logging
            services.AddLogging(logging => logging.SetMinimumLevel(LogLevel.Error));
            services.AddSingleton<LogQueue>();
            services.AddSingleton<LogReader>();
            services.AddSingleton<LogWriter>();
            services.AddSingleton<ILoggerProvider, ApplicationLoggerProvider>();
            services.AddHttpClient("LoggerJob", c => c.BaseAddress = new Uri(applicationSettings.BaseAddress));
            services.AddSingleton<LoggerJob>();

            services.AddScoped<IStdMessagesService, StdMessageService>();


            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            CultureInfo[] supportedCultures = new[] {
                new CultureInfo("fr"),
                new CultureInfo("en")
            };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("fr");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };

            });

            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

            return services;
        }
    }
}
