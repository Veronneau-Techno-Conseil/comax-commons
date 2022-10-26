using IdentityModel.OidcClient.Browser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Server.Helper
{
    public class SystemBrowser : IBrowser
    {
        const int DefaultTimeout = 60 * 5;

        private readonly string _callbackId;
        private readonly IServiceProvider _serviceProvider;
        public SystemBrowser(IServiceProvider serviceProvider, string callbackId)
        {
            _callbackId = callbackId;
            _serviceProvider = serviceProvider;
        }

        private int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            OpenBrowser(options.StartUrl);

            try
            {
                var result = await WaitForCallbackAsync();
                if (String.IsNullOrWhiteSpace(result))
                {
                    return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = "Empty response." };
                }

                return new BrowserResult { Response = result, ResultType = BrowserResultType.Success };
            }
            catch (TaskCanceledException ex)
            {
                return new BrowserResult { ResultType = BrowserResultType.Timeout, Error = ex.Message };
            }
            catch (Exception ex)
            {
                return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = ex.Message };
            }
        }

        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<string> WaitForCallbackAsync(int timeoutInSeconds = DefaultTimeout)
        {
            var source = new CancellationTokenSource(timeoutInSeconds * 1000);

            return await Task.Run(async () =>
            {
                while (true)
                {
                    ITempData tempData = _serviceProvider.GetService<ITempData>();
                    if (tempData.IsOperationResultSet(_callbackId))
                        return tempData.GetOperationResult(_callbackId);
                    await Task.Delay(200, source.Token);
                }
            });
        }
    }
}
