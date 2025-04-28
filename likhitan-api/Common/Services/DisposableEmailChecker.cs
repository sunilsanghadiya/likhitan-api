namespace likhitan_api.Common.Services;
using System.Net.Http;
using System.Threading.Tasks;

public interface IDisposableEmailChecker
{
    Task<bool> IsDisposableEmail(string email);
}

public class DisposableEmailChecker : IDisposableEmailChecker
{
    private readonly HttpClient _httpClient;
    private readonly HashSet<string> _disposableDomains = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        // Add common disposable domains
        "tempmail.com", "mailinator.com", "10minutemail.com",
        "guerrillamail.com", "throwawaymail.com", "yopmail.com",
        "temp-mail.org", "fakeinbox.com", "trashmail.com"
    };

    public DisposableEmailChecker(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<bool> IsDisposableEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Task.FromResult(false);

        var domain = email.Split('@').LastOrDefault();

        if (string.IsNullOrEmpty(domain))
            return Task.FromResult(false);

        // Check against local list
        if (_disposableDomains.Contains(domain))
            return Task.FromResult(true);

        // Optionally check against external API
        return CheckExternalDisposableEmailService(domain);
    }

    private async Task<bool> CheckExternalDisposableEmailService(string domain)
    {
        try
        {
            // Example using MailboxValidator API
            var response = await _httpClient.GetAsync($"https://api.mailboxvalidator.com/v1/validation/single?email=test@{domain}&key=YOUR_API_KEY");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<MailboxValidatorResponse>();
            return result?.IsDisposable == true;
        }
        catch
        {
            return false; // Fail open if service is unavailable
        }
    }

    private class MailboxValidatorResponse
    {
        public bool IsDisposable { get; set; }
    }
}