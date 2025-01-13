using PWManager.model;

namespace PWManager.interfaces
{
    public interface IContextService
    {
        public List<DataContext>? ContextsList { get; set; }
        
        void Add(DataContext newContext);
        void Remove(string name);
        void Edit(string name, DataContext updatedContext);
        IDataContext GetContext(string name);
    }
}
