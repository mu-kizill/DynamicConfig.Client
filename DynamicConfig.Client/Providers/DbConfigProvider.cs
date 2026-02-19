using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using DynamicConfig.Client.Models;

namespace DynamicConfig.Client.Providers;

public class DbConfigProvider
{
    private readonly string _connectionString;

    public DbConfigProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<ConfigItem>> GetAllActiveConfigsAsync(string applicationName)
    {
        var result = new List<ConfigItem>();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT Name, Type, Value, IsActive, ApplicationName, ModifiedAt
            FROM Configurations
            WHERE ApplicationName = @appName
            AND IsActive = 1";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@appName", applicationName);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new ConfigItem
            {
                Name = reader.GetString(0),
                Type = reader.GetString(1),
                Value = reader.GetString(2),
                IsActive = reader.GetBoolean(3),
                ApplicationName = reader.GetString(4),
                ModifiedAt = reader.GetDateTime(5)
            });
        }

        return result;
    }

    public async Task<List<ConfigItem>> GetModifiedConfigsAsync(string applicationName, DateTime lastCheck)
    {
        var result = new List<ConfigItem>();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT Name, Type, Value, IsActive, ApplicationName, ModifiedAt
            FROM Configurations
            WHERE ApplicationName = @appName
            AND ModifiedAt > @lastCheck";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@appName", applicationName);
        command.Parameters.AddWithValue("@lastCheck", lastCheck);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new ConfigItem
            {
                Name = reader.GetString(0),
                Type = reader.GetString(1),
                Value = reader.GetString(2),
                IsActive = reader.GetBoolean(3),
                ApplicationName = reader.GetString(4),
                ModifiedAt = reader.GetDateTime(5)
            });
        }

        return result;
    }
}
