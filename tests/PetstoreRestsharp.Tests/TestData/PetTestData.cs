using PetstoreRestsharp.Core.Models;

namespace PetstoreRestsharp.Tests.TestData;

/// <summary>
/// Factory that produces test data. Keeps the tests focused on assertions (Arrange).
/// </summary>
public static class PetTestData
{
    public static Pet CreatePet(
        long id = 0,
        string name = "Rex",
        string status = "available",
        long categoryId = 1,
        string categoryName = "Dogs",
        params string[] photoUrls) => new()
    {
        Id = id,
        Name = name,
        Status = status,
        Category = new Category { Id = categoryId, Name = categoryName },
        PhotoUrls = photoUrls.Length > 0 ? photoUrls.ToList() : new List<string> { "https://example.com/rex.jpg" },
        Tags = new List<Tag>
        {
            new() { Id = 1, Name = "friendly" }
        }
    };

    public static User CreateUser(
        long id = 10,
        string username = "john_doe",
        string password = "secret123") => new()
    {
        Id = id,
        Username = username,
        FirstName = "John",
        LastName = "Doe",
        Email = $"{username}@example.com",
        Password = password,
        Phone = "555-0100",
        UserStatus = 1
    };

    public static Order CreateOrder(
        long id = 0,
        long petId = 0,
        int quantity = 1,
        string status = "placed") => new()
    {
        Id = id,
        PetId = petId,
        Quantity = quantity,
        ShipDate = DateTime.UtcNow,
        Status = status,
        Complete = false
    };
}