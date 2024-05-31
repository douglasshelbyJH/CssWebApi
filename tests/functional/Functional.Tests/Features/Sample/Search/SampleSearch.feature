# This tag is used for both Hooks and Categorization
@sample-search
Feature: SampleSearch

In order for businesses to manage samples
As an external service
I want to be able to search for samples

 Scenario Outline: External service searches for valid sample
	Given sample service has valid search test data
	When external service searches for sample with name <name>
	Then sample with name <name> should be returned
	Examples: 
		| name |
		| "searchSample1" |
		| "searchSample2" |
		| "searchSample3" |
