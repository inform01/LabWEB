namespace Crypto.Models.Dtos.Exchanges;

public class ExchangeTableDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime RegistrationDate { get; set; }
    public int PairsCount { get; set; }
}
