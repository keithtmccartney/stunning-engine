using FullStackJobs.AuthServer.Infrastructure.Data.Identity;
using FullStackJobs.AuthServer.IntegrationTests.Fixtures;
using FullStackJobs.AuthServer.Models;
using Newtonsoft.Json;
using PuppeteerSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FullStackJobs.AuthServer.IntegrationTests.Controllers
{
    [Collection("WebHost Collection")]
    public class AccountsControllerIntegrationTests : IClassFixture<AuthServerWebApplicationFactory<TestStartup, AppIdentityDbContext>>
    {
        private static readonly IList<SignupRequest> _signupRequests = new List<SignupRequest>
        {
            new SignupRequest() { FullName = "Keith McCartney", Email = "keithtmccartney@hotmail.com", Password = "Password123_N0T53cUR3_0r15it", Role = "Software Developer" },
            new SignupRequest() { FullName = "Mark Macneil", Email = "mark@fullstackmark.com", Password = "Pa$$w0rd!", Role = "applicant" }
        };

        private readonly HttpClient _client;
        private readonly WebHostFixture _webHostFixture;

        public AccountsControllerIntegrationTests(AuthServerWebApplicationFactory<TestStartup, AppIdentityDbContext> factory, WebHostFixture webHostFixture)
        {
            _client = factory.CreateClient();
            _webHostFixture = webHostFixture;
        }

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

        [Fact]
        public async Task CanLogin()
        {
            var httpResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/api/accounts")
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(_signupRequests[1]), Encoding.UTF8, "application/json")
            });

            httpResponse.EnsureSuccessStatusCode();

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }))
            {
                using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync($"http://{_webHostFixture.Host}/test-client/index.html");

                    var navigationTask = page.WaitForNavigationAsync();

                    await Task.WhenAll(navigationTask, page.ClickAsync("button"));

                    await page.TypeAsync("#Username", _signupRequests[1].Email);
                    await page.TypeAsync("#Password", _signupRequests[1].Password);

                    var content = await page.GetContentAsync();

                    await page.CloseAsync();

                    Assert.Contains("User logged in", content);
                    Assert.Contains("Mark Macneil", content);
                    Assert.Contains("mark@fullstackmark.com", content);
                    Assert.Contains("applicant", content);
                }
            }
        }
    }
}
