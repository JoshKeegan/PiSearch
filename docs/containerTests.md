# Container Tests
The purpose of container testing is to run the same container that will be used in production and
then run tests against it. This means that no part of the system is being faked, and the exact compiled code, 
runtime version and environment are being excercised by the tests, giving as much confidence as possible
that what is being deployed works as intended.

Since the system has external dependencies, these must also be run for the SUT container to use.
They are:
 - MySQL DB
 - Digits to search

## Running in the pipeline
Container tests are incorporated into the Gitlab CI pipeline, to be run on every single commit. That allows for confidence 
in builds working before changes get merged to master.

TODO: Not sure Gitlab will trigger on PRs from outside of this repo (think dependabot). Would be nice to consider GH Actions...

## Running locally
```bash
# Build code & build API docker image
make clean publish-api publish-container-tests build-api-image

# Run Container Tests
cd test/StringSearch.Tests.Container
make run-e2e
```