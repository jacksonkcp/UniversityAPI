using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace UniversityAPI
{

    public class UniversityDto
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public string? Country { get; set; }
    }
}
