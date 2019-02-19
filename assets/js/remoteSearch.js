/*
	remoteSearch - search the digits of pi by sending the request to a remote API
	Authors:
		Josh Keegan 30/01/2015
 */
var remoteSearch = 
{
	// Constants
	API_URL: "https://v2.api.pisearch.joshkeegan.co.uk/",
	//API_URL: "http://localhost:5000/",
	COUNT_EP: "api/v1/Count",
	LOOKUP_EP: "api/v1/Lookup",
	NAMED_DIGITS: "pi",
	
	// Variables
	prevSearchResults: {  },
	
	// Methods
	init: function()
	{
		console.log("remoteSearch.init");
	},

	search: function(find, resultId, justCount, successCallback, failureCallback)
	{
		console.log("remoteSearch.search");

		// If it hasn't been specified which result to get for this search, get the first
		if (typeof(resultId) === "undefined")
		{
			resultId = 0;
		}
		else if (!justCount && (typeof(resultId) !== "number" || resultId % 1 !== 0))
		{
			throw "resultId must be an integer number when not just counting";
		}

		// Check the cache
		var cachedResult = remoteSearch.getPrevResult(find, resultId);

		// Cache hit
		if(cachedResult !== null)
		{
			successCallback(cachedResult);
		}
		else // Cache miss
		{
			// If we've searched for this string before (not for this resultId) then check that the resultId we're requesting exists
			var prevResults = remoteSearch.getPrevResults(find);
			if (prevResults !== null && prevResults.numResults <= resultId)
			{
				// We know this doesn't exist
				failureCallback("max possible resultId for the minSuffixArrayIdx and maxSuffixArrayIdx is " + (prevResults.NumResults - 1));
			}
			else // Otherwise we don't know any information client-side that would make this a wasted request
			{
				$.ajax(
				{
					type: "GET",
					data: remoteSearch.buildData(find, resultId, justCount),
                    async: true,
                    url: remoteSearch.API_URL + (justCount ? remoteSearch.COUNT_EP : remoteSearch.LOOKUP_EP),
					cache: true,
					dataType: "json",
					success: function(json)
                    {
						// Store this search result so that it can be used to help reduce API load for future searches
						remoteSearch.storeSearchResult(find, json);

						// Use getPrevResult as the json may be changed around a bit to what we give to the callback & this ensures consistency
						var result = remoteSearch.getPrevResult(find, resultId);

						successCallback(result);
					},
					error: function(jqXhr, strStatus, e)
					{
                        console.log("Error when requesting a search from the PiSearch API");

                        // If we got back a JSON error, display that message
                        if ("responseJSON" in jqXhr &&
                            jqXhr.responseJSON !== null &&
                            "Error" in jqXhr.responseJSON &&
                            jqXhr.responseJSON.Error !== null)
                        {
                            var userFriendlyError = remoteSearch.getUserFriendlyError(jqXhr.responseJSON.Error);
                            failureCallback(userFriendlyError);
                        }
                        // Otherwise we didn't get a nice error message back, display a generic message.
                        else
                        {
                            console.log(jqXhr);
                            console.log(strStatus);
                            console.log(e);

                            failureCallback("Whoops! An error occurred whilst querying the PiSearch API");
                        }
					}
				});
			}
		}
	},

	storeSearchResult: function(find, result)
	{
		console.log("remoteSearch.storeSearchResult");

		// If we've never searched for find before, store the results for find
		if (!(find in remoteSearch.prevSearchResults))
		{
			remoteSearch.prevSearchResults[find] = 
			{
				minSuffixArrayIdx: result.minSuffixArrayIdx,
				maxSuffixArrayIdx: result.maxSuffixArrayIdx,
				numResults: result.numResults,
				processingTimeMs: result.processingTimeMs
			};
		}

		// If we have data for a specific result ID (i.e. we did a Lookup), store them
		if ("resultId" in result)
		{
			remoteSearch.prevSearchResults[find][result.resultId] =
			{
				resultStringIdx: result.resultStringIdx,
				surroundingDigits: result.surroundingDigits,
				processingTimeMs: result.processingTimeMs
			};
		}
	},

	getPrevResults: function(find)
	{
		console.log("remoteSearch.getPrevResults");

		if (find in remoteSearch.prevSearchResults)
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

		// If doing a count, just return the results (which contains all fields that a count would)
		if (typeof (resultId) === "undefined" || resultId === null)
		{
			return results;
		}

		// Otherwise, if we have results for this ID
		if (results !== null && resultId in results)
		{
			var lookupSpecific = results[resultId];
			var toRet =
			{
				// Properties shared between all results for this string
				minSuffixArrayIdx: results.minSuffixArrayIdx,
				maxSuffixArrayIdx: results.maxSuffixArrayIdx,
				numResults: results.numResults,

				// Properties specified to this result ID
				resultStringIdx: lookupSpecific.resultStringIdx,
				surroundingDigits: lookupSpecific.surroundingDigits,
				processingTimeMs: lookupSpecific.processingTimeMs,

				// Result ID (not kept on the object to save on memory)
				resultId: resultId
			};
			return toRet;
        }

		return null;
	},

	buildData: function(find, resultId, justCount)
	{
		console.log("remoteSearch.buildData");

		var postData =
        {
			namedDigits: remoteSearch.NAMED_DIGITS,
			find: find,
			resultId: resultId
		};

		// If there have been previous results for this search, include the additional suffix array information that will have been returned by those searches
		var prevResults = remoteSearch.getPrevResults(find);
		if (prevResults !== null)
        {
            postData.minSuffixArrayIdx = prevResults.minSuffixArrayIdx;
            postData.maxSuffixArrayIdx = prevResults.maxSuffixArrayIdx;
		}
		
		return postData;
	},

	getUserFriendlyError: function(error)
    {
		// In v2 of the API, all limitations have been removed.
		//	Now all known conditions *should* be being checked client-side,
		//	so anything server-side is unexpected, which we won't have a friendly name for.
		//	Have left this here in case it's required though.
		return error;
	}
};