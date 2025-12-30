using Microsoft.AspNetCore.Http;

namespace External.Service.ExternalEndpointClients.DelegatingRequestHandler;

public sealed class AddBearerTokenHeader(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization == null)
        {
            string? token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
            {
                token = token[7..];
            }

            // request.Headers.Authorization = new AuthenticationHeaderValue(token);
            request.Headers.TryAddWithoutValidation("Authorization", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}