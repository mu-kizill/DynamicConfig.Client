using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ConfigAdmin.Mvc.Models;

namespace ConfigAdmin.Mvc.Controllers;

public class ConfigurationsController : Controller
{
    private readonly IHttpClientFactory _factory;
    private readonly IConfiguration _config;

    public ConfigurationsController(IHttpClientFactory factory, IConfiguration config)
    {
        _factory = factory;
        _config = config;
    }

    public async Task<IActionResult> Index()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync(_config["ApiBaseUrl"]);

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<ConfigViewModel>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return View(data);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ConfigViewModel model)
    {
        var client = _factory.CreateClient();

        var content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(model),
            System.Text.Encoding.UTF8,
            "application/json");

        await client.PostAsync(_config["ApiBaseUrl"], content);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"{_config["ApiBaseUrl"]}/{id}");

        var json = await response.Content.ReadAsStringAsync();
        var model = System.Text.Json.JsonSerializer.Deserialize<ConfigViewModel>(json,
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ConfigViewModel model)
    {
        var client = _factory.CreateClient();

        var content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(model),
            System.Text.Encoding.UTF8,
            "application/json");

        await client.PutAsync($"{_config["ApiBaseUrl"]}/{model.Id}", content);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var client = _factory.CreateClient();
        await client.DeleteAsync($"{_config["ApiBaseUrl"]}/{id}");

        return RedirectToAction(nameof(Index));
    }
}