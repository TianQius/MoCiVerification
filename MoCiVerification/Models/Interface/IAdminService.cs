using System.Threading.Tasks;

namespace MoCiVerification.Models;

public interface IAdminService
{
    Task<string> GetAccountCountAsync();
    Task<string> GetReceivedByteAsync();
    Task<string> GetSendedByteAsync();
    Task<string> GetProcessedByteAsync();
    Task<(bool Success, string Message, string? License)> LoginAsync(string username, string password);
    Task<bool> LoginVerificationAsync();

    Task<(bool Success, bool IsActive)> RegisterAsync(string username, string password, string email);

    Task<bool> ActiveEmailAsync(string username, string email);
    // Project
    Task<string[]?> GetProjectListAsync();
    Task<bool> AddProjectAsync(string projectName, string projectType, string announcement, string projectKey);
    Task<bool> StopProjectAsync(string projectName);
    Task<bool> RecoverProjectAsync(string projectName);
    Task<bool> DeleteProjectAsync(string projectName);
    
    Task<bool> ChangeProjectAnnouncementAsync(string projectName, string newAnnouncement);

    // Version
    Task<string[]?> GetVersionListAsync(string projectName);
    Task<bool> DeleteVersion(string projectName, string version);
    Task<bool> StopVersion(string projectName, string version);
    Task<bool> RecoverVersion(string projectName, string version);
    Task<bool> ChangeVersionAnnouncement(string projectName, string version, string newAnnouncement);
    Task<bool> ChangeVersionData(string projectName, string version, string newData);

    // Variable
    Task<string[]?> GetVariableListAsync(string projectName); 
    Task<bool> DeleteVar(string projectName, string var);
    Task<bool> GetVarOption(string projectName);
    Task<bool> SetVarOption(string projectName, bool use);

    // Others
    Task<string[]?> GetUserListAsync(string projectName);
    Task<bool> CreateVersion(string projectName, string version, string announcement, string data, string state);

    Task<bool> DeleteUser(string projectName, string user);
    Task<bool> StopUser(string projectName, string user);
    Task<bool> RecoverUser(string projectName, string user);
    Task<bool> OffUser(string projectName, string user);
    Task<bool> UnBindUser(string projectName, string user);
    Task<bool> ClearUserBindTimes(string projectName, string user);
    Task<bool> ChangeBindTimes(string projectName, string count);
    
    Task<string[]?> GetBlackerListAsync(string projectName);
    Task<bool> CreateBlacker(string projectname, string value, string reason);
    Task<bool> DeleteBlacker(string projectname,string blacker);
    Task<string[]?> GetDataListAsync(string projectName);
    Task<bool> DeleteCustomData(string projectname, string key);
    Task<bool> CreateCustomData(string projectname, string key, string value, string mark, string getway);
    Task<bool> ChangeCustomDataMark(string projectname, string key, string mark);
    Task<bool> ChangeCustomDataGetWay(string projectname, string key, bool direct);
    Task<bool> ChangeCustomDataValue(string projectname, string key, string value);
    Task<string[]?> GetCardListAsync(string projectName);
    Task<bool> DeleteCard(string projectName, string card);
    Task<bool> CreateCard(string projectName, int count, string type, string prefix, string mask);
    Task<string[]?> GetAgentListAsync(string projectName);
    Task<bool> OffAgent(string projectName, string username);
    Task<bool> StopAgent(string projectName, string username);
    Task<bool> RecoverAgent(string projectName, string username);
    Task<bool> DeleteAgent(string projectName, string username);
    Task<bool> AddAgent(string projectName, string username, string password, string money);
    Task<bool> ChangeAgentMoney(string projectName, string agent, string money);
}