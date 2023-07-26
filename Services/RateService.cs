using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ValuteApi.Models;

namespace ValuteApi.Services;

public class RateService
{
    private readonly HttpClient _httpClient;

    public RateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetValuteRates()
    {
        var response = await _httpClient.GetAsync("https://cbr.ru/scripts/XML_daily.asp");
        var valuteRates =  response.Content.ReadAsStringAsync().Result;
        XElement xmlResult = XElement.Parse(valuteRates);

        string result = JsonConvert.SerializeXNode(xmlResult);
        
        return result;
    }
}