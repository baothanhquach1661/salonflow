namespace SalonFlow.Domain.Services;

public sealed class SalonService
{
    private SalonService(
        Guid id,
        Guid salonId,
        string name,
        TimeSpan duration,
        decimal price)
    {
        Id = id;
        SalonId = salonId;
        Name = name;
        Duration = duration;
        Price = price;
        IsActive = true;
    }

    public Guid Id { get; private set; }

    public Guid SalonId { get; private set; }

    public string Name { get; private set; }

    public TimeSpan Duration { get; private set; }

    public decimal Price { get; private set; }

    public bool IsActive { get; private set; }

    public static SalonService Create(
        Guid salonId,
        string name,
        TimeSpan duration,
        decimal price)
    {
        if (salonId == Guid.Empty)
        {
            throw new ArgumentException(
                "Salon ID is required.",
                nameof(salonId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Service name is required.",
                nameof(name));
        }

        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(duration),
                duration,
                "Duration must be greater than zero.");
        }

        if (price < decimal.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(price),
                price,
                "Price cannot be negative.");
        }

        return new SalonService(
            Guid.NewGuid(),
            salonId,
            name.Trim(),
            duration,
            price);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
