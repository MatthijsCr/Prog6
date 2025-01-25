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
        public void ConvertRoleToRealName_InputGebruiker_translatedToRealName()
        {
            string input = "Gebruiker";
            string expectedResult = "Customer";

            string result = _roleHelper.ConvertRoleToRealName(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ConvertRoleToRealName_InputWerknemer_translatedToRealName()
        {
            string input = "Werknemer";
            string expectedResult = "Employee";

            string result = _roleHelper.ConvertRoleToRealName(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ConvertRoleToRealName_InputAdmin_translatedToRealName()
        {
            string input = "Admin";
            string expectedResult = "Admin";

            string result = _roleHelper.ConvertRoleToRealName(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ConvertRoleToRealName_InputEmpty_ReturnsStandardRole()
        {
            string input = " ";
            string expectedResult = "Customer";

            string result = _roleHelper.ConvertRoleToRealName(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ConvertRoleToRealName_InputNotInRoles_ReturnsStandardRole()
        { 
            string input = "Bezoeker";
            string expectedResult = "Customer";

            string result = _roleHelper.ConvertRoleToRealName(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ConvertRoleToShownName_InputCustomer_translatedToShownName()
        {
            string input = "Customer";
            string expectedResult = "Gebruiker";

            string result = _roleHelper.ConvertRoleToShownName(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ConvertRoleToShownName_InputEmployee_translatedToShownName()
        {
            string input = "Employee";
            string expectedResult = "Werknemer";

            string result = _roleHelper.ConvertRoleToShownName(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ConvertRoleToShownName_InputAdmin_translatedToShownName()
        {
            string input = "Admin";
            string expectedResult = "Admin";

            string result = _roleHelper.ConvertRoleToShownName(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ConvertRoleToShownName_InputEmpty_translatedToShownName()
        {
            string input = " ";
            string expectedResult = "Gebruiker";

            string result = _roleHelper.ConvertRoleToShownName(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ConvertRoleToShownName_InputGebruiker_translatedToShownName()
        {
            string input = "Shopper";
            string expectedResult = "Gebruiker";

            string result = _roleHelper.ConvertRoleToShownName(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
