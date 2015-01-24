PiSearch
========
PiSearch is a system designed to search through a large number of digits very quickly for a specified string.  
  
Aims of the project that have been met:  
- Search through the first 2 billion digits of Pi faster than was being achieved (at the time) by http://www.subidiom.com/pi/ (a popular website allowing you to search the first 2 billion digits of pi).  
- Reduce array memory usage (RAM or HDD) whilst maintaining linear access times in order to reduce RAM and HDD storage requirements.  
- Search from the hard drive (without loading all data into memory) to allow for the system to be deployed on low-end server hardware with very little RAM.  
- Search more digits than can be stored in a signed 32 bit integer without having to jump to storing 64 bit integers  
  
Future Project Goals:  
- Perform performance analysis of the search algorithm to identify areas for improvement.  
- Worst-case performance of the current search algorithm happens for small strings when there will be lots of results. Allow for suffix array min & max indices to be pre-computed for strings up to length n (where the value of n would be chosen manually and would depend on the number of digits to be searched). These can then be stored and when doing a search for a small string the pre-computed values would be used rather than performing an expensive search.  
- Create a simple API endpoint to receive search requests that can be used to run a website where users could search the digits of pi. Such an API should also return the min & max suffix array indices for the search as well as the index of the first value, as then if the user wishes to retrieve further values for the same search the server needn't perform the suffix array search again.  
  
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