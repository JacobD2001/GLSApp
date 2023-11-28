using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using GLSApp.Interfaces;
using GLSApp.Models;
using GLSApp.Repositories;
using GLSApp.Services;
using Moq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using static GLSApp.Data.Enums;
using System.Dynamic;
using System.Linq;
using System.Text;
using GLSApp.Data;

namespace GLSApp.ServicesTests
{
    public class GlsApiServiceTests
    {
        private readonly Mock<IConsignRepository> _mockConsignRepository;
        private readonly GlsApiService _glsApiService;

        public GlsApiServiceTests()
        {
            _mockConsignRepository = new Mock<IConsignRepository>();
            _glsApiService = new GlsApiService(_mockConsignRepository.Object);
        }

        [Fact]
        public async Task LoginAsync_ReturnsSession_WhenSuccessful()
        {
            // Arrange
            var mockRestClient = new Mock<IRestClient>();
            var mockRestRequest = new Mock<IRestRequest>();
            var mockRestResponse = new Mock<IRestResponse>();

            mockRestResponse.Setup(x => x.IsSuccessful).Returns(true);
            mockRestResponse.Setup(x => x.Content).Returns("{\"session\": \"test_session\"}");

            mockRestClient.Setup(x => x.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockRestResponse.Object);

            // Act
            var result = await _glsApiService.LoginAsync();

            // Assert
            Assert.Equal("test_session", result);
        }

        [Fact]
        public async Task PrepareBoxAsync_ShouldReturnExpectedValue_WhenCalledWithValidParameters()
        {
            // Arrange
            var session = "testSession";
            var consign = new Consign();

            // Act
            var result = await _glsApiService.PrepareBoxAsync(session, _mockConsignRepository.Object, consign);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetLabelsAsync_ShouldReturnExpectedValue_WhenCalledWithValidParameters()
        {
            // Arrange
            var session = "testSession";
            var mode = LabelMode.one_label_on_a4_pdf;

            // Act
            var result = await _glsApiService.GetLabelsAsync(session, _mockConsignRepository.Object, mode);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<string>>(result);
        }
    }
}
