# Container Testing Contracts
Ideally the contracts would be plain DTOs with no logic, and separated from any domain models.
That would then allow these tests to reference the contracts project & use that.  
In order to write the initial system more quickly this wasn't done for this API, so separate DTO contracts are here for things we want to test.

Note that even if we just decided to reference the entire API from the container tests, since
some contracts reference domain models and they then reference interfaces (in the case of health check response), deserialisation wouldn't be straight forward.