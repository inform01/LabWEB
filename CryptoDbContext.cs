using Crypto.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Crypto;

public class CryptoDbContext : DbContext
{
    public CryptoDbContext()
    {}

    public CryptoDbContext(DbContextOptions<CryptoDbContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<Exchange> Exchanges { get; set; }
        
    public DbSet<Pair> Pairs { get; set; }
    
    public DbSet<ExchangePair> ExchangePairs { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<Customer> Customers { get; set; }
    
    public DbSet<CryptoCurrency> Currencies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity => {
            entity.Property(x => x.Name)
                .IsRequired();
            entity.Property(x => x.IdentityUserId)
                .IsRequired();
            entity.HasIndex(x => x.IdentityUserId)
                .IsUnique();
        });
        
        modelBuilder.Entity<CryptoCurrency>(entity => {
            entity.Property(x => x.Name)
                .IsRequired();
            entity.HasIndex(x => x.Name)
                .IsUnique();
        });
        
        modelBuilder.Entity<Exchange>(entity => {
            entity.HasMany(x => x.Pairs)
                .WithMany(x => x.Exchanges)
                .UsingEntity<ExchangePair>(
                    right => right.HasOne(x => x.Pair)
                        .WithMany()
                        .HasForeignKey(x => x.PairId)
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne(x => x.Exchange)
                        .WithMany(x => x.ExchangePairs)
                        .HasForeignKey(x => x.ExchangeId)
                        .OnDelete(DeleteBehavior.Cascade));
        });
        
        modelBuilder.Entity<Pair>(entity => {
            entity.HasOne(x => x.FirstCrypto)
                .WithMany()
                .HasForeignKey(x => x.FirstCryptoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.SecondCrypto)
                .WithMany()
                .HasForeignKey(x => x.SecondCryptoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => new { x.FirstCryptoId, x.SecondCryptoId })
                .IsUnique();
        });
        
        modelBuilder.Entity<Order>(entity => {
            entity.HasOne(x => x.ExchangePair)
                .WithMany()
                .HasForeignKey(x => x.ExchangePairId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Customer)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
