using challenge.Controllers;
using challenge.Data;
using challenge.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using code_challenge.Tests.Integration.Extensions;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using code_challenge.Tests.Integration.Helpers;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private HttpClient _httpClient;
        private TestServer _testServer;

        // Use TestInitialize rather than ClassInitialize to ensure isolation between test cases
        [TestInitialize]
        public void InitializeClass()
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
        }

        [TestCleanup]
        public void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }



        [TestMethod]
        public void CreateCompensation_Returns_Ok()
        {
            var compensation = new Compensation()
            {
                Employee = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                Salary = 100000,
                EffectiveDate = DateTime.UnixEpoch,
            };
            var newCompensation = CreateCompensation(compensation);
            Assert.AreEqual(compensation.Employee, newCompensation.Employee);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
        }



        [TestMethod]
        public void GetCompensationById_Returns_Ok()
        {
            var compensation = CreateCompensation(new Compensation()
            {
                Employee = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                Salary = 100000,
                EffectiveDate = DateTime.UnixEpoch,
            });

            var getRequestTask = _httpClient.GetAsync($"api/compensation/{compensation.CompensationId}");
            var getResponse = getRequestTask.Result;

            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);
            var fetchedCompensation = getResponse.DeserializeContent<Compensation>();
            Assert.AreEqual(compensation.CompensationId, fetchedCompensation.CompensationId);
            Assert.AreEqual(compensation.Salary, fetchedCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, fetchedCompensation.EffectiveDate);
            Assert.AreEqual(compensation.Employee, fetchedCompensation.Employee);
        }



        [TestMethod]
        public void GetCompensationsByEmployee_Returns_Ok()
        {
            var employee1 = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            // Create two compensations
            var compensation1 = CreateCompensation(new Compensation()
            {
                Employee = employee1,
                Salary = 123,
                EffectiveDate = DateTimeOffset.FromUnixTimeSeconds(1234567890).UtcDateTime,
            });
            var compensation2 = CreateCompensation(new Compensation()
            {
                Employee = employee1,
                Salary = 456,
                EffectiveDate = DateTimeOffset.FromUnixTimeSeconds(1345678901).UtcDateTime,
            });
            // Create a compensation for a different employee
            var compensation3 = CreateCompensation(new Compensation()
            {
                Employee = "b7839309-3348-463b-a7e3-5de1c168beb3",
                Salary = 789,
                EffectiveDate = DateTimeOffset.FromUnixTimeSeconds(1456789012).UtcDateTime,
            });

            var getRequestTask = _httpClient.GetAsync($"api/compensation/byEmployee/{employee1}");
            var getResponse = getRequestTask.Result;

            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);
            var fetchedCompensation = getResponse.DeserializeContent<ISet<Compensation>>();
            Assert.AreEqual(2, fetchedCompensation.Count);

            // There is no guarantee which order the entries came back in, so we need to test for each one
            // Get all entries that match compensation1 (should be 1)
            var fetchedCompensation1s = fetchedCompensation.Where(c => c.CompensationId == compensation1.CompensationId);
            Assert.AreEqual(1, fetchedCompensation1s.Count());
            var fetchedCompensation1 = fetchedCompensation1s.First();

            // Get all entries that match compensation2 (should be 1)
            var fetchedCompensation2s = fetchedCompensation.Where(c => c.CompensationId == compensation2.CompensationId);
            Assert.AreEqual(1, fetchedCompensation2s.Count());
            var fetchedCompensation2 = fetchedCompensation2s.First();

            // Ensure that compensation1 == fetchedCompensation1
            Assert.AreEqual(compensation1.Employee, fetchedCompensation1.Employee);
            Assert.AreEqual(compensation1.Salary, fetchedCompensation1.Salary);
            Assert.AreEqual(compensation1.EffectiveDate, fetchedCompensation1.EffectiveDate);

            // Ensure that compensation2 == fetchedCompensation2
            Assert.AreEqual(compensation2.Employee, fetchedCompensation2.Employee);
            Assert.AreEqual(compensation2.Salary, fetchedCompensation2.Salary);
            Assert.AreEqual(compensation2.EffectiveDate, fetchedCompensation2.EffectiveDate);
        }

        private Compensation CreateCompensation(Compensation compensation)
        {
            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var putRequestTask = _httpClient.PostAsync($"api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, putResponse.StatusCode);
            return putResponse.DeserializeContent<Compensation>();
        }
    }
}
