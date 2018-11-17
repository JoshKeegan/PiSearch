using System.ComponentModel.DataAnnotations;

namespace StringSearch.Api.Contracts.Searches
{
    public abstract class SearchRequest
    {
        [Required]
        [RegularExpression("[0-9]+", ErrorMessage = "Must only contain the characters 0-9")]
        [StringLength(1000)]
        public string Find { get; set; }

        /*
         * TODO:
         * - Trim string "Find"
         */
    }
}
