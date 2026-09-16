using SalonFlow.Domain.Services;

namespace SalonFlow.UnitTests.Services;

public sealed class SalonServiceTests
{
    [Fact]
    public void CreateCreatesActiveSalonService()
    {
        var salonId = Guid.NewGuid();

        var service = SalonService.Create(
            salonId,
            "  Deluxe Manicure  ",
            TimeSpan.FromMinutes(45),
            35m);

        Assert.NotEqual(Guid.Empty, service.Id);
        Assert.Equal(salonId, service.SalonId);
        Assert.Equal("Deluxe Manicure", service.Name);
        Assert.Equal(TimeSpan.FromMinutes(45), service.Duration);
        Assert.Equal(35m, service.Price);
        Assert.True(service.IsActive);
    }

    [Fact]
    public void CreateThrowsWhenSalonIdIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            SalonService.Create(
                Guid.Empty,
                "Manicure",
                TimeSpan.FromMinutes(30),
                25m));
    }

    [Fact]
    public void CreateThrowsWhenNameIsBlank()
    {
        Assert.Throws<ArgumentException>(() =>
            SalonService.Create(
                Guid.NewGuid(),
                "   ",
                TimeSpan.FromMinutes(30),
                25m));
    }

    [Fact]
    public void CreateThrowsWhenDurationIsNotPositive()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SalonService.Create(
                Guid.NewGuid(),
                "Manicure",
                TimeSpan.Zero,
                25m));
    }

    [Fact]
    public void CreateThrowsWhenPriceIsNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SalonService.Create(
                Guid.NewGuid(),
                "Manicure",
                TimeSpan.FromMinutes(30),
                -1m));
    }

    [Fact]
    public void DeactivateMakesSalonServiceInactive()
    {
        var service = SalonService.Create(
            Guid.NewGuid(),
            "Manicure",
            TimeSpan.FromMinutes(30),
            25m);

        service.Deactivate();

        Assert.False(service.IsActive);
    }

    [Fact]
    public void ActivateMakesSalonServiceActiveAgain()
    {
        var service = SalonService.Create(
            Guid.NewGuid(),
            "Manicure",
            TimeSpan.FromMinutes(30),
            25m);

        service.Deactivate();
        service.Activate();

        Assert.True(service.IsActive);
    }
}
