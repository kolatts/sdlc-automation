using System.Net.Http.Headers;

namespace SdlcAutomation.Jira.Auth;

/// <summary>
/// Handles authentication for JIRA Data Center using Bearer token (PAT)
/// </summary>
public class JiraAuthenticationHandler : DelegatingHandler
{
    private readonly string _token;

    public JiraAuthenticationHandler(string token) : base(new HttpClientHandler())
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be null or empty", nameof(token));
        }

        _token = token;
    }

    public JiraAuthenticationHandler(string token, HttpMessageHandler innerHandler) : base(innerHandler)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be null or empty", nameof(token));
        }

        _token = token;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        return base.SendAsync(request, cancellationToken);
    }
}
