using System.ComponentModel.DataAnnotations.Schema;

namespace MyHotels.WebApi.Domain;

public class Hotel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public double Rating { get; set; }
    public Country Country { get; set; }
    [ForeignKey(nameof(Country))]
    public int CountryId { get; set; }
}