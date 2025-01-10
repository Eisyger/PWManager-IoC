using PWManager.interfaces;
using PWManager.model;

namespace PWManager.services
{
    public class ContextService : IContextService
    {
        private List<DataContext> _context = [];

        public void Add(DataContext newContext)
        {
            _context.Add(newContext);
        }
        public void Remove(string name)
        {
             _context.RemoveAll(x => x.Name == name);
        }

        public void Edit(string name, DataContext updatedContext)
        {
             var index = _context.FindIndex(x => x.Name == name);
            if (index >= 0)
            {
                _context[index] = updatedContext;
            }        
        }
        public IDataContext GetContext(string name)
        {
            return _context.FirstOrDefault(x => x.Name == name) ?? throw new ArgumentNullException();
        }
        public List<DataContext> GetAll()
        {
            return _context;
        }

        public void SetAll(List<DataContext> newContext)
        {
            _context = newContext;
        }
    }
}