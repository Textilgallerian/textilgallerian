﻿@login
Feature: Logout
	In order to logout
	As a currently logged in user
	I want to be logged out

Scenario: Logout
	Given I am on the add new discount page
	When I press "Logga ut"
	Then I should be logged out