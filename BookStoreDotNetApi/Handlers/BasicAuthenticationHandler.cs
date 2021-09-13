using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BookStoreDotNetApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookStoreDotNetApi.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly BookStoresDBContext _context;
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock,BookStoresDBContext context) : base(options, logger, encoder, clock)
        {
            _context = context;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("No authorization header was found");
            }

            try
            {
                var authenticationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var bytes = Convert.FromBase64String(authenticationHeaderValue.Parameter);
                string[] credentials = Encoding.UTF8.GetString(bytes).Split(":");
                var userId = credentials[0];
                var password = credentials[1];
                User user = _context.Users.Where(user => user.UserId == userId && user.Password == password)
                    .FirstOrDefault();
                if (user==null)
                {
                    AuthenticateResult.Fail("Invalid username or password");
                }
                else
                {
                    var claims = new[] {new Claim(ClaimTypes.Name, user.UserId)};
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }
            }
            catch (Exception e)
            {
                return AuthenticateResult.Fail("Encountered an error");
            }
            return AuthenticateResult.Fail("Authentication not implemented");
            
        }
    }
}