using PWManager.model;

namespace PWManager.interfaces
{
    public interface IContextService
    {
        void Add(DataContext newContext);
        void Remove(string name);
        void Edit(string name, DataContext updatedContext);
        IDataContext GetContext(string name);
        List<DataContext> GetAll();
        void SetAll(List<DataContext> newContext);
    }
}
