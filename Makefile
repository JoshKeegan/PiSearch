UNAME := $(shell uname)#

clean:
	rm -r */out || true
	rm -r */bin || true
	rm -r */obj || true
	rm -r artefacts || true
	
	mkdir -p artefacts/testResults

#
# Build
#

build: clean
	dotnet build -c Release

unit-tests:
# If not on windows, set a filter to exclude the windows-specific tests
ifeq ($(UNAME), Linux)
	testFilter = --filter TestCategory!=windows#
endif
	
	dotnet test \
		-c Release \
		--no-build \
		$(testFilter) \
		--logger:trx\;logfilename=../../artefacts/testResults/UnitTests.trx \
		UnitTests

publish-api: build
	dotnet publish -c Release --no-build -o out StringSearch.Api