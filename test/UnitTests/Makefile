#
# Unit Tests Makefile
#

run:
# If not on windows, set a filter to exclude the windows-specific tests
ifeq ($(shell uname), Linux)
	$(eval testFilter = --filter TestCategory!=windows)
endif

	dotnet test \
		-o out \
		--no-build \
		$(testFilter) \
		--logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.trx \
		--logger:console;verbosity=detailed