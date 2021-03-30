# StringSearch.IndexGenerator
Takes a directory of text files (containing digits to be searched!) and generates everything necessary to use them as digits in the API.
This is entirely non-interactive, with the primary use-case being to generate the necessary data for container testing from the raw digits.

For an interactive system with more options, run StringSearchConsole. But note that it was built for development of the algorithm and trying different
things out, most of what is available there isn't what you want for a production system...

## Usage
Run with environment variables (or a .env file containing):
 - `INPUT_DIGITS_DIR`
 - `OUTPUT_DIR`

Ensuring that the output directory is either empty or non-existant.

Any *.txt files in the `INPUT_DIGITS_DIR` path will get a corresponding directory in the output with everything necessary to use it in the API:
 - `{name}.4bitDigit` - The raw digits to be searched compressed to use 4 bits per digit.
 - `{name}.suffixArray.bitAligned` - The suffix array for the digits that gets used for performing the bulk of the search.
 - `precomputed_search_results` directory containing `*.precomputed` files. These contain the precomputed 
	results for every possible search of a given length. They are only generated for searches of short lengths where
	suffix array lookups can be expensive.