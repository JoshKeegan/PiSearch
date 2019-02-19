/*
	piProbability - calculates the probability of finding a string of digits of a specified length
		in some number of digits of pi
	Authors:
		Josh Keegan 08/02/2015
 */
var piProbability = 
{
	//Variables
	numPiDigits: null,

	//Methods
	init: function(numPiDigits)
	{
		console.log("piProbability.init");

		piProbability.numPiDigits = numPiDigits;
	},

	calculateProbability: function(searchLength, numPiDigits)
	{
		console.log("piProbability.calculateProbability");

		//Cannot search for anything shorter than 1 digit
		if(searchLength < 1)
		{
			return 0;
		}

		if(typeof(numPiDigits) === "undefined" || numPiDigits === null)
		{
			numPiDigits = piProbability.numPiDigits;
		}

		//Calculate the number of different strings of length searchLength can be searched for (purely comprised on the digits 0-9)
		var numPossibleSearches = piProbability.calculateNumPossibleSearches(searchLength);

		//Calculate the probability using the formula:
		//	probability = 1 - 1/(e ^ (numPiDigits / numPossibleSearches))
		//	as derived at http://www.angio.net/pi/whynotpi.html
		//	using the Poisson approximation
		var power = numPiDigits / numPossibleSearches;
		var divisor = Math.pow(Math.E, power);
		var probabilityNot = 1 / divisor;
		var probability = 1 - probabilityNot;
		return probability;
	},

	calculateNumPossibleSearches: function(len)
	{
		console.log("piProbability.calculateNumPossibleSearches");

		return Math.pow(10, len);
	}
};