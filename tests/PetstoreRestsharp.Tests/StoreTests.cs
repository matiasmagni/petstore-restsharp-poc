using System.Net;
using FluentAssertions;
using PetstoreRestsharp.Core.Models;
using PetstoreRestsharp.Tests.TestData;

namespace PetstoreRestsharp.Tests;

[TestFixture]
public class StoreTests : TestFixtureBase
{
    private static long UniqueIdLong()
        => Math.Abs(BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0));

    // ---------- GET /store/inventory ----------

    [Test]
    public async Task Inventory_Returns200WithStatusMap()
    {
        // Arrange

        // Act
        var response = await Store.GetInventoryAsync();

        // Assert
        response.Should().BeSuccessful();
        var inventory = response.Should().DeserializeAs<Dictionary<string, int>>().Which;
        inventory.Should().ContainKey("available");
        inventory.Should().ContainKey("pending");
        inventory.Should().ContainKey("sold");
    }

    // ---------- POST /store/order ----------

    [Test]
    public async Task PlaceOrder_WithValidOrder_ReturnsOrderWithId()
    {
        // Arrange
        var order = PetTestData.CreateOrder(id: UniqueIdLong(), petId: UniqueIdLong());

        // Act
        var response = await Store.PlaceOrderAsync(order);

        // Assert
        response.Should().BeSuccessful();
        var created = response.Should().DeserializeAs<Order>().Which;
        created.Id.Should().Be(order.Id, "the API echoes the submitted id");
        created.Status.Should().Be("placed");

        // Cleanup
        await Store.DeleteOrderAsync(order.Id);
    }

    // ---------- GET /store/order/{id} ----------

    [Test]
    public async Task GetOrderById_ExistingOrder_ReturnsMatchingOrder()
    {
        // Arrange
        var order = PetTestData.CreateOrder(id: UniqueIdLong(), petId: UniqueIdLong());
        await Store.PlaceOrderAsync(order);

        // Act
        var response = await Store.GetOrderByIdAsync(order.Id);

        // Assert
        response.Should().BeSuccessful();
        var fetched = response.Should().DeserializeAs<Order>().Which;
        fetched.Id.Should().Be(order.Id);
        fetched.Status.Should().Be("placed");

        // Cleanup
        await Store.DeleteOrderAsync(order.Id);
    }

    // ---------- GET /store/order/{id} : 404 ----------

    [Test]
    public async Task GetOrderById_NonExistentOrder_Returns404()
    {
        // Arrange
        const long unknownId = 99999999999;

        // Act
        var response = await Store.GetOrderByIdAsync(unknownId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---------- DELETE /store/order/{id} ----------

    [Test]
    public async Task DeleteOrder_ExistingOrder_Returns200AndRemovesIt()
    {
        // Arrange
        var order = PetTestData.CreateOrder(id: UniqueIdLong(), petId: UniqueIdLong());
        await Store.PlaceOrderAsync(order);

        // Act
        var response = await Store.DeleteOrderAsync(order.Id);

        // Assert
        response.Should().BeSuccessful();

        // Verify deletion
        var getResponse = await Store.GetOrderByIdAsync(order.Id);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}