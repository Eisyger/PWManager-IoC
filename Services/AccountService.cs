using System.Data;
using PWManager.Interfaces;
using PWManager.Model;

namespace PWManager.Services
{
    public class AccountService : IContextService
    {
        public List<AccountData>? AccountList { get; set; } = [];
        public string User { get; set; } = string.Empty;

        public void Add(AccountData newAcc)
        {
            if (AccountList != null && AccountList.Any(x => x.Name == newAcc.Name))
            {
                throw new DuplicateNameException($"Der Accountname ist schon vorhanden.");
            }
            AccountList?.Add(newAcc);
        }
        public void Remove(string name)
        {
            if (AccountList == null) return;
            if (AccountList != null && AccountList.All(x => x.Name != name))
            {
                throw new NullReferenceException($"Der Account konnte nicht gefunden werden.");
            }
            AccountList?.RemoveAll(x => x.Name == name);
        }
        public void Edit(string name, AccountData updated)
        {
            if (AccountList == null) return;
            var index = AccountList.FindIndex(x => x.Name == name);
            if (index >= 0)
            {
                AccountList[index] = updated;
            }
        }
        public IDataContext GetContext(string name)
        {
            return AccountList?.FirstOrDefault(x => x.Name == name) ?? throw new ArgumentNullException();
        }
    }
}