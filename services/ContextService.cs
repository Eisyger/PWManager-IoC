using PWManager.interfaces;
using PWManager.model;

namespace PWManager.services
{
    public class ContextService : IContextService
    {
        public List<DataContext>? ContextsList { get; set; } = [];

        public void Add(DataContext newContext)
        {
            ContextsList?.Add(newContext);
        }
        public void Remove(string name)
        {
             ContextsList?.RemoveAll(x => x.Name == name);
        }
        public void Edit(string name, DataContext updatedContext)
        {
            if (ContextsList == null) return;
            var index = ContextsList.FindIndex(x => x.Name == name);
            if (index >= 0)
            {
                ContextsList[index] = updatedContext;
            }
        }
        public IDataContext GetContext(string name)
        {
            return ContextsList?.FirstOrDefault(x => x.Name == name) ?? throw new ArgumentNullException();
        }
    }
}