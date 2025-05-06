using System.Security.Claims;
using BLL.DTOs.AuthenticateDTO;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;

namespace UnitTest.Controller
{
    [TestFixture]
    public class AuthControllerTest
    {
        private Mock<IAuthenticateService> _mockAuthService;
        private AuthController _authController;

        [SetUp]
        public void SetUp()
        {
            _mockAuthService = new Mock<IAuthenticateService>();
            _authController = new AuthController(_mockAuthService.Object);
        }

        [Test]
        public async Task Login_ValidRequest_ReturnsOkWithTokenResponse()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "testuser",
                Password = "password123"
            };

            var tokenResponse = new TokenResponseDTO
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token"
            };

            _mockAuthService.Setup(service => service.LoginAsync(loginRequest))
                .ReturnsAsync(tokenResponse);

            // Act
            var result = await _authController.Login(loginRequest) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Value, Is.EqualTo(tokenResponse));
            _mockAuthService.Verify(service => service.LoginAsync(loginRequest), Times.Once);
        }

        [Test]
        public async Task Register_ValidRequest_ReturnsOkWithSuccessMessage()
        {
            // Arrange
            var registerRequest = new RegisterRequestDTO
            {
                Username = "newuser",
                Password = "password123",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockAuthService.Setup(service => service.RegisterAsync(registerRequest))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authController.Register(registerRequest) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Value, Is.EqualTo("User registered successfully."));
            _mockAuthService.Verify(service => service.RegisterAsync(registerRequest), Times.Once);
        }

        [Test]
        public async Task RefreshToken_ValidRequest_ReturnsOkWithTokenResponse()
        {
            // Arrange
            var refreshRequest = new RefreshRequestDTO
            {
                AccessToken = "expired-access-token",
                RefreshToken = "valid-refresh-token"
            };

            var tokenResponse = new TokenResponseDTO
            {
                AccessToken = "new-access-token",
                RefreshToken = "new-refresh-token"
            };

            _mockAuthService.Setup(service => service.RetrieveAccessToken(refreshRequest))
                .ReturnsAsync(tokenResponse);

            // Act
            var result = await _authController.RefreshToken(refreshRequest) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Value, Is.EqualTo(tokenResponse));
            _mockAuthService.Verify(service => service.RetrieveAccessToken(refreshRequest), Times.Once);
        }

    }
}