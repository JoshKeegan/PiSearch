/*
	remoteSearch - search the digits of pi by sending the request to a remote API
	By Josh Keegan 30/01/2015
	Last Edit 05/02/2015
 */
var remoteSearch = 
{
	//Constants
	API_URL: "http://api.pisearch.joshkeegan.co.uk/index.aspx",
	
	//Variables
	prevSearchResults: {  },
	
	//Methods
	init: function()
	{
		console.log("remoteSearch.init");

		//IE8 Compatibility: Enable CORS support since the pisearch API is on another domain
		$.support.cors = true;
	},

	search: function(find, resultId, successCallback, failureCallback)
	{
		console.log("remoteSearch.search");

		//If it hasn't been specified which result to get for this search, get the first
		if(typeof(resultId) === "undefined")
		{
			resultId = 0;
		}

		//Check the cache
		var cachedResult = remoteSearch.getPrevResult(find, resultId);

		//Cache hit
		if(cachedResult !== null)
		{
			successCallback(cachedResult);
		}
		else //Cache miss
		{
			//If we've searched for this string before (not for this resultId) then check that the resultId we're requesting exists
			var prevResults = remoteSearch.getPrevResults(find);
			if(prevResults !== null && prevResults.NumResults <= resultId)
			{
				//We know this doesn't exist
				failureCallback("max possible resultId for the minSuffixArrayIdx and maxSuffixArrayIdx is " + (prevResults.NumResults - 1));
			}
			else //Otherwise we don't know any information client-side that would make this a wasted request
			{
				$.ajax(
				{
					type: "POST",
					data: remoteSearch.buildPostData(find, resultId),
					async: true,
					url: remoteSearch.API_URL,
					cache: true,
					dataType: "json",
					success: function(json)
					{
						//If something went wrong
						if("Error" in json)
						{
							console.log(json);
							failureCallback(json.Error);
						}
						else //Otherwise the search was a success
						{
							//Store this search result so that it can be used to help reduce API load for future searches
							remoteSearch.storeSearchResult(find, json);

							//Use getPrevResult as the json may be changed around a bit to what we give to the callback & this ensures consistency
							var result = remoteSearch.getPrevResult(find, resultId);

							successCallback(result);
						}
					},
					error: function(jqXhr, strStatus, e)
					{
						console.log("Error when requesting a search from the PiSearch API");
						console.log(jqXhr);
						console.log(strStatus);
						console.log(e);

						//TODO: Check why this request failed in order to provide a more informative error message
						failureCallback("Whoops! An error occurred whilst querying the PiSearch API");
					}
				});
			}
		}
	},

	storeSearchResult: function(find, result)
	{
		console.log("remoteSearch.storeSearchResult");

		//If we've never searched for find before, store the results for find
		if(!(find in remoteSearch.prevSearchResults))
		{
			remoteSearch.prevSearchResults[find] = 
			{
				SuffixArrayMinIdx: result.SuffixArrayMinIdx,
				SuffixArrayMaxIdx: result.SuffixArrayMaxIdx,
				NumResults: result.NumResults
			};
		}

		//Store the results for this resultId in find
		remoteSearch.prevSearchResults[find][result.ResultId] = 
		{
			NumResults: result.NumResults,
			ResultStringIndex: result.ResultStringIndex,
			ProcessingTimeMs: result.ProcessingTimeMs
		};
	},

	getPrevResults: function(find)
	{
		console.log("remoteSearch.getPrevResults");

		if(find in remoteSearch.prevSearchResults)
		{
			return remoteSearch.prevSearchResults[find];
		}
		else
		{
			return null;
		}
	},

	getPrevResult: function(find, resultId)
	{
		console.log("remoteSearch.getPrevResult");

		var results = remoteSearch.getPrevResults(find);

		if(results !== null && resultId in results)
		{
			return results[resultId];
		}

		return null;
	},

	buildPostData: function(find, resultId)
	{
		console.log("remoteSearch.buildPostData");

		var postData =
		{
			find: find,
			resultId: resultId
		};

		//If there have been previous results for this search, include the additional suffix array information that will have been returned by those searches
		var prevResults = remoteSearch.getPrevResults(find);
		if(prevResults !== null)
		{
			postData.minSuffixArrayIdx = prevResults.SuffixArrayMinIdx;
			postData.maxSuffixArrayIdx = prevResults.SuffixArrayMaxIdx;
		}
		
		return postData;
	}
};