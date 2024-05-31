@sample-get
Feature: SampleGet

 Scenario: Api consumer gets valid sample
    Given a sample is available
	When api consumer gets valid sample
	Then an expected response is returned
