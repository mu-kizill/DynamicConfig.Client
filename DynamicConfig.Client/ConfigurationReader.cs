using DynamicConfig.Client.Caching;
using DynamicConfig.Client.Models;
using DynamicConfig.Client.Providers;
using System.Collections.Concurrent;
using System.Threading;

namespace DynamicConfig.Client;

public class ConfigurationReader : IConfigurationReader, IDisposable
{
    private readonly string _applicationName;
    private readonly DbConfigProvider _provider;
    private readonly ConfigCache _cache = new();

    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cts = new();

    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    // Yeni eklenen alan: İlk yükleme tamamlandığında sinyal verecek
    private readonly TaskCompletionSource<bool> _ready =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private DateTime _lastCheck = DateTime.MinValue;
    private Task? _backgroundTask;

    public ConfigurationReader(
        string applicationName,
        string connectionString,
        int refreshIntervalMs)
    {
        _applicationName = applicationName;
        _provider = new DbConfigProvider(connectionString);

        _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(refreshIntervalMs));

        // Background worker başlat
        _backgroundTask = Task.Run(StartAsync);
    }

    private async Task StartAsync()
    {
        // İlk yükleme
        await InitializeAsync();

        // Periodic loop
        while (await _timer.WaitForNextTickAsync(_cts.Token))
        {
            await RefreshAsync();
        }
    }

    private async Task InitializeAsync()
    {
        try
        {
            var configs = await _provider.GetAllActiveConfigsAsync(_applicationName);

            _cache.Set(configs);

            if (configs.Any())
                _lastCheck = configs.Max(x => x.ModifiedAt);
        }
        catch
        {
            // database gitse bile servis config kullanmaya devam edilir. 
            // cache ram'de duruyor boş olması bilinçli bir durum.
        }
        finally
        {
            // Yükleme başarılı olsa da olmasa da "hazırım" sinyali veriliyor 
            // ki uygulama kilitlenip kalmasın.
            _ready.TrySetResult(true);
        }
    }

    private async Task RefreshAsync()
    {
        if (!_refreshLock.Wait(0))
            return; // aynı anda ikinci refresh çalışmasın

        try
        {
            var changed =
                await _provider.GetModifiedConfigsAsync(_applicationName, _lastCheck);

            foreach (var item in changed)
            {
                if (item.IsActive)
                    _cache.Update(item);

                _lastCheck = item.ModifiedAt;
            }
        }
        catch
        {
            // DB down → cache ile devam
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    // Güncellenen GetValue metodu
    public T GetValue<T>(string key)
    {
        // İlk yükleme bitene kadar burada bekler (Thread-safe blocking)
        _ready.Task.GetAwaiter().GetResult();

        if (!_cache.TryGet(key, out ConfigItem item))
            throw new KeyNotFoundException($"Config '{key}' not found.");

        try
        {
            return (T)Convert.ChangeType(item.Value, typeof(T));
        }
        catch
        {
            throw new InvalidCastException(
                $"Config '{key}' cannot be converted to {typeof(T).Name}");
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _timer.Dispose();
        _refreshLock.Dispose();
        _cts.Dispose();
    }
}