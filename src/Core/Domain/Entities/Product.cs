using Domain.Common.Abstractions;
using Domain.Common.Guards;

namespace Domain.Entities;


/// <summary>
/// Guard kullanım örneği - Product Entity
/// </summary>
public class Product : BaseAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public string SKU { get; private set; }
    // public ProductCategory Category { get; private set; }
    
    private Product() { } // EF Core için

    /// <summary>
    /// Yeni ürün oluşturur
    /// </summary>
    public static Product Create(
        Guid id,
        string name,
        string description,
        decimal price,
        int stockQuantity,
        string sku,
        // ProductCategory category,
        string createdBy)
    {
        // Guard kontrolleri
        Guard.AgainstDefaultGuid(id, nameof(id));
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Guard.AgainstExceedingLength(name, 100, nameof(name));
        Guard.AgainstNull(description, nameof(description));
        Guard.AgainstExceedingLength(description, 500, nameof(description));
        Guard.AgainstNegative(price, nameof(price));
        Guard.AgainstNegative(stockQuantity, nameof(stockQuantity));
        Guard.AgainstNullOrEmpty(sku, nameof(sku));
        // Guard.AgainstNull(category, nameof(category));

        // SKU format kontrolü
        Guard.AgainstBusinessRule(
            () => sku.Length == 8 && sku.All(char.IsLetterOrDigit),
            "InvalidSKUFormat",
            "SKU must be 8 characters long and contain only letters and numbers");

        var product = new Product
        {
            Id = id,
            Name = name,
            Description = description,
            Price = price,
            StockQuantity = stockQuantity,
            SKU = sku.ToUpperInvariant(),
            // Category = category
        };

        // product.AddDomainEvent(new ProductCreatedDomainEvent(id, sku, createdBy));
        return product;
    }

    /// <summary>
    /// Ürün fiyatını günceller
    /// </summary>
    public void UpdatePrice(decimal newPrice, string updatedBy)
    {
        Guard.AgainstNegative(newPrice, nameof(newPrice));
        
        // Maksimum fiyat artış kontrolü
        Guard.AgainstBusinessRule(
            () => newPrice <= Price * 2,
            "PriceIncreaseTooHigh",
            "Price cannot be increased by more than 100% at once");

        var oldPrice = Price;
        Price = newPrice;

        // AddDomainEvent(new ProductPriceChangedDomainEvent(Id, oldPrice, newPrice, updatedBy));
        RegisterChange(updatedBy);
    }

    /// <summary>
    /// Stok miktarını günceller
    /// </summary>
    public void UpdateStock(int quantity, string updatedBy)
    {
        Guard.AgainstNegative(quantity, nameof(quantity));
        
        // Maksimum stok kontrolü
        Guard.AgainstBusinessRule(
            () => quantity <= 1000,
            "StockLimitExceeded",
            "Stock quantity cannot exceed 1000 units");

        StockQuantity = quantity;
        RegisterChange(updatedBy);
    }

    /// <summary>
    /// Stoktan ürün düşer
    /// </summary>
    public void RemoveFromStock(int quantity, string updatedBy)
    {
        Guard.AgainstNegative(quantity, nameof(quantity));
        Guard.AgainstBusinessRule(
            () => StockQuantity >= quantity,
            "InsufficientStock",
            $"Cannot remove {quantity} units from stock. Current stock: {StockQuantity}");

        StockQuantity -= quantity;
        RegisterChange(updatedBy);
    }
}
