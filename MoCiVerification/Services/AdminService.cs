using System;
using System.Threading.Tasks;
using MoCiVerification.Converts;
using MoCiVerification.Models;
using Newtonsoft.Json;

namespace MoCiVerification.Services;

public class AdminService:IAdminService
{
    private readonly MoCiRequestService _moCiService;
    private readonly ClientSettings _clientSettings;

    public AdminService(MoCiRequestService moCiService, ClientSettings st)
    {
        _moCiService = moCiService;
        _clientSettings = st;
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
    private async Task<string> Execute(string header, string type, string?[] parameters,bool noCache)
    {
        try
        {
            return await _moCiService.ExecuteAsync(header, type, parameters, noCache);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MoCi Error] {ex.Message}");
            return "请求失败";
        }
    }

    public async Task<string> GetAccountCountAsync() =>
        await Execute("作者", "取验证注册用户数记录", new[] { "CCYYDS" });

    public async Task<string> GetReceivedByteAsync() =>
        await Execute("作者", "取验证接收字节数记录", new[] { "CCYYDS" });

    public async Task<string> GetSendedByteAsync() =>
        await Execute("作者", "取验证发送字节数记录", new[] { "CCYYDS" });

    public async Task<string> GetProcessedByteAsync() =>
        await Execute("作者", "取验证处理请求次数记录", new[] { "CCYYDS" });

    public async Task<(bool Success, string Message, string? License)> LoginAsync(string username, string password)
    {
        try
        {
            var resultStr = await Execute("作者", "登录", new[] { username, password, _clientSettings.AppVersion });
            var parts = resultStr.Split("|||");

            if (parts.Length == 1)
            {
                int i = parts[0].IndexOf("：");
                if (i != -1)
                {
                    _clientSettings.GlobalMessage = parts[0].Substring(i + 1);
                    return (false, parts[0], null);
                }

                _clientSettings.GlobalMessage = parts[0];
                return (false, parts[0], null);
            }

            _clientSettings.ClientLicense = parts.Length > 1 ? parts[1] : null;
            //Console.WriteLine("[MOCI][TOKEN] :{0}", _clientSettings.ClientLicense);
            _clientSettings.UserName = username;
            _clientSettings.Password = password;
            
            return (true, parts[0], _clientSettings.ClientLicense);
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return (false, "[false] :" +ex.Message, null);
        }
    }

    public async Task<bool> LoginVerificationAsync()
    {
        var result = await Execute("作者", "回调", new[] { _clientSettings.UserName, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "回调成功";
    }

    public async Task<(bool Success,bool IsActive)> RegisterAsync(string username, string password, string email)
    {
        var result =await Execute("作者","注册", new[] { username, password, email });
        _clientSettings.GlobalMessage = result;
        if (result == "注册令牌已发送至您的邮箱，请注意查收")
            return (true, false);
        return (false, false);
    }
    public async Task<bool>  ActiveEmailAsync(string username, string key)
    {
        var result =await Execute("作者","邮箱激活", new[] { username,  key });
        _clientSettings.GlobalMessage = result;
        if (result == "邮箱激活成功")
            return true;
        return false;
    }
    #region Project

    public async Task<string[]?> GetProjectListAsync()
    {
        var result = await Execute("作者", "获取项目列表", new[] { _clientSettings.UserName, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return ProcessListResult(result);
    }

    public async Task<bool> AddProjectAsync(string projectName, string projectType, string announcement, string projectKey)
    {
        var result = await Execute("作者", "新增项目",
            new[]
            {
                _clientSettings.UserName, projectName, projectType, announcement, projectKey,
                _clientSettings.ClientLicense
            });
        _clientSettings.GlobalMessage = result;
        return result == "新增项目成功";
    }

    public async Task<bool> StopProjectAsync(string projectName)
    {
        var result = await Execute("作者", "停用项目", new[] {_clientSettings.UserName, projectName, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "停用项目成功";
    }

    public async Task<bool> RecoverProjectAsync(string projectName)
    {
        var result = await Execute("作者", "恢复项目", new[] {_clientSettings.UserName, projectName, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "恢复项目成功";
    }

    public async Task<bool> DeleteProjectAsync(string projectName)
    {
        var result = await Execute("作者", "删除项目", new[] { _clientSettings.UserName,projectName, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "删除项目成功";
    }

    public async Task<bool> ChangeProjectAnnouncementAsync(string projectName, string newAnnouncement)
    {
        var result = await Execute("作者", "修改项目公告", new[] { _clientSettings.UserName,projectName, newAnnouncement, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "修改项目公告成功";
    }

    #endregion

    #region Version

    public async Task<string[]?> GetVersionListAsync(string projectName)
    {
        var result = await Execute("作者", "获取版本列表", new[] { _clientSettings.UserName, projectName, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return ProcessListResult(result);
    }
    public async Task<bool> CreateVersion(string projectName,string version,string announcement,string data,string state)
    {
        var result = await Execute("作者", "新增版本",
            new[] { _clientSettings.UserName, projectName, version,announcement,data,state, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "新增版本成功";
    }
    public async Task<bool> DeleteVersion(string projectName,string version)
    {
        var result = await Execute("作者", "删除版本",
            new[] { _clientSettings.UserName, projectName, version, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "删除版本成功";
    }
    public async Task<bool> StopVersion(string projectName,string version)
    {
        var result = await Execute("作者", "停用版本",
            new[] { _clientSettings.UserName, projectName, version, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "停用版本成功";
    }
    public async Task<bool> RecoverVersion(string projectName,string version)
    {
        var result = await Execute("作者", "恢复版本",
            new[] { _clientSettings.UserName, projectName, version, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "恢复版本成功";
    }
    public async Task<bool> ChangeVersionAnnouncement(string projectName,string version,string newAnnouncement)
    {
        var result = await Execute("作者", "修改版本公告",
            new[] { _clientSettings.UserName, projectName, version, newAnnouncement, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "修改版本公告成功";
    }
    public async Task<bool> ChangeVersionData(string projectName,string version,string newData)
    {
        var result = await Execute("作者", "修改版本数据",
            new[] { _clientSettings.UserName, projectName, version, newData, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "修改版本数据成功";
    }

    #endregion

    #region Variable

    public async Task<string[]?> GetVariableListAsync(string projectName)
    {
        var result = await Execute("作者", "获取云变量列表", new[] { _clientSettings.UserName, projectName, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return ProcessListResult(result);
    }
    public async Task<bool> DeleteVar(string projectName,string var)
    {
        var result = await Execute("作者", "删除云变量", new[] { _clientSettings.UserName,projectName,var, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "删除云变量成功";
    }
    public async Task<bool> GetVarOption(string projectName)
    {
        var result = await Execute("作者", "获取云变量设置", new[] { _clientSettings.UserName,projectName, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result.Split("|||")[0] == "启用";//[1]为获取方式（登录获取和直接获取）
    }
    public async Task<bool> SetVarOption(string projectName,bool use)
    {
        var result = await Execute("作者", "修改云变量设置",
            new[] { _clientSettings.UserName, projectName, use ? "启用" : "关闭","直接获取", _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "修改云变量设置成功";
    }

    #endregion

    public async Task<string[]?> GetUserListAsync(string projectName)
    {
        var result = await Execute("作者", "获取用户列表", new[] { _clientSettings.UserName, projectName, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return ProcessListResult(result);
    }
    public async Task<bool> DeleteUser(string projectName,string user)
    {
        var result = await Execute("作者", "删除用户", new[] { _clientSettings.UserName,projectName,user, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "删除用户成功";
    }
    public async Task<bool> StopUser(string projectName,string user)
    {
        var result = await Execute("作者", "停用用户", new[] { _clientSettings.UserName,projectName,user, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "停用用户成功";
    }
    public async Task<bool> RecoverUser(string projectName,string user)
    {
        var result = await Execute("作者", "恢复用户", new[] { _clientSettings.UserName,projectName,user, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "恢复用户成功";
    }
    public async Task<bool> OffUser(string projectName,string user)
    {
        var result = await Execute("作者", "下线用户", new[] { _clientSettings.UserName,projectName,user, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "下线用户成功";
    }
    public async Task<bool> UnBindUser(string projectName,string user)
    {
        var result = await Execute("作者", "解绑用户机器码", new[] { _clientSettings.UserName,projectName,user, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "解绑用户机器码成功";
    }
    public async Task<bool> ClearUserBindTimes(string projectName,string user)
    {
        var result = await Execute("作者", "清空用户解绑次数", new[] { _clientSettings.UserName,projectName,user, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "清空用户解绑次数成功";
    }
    public async Task<string[]?> GetBlackerListAsync(string projectName)
    {
        var result = await Execute("作者", "获取黑名列表", new[] { _clientSettings.UserName, projectName, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return ProcessListResult(result);
    }
    public async Task<bool>  CreateBlacker(string projectname,string value,string reason)
    {
        var result =await Execute("作者","添加黑名", new[] { _clientSettings.UserName, projectname,value,reason,_clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "添加黑名成功";
    }
    public async Task<bool>  DeleteBlacker(string projectname,string blacker)
    {
        var result =await Execute("作者","删除黑名", new[] { _clientSettings.UserName, projectname,blacker,_clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "删除黑名成功";
    }

    public async Task<string[]?> GetDataListAsync(string projectName)
    {
        var result = await Execute("作者", "获取自定义数据列表", new[] { _clientSettings.UserName, projectName, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return ProcessListResult(result);
    }
    
    
    public async Task<bool>  DeleteCustomData(string projectname ,string key)
    {
        var result =await Execute("作者","删除自定义数据", new[] { _clientSettings.UserName, projectname,key,_clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "删除自定义数据成功";
    }

    public async Task<bool> CreateCustomData(string projectname, string key, string value, string mark, string getway)
    {
        var result =await Execute("作者","新增自定义数据", new[] { _clientSettings.UserName, projectname,key,value,mark,getway,_clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "新增自定义数据成功";
    }
    public async Task<bool>  ChangeCustomDataValue(string projectname,string key ,string value)
    {
        var result = await Execute("作者", "修改自定义数据Value",
            new[] { _clientSettings.UserName, projectname, key, value, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "修改自定义数据Value成功";
    }
    public async Task<bool>  ChangeCustomDataMark(string projectname ,string key,string mark)
    {
        var result =await Execute("作者","修改自定义数据备注", new[] { _clientSettings.UserName, projectname,key,mark,_clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "修改自定义数据备注成功";
    }
    public async Task<bool>  ChangeCustomDataGetWay(string projectname ,string key,bool direct)
    {
        var result = String.Empty;
        if(direct) 
            result =await Execute("作者","修改自定义数据直接获取", new[] { _clientSettings.UserName, projectname,key,_clientSettings.ClientLicense });
        else
            result = await Execute("作者","修改自定义数据登录获取", new[] { _clientSettings.UserName, projectname,key,_clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "修改自定义数据直接获取成功" || result == "修改自定义数据登录获取成功";

    }
    
    public async Task<string[]?> GetCardListAsync(string projectName)
    {
        var result = await Execute("作者", "获取卡密列表", new[] { _clientSettings.UserName, projectName, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return ProcessListResult(result);
    }

    public async Task<bool> DeleteCard(string projectName,string card)
    {
        var result = await Execute("作者", "删除卡密",
            new[] { _clientSettings.UserName, projectName, card, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return (result.IndexOf("删除卡密成功") != -1) || (result.IndexOf("删除用户成功") != -1);
    }
    public async Task<bool> CreateCard(string projectName,int count,string type,string prefix ,string mask)
    {
        var result = await Execute("作者", "新增卡密",
            new[] { _clientSettings.UserName, projectName, count.ToString(),type,prefix,mask,_clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        //返回卡密
        return result.IndexOf("新增卡密失败") == -1;
    }
    
    public async Task<bool> ChangeBindTimes(string projectName,string count)
    {
        var result = await Execute("作者", "修改解绑次数上限",
            new[] { _clientSettings.UserName, projectName, count,_clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return result == "修改解绑次数上限成功";
    }
    
    public async Task<string[]?> GetAgentListAsync(string projectName)
    {
        var result = await Execute("作者", "获取代理列表", new[] { _clientSettings.UserName, projectName, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return ProcessListResult(result);
    }
    
    public async Task<bool> OffAgent(string projectName,string username)
    {
        var result = await Execute("作者", "下线代理",
            new[] { _clientSettings.UserName, projectName, username, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return (result.IndexOf("下线代理成功") != -1);
    }
    public async Task<bool> StopAgent(string projectName,string username)
    {
        var result = await Execute("作者", "停用代理",
            new[] { _clientSettings.UserName, projectName, username, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return (result.IndexOf("停用代理成功") != -1);
    }
    public async Task<bool> RecoverAgent(string projectName,string username)
    {
        var result = await Execute("作者", "恢复代理",
            new[] { _clientSettings.UserName, projectName, username, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return (result.IndexOf("恢复代理成功") != -1);
    }
    public async Task<bool> AddAgent(string projectName,string username,string password,string money)
    {
        var result = await Execute("作者", "新增代理",
            new[] { _clientSettings.UserName, projectName, username,password,money, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return (result.IndexOf("新增代理成功") != -1);
    }
    public async Task<bool> DeleteAgent(string projectName,string username)
    {
        var result = await Execute("作者", "删除代理",
            new[] { _clientSettings.UserName, projectName, username, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return (result.IndexOf("删除代理成功") != -1);
    }
    public async Task<bool> ChangeAgentMoney(string projectName,string agent,string money)
    {
        var result = await Execute("作者", "修改代理余额",
            new[] { _clientSettings.UserName, projectName, agent,money, _clientSettings.ClientLicense });
        _clientSettings.GlobalMessage = result;
        return (result.IndexOf("修改代理余额成功") != -1);
    }
    
    public async Task<ProjectCardPrice?> GetProjectCardPrice(string projectName)
    {
        try
        {
            var json = await Execute("作者", "获取项目卡密价格",
                new[] { _clientSettings.UserName, projectName, _clientSettings.ClientLicense });
            _clientSettings.GlobalMessage = json;
            Console.WriteLine($"serverData2: {json}");
            var serverData = JsonConvert.DeserializeObject<ServerCardPrice>(System.Net.WebUtility.HtmlDecode(json));
            Console.WriteLine("serverData: " + System.Net.WebUtility.HtmlDecode(json));
            
            
            
            if (serverData == null) return null;
            
            return  ProjectCardJsonConverter.ConvertFromServerFormat(serverData);
        }
        catch(Exception e)
        {
            Console.WriteLine($"[MoCi Error] {e.Message}");
            return null;
        }
    }
    public async Task<bool> ChangeProjectCardPrice(string projectName,ProjectCardPrice? config)
    {
        try
        {
            if (config == null) return false;
            var serverFormat = ProjectCardJsonConverter.ConvertToServerFormat(config);
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore, 
                DefaultValueHandling = DefaultValueHandling.Ignore 
            };
            var serializedConfig = JsonConvert.SerializeObject(serverFormat, Formatting.None, jsonSettings);
            Console.WriteLine("[send json]:" + serializedConfig.Trim());
            var json = await Execute("作者", "修改项目卡密价格",
                new[] { _clientSettings.UserName, projectName, serializedConfig, _clientSettings.ClientLicense });
            _clientSettings.GlobalMessage = json;
            return json == "修改项目卡密价格成功";
        }
        catch (Exception e)
        {
            Console.WriteLine($"[MoCi Error] {e.Message}");
            return false;
        }
        
            
    }
    

    private string[]? ProcessListResult(string result)
    {
        if (result.Contains("无项目") || result.Contains("无版本") || result.Contains("无自定义数据") ||
            result.Contains("无用户") || result.Contains("无黑名") || result.Contains("无卡密") || result.Contains("无卡密"))
        {
            _clientSettings.GlobalMessage = result;
            return null;
        }

        if (result.Contains("请求失败") || result.Contains("请求超时"))
        {
            _clientSettings.GlobalMessage = result;
            return null;
        }

        return result.Split("\n", StringSplitOptions.RemoveEmptyEntries);
    }
    
    
}