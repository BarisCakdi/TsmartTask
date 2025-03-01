using TsmartTask.Model;

namespace TsmartTask.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
