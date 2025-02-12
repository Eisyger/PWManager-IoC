using PWManager.Model;

namespace PWManager.Interfaces
{
    public interface IContextService
    {
        public List<AccountData>? AccountList { get; set; }
        public string User {get;set;}
        
        void Add(AccountData @new);
        void Remove(string name);
        void Edit(string name, AccountData updated);
        IDataContext GetContext(string name);
    }
}
