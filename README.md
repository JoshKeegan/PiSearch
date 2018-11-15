# PiSearch
PiSearch is a system designed to search through a large number of digits very quickly for a specified string.  
To see it in action, go to [PiSearch](http://pisearch.joshkeegan.co.uk/) to search through the first 5 billion digits of Pi online.

## CI Pipelines:  
Appveyor: [![Build status](https://ci.appveyor.com/api/projects/status/nh3pv5yqt5nn0wby?svg=true)](https://ci.appveyor.com/project/JoshKeegan/pisearch)  
GitLab: [![GitLab CI status](https://gitlab.com/JoshKeegan/PiSearch/badges/master/pipeline.svg)](https://gitlab.com/JoshKeegan/PiSearch/commits/master)

## Docs
- [Hosting the API](StringSearch.Api/hosting.md)
- [Unit Tests](unitTests.md)
  
## Aims
### Aims of the project that have been met:  
- Search through the first 2 billion digits of Pi faster than was being achieved (at the time) by http://www.subidiom.com/pi/ (a popular website allowing you to search the first 2 billion digits of pi).  
- Reduce array memory usage (RAM or HDD) whilst maintaining linear access times in order to reduce RAM and HDD storage requirements.  
- Search from the hard drive (without loading all data into memory) to allow for the system to be deployed on low-end server hardware with very little RAM.  
- Search more digits than can be stored in a signed 32 bit integer without having to jump to storing 64 bit integers  
- Worst-case performance of the main search algorithm happens for small strings when there will be lots of results. Now suffix array min & max indices can be pre-computed for strings up to length n (where the value of n would be chosen manually and would depend on the number of digits to be searched). These are then be stored separately to the suffix array and when doing a search for a small string the pre-computed values are used rather than performing an expensive search.  
- API to process search requests for the digits of pi (hosted at http://api.pisearch.joshkeegan.co.uk/ with 5 billion digits of Pi and used by website http://pisearch.joshkeegan.co.uk/). The source code for this is currently private, if you are interested please contact me. The API also returns the min & max suffix array indices for the search as well as the index of the first value, as then if the user wishes to retrieve further values for the same search the server needn't perform the suffix array search again.  
  
### Future Project Goals:  
- Perform performance analysis of the search algorithm to identify areas for improvement.  
- Refactor PiSearch code.
- Documentation for: generating suffix array, performing search with suffix array, pre-computing suffix array indices for searches up to length n, checking if all strings of digits up to length n exist in some digits.
  
## License
Copyright (c) 2014-2018 Josh Keegan

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
