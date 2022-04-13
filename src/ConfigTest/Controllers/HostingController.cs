using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ConfigTest.Controllers;

[Route("api/[controller]")]
public class HostingController : ControllerBase
{
    private readonly IWebHostEnvironment _hostingEnvironment;

    public HostingController(IWebHostEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    [HttpGet]
    public IWebHostEnvironment Get()
    {
        return _hostingEnvironment;
    }
}