using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;

namespace E2ETesting.Steps
{
    [Binding]
    public class CreateShoppinglistStepDefinitions
    {
        private IPage _page;
        private IBrowser _browser;
        private IBrowserContext _context;

        [BeforeScenario]
        public async Task Setup()
        {
            var playwright = await Playwright.CreateAsync();
            _browser = await playwright.Chromium.LaunchAsync(new() { Headless = true});
            _context = await _browser.NewContextAsync();
            _page = await _context.NewPageAsync();

            // Navigate to the application URL
            await _page.GotoAsync("http://localhost:5240");
        }

        [AfterScenario]
        public async Task Cleanup()
        {
            await _page.CloseAsync();
            await _context.CloseAsync();
            await _browser.CloseAsync();
        }

        
        [Given(@"the user is logged in")]
        public async Task GivenTheUserIsLoggedIn()
        {
            //Using helper class to login
            await LoginHelper.LoginAsync(_page, "admin@admin.com", "Admin1!");

            // Wait for successful login redirect
            await _page.WaitForURLAsync("**/MyPage");
        }

        [When(@"the user navigates to the create shopping list page")]
        public async Task WhenTheUserNavigatesToTheCreateShoppingListPage()
        {
            // Trigger the Create new list button
            await _page.ClickAsync("#createnewlistform button");

            // Wait for an element that indicates the page is loaded, in this case the shopping list title input field
            await _page.WaitForSelectorAsync("#listTitleInput");
        }

        [When(@"the user enters a shopping list name ""([^""]*)""")]
        public async Task WhenTheUserEntersAShoppingListName(string listName)
        {
            //wait for the input field to be visible before filling it
            await _page.WaitForSelectorAsync("#listTitleInput", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            
            // Fill the shopping list name input field
            await _page.FillAsync("#listTitleInput", listName);
        }

        [When(@"the user adds the following items to the list:")]
        public async Task WhenTheUserAddsTheFollowingItemsToTheList(Table table)
        {
            //loop through the table rows in the feature file and add the items to the list one by one
            foreach (var row in table.Rows)
            {
                // Wait for the fields before filling them
                await _page.WaitForSelectorAsync("#product");
                await _page.FillAsync("#product", row["Name"]);

                await _page.WaitForSelectorAsync("#amount");
                await _page.FillAsync("#amount", row["Amount"]);

                // Use the category value from the feature file
                string categoryValue = row["Category"]; 

                // Select the category from the dropdown
                await _page.SelectOptionAsync("#category", new SelectOptionValue { Value = categoryValue });

                // Wait for and click the Add to list button
                await _page.WaitForSelectorAsync("text=Add to list");
                await _page.ClickAsync("text=Add to list");
            }
        }

        [When(@"the user saves the list")]
        public async Task WhenTheUserSavesTheList()
        {
            // Wait for the Save button before clicking
            await _page.WaitForSelectorAsync("text=Create shopping list");
            await _page.ClickAsync("text=Create shopping list");
        }

        [Then(@"the list ""([^""]*)"" should be visible on the dashboard")]
        public async Task ThenTheListShouldBeVisibleOnTheDashboard(string expectedListName)
        {
            // Wait for the page to load or refresh after saving the list
            await _page.WaitForSelectorAsync("body");

            //checks if the list name is present somewhere on the page after creation
            var listText = await _page.InnerTextAsync("body");
            Assert.Contains(expectedListName, listText);
        }
    }
}
