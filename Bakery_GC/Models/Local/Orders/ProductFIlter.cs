using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;
using System;

/// Pseudocode / Plan
/// - Introduce PriceRange struct with nullable Min / Max, validation (Min <= Max if both set)
/// - Add backing fields _minPrice, _maxPrice, _range
/// - Setting MinPrice or MaxPrice updates _range (if both null -> range = Empty)
/// - Setting Range updates MinPrice / MaxPrice (must have at least one non-null if explicitly set)
/// - Provide HasRange boolean helper
/// - Remove invalid decimal indexer
/// - Preserve existing filter properties

public readonly record struct PriceRange
{
    public decimal? Min { get; }
    public decimal? Max { get; }

    public bool HasMin => Min.HasValue;
    public bool HasMax => Max.HasValue;
    public bool IsEmpty => !HasMin && !HasMax;

    public PriceRange(decimal? min, decimal? max)
    {
        if (min.HasValue && max.HasValue && min > max)
            throw new ArgumentException("Min cannot be greater than Max.", nameof(min));
        Min = min;
        Max = max;
    }

    public static PriceRange Empty => new(null, null);

    public override string ToString() =>
        IsEmpty ? "(empty)" :
        HasMin && HasMax ? $"[{Min} - {Max}]" :
        HasMin ? $">= {Min}" : $"<= {Max}";
}

public class ProductFilter
{
    public ProductType? ProductType { get; set; }
    public PizzaType? PizzaType { get; set; }
    public BreadType? BreadType { get; set; }
    public CakeType? CakeType { get; set; }
    public PastryType? PastryType { get; set; }

    private decimal? _minPrice;
    private decimal? _maxPrice;
    private PriceRange _range = PriceRange.Empty;

    /// <summary>
    /// Minimum price (nullable). Updating this keeps Range in sync.
    /// </summary>
    public decimal? MinPrice
    {
        get => _minPrice;
        set
        {
            _minPrice = value;
            SyncRangeFromPrices();
        }
    }

    /// <summary>
    /// Maximum price (nullable). Updating this keeps Range in sync.
    /// </summary>
    public decimal? MaxPrice
    {
        get => _maxPrice;
        set
        {
            _maxPrice = value;
            SyncRangeFromPrices();
        }
    }

    /// <summary>
    /// Combined price range. At least one of Min or Max must be non-null if you set this explicitly.
    /// Setting this updates MinPrice / MaxPrice.
    /// </summary>
    public PriceRange Range
    {
        get => _range;
        set
        {
            if (value.IsEmpty)
                throw new ArgumentException("At least one between Min and Max must be provided.", nameof(Range));

            _range = value;
            _minPrice = value.Min;
            _maxPrice = value.Max;
        }
    }

    /// <summary>
    /// True if at least one between MinPrice and MaxPrice is set.
    /// </summary>
    public bool HasRange => _minPrice.HasValue || _maxPrice.HasValue;

    private void SyncRangeFromPrices()
    {
        if (!_minPrice.HasValue && !_maxPrice.HasValue)
        {
            _range = PriceRange.Empty;
            return;
        }

        if (_minPrice.HasValue && _maxPrice.HasValue && _minPrice > _maxPrice)
            throw new InvalidOperationException("MinPrice cannot be greater than MaxPrice.");

        _range = new PriceRange(_minPrice, _maxPrice);
    }

    /// <summary>
    /// Search the market by its key.
    /// </summary>
    public Guid? MarketId { get; set; }

    /// <summary>
    /// Search the market by its name.
    /// </summary>
    public string? NameMarket { get; set; }

    /// <summary>
    /// Whether the product is currently available in the specified market.
    /// </summary>
    public bool? IsAvailable { get; set; }

    /// <summary>
    /// Works as a search term for product name or description.
    /// </summary>
    public string? SearchTerm { get; set; }
}