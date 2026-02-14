using Bogus;
using AlertHub.Domain.Alert;
using DomainAlert = AlertHub.Domain.Alert.Alert;

namespace AlertHub.Tests.Domain.Alert;

internal static class AlertFaker
{
    private static readonly Faker Faker;

    static AlertFaker()
    {
        Randomizer.Seed = new Random(12026);
        Faker = new Faker();
    }

    public static DomainAlert CreateAlert(
        AlertMessageType messageType = AlertMessageType.Alert,
        AlertScope scope = AlertScope.Public,
        AlertStatus status = AlertStatus.Actual)
    {
        return DomainAlert.Create(
            identifier: NextIdentifier(),
            sender: NextSender(),
            sent: Faker.Date.RecentOffset(),
            status: status,
            messageType: messageType,
            scope: scope);
    }

    public static string NextEvent() => $"event-{Faker.Random.AlphaNumeric(8)}";

    public static string NextAreaDescription() => Faker.Lorem.Sentence(3);

    public static string NextResourceDescription() => Faker.Lorem.Word();

    private static string NextIdentifier() => $"id-{Faker.Random.AlphaNumeric(12)}";

    private static string NextSender()
    {
        var sender = Faker.Internet.Email();
        return sender
            .Replace(" ", string.Empty)
            .Replace(",", string.Empty)
            .Replace("<", string.Empty)
            .Replace("&", string.Empty);
    }
}
