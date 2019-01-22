using System.ComponentModel.DataAnnotations;
using StringSearch.Api.Attributes.Binding;

namespace StringSearch.Api.Contracts.Searches
{
    public abstract class SearchRequest
    {
        [Required]
        [RegularExpression("[0-9]+", ErrorMessage = "Must only contain the characters 0-9")]
        [StringLength(1000)]
        [Trim]
        public string Find { get; set; }
    }
}
