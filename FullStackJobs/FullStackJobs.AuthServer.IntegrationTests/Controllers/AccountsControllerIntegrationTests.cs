using FullStackJobs.AuthServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FullStackJobs.AuthServer.IntegrationTests.Controllers
{
    public class AccountsControllerIntegrationTests
    {
        private static readonly IList<SignupRequest> _signupRequests = new List<SignupRequest>
        {
            new SignupRequest() { FullName = "Keith McCartney", Email = "keithtmccartney@hotmail.com", Password = "Password123_N0T53cUR3_0r15it", Role = "Software Developer" },
            new SignupRequest() { FullName = "Mark Macneil", Email = "mark@fullstackmark.com", Password = "Pa$$w0rd!", Role = "applicant" }
        };

        private readonly HttpClient _client;

        [Fact]
        public async Task CanCreateAccount()
        {
            var httpReponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/api/accounts")
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(_signupRequests[0]), Encoding.UTF8, "application/json")
            });

            httpReponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpReponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<SignupResponse>(stringResponse);

            Assert.Equal(_signupRequests[0].FullName, response.FullName);
            Assert.Equal(_signupRequests[0].Email, response.Email);
            Assert.Equal(_signupRequests[0].Role, response.Role);

            Assert.True(Guid.TryParse(response.Id, out _));
        }
    }
}
