@sample-update
Feature: SampleUpdate

Scenario: External service updates existing sample
	Given sample service has valid update test data
	When external service updates an existing sample
	Then the sample should be updated
