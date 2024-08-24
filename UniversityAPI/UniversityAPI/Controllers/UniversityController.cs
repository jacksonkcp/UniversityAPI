using Microsoft.AspNetCore.Mvc;

namespace UniversityAPI.Controllers
{
    [ApiController]
    [Route("api/university")]
    public class UniversityController : ControllerBase
    {
        private readonly ILogger<UniversityController> _logger;

        public UniversityController(ILogger<UniversityController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetUniversity")]
        public IEnumerable<University> Get()
        {
            string format = "dd-MM-yyyy";

            return new List<University>()
            {
                new University {
                    Id = 1,
                    Name = "NUS",
                    Country = "Singapore",
                    Webpages = new List<string?>(){"www.test.com", "www.test2.com" },
                    Created = DateTime.ParseExact("01-01-2024", format, null),
                    LastModified = DateTime.ParseExact("29-05-2024", format, null),
                    IsActive = true,
                    DeletedAt = null,
                },
                new University {
                    Id = 2,
                    Name = "NTU",
                    Country = "Singapore",
                    Webpages = new List<string?>(){"www.testy.com", "www.testy2.com" },
                    Created = DateTime.ParseExact("03-01-2024", format, null),
                    LastModified = DateTime.ParseExact("05-07-2024", format, null),
                    IsActive = true,
                    DeletedAt = null,
                },
                new University {
                    Id = 3,
                    Name = "SMU",
                    Country = "Singapore",
                    Webpages = new List<string?>(){"www.test123.com", "www.test1234.com" },
                    Created = DateTime.ParseExact("05-02-2022", format, null),
                    LastModified = DateTime.ParseExact("12-12-2023", format, null),
                    IsActive = true,
                    DeletedAt = null,
                }
            }.ToArray();
        }
    }
}
