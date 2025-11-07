using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DistributionManagement.Application.DTOs;
using DistributionManagement.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DistributionManagement.Infrastructure.ExternalServices;

public class WSO2AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _tokenEndpoint;
    private readonly string _userInfoEndpoint;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public WSO2AuthenticationService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _tokenEndpoint = _configuration["WSO2:TokenEndpoint"] ?? throw new InvalidOperationException("TokenEndpoint not configured");
        _userInfoEndpoint = _configuration["WSO2:UserInfoEndpoint"] ?? throw new InvalidOperationException("UserInfoEndpoint not configured");
        _clientId = _configuration["WSO2:ClientId"] ?? throw new InvalidOperationException("ClientId not configured");
        _clientSecret = _configuration["WSO2:ClientSecret"] ?? throw new InvalidOperationException("ClientSecret not configured");
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var tokenRequestData = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username", request.Username },
                { "password", request.Password },
                { "scope", "openid profile email roles groups" }
            };

            var content = new FormUrlEncodedContent(tokenRequestData);
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

            var response = await _httpClient.PostAsync(_tokenEndpoint, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException($"Authentication failed: {responseBody}");
            }

            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new InvalidOperationException("Invalid token response from WSO2 IS");
            }

            var userInfo = await GetUserInfoAsync(tokenResponse.AccessToken);

            return new LoginResponse
            {
                AccessToken = tokenResponse.AccessToken,
                TokenType = tokenResponse.TokenType ?? "Bearer",
                ExpiresIn = tokenResponse.ExpiresIn,
                RefreshToken = tokenResponse.RefreshToken,
                UserInfo = userInfo
            };
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Failed to communicate with WSO2 IS: {ex.Message}", ex);
        }
    }

    public async Task<UserInfoDto> GetUserInfoAsync(string accessToken)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync(_userInfoEndpoint);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException($"Failed to get user info: {responseBody}");
            }

            var userInfoResponse = JsonSerializer.Deserialize<WSO2UserInfoResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (userInfoResponse == null)
            {
                throw new InvalidOperationException("Invalid user info response");
            }

            var primaryRole = userInfoResponse.Roles?.FirstOrDefault() ?? string.Empty;

            return new UserInfoDto
            {
                Username = userInfoResponse.Username ?? userInfoResponse.PreferredUsername ?? string.Empty,
                Email = userInfoResponse.Email ?? string.Empty,
                FirstName = userInfoResponse.GivenName,
                LastName = userInfoResponse.FamilyName,
                Role = primaryRole,
                Roles = userInfoResponse.Roles ?? new List<string>(),
                Groups = userInfoResponse.Groups ?? new List<string>()
            };
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Failed to get user info from WSO2 IS: {ex.Message}", ex);
        }
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string? TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
        public string? IdToken { get; set; }
        public string? Scope { get; set; }
    }

    private class WSO2UserInfoResponse
    {
        public string? Sub { get; set; }
        public string? Username { get; set; }
        public string? PreferredUsername { get; set; }
        public string? Email { get; set; }
        public bool? EmailVerified { get; set; }
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }
        public string? Name { get; set; }
        public List<string>? Roles { get; set; }
        public List<string>? Groups { get; set; }
    }
}
