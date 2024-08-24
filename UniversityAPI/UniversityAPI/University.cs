namespace UniversityAPI
{
    public class University
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Country { get; set; }

        public List<string?>? Webpages { get; set; }

        public bool IsBookmark { get; set; }

        public bool IsActive { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? LastModified { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
