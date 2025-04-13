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
    class LoginHelper
    {
        public static async Task LoginAsync(IPage page, string email, string password)
        {
            await page.GotoAsync("http://localhost:5240/Identity/Account/Login"); 
            await page.FillAsync("#Input_Email", email);
            await page.FillAsync("#Input_Password", password);
            await page.ClickAsync("#login-submit");


        }


    }
}
