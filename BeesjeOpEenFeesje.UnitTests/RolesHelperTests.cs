using BeestjeOpEenFeestje.Controllers;
using BeestjeOpEenFeestje.Models;
using BeestjeOpEenFeestje.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeesjeOpEenFeesje.UnitTests
{
    [TestClass]
    public class RolesHelperTests
    {
        private RolesHelper _roleHelper;

        [TestInitialize]
        public void Setup()
        {
            _roleHelper = new RolesHelper();
        }

        [TestMethod]
        public async Task ConvertRoleToRealName_InputGebruiker_translatedToRealName()
        {
            // Arrange
            string input = "Gebruiker";
            string expectedResult = "Customer";

            // Act
            string result = _roleHelper.ConvertRoleToRealName(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public async Task ConvertRoleToRealName_InputWerknemer_translatedToRealName()
        {
            // Arrange
            string input = "Werknemer";
            string expectedResult = "Employee";

            // Act
            string result = _roleHelper.ConvertRoleToRealName(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public async Task ConvertRoleToRealName_InputAdmin_translatedToRealName()
        {
            // Arrange
            string input = "Admin";
            string expectedResult = "Admin";

            // Act
            string result = _roleHelper.ConvertRoleToRealName(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public async Task ConvertRoleToRealName_InputEmpty_ReturnsStandardRole()
        {
            // Arrange
            string input = " ";
            string expectedResult = "Customer";

            // Act
            string result = _roleHelper.ConvertRoleToRealName(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public async Task ConvertRoleToRealName_InputNotInRoles_ReturnsStandardRole()
        {
            // Arrange
            string input = "Bezoeker";
            string expectedResult = "Customer";

            // Act
            string result = _roleHelper.ConvertRoleToRealName(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public async Task ConvertRoleToShownName_InputCustomer_translatedToShownName()
        {
            // Arrange
            string input = "Customer";
            string expectedResult = "Gebruiker";

            // Act
            string result = _roleHelper.ConvertRoleToShownName(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public async Task ConvertRoleToShownName_InputEmployee_translatedToShownName()
        {
            // Arrange
            string input = "Employee";
            string expectedResult = "Werknemer";

            // Act
            string result = _roleHelper.ConvertRoleToShownName(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public async Task ConvertRoleToShownName_InputAdmin_translatedToShownName()
        {
            // Arrange
            string input = "Admin";
            string expectedResult = "Admin";

            // Act
            string result = _roleHelper.ConvertRoleToShownName(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public async Task ConvertRoleToShownName_InputEmpty_translatedToShownName()
        {
            // Arrange
            string input = " ";
            string expectedResult = "Gebruiker";

            // Act
            string result = _roleHelper.ConvertRoleToShownName(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public async Task ConvertRoleToShownName_InputGebruiker_translatedToShownName()
        {
            // Arrange
            string input = "Shopper";
            string expectedResult = "Gebruiker";

            // Act
            string result = _roleHelper.ConvertRoleToShownName(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
