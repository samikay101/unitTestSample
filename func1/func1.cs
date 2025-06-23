using System.Net.Http.Headers;
using System.Text;
using Azure.Core;
using Azure.Identity;

namespace LogicAppRunner;

internal static class Program
{
    /// <summary>
    /// Application entry‑point (async).  Reads trigger URL from args or env var,
    /// acquires an Azure AD token for the Logic Apps resource, calls the
    /// workflow, and prints the response.
    ///
    /// Usage:  dotnet run "<TRIGGER_URL>"
    /// or set the LOGICAPP_URL environment variable.
    /// </summary>
    /// <param name="args">Cmd‑line arguments; first parameter may contain the trigger URL.</param>
    public static async Task Main(string[] args)
    {
        // ------------------------------------------------------------------
        // 1. Resolve the Logic App trigger URL.
        // ------------------------------------------------------------------
        string? triggerUrl = args.Length > 0
            ? args[0]
            : Environment.GetEnvironmentVariable("LOGICAPP_URL");

        if (string.IsNullOrWhiteSpace(triggerUrl))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: Supply the Logic App trigger URL as an argument or in the LOGICAPP_URL environment variable.");
            Console.ResetColor();
            return;
        }

        // ------------------------------------------------------------------
        // 2. Acquire bearer token using DefaultAzureCredential.
        //    This will try (in order):
        //      • Environment variables (client secret / cert)
        //      • AZ CLI (az login)
        //      • Visual Studio / VS Code signed‑in account
        //      • Managed Identity (if deployed in Azure)
        // ------------------------------------------------------------------
        var userAssignedClientId = "222e4ad7-39ef-4ad2-abb5-67caebcdcf45"; // my tenant ID

        var credential = new DefaultAzureCredential(
            new DefaultAzureCredentialOptions
            {
                ManagedIdentityClientId = userAssignedClientId
            });
        var token      = await GetAccessTokenAsync(credential);

        // ------------------------------------------------------------------
        // 3. Execute the HTTP POST against the trigger URL.
        // ------------------------------------------------------------------
        var runId = await InvokeWorkflowAsync(triggerUrl!, token);
                //var runId = await InvokeWorkflowAsync(triggerUrl);


        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✔ Workflow invoked successfully. Run Id: {runId}");
        Console.ResetColor();
    }

    /// <summary>
    /// Requests an access token for the Logic Apps resource‑ID.
    /// </summary>
    /// <param name="credential">The <see cref="TokenCredential"/> implementation to use.</param>
    /// <returns>Access token string.</returns>
    private static async Task<string> GetAccessTokenAsync(TokenCredential credential)
    {
        // Scope (MSAL style) for Logic Apps – includes /.default
        const string scope = "https://logic.azure.com/.default";

        AccessToken accessToken = await credential.GetTokenAsync(
            new TokenRequestContext(new[] { scope }),
            CancellationToken.None);

        return accessToken.Token;
    }

    /// <summary>
    /// Sends a POST request to the Logic App trigger endpoint with the bearer
    /// token in the Authorization header.
    /// </summary>
    /// <param name="triggerUrl">Fully qualified workflow trigger URL.</param>
    /// <param name="bearerToken">Azure AD bearer token.</param>
    /// <returns>The Logic App run identifier extracted from the Location header (if present).</returns>
    private static async Task<string?> InvokeWorkflowAsync(string triggerUrl, string bearerToken)
    {
        using var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(100)
        };

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Example payload – modify as required by the workflow's schema.
        var content = new StringContent("{}", Encoding.UTF8, "application/json");

        using HttpResponseMessage response = await httpClient.PostAsync(triggerUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            string body = await response.Content.ReadAsStringAsync();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✖ Call failed ({(int)response.StatusCode}) – {response.ReasonPhrase}\n{body}");
            Console.ResetColor();
            response.EnsureSuccessStatusCode(); // throws
        }

        // Azure Logic Apps returns a Location header with the run details.
        response.Headers.TryGetValues("Location", out var locations);
        string? runLocation = locations?.FirstOrDefault();
        string? runId       = runLocation?.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

        return runId;
    }
}
