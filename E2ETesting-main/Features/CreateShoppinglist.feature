Feature: CreateShoppinglist

User creates a shoppinglist with 5 different items in it

@tag1
Scenario: User creates a shoppinglist with 5 items
    Given the user is logged in
    When the user navigates to the create shopping list page
    And the user enters a shopping list name "Weekend Groceries"
    And the user adds the following items to the list:
        | Name    | Amount | Category            |
        | Apples  | 4      | FruitsVegetables    |
        | Milk    | 1      | Fridge              |
        | Bread   | 2      | Pantry              |
        | Chicken | 1      | Freezer             |
        | Razor   | 3      | Hygiene             |
  And the user saves the list
  Then the list "Weekend Groceries" should be visible on the dashboard
