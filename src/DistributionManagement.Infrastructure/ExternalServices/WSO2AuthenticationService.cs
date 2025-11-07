using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DistributionManagement.Application.DTOs;
using DistributionManagement.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DistributionManagement.Infrastructure.ExternalServices;

public class WSO2AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WSO2AuthenticationService> _logger;
    private readonly string _tokenEndpoint;
    private readonly string _userInfoEndpoint;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public WSO2AuthenticationService(
        HttpClient httpClient, 
        IConfiguration configuration,
        ILogger<WSO2AuthenticationService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _tokenEndpoint = _configuration["WSO2:TokenEndpoint"] ?? throw new InvalidOperationException("TokenEndpoint not configured");
        _userInfoEndpoint = _configuration["WSO2:UserInfoEndpoint"] ?? throw new InvalidOperationException("UserInfoEndpoint not configured");
        _clientId = _configuration["WSO2:ClientId"] ?? throw new InvalidOperationException("ClientId not configured");
        _clientSecret = _configuration["WSO2:ClientSecret"] ?? throw new InvalidOperationException("ClientSecret not configured");
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Attempting login for user: {Username}", request.Username);

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

            _logger.LogInformation("WSO2 Response Status: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Authentication failed with status {StatusCode}: {ResponseBody}", response.StatusCode, responseBody);
                throw new UnauthorizedAccessException($"Authentication failed: {responseBody}");
            }

            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(
                responseBody, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                _logger.LogError("Token response missing AccessToken. Response: {@TokenResponse}", tokenResponse);
                throw new InvalidOperationException("Invalid token response from WSO2 IS - missing AccessToken");
            }

            _logger.LogInformation("Token received successfully");

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
            _logger.LogError(ex, "HTTP error communicating with WSO2 IS");
            throw new InvalidOperationException($"Failed to communicate with WSO2 IS: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login");
            throw;
        }
    }

    public async Task<UserInfoDto> GetUserInfoAsync(string accessToken)
    {
        try
        {
            _logger.LogInformation("Getting user info from WSO2...");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync(_userInfoEndpoint);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get user info: {ResponseBody}", responseBody);
                throw new UnauthorizedAccessException($"Failed to get user info: {responseBody}");
            }

            var userInfoResponse = JsonSerializer.Deserialize<WSO2UserInfoResponse>(
                responseBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (userInfoResponse == null)
            {
                _logger.LogError("User info response is null");
                throw new InvalidOperationException("Invalid user info response");
            }

            // ✅ FIX: Clean up role names - remove "Internal/" prefix
            var cleanedRoles = userInfoResponse.Roles?
                .Select(r => r.Replace("Internal/", "").Replace("internal/", ""))
                .ToList() ?? new List<string>();

            var primaryRole = cleanedRoles.FirstOrDefault() ?? string.Empty;

            _logger.LogInformation("User info retrieved - Username: {Username}, Role: {Role}, All Roles: {AllRoles}", 
                userInfoResponse.Username, primaryRole, string.Join(", ", cleanedRoles));

            return new UserInfoDto
            {
                Username = userInfoResponse.Username ?? userInfoResponse.PreferredUsername ?? string.Empty,
                Email = userInfoResponse.Email ?? string.Empty,
                FirstName = userInfoResponse.GivenName,
                LastName = userInfoResponse.FamilyName,
                Role = primaryRole,
                Roles = cleanedRoles,
                Groups = userInfoResponse.Groups ?? new List<string>()
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error getting user info from WSO2 IS");
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
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("id_token")]
        public string? IdToken { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }
    }

    private class WSO2UserInfoResponse
    {
        public string? Sub { get; set; }
        public string? Username { get; set; }
        
        [JsonPropertyName("preferred_username")]
        public string? PreferredUsername { get; set; }
        
        public string? Email { get; set; }
        
        [JsonPropertyName("email_verified")]
        public bool? EmailVerified { get; set; }
        
        [JsonPropertyName("given_name")]
        public string? GivenName { get; set; }
        
        [JsonPropertyName("family_name")]
        public string? FamilyName { get; set; }
        
        public string? Name { get; set; }
        public List<string>? Roles { get; set; }
        public List<string>? Groups { get; set; }
    }
}
