using Moq;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Services;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Interfaces.Persistence;
using StellarWallet.Domain.Interfaces.Services;
using StellarWallet.Domain.Result;

namespace StellarWallet.UnitTest.Application.Services;

public class AuthServiceTest
{
    [Fact]
    public async Task When_ValidLoginDto_Expected_ReturnLoggedDto()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockJwtService = new Mock<IJwtService>();
        var mockEncryptionService = new Mock<IEncryptionService>();

        var sut = new AuthService(mockJwtService.Object, mockEncryptionService.Object, mockUnitOfWork.Object);

        var loggedDto = new LoggedDto(true, "token", "GBXBDKPGYO74VVO64A7PBIHV7XN4QH4PJIRBSQT4OVE4U7JYY345PQMA");

        var loginDto = new LoginDto("john.doe@mail.com", "MyPassword123.");

        mockUnitOfWork.Setup(x => x.User.GetBy(nameof(User.Email), loginDto.Email)).ReturnsAsync(new User(
            "John",
            "Doe",
            "john.doe@mail.com",
            "EncryptedPassword",
            "GBXBDKPGYO74VVO64A7PBIHV7XN4QH4PJIRBSQT4OVE4U7JYY345PQMA",
            "SC6KTRKOT33RRH2KXK2BMGJWJ7TE5NQGRE5NIWTBGNMSPLKGQ2C63KDB"
        ));

        mockEncryptionService.Setup(x => x.Verify(loginDto.Password, "EncryptedPassword")).Returns(true);

        mockJwtService.Setup(x => x.CreateToken("john.doe@mail.com", "user")).Returns(Result<string, CustomError>.Success("token"));

        var result = await sut.Login(loginDto);

        Assert.True(result.Value.Success);
        Assert.Equal(loggedDto.Token, result.Value.Token);
        Assert.Equal(loggedDto.PublicKey, result.Value.PublicKey);
    }

    [Fact]
    public async Task When_InvalidLoginDto_Expected_UnsuccessfulResponse()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockJwtService = new Mock<IJwtService>();
        var mockEncryptionService = new Mock<IEncryptionService>();

        var sut = new AuthService(mockJwtService.Object, mockEncryptionService.Object, mockUnitOfWork.Object);

        Mock<User> user = new("John", "Doe", "john.doe@mail.com", "EncryptedPassword", "GBXBDKPGYO74VVO64A7PBIHV7XN4QH4PJIRBSQT4OVE4U7JYY345PQMA", "SC6KTRKOT33RRH2KXK2BMGJWJ7TE5NQGRE5NIWTBGNMSPLKGQ2C63KDB", "admin");


        mockUnitOfWork.Setup(x => x.User.GetBy(nameof(User.Email), "john.doe@mail.com")).ReturnsAsync(user.Object);

        var loginDto = new LoginDto("john.doe@mail.com", "MyPassword123.");
        mockEncryptionService.Setup(x => x.Verify(loginDto.Password, "EncryptedPassword")).Returns(false);

        var result = await sut.Login(loginDto);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void When_UnexistingUser_Expected_ThrowException()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockJwtService = new Mock<IJwtService>();
        var mockEncryptionService = new Mock<IEncryptionService>();

        var sut = new AuthService(mockJwtService.Object, mockEncryptionService.Object, mockUnitOfWork.Object);

        var loginDto = new LoginDto("john.doe@mail.com", "MyPassword123.");
        mockUnitOfWork.Setup(x => x.User.GetBy(nameof(User.Email), loginDto.Email)).ReturnsAsync((User?)null);

        Assert.ThrowsAsync<Exception>(() => sut.Login(loginDto));
    }

    [Fact]
    public void When_ValidJwt_Expected_ReturnTrue()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockJwtService = new Mock<IJwtService>();
        var mockEncryptionService = new Mock<IEncryptionService>();

        var sut = new AuthService(mockJwtService.Object, mockEncryptionService.Object, mockUnitOfWork.Object);

        var jwt = "token";
        var email = "john.doe@mail.com";

        mockJwtService.Setup(x => x.DecodeToken(jwt)).Returns(Result<string, CustomError>.Success(email));

        var result = sut.AuthenticateEmail(jwt, email);

        Assert.True(result.Value);
    }

    [Fact]
    public void When_InvalidJwt_Expected_ThrowException()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockJwtService = new Mock<IJwtService>();
        var mockEncryptionService = new Mock<IEncryptionService>();

        var sut = new AuthService(mockJwtService.Object, mockEncryptionService.Object, mockUnitOfWork.Object);

        var jwt = "token";
        var email = "john.doe@mail.com";

        mockJwtService.Setup(x => x.DecodeToken(jwt)).Throws(new Exception("Email or role claim not found"));

        Assert.Throws<Exception>(() => sut.AuthenticateEmail(jwt, email));
    }
}
