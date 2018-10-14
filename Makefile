UNAME := $(shell uname)#

build:
	dotnet build

unit-tests-windows:
	dotnet test --logger:trx\;logfilename=UnitTests.trx UnitTests

unit-tests-linux:
	dotnet test --filter TestCategory!=windows --logger:trx\;logfilename=UnitTests.trx UnitTests

ifeq ($(UNAME), Linux)
unit-tests: unit-tests-linux
else
unit-tests: unit-tests-windows
endif