using System.Net;
using System.Net.Http.Json;
using CustomerEnrollment.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerEnrollment.Web.Controllers;

public class CustomersController : Controller
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _config;

    public CustomersController(IHttpClientFactory httpFactory, IConfiguration config)
    {
        _httpFactory = httpFactory;
        _config = config;
    }

    [HttpGet]
    public IActionResult Enroll()
    {
        ViewBag.AesKeyBase64 = _config["ClientCrypto:AesKeyBase64"];
        ViewBag.AesIvBase64 = _config["ClientCrypto:AesIvBase64"];
        ViewBag.ApiBaseUrl = _config["ApiBaseUrl"];
        return View(new CustomerInputModel());
    }

    [HttpGet]
    public IActionResult Enrolled(int id)
    {
        var model = new EnrollResultViewModel
        {
            CustomerId = id,
            Message = $"Customer {id} successfully enrolled."
        };
        return View(model);
    }

    [HttpGet]
    public IActionResult Search()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var http = _httpFactory.CreateClient("Api");
        var resp = await http.GetAsync($"api/customers/{id}", ct);

        if (resp.StatusCode == HttpStatusCode.NotFound)
        {
            TempData["Error"] = $"No customer found with id {id}.";
            return RedirectToAction(nameof(Search));
        }

        resp.EnsureSuccessStatusCode();
        var model = await resp.Content.ReadFromJsonAsync<CustomerView>(cancellationToken: ct);
        return View(model);
    }
}
