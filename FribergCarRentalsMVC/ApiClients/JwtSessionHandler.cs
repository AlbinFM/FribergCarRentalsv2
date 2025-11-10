namespace FribergCarRentalsMVC.ApiClients;

public class JwtSessionHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _ctx;
    public JwtSessionHandler(IHttpContextAccessor ctx) => _ctx = ctx;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var http = _ctx.HttpContext;
        var token = http?.Session.GetString("Jwt");

        if (!string.IsNullOrEmpty(token) &&
            !request.Headers.Contains("Authorization"))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}