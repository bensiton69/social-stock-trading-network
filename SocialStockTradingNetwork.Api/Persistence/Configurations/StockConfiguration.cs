using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialStockTradingNetwork.Api.Entities;

namespace SocialStockTradingNetwork.Api.Persistence.Configurations;

internal sealed class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable("stocks");

        builder.HasKey(stock => stock.Id);

        builder.Property(stock => stock.Symbol)
            .IsRequired()
            .HasMaxLength(16);

        builder.HasIndex(stock => stock.Symbol)
            .IsUnique();

        builder.Property(stock => stock.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(stock => stock.CurrentPrice)
            .HasPrecision(18, 4);

        builder.Property(stock => stock.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(stock => stock.IsTradable)
            .IsRequired();

        builder.Property(stock => stock.UpdatedAt)
            .IsRequired()
            .IsConcurrencyToken();
    }
}
