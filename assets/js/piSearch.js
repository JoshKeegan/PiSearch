/*
	piSearch - Main entry point for searching pi on the website
	By Josh Keegan 30/01/2015
	Last Edit 14/03/2015
 */
var piSearch = 
{
	//Constants
	NUM_DIGITS: 5000000000,
	ORDINALS: 
	[
		"st",
		"nd",
		"rd"
	],
	
	//Variables
	
	//Methods
	init: function()
	{
		console.log("piSearch.init");

		piProbability.init(piSearch.NUM_DIGITS);
		localSearch.init();
		remoteSearch.init();

		piSearch.bindEvents();

		//Enable the bootrap tooltip plugin
		$('[data-toggle="tooltip"]').tooltip();
	},

	bindEvents: function()
	{
		//Listen for the content of the search input changing so we can display the probability of finding the entered string
		$("#searchFor").on("input propertychange paste", function()
		{
			var len = $(this).val().length;
			var probability = piProbability.calculateProbability(len);
			var percentage = probability * 100;
			var userFriendlyPercentage;
			if(percentage !== 0 && percentage < 0.01)
			{
				userFriendlyPercentage = "< 0.01";
			}
			else
			{
				userFriendlyPercentage = numberHelpers.roundWithoutTrailingZeros(percentage, 2)
			}

			$(".searchHitProbability").html(userFriendlyPercentage);
			$("#searchHitProbabilityProgressBar")
				.attr("valuenow", percentage)
				.css("width", percentage + "%");

			//TODO: Cleverer solution so that it changes colour as the animation goes over it??
			$("#searchHitProbabilityProgress").css("color", (percentage > 50 ? "#fff" : "#000"));
		});
	},

	search: function(find, fromStringIdx, afterResultId)
	{
		console.log("piSearch.search");

		if(typeof(find) === "undefined" || find === null)
		{
			var find = $("#searchFor").val();

			//Validation
			if(find == "")
			{
				piSearch.errorMessage("Enter some digits to search for", "Validation", "#searchFor");
				return;
			}
			else if(!/^[0-9]+$/.test(find))
			{
				piSearch.errorMessage("Can only search for a string of decimal digits (0-9)", "Validation", "#searchFor");
				return;
			}
		}

		if(typeof(afterResultId) === "undefined" || afterResultId === null)
		{
			afterResultId = -1;
		}
		else if(typeof(afterResultId) !== "number" || afterResultId % 1 !== 0)
		{
			throw "afterResultId must be an integer number";
		}

		if(typeof(fromStringIdx) === "undefined" || fromStringIdx === null)
		{
			fromStringIdx = 0;
		}
		else if(typeof(fromStringIdx) !== "number" || fromStringIdx % 1 !== 0)
		{
			throw "fromStringIdx must be an integer number";
		}

		piSearch.setLoading(true);

		//Disable the find next button
		$("#btnFindNext").prop("disabled", true);

		var localResult = localSearch.compatibleSearch(find, fromStringIdx, afterResultId);

		//If a result was found locally
		if(localResult.ResultStringIndex !== -1)
		{
			//TODO: If the digits are found at the very end of what's available locally, the surrounding digits after should be fetched from the server
			piSearch.displayResult(find, localResult);

			//Count the number of times this string occurs in the first 5 billion digits on the server
			remoteSearch.search(find, null, true, function(result)
			{
				piSearch.displayCountAndProcessingTime(result);

				piSearch.setLoading(false);
			},
			function(strError)
			{
				piSearch.setLoading(false);

				piSearch.errorMessage(strError, "API Error");
			});
		}
		else //Otherwise it wasn't in the local digits, send the request to the PiSearch API
		{
			remoteSearch.search(find, afterResultId + 1, false, function(result)
			{
				piSearch.displayResult(find, result);

				piSearch.setLoading(false);
			},
			function(strError)
			{
				piSearch.setLoading(false);

				piSearch.errorMessage(strError, "API Error");
			});
		}

		//Tell analytics about this search
		//	Category: search
		//	Action: local (if found) or remote (if not found locally, so a request will be sent to the server)
		//	Label: The string being searched for
		//	Value: The ResultID being fetched
		ga("send", "event", "search", (localResult.ResultStringIndex !== -1 ? "local" : "remote"), find, afterResultId + 1);	
	},

	searchNext: function()
	{
		console.log("piSearch.searchNext");

		var find = $("#searchResultDigitsFound").html();
		var strResultId = numberHelpers.removeCommas($("#searchResultOccurrenceNumber").html());
		var strSearchResultIdx = numberHelpers.removeCommas($("#searchResultIndex").html());

		var resultId = parseInt(strResultId) - 1;
		var from = parseInt(strSearchResultIdx);

		piSearch.search(find, from, resultId);
	},

	displayResult: function(find, result)
	{
		console.log("piSearch.displayResult");
		console.log(result);

		//TODO: UI Improvements
		$("#noSearchPerformed").addClass("hide");
		if(result.NumResults === 0)
		{
			$("#searchResult").addClass("hide");
			$("#searchNoResults").removeClass("hide");
		}
		else //There are results
		{
			$("#searchResultIndex").html(numberHelpers.insertCommas(result.ResultStringIndex + 1));
			$("#searchResultOrdinal").html(piSearch.getOrdinal(result.ResultStringIndex + 1));

			$("#searchNoResults").addClass("hide");
			$("#searchResult").removeClass("hide");
		}

		//Surrounding digits
		var digitsBefore = "";
		var digitsFound = "";
		var digitsAfter = "";
		if(result.SurroundingDigits !== null)
		{
			digitsBefore = result.SurroundingDigits.Before;
			digitsFound = find;
			digitsAfter = result.SurroundingDigits.After;
		}
		$("#searchResultDigitsBefore").html(digitsBefore);
		$("#searchResultDigitsFound").html(digitsFound);
		$("#searchResultDigitsAfter").html(digitsAfter);

		//Occurrence Number
		$("#searchResultOccurrenceNumber").html(result.ResultId + 1);

		//Set the current processing time to 0, as it gets added to
		$("#searchResultProcessingTimeMs").html(0);

		piSearch.displayCountAndProcessingTime(result);
	},

	displayCountAndProcessingTime: function(result)
	{
		//If a local search has been performed, NumResults is set to -1
		var strNumResults = result.NumResults < 0 ? "Loading . . ." : numberHelpers.insertCommas(result.NumResults);
		$("#searchResultNumResults").html(strNumResults);

		//Get the existing processing time
		var currProcTime = parseFloat(numberHelpers.removeCommas($("#searchResultProcessingTimeMs").html()));
		//Add this processing time to it
		var procTime = currProcTime + result.ProcessingTimeMs;
		//Display the summed procesing time
		$("#searchResultProcessingTimeMs").html(numberHelpers.insertCommas(procTime.toFixed(0)));

		//If there are more results to be displayed after this one, enable the find next occurrence button
		var resultId = parseInt(numberHelpers.removeCommas($("#searchResultOccurrenceNumber").html())) - 1;
		if(resultId != result.NumResults - 1)
		{
			$("#btnFindNext").prop("disabled", false);
		}
	},

	errorMessage: function(strError, strTitle, focusSelector)
	{
		console.log("piSearch.errorMessage");

		if(typeof(strTitle) !== "string")
		{
			strTitle = "Error";
		}

		console.log(strTitle + ": " + strError);

		//If a focus selector has been supplied, give that element focus
		if(typeof(focusSelector) !== "undefined")
		{
			$(focusSelector).focus();
		}

		//TODO: Sexy UI
		alert(strError);
	},

	setLoading: function(isLoading)
	{
		//TODO: Loading icon, block inputs etc...
		var strLoading = isLoading ? "Loading . . ." : "";

		$("#loadingStatus").html(strLoading);
	},

	//Get the ordinal of an integer number.
	//	Logic from http://cwestblog.com/2012/09/28/javascript-number-getordinalfor/
	getOrdinal: function(num)
	{
		return piSearch.ORDINALS[(((num = Math.abs(num % 100)) - 20) % 10) - 1] || piSearch.ORDINALS[num - 1] || "th";
	}
};