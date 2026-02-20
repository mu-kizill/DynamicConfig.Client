using DynamicConfig.Client;
using Microsoft.AspNetCore.Mvc;

namespace ServiceA.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{    
    private readonly IConfigurationReader _reader;
    
    public TestController(IConfigurationReader reader)
    {
        _reader = reader;
    }

    [HttpGet]
    public string Get()
    {        
        return _reader.GetValue<string>("SiteName");
    }


}