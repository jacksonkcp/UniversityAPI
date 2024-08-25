using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace UniversityAPI.Models
{
    public class University
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "The Country field is required.")]
        public string? Country { get; set; }

        [Required(ErrorMessage = "The Webpages field is required.")]
        public List<string?>? Webpages { get; set; }

        public bool IsBookmark { get; set; }

        public bool IsActive { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? LastModified { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
