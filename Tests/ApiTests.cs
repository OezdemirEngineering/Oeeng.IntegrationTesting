using System.Net.Http;

namespace Tests;

public class ApiTests
{

    [Fact]
    public async void Catfact_ApiRequest_ExpectedFact()
    {
        // Arrange
        var client = new HttpClient();
        string url = "https://catfact.ninja/fact";

        // Act
        var response = await client.GetAsync(url);


        // Assert
        var content = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("fact", content);
    }
}