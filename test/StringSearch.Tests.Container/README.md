# Container Tests
The idea behind container tests is to be a suite of black-box tests run against a running container of the built docker image that is to be published and later used in production. This verifies that the container starts, listens for web requests and integrates with infrastructure (e.g. files, DB) etc... all during CI. It has a very broad scope, allowing for e2e-style tests to be written that we can use to sense-check that the overall system is working together as intended.

This type of test is designed to be particularly valueable when changing components in use (e.g. switching to a more efficient search algorithm), or changing parts of the image (e.g. a dotnet SDK upgrade or nuget). In what appear to be trivial changes it's easy for bugs to creep in and nothing to protect it from reaching production, but this test suite should catch such issues.

## Not E2E
The difference with e2e tests that allows for this to be run during CI and not need an environment is that if there were external dependencies they would be faked. We're only interested in testing this container, not the fully deployed stack. For this standalone API that is a subtle difference as the only thing that is really out of scope is the nginx reverse proxy infront of the API. For more complex systems the scope difference can mean much of the overall system is not included (e.g. if you had 100 microservices and this container is 1 then the other 99 are not in scope).

## Running Locally
Generally these will be run in CI. You can also run them locally. This will require docker, docker-compose & make.

### Within an IDE
TODO: Document running within an IDE (common use case for developing new test cases without leaving the IDE).

### From the terminal
From the root of the project run `make publish-api publish-index-generator build-api-image` to compile the code, tests & build the SUT docker image. 
Change directory to `test/StringSearch.Tests.Container` and run `make run-e2e` to run the entire (e2e) flow of container tests.