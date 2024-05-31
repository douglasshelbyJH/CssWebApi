Feature: SampleDelete

Scenario: External service deletes existing sample
	Given sample service has valid delete test data
	When external service deletes an existing sample
	Then the sample should be deleted
