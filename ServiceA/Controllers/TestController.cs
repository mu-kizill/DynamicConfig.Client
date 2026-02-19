using DynamicConfig.Client;
using Microsoft.AspNetCore.Mvc;

namespace ServiceA.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private static ConfigurationReader _reader;

    public TestController(IConfiguration configuration)
    {
        _reader ??= new ConfigurationReader(
            "SERVICE-A",
            configuration.GetConnectionString("ConfigDb"),
            5000);
    }

    [HttpGet]
    public string Get()
    {
        return _reader.GetValue<string>("SiteName");
    }
}
