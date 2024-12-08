using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWManager
{
    public class ContextService : IContextService
    {
        private List<DataContext> _context;

        public ContextService()
        {
            _context = [];
        }
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
        public DataContext Get(string name)
        {
            return _context.FirstOrDefault(x => x.Name == name);
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