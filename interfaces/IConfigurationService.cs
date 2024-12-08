using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWManager
{
    public interface IConfigurationService
    {
       void SendData(string endpoint, string data);
        string ReceiveData(string endpoint); 
    }
}