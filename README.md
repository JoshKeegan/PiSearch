PiSearch
========
PiSearch is a system designed to search through a large number of digits very quickly for a specified string.  
  
The initial was to search through the first 2 billion digits of Pi faster than was being achieved (at the time) by http://www.subidiom.com/pi/ on similar consumer grade hardware  
This has been achieved, and the projects aims have changed to  
- Reduce memory usage  
- Search more digits than can be stored in a signed 32 bit integer without having to jump to storing 64 bit integers
- Investigate the impact of not loading the whole string into memory, but leaving it on disk and only bringing commonly used values into memory (predicted based on the predictable nature of the binary search for hitting certain numbers frequently)
