using PWManager.Interfaces;
using PWManager.Model;

namespace PWManager.Services;

public class AppKeyService : IAppKeyService
{
    public AppKey Key { get; set; } = new AppKey(string.Empty);
}