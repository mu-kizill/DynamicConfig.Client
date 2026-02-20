using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfig.Client
{
    public interface IConfigurationReader
    {
        T GetValue<T>(string key);
    }
}
