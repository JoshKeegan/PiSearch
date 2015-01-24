PiSearch
========
PiSearch is a system designed to search through a large number of digits very quickly for a specified string.  
  
The initial aim was to search through the first 2 billion digits of Pi faster than was being achieved (at the time) by http://www.subidiom.com/pi/ on similar consumer grade hardware. This was approximately 15s when searching for a string of digits that was not a substring of the first 2 billion digits of pi (worst case scenario). The same search has now been achieved in under .001s on similar hardware (3rd gen i5).  
  
The projects aims have now changed to  
- Reduce memory usage (currently the system stores all of the digits in RAM ~1GB as well as a suffix array ~8GB)
- Search more digits than can be stored in a signed 32 bit integer without having to jump to storing 64 bit integers
- Investigate the impact of not loading the whole string into memory, but leaving it on disk and only bringing commonly used values into memory (predicted based on the predictable nature of the binary search for hitting certain numbers frequently)

## License ##
Copyright (c) 2014-2015 Josh Keegan

This file is part of PiSearch.

PiSearch is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License version 3 as 
published by the Free Software Foundation.

PiSearch is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PiSearch.  If not, see <http://www.gnu.org/licenses/>.