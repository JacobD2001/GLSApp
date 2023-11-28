using GLSApp.Services;
using Moq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using GLSApp.Interfaces;
using System.Linq;
using System.Text;

namespace GLSApp.Tests
{
    public class PrinterServiceTests
    {
        private readonly Mock<IRestClient> _mockRestClient;
        private readonly PrinterService _printerService;

        public PrinterServiceTests()
        {
            _mockRestClient = new Mock<IRestClient>();
            _printerService = new PrinterService(); 
        }

        [Fact]
        public async Task PrintLabelsAsync_WhenLabelsCountIsMoreThan10_ThrowsInvalidOperationException()
        {
            // Arrange
            var labels = new List<string>(11);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _printerService.PrintLabelsAsync(labels));
        }

        [Fact]
        public async Task PrintLabelsAsync_WhenResponseStatusCodeIsNotOK_ThrowsException()
        {
            // Arrange
            var labels = new List<string> { "label1", "label2" };
            var response = new Mock<IRestResponse>();
            response.Setup(r => r.StatusCode).Returns(System.Net.HttpStatusCode.BadRequest);
            _mockRestClient.Setup(c => c.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(response.Object); // Specified the type of the second argument

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _printerService.PrintLabelsAsync(labels));
        }

        [Fact]
        public async Task PrintLabelsAsync_WhenCalledWithValidLabels_SendsRequestToPrinter()
        {
            // Arrange
            var labels = new List<string> { "label1", "label2" };
            var response = new Mock<IRestResponse>();
            response.Setup(r => r.StatusCode).Returns(System.Net.HttpStatusCode.OK);
            _mockRestClient.Setup(c => c.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(response.Object); // Specified the type of the second argument

            // Act
            await _printerService.PrintLabelsAsync(labels);

            // Assert
            _mockRestClient.Verify(c => c.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()), Times.Once); // Specified the type of the second argument
        }
    }
}
