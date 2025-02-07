using Domain.Common.Enumerations;

namespace Domain.Enums;

/// <summary>
/// Üyelik tiplerini temsil eden smart enum
/// </summary>
public class MembershipType : Enumeration
{
    public static readonly MembershipType Free = new(1, nameof(Free), 0, 0);
    public static readonly MembershipType Basic = new(2, nameof(Basic), 10, 5);
    public static readonly MembershipType Premium = new(3, nameof(Premium), 20, 15);
    public static readonly MembershipType VIP = new(4, nameof(VIP), 30, 25);

    /// <summary>
    /// İndirim yüzdesi
    /// </summary>
    public decimal DiscountPercentage { get; private set; }

    /// <summary>
    /// Kazanılan puan yüzdesi
    /// </summary>
    public decimal EarnedPointsPercentage { get; private set; }

    private MembershipType(int id, string name, decimal discountPercentage, decimal earnedPointsPercentage) 
        : base(id, name)
    {
        DiscountPercentage = discountPercentage;
        EarnedPointsPercentage = earnedPointsPercentage;
    }

    /// <summary>
    /// İndirim tutarını hesaplar
    /// </summary>
    public decimal CalculateDiscount(decimal amount)
    {
        return Math.Round(amount * (DiscountPercentage / 100), 2);
    }

    /// <summary>
    /// Kazanılacak puanı hesaplar
    /// </summary>
    public decimal CalculateEarnedPoints(decimal amount)
    {
        return Math.Round(amount * (EarnedPointsPercentage / 100), 0);
    }

    /// <summary>
    /// Bir üst seviyeye yükseltilebilir mi?
    /// </summary>
    public bool CanUpgrade()
    {
        return this != VIP;
    }

    /// <summary>
    /// Bir alt seviyeye düşürülebilir mi?
    /// </summary>
    public bool CanDowngrade()
    {
        return this != Free;
    }
}