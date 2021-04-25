using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;

        public HomeController(IHttpClientFactory httpClient)
        {
            _httpFactory = httpClient;
        }


        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            // if your client's startup is set to config.GetClaimsFromUserInfoEndpoint = true;
            // then you can get a smaller id_token, cause the claims will be available bellow
            // You must set to your client's startup config.ClaimActions.MapUniqueJsonKey("", "") to see/map the claims
            var claims = User.Claims;

            var _accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var _idToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);

            var resultFromApiOne = await GetDataFromApiOneAsync(accessToken);

            await RefreshAccessToken();

            return View();
        }

        public IActionResult Logout()
        {
            return SignOut("ClientMvc.Cookie", "oidc");
        }

        private async Task<string> GetDataFromApiOneAsync(string accessToken)
        {
            var apiClient = _httpFactory.CreateClient();
            apiClient.SetBearerToken(accessToken);
            var responseApiOne = await apiClient.GetAsync("https://localhost:44306/secret"); // ApiOne
            return await responseApiOne.Content.ReadAsStringAsync();
        }

        private async Task RefreshAccessToken()
        {
            // Discovery Document
            var serverClient = _httpFactory.CreateClient();
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:44353/"); // IdentityServer

            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var refreshTokenClient = _httpFactory.CreateClient();
            var responseRefreshToken = await refreshTokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                RefreshToken = refreshToken,
                ClientId = "client_id_mvc",
                ClientSecret = "client_secret_mvc",
            });

            var authInfo = await HttpContext.AuthenticateAsync("ClientMvc.Cookie");
            authInfo.Properties.UpdateTokenValue("access_token", responseRefreshToken.AccessToken);
            authInfo.Properties.UpdateTokenValue("id_token", responseRefreshToken.IdentityToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", responseRefreshToken.RefreshToken);

            await HttpContext.SignInAsync("ClientMvc.Cookie", authInfo.Principal, authInfo.Properties);
        }
    }
}
