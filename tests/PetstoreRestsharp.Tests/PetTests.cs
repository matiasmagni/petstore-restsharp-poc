using System.Net;
using FluentAssertions;
using PetstoreRestsharp.Core.Models;
using PetstoreRestsharp.Tests.TestData;

namespace PetstoreRestsharp.Tests;

[TestFixture]
public class PetTests : TestFixtureBase
{
    // ---------- Data sources (DDT) ----------

    private static long UniqueIdLong()
        => Math.Abs(BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0));

    // ---------- POST /pet : Add a new pet ----------

    [Test]
    public async Task AddPet_WithValidPet_Returns200AndPersists()
    {
        // Arrange
        var pet = PetTestData.CreatePet(id: UniqueIdLong(), name: "Buddy", status: "available");
        Test.Log(Status.Info, $"Attempting to add pet with ID: {pet.Id} and Name: {pet.Name}");

        // Act
        var response = await Pets.AddPetAsync(pet);

        // Assert
        response.Should().BeSuccessful("the API should accept a well-formed pet");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Test.Log(Status.Pass, $"Successfully added pet. Status Code: {response.StatusCode}");

        var created = await Pets.GetByIdAsync(pet.Id);
        created.Should().BeSuccessful();
        var fetched = created.Should().DeserializeAs<Pet>().Which;
        fetched.Name.Should().Be("Buddy");
        fetched.Status.Should().Be("available");
        Test.Log(Status.Pass, $"Verified pet details: Name={fetched.Name}, Status={fetched.Status}");

        // Cleanup
        await Pets.DeleteAsync(pet.Id);
    }

    // ---------- GET /pet/findByStatus : Filter by status (DDT) ----------

    [TestCase("available")]
    [TestCase("pending")]
    [TestCase("sold")]
    public async Task FindByStatus_ForEachStatus_Returns200WithNonEmptyList(string status)
    {
        // Arrange
        var pet = PetTestData.CreatePet(id: UniqueIdLong(), name: "StatusDog", status: status);
        await Pets.AddPetAsync(pet);

        // Act
        var response = await Pets.GetByStatusAsync(status);

        // Assert
        response.Should().BeSuccessful();
        var pets = response.Should().DeserializeAs<List<Pet>>().Which;
        pets.Should().NotBeEmpty("the store always has some pets in each state");
        pets.Should().OnlyContain(p => p.Status == status,
            "every returned pet must match the requested status");

        // Cleanup
        await Pets.DeleteAsync(pet.Id);
    }

    // ---------- GET /pet/findByTags : Filter by tags ----------

    [Test]
    public async Task FindByTags_ExistingTag_Returns200()
    {
        // Arrange
        var pet = PetTestData.CreatePet(id: UniqueIdLong(), name: "TaggedDog");
        await Pets.AddPetAsync(pet);

        // Act
        var response = await Pets.GetByTagsAsync("friendly");

        // Assert
        response.Should().BeSuccessful();
        var pets = response.Should().DeserializeAs<List<Pet>>().Which;
        pets.Should().NotBeEmpty();

        // Cleanup
        await Pets.DeleteAsync(pet.Id);
    }

    // ---------- GET /pet/{id} : Fetch by id ----------

    [Test]
    public async Task GetById_ExistingPet_Returns200WithMatchingPet()
    {
        // Arrange
        var pet = PetTestData.CreatePet(id: UniqueIdLong(), name: "Findable");
        await Pets.AddPetAsync(pet);

        // Act
        var response = await Pets.GetByIdAsync(pet.Id);

        // Assert
        response.Should().BeSuccessful();
        var fetched = response.Should().DeserializeAs<Pet>().Which;
        fetched.Id.Should().Be(pet.Id);
        fetched.Name.Should().Be("Findable");

        // Cleanup
        await Pets.DeleteAsync(pet.Id);
    }

    // ---------- GET /pet/{id} : 404 for unknown id ----------

    [Test]
    public async Task GetById_NonExistentPet_Returns404()
    {
        // Arrange
        var unknownId = 999999999;

        // Act
        var response = await Pets.GetByIdAsync(unknownId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---------- PUT /pet : Update existing pet ----------

    [Test]
    public async Task UpdatePet_ExistingPet_Returns200AndChangesName()
    {
        // Arrange
        var pet = PetTestData.CreatePet(id: UniqueIdLong(), name: "BeforeUpdate");
        await Pets.AddPetAsync(pet);
        pet.Name = "AfterUpdate";

        // Act
        var response = await Pets.UpdatePetAsync(pet);

        // Assert
        response.Should().BeSuccessful();

        // Verify persisted change
        var getResponse = await Pets.GetByIdAsync(pet.Id);
        var fetched = getResponse.Should().DeserializeAs<Pet>().Which;
        fetched.Name.Should().Be("AfterUpdate");

        // Cleanup
        await Pets.DeleteAsync(pet.Id);
    }

    // ---------- POST /pet/{id} : Update pet with form data ----------

    [Test]
    public async Task UpdatePetWithForm_ExistingPet_ChangesNameAndStatus()
    {
        // Arrange
        var pet = PetTestData.CreatePet(id: UniqueIdLong(), name: "FormPet", status: "available");
        await Pets.AddPetAsync(pet);

        // Act
        var response = await Pets.UpdateWithFormAsync(pet.Id, "FormPetNew", "sold");

        // Assert
        response.Should().BeSuccessful();

        // Verify persisted change
        var getResponse = await Pets.GetByIdAsync(pet.Id);
        var fetched = getResponse.Should().DeserializeAs<Pet>().Which;
        fetched.Name.Should().Be("FormPetNew", "the form update must persist the new name");
        fetched.Status.Should().Be("sold", "the form update must persist the new status");

        // Cleanup
        await Pets.DeleteAsync(pet.Id);
    }

    // ---------- POST /pet/{id}/uploadImage : Upload an image ----------

    [Test]
    public async Task UploadImage_ExistingPet_Returns200WithUploadMessage()
    {
        // Arrange
        var pet = PetTestData.CreatePet(id: UniqueIdLong(), name: "PhotoDog");
        await Pets.AddPetAsync(pet);
        var imageBytes = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "TestData", "img", "dog_photo.jpg"));

        // Act
        var response = await Pets.UploadImageAsync(pet.Id, imageBytes, "photo.jpg", "metadata-foo");

        // Assert
        response.Should().BeSuccessful();
        response.Content.Should().Contain("metadata-foo", "the response echoes the additional metadata");
        response.Content.Should().Contain("photo.jpg", "the response echoes the uploaded file name");

        // Cleanup
        await Pets.DeleteAsync(pet.Id);
    }

    // ---------- DELETE /pet/{id} : Delete existing pet ----------

    [Test]
    public async Task DeletePet_ExistingPet_Returns200AndRemovesIt()
    {
        // Arrange
        var pet = PetTestData.CreatePet(id: UniqueIdLong(), name: "ToDelete");
        await Pets.AddPetAsync(pet);

        // Act
        var response = await Pets.DeleteAsync(pet.Id);

        // Assert (delete returns 200 on demo server)
        response.Should().BeSuccessful();

        var getResponse = await Pets.GetByIdAsync(pet.Id);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---------- Validity boundaries (DDT) ----------

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(-1)]
    public async Task GetById_WithNonExistentId_DoesNotReturnServerError(long id)
    {
        // Arrange (use ids that are unlikely to exist but are valid <int64>)
        // Act
        var response = await Pets.GetByIdAsync(id);

        // Assert
        response.StatusCode.Should().BeOneOf(
            new[] { HttpStatusCode.OK, HttpStatusCode.NotFound },
            "the API must not crash for well-formed but unknown long ids");
    }
}