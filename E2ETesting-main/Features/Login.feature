Feature: Login attempts

Trying to log in with both existing and non existing users and credentials

@login
Scenario Outline: User logs in with various credentials
    Given the user navigates to the login page
    When the user logs in with email "<email>" and password "<password>"
    Then the login should be "<expectedResult>"

    Examples:
	| email                        | password       | expectedResult | 
    | admin@admin.com              | Admin1!        | Success        |
    | invaliduser@example.com      | InvalidPass123 | Failure        |
    | admin@admin.com              | WrongPassword  | Failure        |
    |                              | SomePassword   | Failure        |  # Empty email
    | admin@admin.com              |                | Failure        |  # Empty password
    | nonexistinguser@example.com  | SomePassword   | Failure        |  # Non-existent user
    |                              |                | Failure        |  # Empty email and password
