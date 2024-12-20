namespace PWManager
{
    public interface IContextService
    {
        void Add(DataContext newContext);
        void Remove(string name);
        void Edit(string name, DataContext updatedContext);
        DataContext Get(string name);
        List<DataContext> GetAll();
        void SetAll(List<DataContext> newContext);
    }
}
