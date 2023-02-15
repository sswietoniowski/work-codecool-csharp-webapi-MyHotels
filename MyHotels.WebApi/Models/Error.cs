using Newtonsoft.Json;

namespace MyHotels.WebApi.Models;

public class Error
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public override string ToString() => JsonConvert.SerializeObject(this);
}