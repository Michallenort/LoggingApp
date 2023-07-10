using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace LoggingApp.Frontend;

public class CustomAuthStateProvider : AuthenticationStateProvider 
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _http;
    
    public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
    {
        _localStorage = localStorage;
        _http = http;
    }

    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string token = await _localStorage.GetItemAsStringAsync("token");
        //string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjQiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiTHVrYXN6IEZpbGlwZWsiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsIkRhdGVPZkJpcnRoIjoiMjAwMS0wMS0wMiIsIk5hdGlvbmFsaXR5IjoiUG9sYW5kIiwiZXhwIjoxNjkwMTExNTI3LCJpc3MiOiJodHRwOi8vYWNjb3VudGFwaS5jb20iLCJhdWQiOiJodHRwOi8vYWNjb3VudGFwaS5jb20ifQ.oOY5g_GFnzVAIT5KbrlmR1dFhk3VYirggVKjCr72980";

        var identity = new ClaimsIdentity();
        _http.DefaultRequestHeaders.Authorization = null;
        
        if (!string.IsNullOrEmpty(token))
        {
            identity = new ClaimsIdentity(ParseClaimFromJwt(token), "jwt");
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
        }

        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);
        
        NotifyAuthenticationStateChanged(Task.FromResult(state));

        return state;
    }
    
    public static IEnumerable<Claim> ParseClaimFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuesPairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuesPairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }
    
    public static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: 
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }
}