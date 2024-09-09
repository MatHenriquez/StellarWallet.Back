using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;
using System.Net.Http.Json;

namespace StellarWallet.IntegrationTest
{
    public class UserControllerTests
    {
        public UserControllerTests()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "test");
        }

        [Fact]
        public async Task When_ValidUserCreationDto_Expected_ReturnLoggedDto()
        {
            var application = new StellarWalletWebApplicationFactory();

            UserCreationDto request = new("John", "Doe", "my.mail2@test.com", "MyPassword123.", null, null);

            var client = application.CreateClient();

            var response = await client.PostAsJsonAsync("/User", request);

            response.EnsureSuccessStatusCode();
            var createUserResponse = await response.Content.ReadFromJsonAsync<Result<LoggedDto, DomainError>>();

            Assert.True(createUserResponse?.Value.Success);
            Assert.Null(createUserResponse?.Value.Token);
            Assert.NotNull(createUserResponse?.Value.PublicKey);
        }
    }
}
