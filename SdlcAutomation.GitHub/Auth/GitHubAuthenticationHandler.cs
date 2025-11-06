using System.Net.Http.Headers;

namespace SdlcAutomation.GitHub.Auth;

/// <summary>
/// Handles authentication for GitHub using Personal Access Token (PAT)
/// </summary>
public class GitHubAuthenticationHandler : DelegatingHandler
{
    private readonly string _token;

    public GitHubAuthenticationHandler(string token) : base(new HttpClientHandler())
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be null or empty", nameof(token));
        }

        _token = token;
    }

    public GitHubAuthenticationHandler(string token, HttpMessageHandler innerHandler) : base(innerHandler)
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
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("SdlcAutomation", "1.0"));
        return base.SendAsync(request, cancellationToken);
    }
}
