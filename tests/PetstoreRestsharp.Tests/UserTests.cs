using System.Net;
using FluentAssertions;
using PetstoreRestsharp.Core.Models;
using PetstoreRestsharp.Tests.TestData;

namespace PetstoreRestsharp.Tests;

[TestFixture]
public class UserTests : TestFixtureBase
{
    private static string UniqueUsername2()
        => "u_" + Guid.NewGuid().ToString("N")[..12];

    // ---------- POST /user : create user ----------

    [Test]
    public async Task CreateUser_WithValidUser_Returns200()
    {
        // Arrange
        var username = UniqueUsername2();
        var user = PetTestData.CreateUser(id: 7777 + Random.Shared.Next(1000), username: username);

        // Act
        var response = await Users.CreateAsync(user);

        // Assert
        response.Should().BeSuccessful();

        // Cleanup
        await Users.DeleteAsync(username);
    }

    // ---------- GET /user/{username} : fetch by username ----------

    [Test]
    public async Task GetByUsername_ExistingUser_ReturnsUser()
    {
        // Arrange
        var username = UniqueUsername2();
        var user = PetTestData.CreateUser(id: 8888 + Random.Shared.Next(1000), username: username);
        await Users.CreateAsync(user);

        // Act
        var response = await Users.GetByUsernameAsync(username);

        // Assert
        response.Should().BeSuccessful();
        var fetched = response.Should().DeserializeAs<User>().Which;
        fetched.Username.Should().Be(username);
        fetched.FirstName.Should().Be("John");

        // Cleanup
        await Users.DeleteAsync(username);
    }

    // ---------- GET /user/{username} : 404 for unknown ----------

    [Test]
    public async Task GetByUsername_NonExistentUser_Returns404()
    {
        // Arrange
        const string unknownUsername = "no_such_user_42";

        // Act
        var response = await Users.GetByUsernameAsync(unknownUsername);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---------- PUT /user/{username} : update user ----------

    [Test]
    public async Task UpdateUser_ExistingUser_Returns200AndChangesFirstName()
    {
        // Arrange
        var username = UniqueUsername2();
        var user = PetTestData.CreateUser(id: 6666 + Random.Shared.Next(1000), username: username);
        await Users.CreateAsync(user);
        user.FirstName = "Jane";

        // Act
        var response = await Users.UpdateAsync(username, user);

        // Assert
        response.Should().BeSuccessful();

        // Verify persistence
        var getResponse = await Users.GetByUsernameAsync(username);
        var updated = getResponse.Should().DeserializeAs<User>().Which;
        updated.FirstName.Should().Be("Jane");

        // Cleanup
        await Users.DeleteAsync(username);
    }

    // ---------- DELETE /user/{username} ----------

    [Test]
    public async Task DeleteUser_ExistingUser_Returns200AndRemoves()
    {
        // Arrange
        var username = UniqueUsername2();
        var user = PetTestData.CreateUser(id: 5555 + Random.Shared.Next(1000), username: username);
        await Users.CreateAsync(user);

        // Act
        var response = await Users.DeleteAsync(username);

        // Assert
        response.Should().BeSuccessful();

        // Verify deletion
        var getResponse = await Users.GetByUsernameAsync(username);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---------- GET /user/login, GET /user/logout ----------

    [Test]
    public async Task LoginAndLogout_ReturnsSuccess()
    {
        // Arrange
        var username = UniqueUsername2();
        var user = PetTestData.CreateUser(id: 4444 + Random.Shared.Next(1000), username: username);
        await Users.CreateAsync(user);

        // Act
        var login = await Users.LoginAsync(username, user.Password);
        var logout = await Users.LogoutAsync();

        // Assert
        login.Should().BeSuccessful();
        login.Content.Should().Contain("logged in", "the login response contains a session message");
        logout.Should().BeSuccessful();

        // Cleanup
        await Users.DeleteAsync(username);
    }

    // ---------- POST /user/createWithArray ----------

    [Test]
    public async Task CreateWithArray_MultipleUsers_Returns200()
    {
        // Arrange
        var users = new List<User>
        {
            PetTestData.CreateUser(id: 1212, username: UniqueUsername2()),
            PetTestData.CreateUser(id: 1313, username: UniqueUsername2())
        };

        // Act
        var response = await Users.CreateWithArrayAsync(users);

        // Assert
        response.Should().BeSuccessful();

        // Verify one of the created users is queryable
        var getResponse = await Users.GetByUsernameAsync(users[0].Username);
        getResponse.Should().BeSuccessful();

        // Cleanup
        foreach (var u in users)
            await Users.DeleteAsync(u.Username);
    }

    // ---------- POST /user/createWithList ----------

    [Test]
    public async Task CreateWithList_MultipleUsers_Returns200()
    {
        // Arrange
        var users = new List<User>
        {
            PetTestData.CreateUser(id: 1111, username: UniqueUsername2()),
            PetTestData.CreateUser(id: 2222, username: UniqueUsername2()),
            PetTestData.CreateUser(id: 3333, username: UniqueUsername2())
        };

        // Act
        var response = await Users.CreateWithListAsync(users);

        // Assert
        response.Should().BeSuccessful();

        // Cleanup
        foreach (var u in users)
            await Users.DeleteAsync(u.Username);
    }
}