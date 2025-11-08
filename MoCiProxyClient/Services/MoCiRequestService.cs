using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MoCiVerification.Services;

public class MoCiRequestService
{
    private readonly HttpClient _client;

    public MoCiRequestService(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }
    public async Task<string> ExecuteAsync(string header, string type, string[] parameters,bool NoCache)
    {
        
        var data = BuildRequestData(header, type, parameters,NoCache);
        var requestUrl = GetRequestUrl(data); 

        Console.WriteLine($"[MoCi] 请求: {requestUrl}");

        var response = await _client.GetAsync(requestUrl).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        
        return result;
    }

    private string BuildRequestData(string header, string type, string[] parameters,bool NoCache)
    {
        var sb = new StringBuilder();
        sb.Append(header).Append("|||").Append(type);
        foreach (var par in parameters)
            sb.Append("|||").Append(par);
        if (NoCache) sb.Append("&v=").Append(new Random().Next(1,1000));
        return sb.ToString();
    }

    private string GetRequestUrl(string data)
    {
        var encodedData = Uri.EscapeDataString(data);
        return $"?parm={encodedData}";
    }
    
    
    
}