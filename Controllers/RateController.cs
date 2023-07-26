using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ValuteApi.Data;
using ValuteApi.Models;
using ValuteApi.Services;

namespace ValuteApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RateController : Controller
{
    private readonly DataContext _context;
    private readonly IBackgroundJobClient _backGroundJobClient;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly RateService _rateService;


    public RateController(DataContext context, IBackgroundJobClient backGroundJobClient, IRecurringJobManager recurringJobManager, RateService rateService)
    {
        _context = context;
        _backGroundJobClient = backGroundJobClient;
        _recurringJobManager = recurringJobManager;
        _rateService = rateService;
    }

    [HttpPost("fetch")]
    public IActionResult FetchValuteRates()
    {
        _recurringJobManager.AddOrUpdate("FetchValuteRates" , ()=>UpdateValuteRates(), Cron.Minutely);
        
        return Ok("Подгрузка выполнена успешно");
    }
    [HttpGet]
    [AutomaticRetry(Attempts = 3)]
    public async Task UpdateValuteRates()
    {
        var valuteRates = await _rateService.GetValuteRates();
        JObject obj = JObject.Parse(valuteRates);
        var dateNotParsed = obj["ValCurs"]["@Date"].ToString();
        var date = DateOnly.Parse(dateNotParsed);
        Console.WriteLine(date);
        JArray valutes = (JArray)obj["ValCurs"]["Valute"];
        foreach (var valute in valutes)
        {
            if (!_context.Valutes.Any(v => v.Id == (string)valute["@ID"]))
            {
                var newValute = new Valute
                {
                    Id = (string)valute["@ID"],
                    CharCode = (string)valute["CharCode"],
                    Name = (string)valute["Name"],
                    
                };
                _context.Valutes.Add(newValute);
                var newRate = new Rate
                {
                    Date = date,
                    Valute = newValute
                };
                _context.Rates.Add(newRate);
            }
            else
            {
                var newRate = new Rate
                {
                    Date = date,
                    Valute = _context.Valutes.FirstOrDefault(v => v.Id == (string)valute["@ID"])
                };
                _context.Rates.Add(newRate);
            }

            await _context.SaveChangesAsync();
            Console.WriteLine(valute);
            
        }
    }
}