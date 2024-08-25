using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Formats.Asn1;
using UniversityAPI.Models;

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
        public async Task<IEnumerable<University>> GetUniversities(string? name = null, string? country = null, bool? isBookmark = null, bool? isActive = null)
        {
            var universities = new List<University>();

            string format = "dd-MM-yyyy";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                var query = new SqlCommand("SELECT Id, Name, Country, Created, Webpages, IsBookmark, LastModified, IsActive, DeletedAt FROM Universities " +
                    "WHERE (@Name IS NULL OR name LIKE '%' + @Name + '%')" +
                    "AND (@Country IS NULL OR country Like '%' + @Country + '%')" +
                    "AND (@IsBookmark IS NULL OR isBookmark = @IsBookmark)" +
                    "AND (@IsActive IS NULL OR isActive = @IsActive)", connection);

                query.Parameters.AddRange(
                    new SqlParameter[] {
                       new SqlParameter("@Name",  string.IsNullOrEmpty(name) ? (object)DBNull.Value : name),
                       new SqlParameter("@Country", string.IsNullOrEmpty(country) ? (object)DBNull.Value : country),
                       new SqlParameter("@IsBookmark", isBookmark.HasValue ? isBookmark : DBNull.Value),
                       new SqlParameter("@IsActive", isActive.HasValue ? isActive : DBNull.Value)
                    }
                );

                using (var reader = await query.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {   
                        universities.Add(new University
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Country = reader.GetString(reader.GetOrdinal("country")),
                            Webpages = [.. reader.GetString(reader.GetOrdinal("webpages")).Split(';')],
                            IsBookmark = reader.GetBoolean(reader.GetOrdinal("IsBookmark")),
                            Created = reader.GetDateTime(reader.GetOrdinal("created")),
                            LastModified = reader.IsDBNull(reader.GetOrdinal("LastModified")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LastModified")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            DeletedAt = reader.IsDBNull(reader.GetOrdinal("DeletedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DeletedAt"))
                        });
                    }
                }  
            }

            return universities;
        }

        [HttpGet("{id}", Name = "GetUniversityById")]
        public async Task<ActionResult<University>> GetUniversityById(int id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                var query = new SqlCommand("SELECT TOP 1 Id, Name, Country, Created, webpages, IsBookmark, LastModified, IsActive, DeletedAt FROM Universities WHERE Id=@Id", connection);
                query.Parameters.Add(
                    new SqlParameter("@Id", id)
                );


                using (var reader = await query.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var university = new University
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Country = reader.GetString(reader.GetOrdinal("country")),
                            Webpages = [.. reader.GetString(reader.GetOrdinal("webpages")).Split(';')],
                            IsBookmark = reader.GetBoolean(reader.GetOrdinal("isbookmark")),
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

        [HttpPost(Name = "PostUniversity")]
        public async Task<ActionResult<University>> PostUniversity(University university)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                var query = new SqlCommand(
                    "INSERT INTO UNIVERSITIES ( Name, Country, Webpages, IsBookmark, Created, IsActive) VALUES (@Name, @Country, @Webpages, @IsBookmark, GETDATE(), 1);" +
                    "SELECT Id, Name, Country, Webpages, IsBookmark, Created, IsActive FROM Universities WHERE Id = SCOPE_IDENTITY()", 
                    connection);

                query.Parameters.AddRange(
                    new SqlParameter[] {
                       new SqlParameter("@Name", university.Name),
                       new SqlParameter("@Country", university.Country),
                       new SqlParameter("@Webpages", String.Join(';', university.Webpages)),
                       new SqlParameter("@IsBookmark", university.IsBookmark)
                    }
                );

                using (var reader = await query.ExecuteReaderAsync()) {
                    if(await reader.ReadAsync())
                    {
                        var createdUniversity = new University
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Country = reader.GetString(reader.GetOrdinal("country")),
                            Webpages = [.. reader.GetString(reader.GetOrdinal("webpages")).Split(';')],
                            IsBookmark = reader.GetBoolean(reader.GetOrdinal("isbookmark")),
                            Created = reader.GetDateTime(reader.GetOrdinal("created")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        };

                        return Ok(createdUniversity);
                    } else
                    {
                        return StatusCode(500, "An error occurred while creating the university.");
                    }
                }
            }

        }

        [HttpPut("{id}", Name = "PutUniversity")]
        public async Task<ActionResult> PutUniversity(long id, University university)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                var query = new SqlCommand(
                    "UPDATE UNIVERSITIES SET Name = @Name, Country = @Country, Webpages = @Webpages, IsBookmark = @IsBookmark, LastModified = GetDate() WHERE ID = @Id;" +
                    "SELECT Id, Name, Country, Webpages, IsBookmark, Created, IsActive, LastModified FROM Universities WHERE Id = @Id",
                    connection);

                query.Parameters.AddRange(
                    new SqlParameter[] {
                       new SqlParameter("@Id", id),
                       new SqlParameter("@Name", university.Name),
                       new SqlParameter("@Country", university.Country),
                       new SqlParameter("@Webpages", String.Join(';', university.Webpages)),
                       new SqlParameter("@IsBookmark", university.IsBookmark)
                    }
                );

                using (var reader = await query.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var updatedUniversity = new University
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Country = reader.GetString(reader.GetOrdinal("country")),
                            Webpages = [.. reader.GetString(reader.GetOrdinal("webpages")).Split(';')],
                            IsBookmark = reader.GetBoolean(reader.GetOrdinal("isbookmark")),
                            Created = reader.GetDateTime(reader.GetOrdinal("created")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            LastModified = reader.GetDateTime(reader.GetOrdinal("LastModified"))
                        };

                        return Ok(updatedUniversity);
                    }
                    else
                    {
                        return StatusCode(500, $"Update failed: No university with ID {id} found.");
                    }
                }
            }

        }

        [HttpDelete("{id}", Name = "DeleteUniversity")]
        public async Task<ActionResult> DeleteUniversity(long id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                var query = new SqlCommand(
                "UPDATE UNIVERSITIES SET IsActive = 0, DeletedAt=GETDATE() WHERE ID = @Id;" +
                "SELECT Id, Name, Country, IsActive, DeletedAt FROM Universities WHERE Id = @Id",
                connection);

                query.Parameters.AddRange(
                    new SqlParameter[] {
                       new SqlParameter("@Id", id)
                    }
                );

                using (var reader = await query.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var updatedUniversity = new UniversityDto
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Country = reader.GetString(reader.GetOrdinal("country")),
                        };

                        return Ok(updatedUniversity);
                    }
                    else
                    {
                        return StatusCode(500, $"Delete failed: No university with ID {id} found.");
                    }
                }
            }
        }


        [HttpPost("bookmark/{id}", Name = "BookmarkUniversity")]
        public async Task<ActionResult> BookmarkUniversity(long id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                var query = new SqlCommand(
                    "UPDATE UNIVERSITIES SET IsBookmark = 1, LastModified=GETDATE() WHERE ID = @Id;" +
                    "SELECT Id, Name, Country, Webpages, IsBookmark, Created, IsActive, LastModified FROM Universities WHERE Id = @Id",
                    connection);

                query.Parameters.AddRange(
                    new SqlParameter[] {
                       new SqlParameter("@Id", id)
                    }
                );

                using (var reader = await query.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var updatedUniversity = new University
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Country = reader.GetString(reader.GetOrdinal("country")),
                            Webpages = [.. reader.GetString(reader.GetOrdinal("webpages")).Split(';')],
                            IsBookmark = reader.GetBoolean(reader.GetOrdinal("isbookmark")),
                            Created = reader.GetDateTime(reader.GetOrdinal("created")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            LastModified = reader.GetDateTime(reader.GetOrdinal("lastmodified"))
                        };

                        return Ok(updatedUniversity);
                    }
                    else
                    {
                        return StatusCode(500, $"Bookmark failed: No university with ID {id} found.");
                    }
                }
            }

        }
    }
}
