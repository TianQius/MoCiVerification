using System;
using System.Threading.Tasks;
using MoCiProxyClient.Models;

namespace MoCiVerification.Services;

public class ProxyService:IProxyService
{
    private readonly MoCiRequestService _moCiService;
    private readonly ClientSettings _clientSettings;
    
    public ProxyService(MoCiRequestService moCiService,ClientSettings clientSettings)
    {
        _moCiService = moCiService;
        _clientSettings = clientSettings;
    }
    private async Task<string> Execute(string header, string type, string?[] parameters)
    {
        try
        {
            return await _moCiService.ExecuteAsync(header, type, parameters,false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MoCi Error] {ex.Message}");
            return "请求失败";
        }
    }
    public async Task<bool> AgentLogin(string token,string username, string password)
    {
        var result = await Execute("代理", "代理登录", new[] {token ,username, password });
        var parts = result.Split("|||");
        _clientSettings.GlobalMessage = result;
        if (parts.Length == 2)
        {
            if (parts[0] == "代理登录成功")
            {
                var r = await Execute("代理", "代理回调", new[] {token ,parts[1] });
                if (r.Contains("代理回调成功"))
                {
                    _clientSettings.ClientLicense = parts.Length > 1 ? parts[1] : null;
                    _clientSettings.UserName = username;
                    _clientSettings.PassWord = password;
                    _clientSettings.ProjectToken = token;
                    return true;
                }
                
            }
        }
        return false;
    }
    
    public async Task<string[]?> GetAgentCardList()
    {
        var result = await Execute("代理", "代理获取卡密列表", new[] {_clientSettings.ProjectToken ,_clientSettings.ClientLicense});
        _clientSettings.GlobalMessage = result;
        if (result.Contains("无卡密") || result.Contains("请求失败") || result.Contains("请求超时")) return null;
        return result.Split("\n", StringSplitOptions.RemoveEmptyEntries);
    }
    public async Task<bool> DeleteCard(string card)
    {
        var result = await Execute("代理", "代理删除卡密", new[] {_clientSettings.ProjectToken ,card,_clientSettings.ClientLicense});
        _clientSettings.GlobalMessage = result;
        return result == "代理删除卡密成功";
    }
}