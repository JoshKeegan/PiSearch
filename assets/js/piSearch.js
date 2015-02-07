/*
	piSearch - Main entry point for searching pi on the website
	By Josh Keegan 30/01/2015
	Last Edit 07/02/2015
 */
var piSearch = 
{
	//Constants
	NUM_DIGITS: 5000000000,
	
	//Variables
	
	//Methods
	init: function()
	{
		console.log("piSearch.init");
	},

	search: function()
	{
		console.log("piSearch.search");

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

		piSearch.setLoading(true);

		var localResult = localSearch.compatibleSearch(find);

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
			remoteSearch.search(find, 0, false, function(result)
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
	},

	displayResult: function(find, result)
	{
		console.log("piSearch.displayResult");
		console.log(result);

		//TODO: UI
		if(result.NumResults === 0)
		{
			$("#searchResultIndex").html("Not found in the first " + numberHelpers.insertCommas(piSearch.NUM_DIGITS) + " digits of Pi");
		}
		else //There are results
		{
			$("#searchResultIndex").html(numberHelpers.insertCommas(result.ResultStringIndex));
		}

		//Surrounding digits
		var digitsBefore = "";
		var digitsAfter = "";
		if(result.SurroundingDigits !== null)
		{
			digitsBefore = result.SurroundingDigits.Before;
			digitsAfter = result.SurroundingDigits.After;
		}
		$("#searchResultDigitsBefore").html(digitsBefore);
		$("#searchResultDigitsFound").html(find);
		$("#searchResultDigitsAfter").html(digitsAfter);

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
	}
};