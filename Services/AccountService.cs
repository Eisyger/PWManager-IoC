using PWManager.Interfaces;
using PWManager.Model;

namespace PWManager.Services
{
    public class AccountService : IContextService
    {
        public List<AccountData>? ContextsList { get; set; } = [];
        public string User { get; set; }

        public void Add(AccountData @new)
        {
            ContextsList?.Add(@new);
        }
        public void Remove(string name)
        {
             ContextsList?.RemoveAll(x => x.Name == name);
        }
        public void Edit(string name, AccountData updated)
        {
            if (ContextsList == null) return;
            var index = ContextsList.FindIndex(x => x.Name == name);
            if (index >= 0)
            {
                ContextsList[index] = updated;
            }
        }
        public IDataContext GetContext(string name)
        {
            return ContextsList?.FirstOrDefault(x => x.Name == name) ?? throw new ArgumentNullException();
        }
    }
}