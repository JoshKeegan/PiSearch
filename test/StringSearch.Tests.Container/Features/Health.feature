Feature: Health
	Ensure the SUT thinks it is healthy

Background: 
Given the API version is 'v1'

Scenario: SUT Health
	When I request the system health
	Then all critical resources should be healthy
	And the HTTP Status code should be 'Ok'