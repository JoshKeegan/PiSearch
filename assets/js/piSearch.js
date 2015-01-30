/*
	piSearch - Main entry point for searching pi on the website
	By Josh Keegan 30/01/2015
 */
var piSearch = 
{
	//Constants
	
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

		var localResult = localSearch.search(find);

		$("#searchResultIndex").html(localResult);
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