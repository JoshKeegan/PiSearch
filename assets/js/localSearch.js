/*
	localSearch - search the digits of pi that are stored locally (fetched from the server)
	By Josh Keegan 30/01/2015
	Last Edit 07/02/2015
 */
var localSearch = 
{
	//Constants
	DIGITS_URL: "assets/digits/pi/pi_raw_100thou.txt",
	RETURN_NUM_SURROUNDING_DIGITS: 20,
	
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
	compatibleSearch: function(find, from, afterResultId)
	{
		console.log("localSearch.compatibleSearch");

		if(typeof(afterResultId) !== "number")
		{
			afterResultId = -1;
		}

		var startTime = performance.now();
		var result = localSearch.search(find, from);

		//If there is a result, find its surrounding digits
		var surroundingDigits = null;
		if(result !== -1)
		{
			surroundingDigits = localSearch.getSurroundingDigits(result, find.length);
		}

		var endTime = performance.now();

		var toRet = 
		{
			ResultStringIndex: result,
			NumResults: -1,
			ResultId: afterResultId + 1,
			ProcessingTimeMs: Math.max(0, endTime - startTime),
			SurroundingDigits: surroundingDigits
		};
		return toRet;
	},

	getSurroundingDigits: function(idx, len)
	{
		console.log("localSearch.getSurroundingDigits");

		var beforeStartIdx = Math.max(0, idx - localSearch.RETURN_NUM_SURROUNDING_DIGITS);
		var before = "";
		for(var i = beforeStartIdx; i < idx; i++)
		{
			before += localSearch.digits[i];
		}

		var afterStartIdx = Math.min(localSearch.digits.length, idx + len);
		var afterEndIdx = Math.min(localSearch.digits.length, afterStartIdx + localSearch.RETURN_NUM_SURROUNDING_DIGITS);
		var after = "";
		for(var i = afterStartIdx; i < afterEndIdx; i++)
		{
			after += localSearch.digits[i];
		}

		var surroundingDigits = 
		{
			Before: before,
			After: after
		};
		return surroundingDigits;
	}
};