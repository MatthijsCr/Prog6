using BeestjeOpEenFeestje.Models;
using BeestjeOpEenFeestje.ViewModels;
using BeestjeOpEenFeestje.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeesjeOpEenFeesje.UnitTests
{
    [TestClass]
    public class AccountControllerTests
    {
        private Mock<UserManager<AppUser>> _mockUserManager;
        private Mock<SignInManager<AppUser>> _mockSignInManager;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private AccountController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Mock UserManager
            var userStoreMock = new Mock<IUserStore<AppUser>>();
            _mockUserManager = new Mock<UserManager<AppUser>>(
                userStoreMock.Object,
                null, null, null, null, null, null, null, null);

            // Mock SignInManager
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<AppUser>>();
            _mockSignInManager = new Mock<SignInManager<AppUser>>(
                _mockUserManager.Object,
                contextAccessorMock.Object,
                userClaimsPrincipalFactoryMock.Object,
                null, null, null, null);

            // Mock RoleManager
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                roleStoreMock.Object,
                null, null, null, null);

            _controller = new AccountController(
                _mockSignInManager.Object,
                _mockUserManager.Object,
                _mockRoleManager.Object
            );
        }

        [TestMethod]
        public async Task Login_ValidCredentials_RedirectsToHomeIndex()
        {
            var loginModel = new LoginModel { Email = "test@example.com", Password = "Password123" };
            var user = new AppUser { Email = loginModel.Email };

            _mockUserManager.Setup(um => um.FindByEmailAsync(loginModel.Email))
                .ReturnsAsync(user);

            _mockSignInManager.Setup(sm => sm.PasswordSignInAsync(user, loginModel.Password, true, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var result = await _controller.Login(loginModel) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
        }

        [TestMethod]
        public async Task Login_InvalidCredentials_ReturnsViewWithModel()
        {
            var loginModel = new LoginModel { Email = "test@example.com", Password = "WrongPassword" };

            _mockUserManager.Setup(um => um.FindByEmailAsync(loginModel.Email))
                .ReturnsAsync((AppUser)null);

            var result = await _controller.Login(loginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(loginModel, result.Model);
        }

        [TestMethod]
        public async Task CreateUser_ValidModel_RedirectsToPasswordView()
        {
            var createUserModel = new CreateUserModel
            {
                Email = "newuser@example.com",
                Username = "newuser",
                Role = "Customer",
                Address = "123 Street",
                PhoneNumber = "1234567890",
                CustomerCard = CustomerCardType.Geen
            };

            _mockUserManager.Setup(um => um.FindByNameAsync(createUserModel.Username))
                .ReturnsAsync((AppUser)null);

            _mockRoleManager.Setup(rm => rm.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            var result = await _controller.CreateUser(createUserModel) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Password", result.ActionName);
        }

        [TestMethod]
        public async Task CreateUser_ExistingUser_ReturnsViewWithModelError()
        {
            var createUserModel = new CreateUserModel
            {
                Email = "existinguser@example.com",
                Username = "existinguser",
                Role = "Admin",
                Address = "123 Street",
                PhoneNumber = "1234567890",
                CustomerCard = CustomerCardType.Geen
            };

            var existingUser = new AppUser { UserName = createUserModel.Username };
            _mockUserManager.Setup(um => um.FindByNameAsync(createUserModel.Username))
                .ReturnsAsync(existingUser);

            var result = await _controller.CreateUser(createUserModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(createUserModel, result.Model);
            Assert.IsTrue(_controller.ModelState.ContainsKey("Username"));
        }

        [TestMethod]
        public async Task Logout_RedirectsToLogin()
        {
            _mockSignInManager.Setup(sm => sm.SignOutAsync())
                .Returns(Task.CompletedTask);

            var result = await _controller.Logout() as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ActionName);
            Assert.AreEqual("Account", result.ControllerName);
        }

        [TestMethod]
        public void AccountsList_ReturnsViewWithUsers()
        {
            var users = new List<AppUser>
            {
                new AppUser { UserName = "user1", Email = "user1@example.com" },
                new AppUser { UserName = "user2", Email = "user2@example.com" }
            };

            _mockUserManager.Setup(um => um.Users)
                .Returns(users.AsQueryable());

            var result = _controller.AccountsList() as ViewResult;

            Assert.IsNotNull(result);
            var model = result.Model as List<UpdateUserModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual("user1", model[0].Username);
        }
    }
}
