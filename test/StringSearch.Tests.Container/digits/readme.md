# Container Test Digits
These are the raw digits that will be accessible by the SUT API in the container tests.
Common constants can gen generated with [y-cruncher](http://www.numberworld.org/y-cruncher), although note that it outputs the digit(s) 
before the decimal point, e.g. 3.141... but StringSearch expects only the decimal part, e.g. 141... so that needs removing.

Before the Container tests can make use of this text file of digits we must do some preprocessing that is handled by 
the StringSearch.IndexGenerator project.