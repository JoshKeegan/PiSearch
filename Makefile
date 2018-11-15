UNAME := $(shell uname)#

clean:
	rm -r */out || true
	rm -r */bin || true
	rm -r */obj || true
	rm -r UnitTests/TestResults || true

build: clean
	dotnet build -c Release

unit-tests-windows:
	dotnet test -c Release --no-build --logger:trx\;logfilename=UnitTests.trx UnitTests

unit-tests-linux:
	dotnet test -c Release --no-build --filter TestCategory!=windows --logger:trx\;logfilename=UnitTests.trx UnitTests

ifeq ($(UNAME), Linux)
unit-tests: unit-tests-linux
else
unit-tests: unit-tests-windows
endif

publish-api: build
	dotnet publish -c Release --no-build -o out StringSearch.Api