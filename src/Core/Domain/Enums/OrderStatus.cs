using Domain.Common.Enumerations;

namespace Domain.Enums;

/// <summary>
/// Sipariş durumlarını temsil eden smart enum
/// </summary>
public class OrderStatus : Enumeration
{
    /// <summary>
    /// Sipariş oluşturuldu
    /// </summary>
    public static readonly OrderStatus Created = new(1, nameof(Created));
    
    /// <summary>
    /// Ödeme bekleniyor
    /// </summary>
    public static readonly OrderStatus WaitingForPayment = new(2, nameof(WaitingForPayment));
    
    /// <summary>
    /// Ödeme alındı
    /// </summary>
    public static readonly OrderStatus PaymentReceived = new(3, nameof(PaymentReceived));
    
    /// <summary>
    /// Sipariş hazırlanıyor
    /// </summary>
    public static readonly OrderStatus Processing = new(4, nameof(Processing));
    
    /// <summary>
    /// Kargoya verildi
    /// </summary>
    public static readonly OrderStatus Shipped = new(5, nameof(Shipped));
    
    /// <summary>
    /// Teslim edildi
    /// </summary>
    public static readonly OrderStatus Delivered = new(6, nameof(Delivered));
    
    /// <summary>
    /// İptal edildi
    /// </summary>
    public static readonly OrderStatus Cancelled = new(7, nameof(Cancelled));

    /// <summary>
    /// Constructor
    /// </summary>
    private OrderStatus(int id, string name) : base(id, name) { }

    /// <summary>
    /// Bir sonraki duruma geçiş yapılabilir mi?
    /// </summary>
    public bool CanTransitionTo(OrderStatus nextStatus)
    {
        return nextStatus.Id > this.Id && nextStatus != Cancelled;
    }

    /// <summary>
    /// Sipariş iptal edilebilir mi?
    /// </summary>
    public bool CanBeCancelled()
    {
        return this != Shipped && this != Delivered && this != Cancelled;
    }

    /// <summary>
    /// İnsan dostu durum açıklaması
    /// </summary>
    public string GetFriendlyDescription()
    {
        return this switch
        {
            _ when this == Created => "Sipariş oluşturuldu, ödeme bekleniyor",
            _ when this == WaitingForPayment => "Ödeme bekleniyor",
            _ when this == PaymentReceived => "Ödeme alındı, sipariş hazırlanacak",
            _ when this == Processing => "Sipariş hazırlanıyor",
            _ when this == Shipped => "Kargoya verildi",
            _ when this == Delivered => "Teslim edildi",
            _ when this == Cancelled => "İptal edildi",
            _ => throw new ArgumentException($"Geçersiz sipariş durumu: {Name}")
        };
    }
}