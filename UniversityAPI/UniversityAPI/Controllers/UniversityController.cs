using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace UniversityAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversityController : ControllerBase
    {
        private readonly ILogger<UniversityController> _logger;
        private readonly IConfiguration _configuration;

        public UniversityController(ILogger<UniversityController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

        }

        [HttpGet(Name = "GetUniversity")]
        public async Task<IEnumerable<University>> Get()
        {
            var universities = new List<University>();

            string format = "dd-MM-yyyy";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                var query = "SELECT Id, Name, Country, Created, webpages, LastModified, IsActive, DeletedAt FROM Universities";
               
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            
                            universities.Add(new University
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                Country = reader.GetString(reader.GetOrdinal("country")),
                                Webpages = [.. reader.GetString(reader.GetOrdinal("webpages")).Split(';')],
                                Created = reader.GetDateTime(reader.GetOrdinal("created")),
                                LastModified = reader.IsDBNull(reader.GetOrdinal("LastModified")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LastModified")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                DeletedAt = reader.IsDBNull(reader.GetOrdinal("DeletedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DeletedAt"))
                            });
                        }
                    }
                }
            }

            return universities;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<University>> GetUniversityById(int id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                var query = new SqlCommand("SELECT TOP 1 Id, Name, Country, Created, webpages, LastModified, IsActive, DeletedAt FROM Universities WHERE Id=@Id", connection);
                query.Parameters.Add(
                    new SqlParameter("@Id", id)
                );


                using (var reader = await query.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var university = new University
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Country = reader.GetString(reader.GetOrdinal("country")),
                            Webpages = [.. reader.GetString(reader.GetOrdinal("webpages")).Split(';')],
                            Created = reader.GetDateTime(reader.GetOrdinal("created")),
                            LastModified = reader.IsDBNull(reader.GetOrdinal("LastModified")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LastModified")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            DeletedAt = reader.IsDBNull(reader.GetOrdinal("DeletedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DeletedAt"))
                        };

                        return Ok(university);
                    } else
                    {
                        return BadRequest($"No university with ID {id} found");
                    }
                }
                
            }

            
        }
    }
}
