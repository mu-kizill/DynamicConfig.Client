using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicConfig.Client.Caching;
using DynamicConfig.Client.Models;
using DynamicConfig.Client.Providers;
using System.Timers;

namespace DynamicConfig.Client;

public class ConfigurationReader
{
    private readonly string _applicationName;
    private readonly DbConfigProvider _provider;
    private readonly ConfigCache _cache = new();
    private readonly System.Timers.Timer _timer;

    private DateTime _lastCheck = DateTime.MinValue;
    private bool _initialized = false;

    public ConfigurationReader(string applicationName, string connectionString, int refreshIntervalMs)
    {
        _applicationName = applicationName;
        _provider = new DbConfigProvider(connectionString);

        // İlk yükleme
        Initialize().GetAwaiter().GetResult();

        // Arka plan kontrolü
        _timer = new System.Timers.Timer(refreshIntervalMs);
        _timer.Elapsed += async (_, __) => await Refresh();
        _timer.AutoReset = true;
        _timer.Start();
    }

    private async Task Initialize()
    {
        var configs = await _provider.GetAllActiveConfigsAsync(_applicationName);

        _cache.Set(configs);

        if (configs.Any())
            _lastCheck = configs.Max(x => x.ModifiedAt);

        _initialized = true;
    }

    private async Task Refresh()
    {
        if (!_initialized)
            return;

        try
        {
            var changed = await _provider.GetModifiedConfigsAsync(_applicationName, _lastCheck);

            foreach (var item in changed)
            {
                if (item.IsActive)
                    _cache.Update(item);

                _lastCheck = item.ModifiedAt;
            }
        }
        catch
        {
            //database gitse bile servis config kullanmaya devam edilir. cache ram'de duruyor boş olması bilinçli bir durum.
        }
    }

    public T GetValue<T>(string key)
    {
        if (!_cache.TryGet(key, out ConfigItem item))
            throw new Exception($"Config '{key}' not found");

        return (T)Convert.ChangeType(item.Value, typeof(T));
    }
}

