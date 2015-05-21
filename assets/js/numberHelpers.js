/*
	numberHelpers - helper methods for manipulating numbers
	By Josh Keegan 05/02/2015
	Last Edit 21/05/2015
 */
var numberHelpers = 
{
	insertCommas: function(numStr)
	{
		console.log("numberHelpers.insertCommas");

		//If this is a value that this function shouldn't be operating on, don't
		if(!$.isNumeric(numStr))
		{
			console.log("Not a valid input for insertCommas");
			console.log(numStr);
			return numStr;
		}

		//Force a string
		numStr += "";

		var parts = numStr.split('.');
		var before = parts[0];
		var after = "";
		if(parts.length > 1)
		{
			after = parts[1];
		}

		//Do not check the first digit (don't want number to start with a comma)
		var commaBefore = "";
		var digit = 0;
		for(var i = before.length - 1; i > 0; i--)
		{
			digit++;
			commaBefore = before.charAt(i) + commaBefore;
			if(digit % 3 === 0)
			{
				commaBefore = "," + commaBefore;
			}
		}
		//If there is at least one character before the decimal point, the first one won't have been put in by the loop
		if(before.length > 0)
		{
			commaBefore = before.charAt(0) + commaBefore;
		}
		else //Otherwise there is not part to the number before the decimal place, replace this with 0
		{
			commaBefore = "0";
		}

		var numWithCommas = commaBefore;
		if(after !== "")
		{
			numWithCommas += "." + after;
		}
		return numWithCommas;
	},

	removeCommas: function(numStr)
	{
		console.log("numberHelpers.removeCommas");

		//TODO: Validation
		return numStr.replace(",", "");
	},

	roundWithoutTrailingZeros: function(num, numDecimalPlaces)
	{
		console.log("numberHelpers.roundWithoutTrailingZeros");

		//Ensure that the value is a float before performing any numerical operations
		var floatNum = parseFloat(num);

		//Round it to the correct number of decimal places
		var rounded = floatNum.toFixed(numDecimalPlaces);

		//Remove trailing zeroes
		rounded = parseFloat(rounded);

		return rounded;
	}
};