using Azure.Security.KeyVault.Secrets;
using GLSApp.Interfaces;
using GLSApp.Models;
using GLSApp.Services;
using Moq;
using RestSharp;

[Fact]
public async Task PrepareBoxAsync_ShouldReturnConsignmentId_WhenSuccessful()
{
    // Arrange
    var apiService = SetupApiService();
    var consign = new Consign { /* set consign properties here */ };

    // Act
    var result = await apiService.PrepareBoxAsync("mockedSession", consign);

    // Assert
    Assert.NotNull(result);
    Assert.IsType<int>(result);
}

[Fact]
public async Task PrepareBoxAsync_ShouldReturnNull_WhenDatabaseError()
{
    // Arrange
    var apiService = SetupApiService();
    var consign = new Consign { /* set consign properties here */ };

    // Mocking a failure when saving to the repository
    var consignRepositoryMock = new Mock<IConsignRepository>();
    consignRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Consign>())).ReturnsAsync((int?)null);
    var apiServiceWithMockedRepository = new GlsApiService(consignRepositoryMock.Object);

    // Act
    var result = await apiServiceWithMockedRepository.PrepareBoxAsync("mockedSession", consign);

    // Assert
    Assert.Null(result);
}

private GlsApiService SetupApiService()
{
    var consignRepositoryMock = new Mock<IConsignRepository>();
    consignRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Consign>())).ReturnsAsync(1);

    var restClientMock = new Mock<IRestClient>();
    restClientMock.Setup(x => x.ExecuteAsync(It.IsAny<IRestRequest>())).ReturnsAsync(new RestResponse { IsSuccessful = true, Content = "{ \"Id\": 1 }" });

    var secretClientMock = new Mock<SecretClient>();
    secretClientMock.Setup(client => client.GetSecretAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new KeyVaultSecret("ApiUsername", "mockedUsername"))
                    .Verifiable();

    secretClientMock.Setup(client => client.GetSecretAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new KeyVaultSecret("ApiPassword", "mockedPassword"))
                    .Verifiable();

    var apiService = new GlsApiService(consignRepositoryMock.Object, restClientMock.Object, secretClientMock.Object);

    return apiService;
}
