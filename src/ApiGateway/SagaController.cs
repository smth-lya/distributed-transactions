using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class SagaController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SagaController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
}
