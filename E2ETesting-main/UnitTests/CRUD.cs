using ListLife.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using ListLife.Data;
using Microsoft.EntityFrameworkCore;
using E2ETesting.UnitTests.Helpers;


namespace E2ETesting.UnitTests
{
    
    public class ShoppingListTests
    {
        // Unit tests that don't require a database

        [Fact]
        public void CreateList_Should_Initialize_With_Title_Bound_To_User_Without_Products_Or_Being_shared()
        {
            // Arrange
            var shoppingList = new ShoppingList
            {
                Title = "Groceries",
                UserId = "user123" 
            };

            // Act & Assert
            shoppingList.Title.Should().Be("Groceries");
            shoppingList.UserId.Should().Be("user123");
            shoppingList.Products.Should().BeEmpty();
            shoppingList.SharedWith.Should().BeEmpty();
            
        }


        [Theory]
        [InlineData("Bananas", -3, "Fruit", true)]  
        [InlineData("Apples", 0, "Fruit", true)]    
        [InlineData("Milk", 2, "Fridge", false)]     
        public void Product_Should_Validate_Amount(string name, int amount, string category, bool shouldThrow)
        {
            if (shouldThrow)
            {
                var ex = Assert.Throws<ArgumentException>(() => new Product
                {
                    Name = name,
                    Amount = amount,
                    Category = category
                });

                ex.Message.Should().Be("Amount cannot be negative");
            }
            else
            {
                var product = new Product
                {
                    Name = name,
                    Amount = amount,
                    Category = category
                };

                product.Amount.Should().Be(amount);
                product.Name.Should().Be(name);
                product.Category.Should().Be(category);
            }
        }

        // Using in-memory database
        [Fact]

        public void AddProduct_Should_Add_Product_To_List_With_The_Right_ListId()
        {
            //Arrange
          
            using var context = InMemoryDbHelper.GetContext();
            var shoppingList = new ShoppingList
                {
                    Title = "Groceries",
                    UserId = "user123"
                };
                context.ShoppingLists.Add(shoppingList);
                context.SaveChanges();

                var product = new Product
                {
                    Name = "Eggs",
                    Amount = 12,
                    Category = "Fridge"
                };

                shoppingList.Products.Add(product);
                context.SaveChanges();

                // Assert
              var listFromDb = context.ShoppingLists
                    .Include(s => s.Products)
                    .FirstOrDefault(s => s.Id == shoppingList.Id);

            listFromDb.Should().NotBeNull("must exist in DB");

            listFromDb.Products.Should().ContainSingle(p => p.Name == "Eggs" && p.Amount == 12 && p.Category == "Fridge" && p.ShoppingListId == shoppingList.Id);
            
        }
        

        [Fact]
        public async Task DeleteProduct_Should_Remove_Product_From_List()
        {

            // Arrange

            using var context = InMemoryDbHelper.GetContext();

            var shoppingList = new ShoppingList
            {
                Title = "Groceries",
                UserId = "user123"
            };

            var product1 = new Product { Id = 1, Name = "Milk", Amount = 2, Category = "Fridge" };
            var product2 = new Product { Id = 2, Name = "Eggs", Amount = 12, Category = "Fridge" };

            shoppingList.Products.Add(product1);
            shoppingList.Products.Add(product2);
            context.ShoppingLists.Add(shoppingList);
            await context.SaveChangesAsync();

            // Act removing product with id 1
            var productToDelete = shoppingList.Products.FirstOrDefault(p => p.Id == 1);
            context.Products.Remove(productToDelete);
            await context.SaveChangesAsync();

            // Assert checking if product with id=1 is removed and other product with id = 2 is still there
            var deletedProduct = await context.Products.FindAsync(1);
            deletedProduct.Should().BeNull("Milk should be removed");
            var remainingProducts = await context.Products.FindAsync(2);
            remainingProducts.Should().NotBeNull("Eggs should still exist");
        }

        [Fact]

        public async Task EditProduct_Should_Update_Product_In_List()
        {
            // Arrange
            using var context = InMemoryDbHelper.GetContext();
            
            var shoppingList = new ShoppingList
            {
                Title = "Groceries",
                UserId = "user123"
            };
            var product = new Product { Id = 1, Name = "Milk", Amount = 2, Category = "Fridge" };
            shoppingList.Products.Add(product);
            context.ShoppingLists.Add(shoppingList);
            await context.SaveChangesAsync();

            // Act gets the product with id 1 and updates it
            var productToUpdate = await context.Products.FindAsync(1);
            if (productToUpdate != null)
            {
                productToUpdate.Name = "Choclate";
                productToUpdate.Amount = 6;
                productToUpdate.Category = "Pantry";
                context.Products.Update(productToUpdate);
                await context.SaveChangesAsync();
            }


            // Assert checks if the product with id 1 is updated and is not null
            var updatedProduct = await context.Products.FindAsync(1);
            updatedProduct.Should().NotBeNull("Product should exist");
            updatedProduct.Name.Should().Be("Choclate");
            updatedProduct.Amount.Should().Be(6);
        }
    }


}
