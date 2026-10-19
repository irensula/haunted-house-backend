using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using GameBackend.Configuration;

namespace GameBackend.Services;

public class EmailService
{
    private readonly HttpClient _httpClient;
    private readonly ResendOptions _options;

    public EmailService(
        HttpClient httpClient,
        IOptions<ResendOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string html,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, _options.ApiUrl);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        request.Content = JsonContent.Create(new
        {
            from = $"{_options.FromName} <{_options.FromEmail}>",
            to = new[] { to },
            subject,
            html
        });

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}