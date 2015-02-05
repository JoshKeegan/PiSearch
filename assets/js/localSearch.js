/*
	localSearch - search the digits of pi that are stored locally (fetched from the server)
	By Josh Keegan 30/01/2015
	Last Edit 05/02/2015
 */
var localSearch = 
{
	//Constants
	DIGITS_URL: "assets/digits/pi/pi_raw_100thou.txt",
	
	//Variables
	digits: null,
	
	//Methods
	init: function()
	{
		console.log("localSearch.init");

		//Fetch the first digits of pi from the server
		this.fetchDigits();
	},

	fetchDigits: function()
	{
		console.log("localSearch.fetchDigits");

		$.ajax(
		{
			type: "GET",
			async: true,
			url: localSearch.DIGITS_URL,
			cache: true,
			dataType: "text",
			success: function(data)
			{
				localSearch.digits = data;
			}
		});
	},

	search: function(find, from)
	{
		console.log("localSearch.search");

		var digits = localSearch.digits;

		//If the digits have been downloaded from the server
		if(digits !== null)
		{
			if(typeof(from) !== "number")
			{
				from = 0;
			}

			//find must be a string
			if(typeof(find) !== "string");
			{
				if(typeof(find) === "number")
				{
					find = find.toString();
				}
				else if(typeof(find) === "object")
				{
					//use toString() if there is one
					if("toString" in find && typeof(find.toString) === "function")
					{
						find = find.toString();
					}
					else //no toString()
					{
						throw "find must be a string or have a toString() method";
					}
				}
			}

			var lastStartIdx = digits.length - find.length;

			//Check there is enough digits left to be searched
			if(lastStartIdx < from)
			{
				return -1;
			}

			//Initial fill of digitsQueue
			var digitsQueue = [];
			
			for(var i = 0; i < find.length; i++)
			{
				var digit = digits[i + from];
				digitsQueue.push(digit);
			}

			for(var i = from; true; i++)
			{
				//Do the digits at the current position match the ones in the queue
				var match = true;
				for(var j = 0; j < digitsQueue.length; j++)
				{
					if(digitsQueue[j] !== find[j])
					{
						match = false
						break;
					}
				}

				if(match)
				{
					return i;
				}

				//If there is another iteration yet to go, update the digits queue
				if(i < lastStartIdx)
				{
					digitsQueue.shift();
					digitsQueue.push(digits[i + find.length]);
				}
				else //Otherwise break
				{
					break;
				}
			}

			//Not found
			return -1;
		}
		else //Otherwise the digits haven't been fetched yet
		{
			throw "Digits not fetched yet";
		}
	},

	//Search whose results match the format of the remote search
	compatibleSearch: function(find, from)
	{
		var result = localSearch.search(find, from);

		var toRet = 
		{
			ResultStringIndex: result,
			NumResults: -1,
			ResultId: 0,
			ProcessingTimeMs: -1 //TODO
		};
		return toRet;
}
};