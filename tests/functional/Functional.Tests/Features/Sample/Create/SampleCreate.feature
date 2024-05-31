# This tag is used for both Hooks and Categorization
@sample-create
Feature: SampleCreate

Scenario: External service creates valid sample
	When service requests to create a valid sample
	Then a valid sample should be created
