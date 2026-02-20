using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicConfig.Client.Models;

namespace DynamicConfig.Client.Providers
{
    public interface IConfigProvider
    {
        Task<List<ConfigItem>> GetAllActiveConfigsAsync(string applicationName);
        Task<List<ConfigItem>> GetModifiedConfigsAsync(string applicationName, DateTime lastCheck);
    }
}
