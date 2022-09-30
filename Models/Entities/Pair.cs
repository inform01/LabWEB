namespace Crypto.Models.Entities;

public class Pair : BaseEntity
{ 
    public int FirstCryptoId { get; set; }
    public CryptoCurrency FirstCrypto { get; set; }
    
    public int SecondCryptoId { get; set; }
    public CryptoCurrency SecondCrypto { get; set; }

    public ICollection<Exchange> Exchanges { get; set; } = new List<Exchange>();
}
