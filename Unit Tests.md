Unit Tests
==========

Unit Tests are hugely important for this project, since many bugs could easily go unnoticed by the used leading to incorrect results.  
PiSearch uses NUnit 3 for its Unit Tests, so you should be able to run them under any NUnit 3 test runner.  
Since PiSearch is designed to use large amounts of RAM, some tests emulate this requiring the UnitTests dll to be built targeting x64. 
Please note that not all NUnit 3 test runners will automatically in x64. e.g. for the NUnit 3 test adapter for Visual Studio you must select Test > Test Settings > Default Processor Architecture > x64.
