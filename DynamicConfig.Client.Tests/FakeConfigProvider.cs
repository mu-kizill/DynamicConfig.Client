using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicConfig.Client.Models;
using DynamicConfig.Client.Providers;

public class FakeConfigProvider : IConfigProvider
{
    public Task<List<ConfigItem>> GetAllActiveConfigsAsync(string applicationName)
    {
        return Task.FromResult(new List<ConfigItem>
        {
            new ConfigItem
            {
                Name = "SiteName",
                Type = "string",
                Value = "soty.io",
                IsActive = true,
                ApplicationName = "TEST",
                ModifiedAt = DateTime.UtcNow
            },
            new ConfigItem
            {
                Name = "MaxItemCount",
                Type = "int",
                Value = "50",
                IsActive = true,
                ApplicationName = "TEST",
                ModifiedAt = DateTime.UtcNow
            }
        });
    }

    public Task<List<ConfigItem>> GetModifiedConfigsAsync(string applicationName, DateTime lastCheck)
    {
        return Task.FromResult(new List<ConfigItem>());
    }
}
