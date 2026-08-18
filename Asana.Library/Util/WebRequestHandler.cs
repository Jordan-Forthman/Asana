using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Asana.Library.Util
{
    /// <summary>
    /// Thin HTTP client for the Asana API.
    ///
    /// The base address comes from the ASANA_API_URL environment variable and
    /// falls back to the address the API's "http" launch profile listens on,
    /// so the CLI and the MAUI app reach a locally running API with no setup.
    /// It previously hardcoded https://localhost:7172, which required trusting
    /// the ASP.NET development certificate first and broke silently whenever
    /// the API was started on any other profile.
    /// </summary>
    public class WebRequestHandler
    {
        public const string DefaultBaseUrl = "http://localhost:5206";

        // One HttpClient for the process. Creating one per request, as this
        // used to, leaks sockets in TIME_WAIT under any real load.
        private static readonly HttpClient Client = new HttpClient();

        private static bool _warned;

        /// <summary>Base address of the API, without a trailing slash.</summary>
        public static string BaseUrl =>
            (Environment.GetEnvironmentVariable("ASANA_API_URL") ?? DefaultBaseUrl)
                .TrimEnd('/');

        private static string Absolute(string url) => $"{BaseUrl}{url}";

        /// <summary>
        /// Report an unreachable API once, with the address tried and how to
        /// start it. The previous code caught every exception into an empty
        /// block, so a refused connection was indistinguishable from an empty
        /// result and callers crashed on the null that came back.
        /// </summary>
        private static void ReportUnreachable(Exception ex)
        {
            if (_warned) return;
            _warned = true;

            Console.Error.WriteLine($"Could not reach the Asana API at {BaseUrl}");
            Console.Error.WriteLine($"  {ex.GetType().Name}: {ex.Message}");
            Console.Error.WriteLine("  Start it with: dotnet run --project Asana.API");
            Console.Error.WriteLine("  Or point this client elsewhere with ASANA_API_URL.");
        }

        public async Task<string?> Get(string url)
        {
            try
            {
                return await Client.GetStringAsync(Absolute(url)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ReportUnreachable(ex);
                return null;
            }
        }

        public async Task<string?> Delete(string url)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, Absolute(url));
                using var response = await Client
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    Console.Error.WriteLine($"DELETE {url} failed: {(int)response.StatusCode} {response.ReasonPhrase}");
                    return null;
                }

                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ReportUnreachable(ex);
                return null;
            }
        }

        public async Task<string?> Post(string url, object obj)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, Absolute(url));
                var json = JsonConvert.SerializeObject(obj);
                using var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = stringContent;

                using var response = await Client
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    Console.Error.WriteLine($"POST {url} failed: {(int)response.StatusCode} {response.ReasonPhrase}");
                    return null;
                }

                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ReportUnreachable(ex);
                return null;
            }
        }
    }
}
