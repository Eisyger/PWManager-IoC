using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWManager;
    public interface IPersistenceService
{
    void SaveData(string data);
    (bool Succsess, string data) LoadData();
    string GetPath();
}