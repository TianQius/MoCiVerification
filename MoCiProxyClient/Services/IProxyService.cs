using System.Threading.Tasks;

namespace MoCiVerification.Services;

public interface IProxyService
{
    Task<bool> AgentLogin(string token, string username, string password);
    Task<string[]?> GetAgentCardList();
    Task<bool> DeleteCard(string card);
}