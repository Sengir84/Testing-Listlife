using ListLife.Data;
using ListLife.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace E2ETesting.Steps
{
    [Binding]
    public class LoginStepDefinitions
    {
        private IPage _page;
        private IBrowser _browser;
        private IBrowserContext _context;

        [BeforeScenario]
        public async Task Setup()
        {
            var playwright = await Playwright.CreateAsync();
            _browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
            _context = await _browser.NewContextAsync(new()
            {
                IgnoreHTTPSErrors = true
            });
            _page = await _context.NewPageAsync();
        }

        [AfterScenario]
        public async Task Cleanup()
        {
            await _page.CloseAsync();
            await _context.CloseAsync();
            await _browser.CloseAsync();
        }

        private string _email;
        private string _password;

        [Given(@"the user ""([^""]*)"" with password ""([^""]*)"" exists")]
        public async Task GivenTheUserWithPasswordExists(string email, string password)
        {
            await Task.CompletedTask;
        }

        [Given(@"the user navigates to the login page")]
        public async Task GivenTheUserNavigatesToTheLoginPage()
        {
            await Task.Delay(2000);
            await _page.GotoAsync("http://localhost:5240/");
            await _page.WaitForSelectorAsync("a:has-text('Login')");
            await _page.ClickAsync("a:has-text('Login')");
        }

        [When(@"the user logs in with email ""(.*)"" and password ""(.*)""")]
        public async Task WhenTheUserLogsInWithEmailAndPassword(string email, string password)
        {
            _email = email;  
            _password = password;  

            // Fill the email and password fields on the login page
            await _page.FillAsync("input[placeholder='name@example.com']", email);
            await _page.FillAsync("input[placeholder='password']", password);

            // Click the login submit button
            await _page.ClickAsync("#login-submit");

        }

        [Then(@"the login should be ""(.*)""")]
        public async Task ThenTheLoginShouldBe(string expectedResult)
        {
            if (expectedResult == "Success")
            {
                // Check if user was redirected to the home/dashboard page or another page after successful login
                await _page.WaitForURLAsync("**/MyPage");
                var url = _page.Url;
                Assert.Contains("/MyPage", url);  // You can adjust this based on your successful login URL
            }
            else if (expectedResult == "Failure")
            {
                // Check for error message or remain on the login page
                string errorMessage = await _page.InnerTextAsync(".text-danger");  // Use .text-danger for general errors

                // Check for the email validation error message when email is empty
                if (string.IsNullOrEmpty(_email))
                {
                    Assert.Contains("The Email field is required.", errorMessage);
                }

                // Check for the password validation error message when password is empty
                if (string.IsNullOrEmpty(_password))
                {
                    var passwordErrorMessage = await _page.InnerTextAsync("span#Input_Password-error");
                    Assert.Contains("The Password field is required.", passwordErrorMessage);
                }

                // If both email and password are provided but login fails, check the invalid login attempt message
                if (!string.IsNullOrEmpty(_email) && !string.IsNullOrEmpty(_password))
                {
                    Assert.Contains("Invalid login attempt", errorMessage);
                }
            }
        }
    }
}
