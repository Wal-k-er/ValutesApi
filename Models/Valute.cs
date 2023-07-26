using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ValuteApi.Models;

public class Valute
{
    [Key]
    [JsonProperty("@ID")]
    public string Id { get; set; }
    [JsonProperty("Name")]
    public string Name { get; set; }
    [JsonProperty("CharCode")]
    public string CharCode { get; set; }
    public ICollection<Rate> Rates { get; set; }
}