PiSearch
========
PiSearch is a system designed to search through a large number of digits very quickly for a specified string.  
  
The initial aim was to search through the first 2 billion digits of Pi faster than was being achieved (at the time) by http://www.subidiom.com/pi/ on similar consumer grade hardware. This was approximately 15s when searching for a string of digits that was not a substring of the first 2 billion digits of pi (worst case scenario). The same search has now been achieved in under .001s on similar hardware (3rd gen i5).  
  
The projects aims have now changed to  
- Reduce memory usage (currently the system stores all of the digits in RAM ~1GB as well as a suffix array ~8GB)
- Search more digits than can be stored in a signed 32 bit integer without having to jump to storing 64 bit integers
- Investigate the impact of not loading the whole string into memory, but leaving it on disk and only bringing commonly used values into memory (predicted based on the predictable nature of the binary search for hitting certain numbers frequently)
